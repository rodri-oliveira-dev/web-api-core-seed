# Multi-Stage Build Patterns

Multi-stage builds separate build-time toolchains from the runtime image. Every language has a common pattern.

## General structure

```dockerfile
# syntax=docker/dockerfile:1

# Stage 1: build
FROM <sdk-image> AS build
WORKDIR /src
COPY <dependency-manifest> .
RUN <install-dependencies>
COPY . .
RUN <compile-or-bundle>

# Stage 2: runtime
FROM <minimal-base> AS runtime
WORKDIR /app
COPY --from=build --chown=appuser:appgroup /src/<artifact> .
USER appuser
ENTRYPOINT ["./artifact"]
```

## Go

Go produces static binaries, so the runtime stage can use `scratch` or distroless.

```dockerfile
# syntax=docker/dockerfile:1

FROM golang:1.23-alpine AS build
WORKDIR /src
COPY go.mod go.sum ./
RUN --mount=type=cache,target=/go/pkg/mod go mod download
COPY . .
RUN --mount=type=cache,target=/go/pkg/mod \
    --mount=type=cache,target=/root/.cache/go-build \
    CGO_ENABLED=0 go build -ldflags="-s -w" -o /app/server ./cmd/server

FROM gcr.io/distroless/static-debian12:nonroot AS runtime
WORKDIR /app
COPY --from=build --link /app/server .
USER nonroot:nonroot
EXPOSE 8080
ENTRYPOINT ["./server"]
```

Key points:

- Use `CGO_ENABLED=0` for a fully static binary when cgo is not needed.
- Use `-ldflags="-s -w"` to strip debug symbols and reduce binary size.
- Cache both `/go/pkg/mod` (downloaded modules) and `/root/.cache/go-build` (compilation cache).
- Distroless static images include a built-in `nonroot` user.
- For `GOPRIVATE` modules fetched via Git SSH, use `--mount=type=ssh` instead of baking keys or tokens. Populate `known_hosts` inside the same `RUN`, pair the git-config rewrite with the download so the config does not persist into later stages, and set `GOPRIVATE` inline so `go mod download` skips the public proxy and checksum database (`GOPRIVATE` implies `GONOSUMDB` and `GONOPROXY`):
  ```dockerfile
  RUN --mount=type=ssh \
      --mount=type=cache,target=/go/pkg/mod \
      mkdir -p -m 0700 /root/.ssh && \
      ssh-keyscan -t ed25519 github.com >> /root/.ssh/known_hosts && \
      git config --global url."git@github.com:".insteadOf "https://github.com/" && \
      GOPRIVATE="github.com/your-org/*" go mod download
  ```
  Invoke with `docker buildx build --ssh default .` (uses the host's SSH agent — ensure it is running and the key is loaded: `eval "$(ssh-agent -s)" && ssh-add ~/.ssh/id_ed25519`). To pass a key file directly without an agent, use `--ssh default=$HOME/.ssh/id_ed25519`.

## Node.js

Node.js applications require the Node runtime, so use a slim base for the runtime stage.

```dockerfile
# syntax=docker/dockerfile:1

FROM node:22-alpine AS deps
WORKDIR /src
COPY package.json package-lock.json ./
RUN --mount=type=secret,id=npmrc,target=/root/.npmrc,required=false \
    --mount=type=cache,target=/root/.npm \
    npm ci --omit=dev

FROM node:22-alpine AS build
WORKDIR /src
COPY package.json package-lock.json ./
RUN --mount=type=secret,id=npmrc,target=/root/.npmrc,required=false \
    --mount=type=cache,target=/root/.npm \
    npm ci
COPY . .
RUN npm run build

FROM node:22-alpine AS runtime
WORKDIR /app
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser
COPY --from=deps --chown=appuser:appgroup /src/node_modules ./node_modules
COPY --from=build --chown=appuser:appgroup /src/dist ./dist
COPY --from=build --chown=appuser:appgroup /src/package.json .
USER appuser
EXPOSE 3000
ENTRYPOINT ["node", "dist/index.js"]
```

Invoke with the registry credential mounted only during `npm ci`:

```sh
docker buildx build --secret id=npmrc,src=$HOME/.npmrc .
```

Key points:

- Use a separate `deps` stage that installs only production dependencies (`--omit=dev`).
- Use a `build` stage with all dependencies for compilation/bundling.
- Copy production `node_modules` from the `deps` stage, not the `build` stage.
- **Never `COPY .npmrc`** — registry credentials must be mounted with `--mount=type=secret`, not copied into a layer.
- If the project uses a bundler that produces a standalone output (e.g., Next.js standalone mode), copy only the standalone output and skip `node_modules` entirely.

## Python

Python applications use a virtual environment to isolate dependencies. Copy the venv into the runtime stage.

```dockerfile
# syntax=docker/dockerfile:1

FROM python:3.13-slim AS build
WORKDIR /src
RUN python -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"
COPY requirements.txt .
RUN --mount=type=secret,id=pip-conf,target=/etc/pip.conf,required=false \
    --mount=type=cache,target=/root/.cache/pip \
    pip install --no-compile -r requirements.txt
COPY . .

FROM python:3.13-slim AS runtime
WORKDIR /app
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser
COPY --from=build --chown=appuser:appgroup /opt/venv /opt/venv
COPY --from=build --chown=appuser:appgroup /src /app
ENV PATH="/opt/venv/bin:$PATH"
USER appuser
EXPOSE 8000
ENTRYPOINT ["python", "-m", "uvicorn", "main:app", "--host", "0.0.0.0"]
```

Key points:

- Build the virtual environment in the build stage and copy it whole into the runtime stage.
- Set `PATH` to use the venv in both stages.
- Use `--no-compile` during pip install to skip `.pyc` generation (Python will compile at first import).
- For Poetry or PDM projects, export to `requirements.txt` first or use the tool's built-in export.
- For private package indexes, pass `pip.conf` via `--mount=type=secret,id=pip-conf,target=/etc/pip.conf` instead of `COPY pip.conf` (which would leak into a layer). Invoke with `docker buildx build --secret id=pip-conf,src=$HOME/.config/pip/pip.conf .` (XDG path, pip ≥ 19.1) or the legacy `$HOME/.pip/pip.conf`.

## Java

Java applications compile to JARs. Use a JDK for building and a JRE for runtime.

```dockerfile
# syntax=docker/dockerfile:1

FROM eclipse-temurin:21-jdk-alpine AS build
WORKDIR /src
COPY pom.xml .
COPY .mvn .mvn
COPY mvnw .
RUN --mount=type=cache,target=/root/.m2 \
    --mount=type=secret,id=maven-settings,target=/root/.m2/settings.xml,required=false \
    ./mvnw dependency:go-offline -B
COPY src ./src
RUN --mount=type=cache,target=/root/.m2 \
    --mount=type=secret,id=maven-settings,target=/root/.m2/settings.xml,required=false \
    ./mvnw package -DskipTests -B

FROM eclipse-temurin:21-jre-alpine AS runtime
WORKDIR /app
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 -G appgroup appuser
COPY --from=build --chown=appuser:appgroup /src/target/*.jar app.jar
USER appuser
EXPOSE 8080
ENTRYPOINT ["java", "-jar", "app.jar"]
```

Key points:

- Use `dependency:go-offline` (Maven) or a Gradle dependency resolution task to cache dependencies before copying source.
- Cache the `.m2` or `.gradle` directory with a cache mount.
- Use a JRE image (not JDK) for the runtime stage.
- For GraalVM native images, the runtime stage can use `scratch` or distroless, similar to Go.
- For private Maven repositories, pass `~/.m2/settings.xml` via `--mount=type=secret,id=maven-settings,target=/root/.m2/settings.xml` instead of `COPY settings.xml`. Invoke with `docker buildx build --secret id=maven-settings,src=$HOME/.m2/settings.xml .`.

## When to add more stages

Add intermediate stages when:

- You need separate dependency resolution (production vs. dev dependencies, as in Node.js).
- You want to run tests in a dedicated stage without polluting the build or runtime stages.
- You are generating assets (CSS, static files) in a separate tool from the main application build.

Name every stage. Never rely on numeric stage indices.
