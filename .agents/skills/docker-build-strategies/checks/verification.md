# Verification Runbook

Use these checks to verify a generated Dockerfile meets quality standards.

## Scripted verification

Run the bundled script from the project root:

```bash
bash scripts/verify-build.sh [--help] [IMAGE_NAME]
```

The image name defaults to `verify-build-test`. Exit status is `0` when the build and inspection commands succeed or help is requested, the failing Docker command's non-zero status when verification fails, and `2` for invalid arguments.

## 1. Build succeeds

```bash
docker buildx build -t test-image .
```

The build must complete without errors. Use `--progress=plain` to inspect each step if debugging is needed (avoid this flag in CI where build logs are persisted, since it may expose the content of any secret that a `RUN` step accidentally echoes).

## 2. Image size is reasonable

```bash
docker images test-image --format "{{.Size}}"
```

Expected baselines for a minimal application:

| Language | Reasonable upper bound |
|---|---|
| Go (distroless/scratch) | < 30 MB |
| Node.js (Alpine) | < 200 MB |
| Python (slim) | < 250 MB |
| Java (JRE Alpine) | < 300 MB |

If the image exceeds these bounds, check for:

- Missing multi-stage build (build tools included in runtime image)
- Large unnecessary files copied into the image
- Missing `.dockerignore`
- Package manager caches not cleaned

Use `docker history test-image` to identify which layers are largest.

## 3. Runs as non-root

```bash
docker run --rm test-image whoami
```

Expected output: `appuser`, `nonroot`, or another non-root username. Must not return `root`.

If the image does not have `whoami` (e.g., distroless), verify with:

```bash
docker inspect test-image --format '{{.Config.User}}'
```

The output must be non-empty and must not be `0` or `root`.

## 4. No secrets in image

### 4a. Static check of the Dockerfile pattern (primary)

Before building, verify the Dockerfile does not `COPY` credential files or pass credentials through `ARG`/`ENV`. Any match below is a leak:

```bash
# Credential files copied into the build context
grep -nE "^(COPY|ADD) .*(\.npmrc|\.pypirc|\.netrc|pip\.conf|settings\.xml|\.env|\.aws/credentials|\.config/gcloud|\.azure/|\.vault-token|\.cargo/credentials|id_(rsa|dsa|ed25519|ecdsa)|service.account.*\.json|\.pem([[:space:]]|$)|\.p12([[:space:]]|$)|kubeconfig)" Dockerfile

# Credentials passed as build args (visible in docker history) — case-insensitive
grep -inE "^ARG .*(TOKEN|KEY|SECRET|PASSWORD)" Dockerfile

# Credentials baked into image env (visible to anyone with the image) — case-insensitive
grep -inE "^ENV .*(TOKEN|KEY|SECRET|PASSWORD)=" Dockerfile
```

If the project needs registry credentials, the Dockerfile must use `RUN --mount=type=secret` and the build invocation must pass the secret:

```bash
docker buildx build --secret id=<id>,src=<host-path> --progress=plain .
```

The `--progress=plain` output should show the secret being consumed inside the right `RUN` step **without printing its value**. If you see the secret content in the log, the `RUN` is leaking it (e.g., via `echo`, `cat`, or shell substitution into a logged command) — that is a build-log leak even when the layer itself is clean. For private Git access, use `--mount=type=ssh` and `docker buildx build --ssh default .`.

### 4b. Backstop: scan the built image

```bash
docker history test-image --no-trunc
```

Inspect the output for any `ENV` instructions or `COPY` steps that might include `.env` files, API keys, or credentials.

**Note:** `docker history` shows layers of the final exported image only. It will **not** reveal credentials that were `COPY`-ed in an intermediate stage but not carried forward — those files still exist in BuildKit's build cache on the builder host. The static Dockerfile check in 4a is the only way to catch that class of leak. This step is a backstop.

## 5. Layer count

```bash
docker history test-image --format "{{.CreatedBy}}" | wc -l
```

A well-structured image typically has 8-15 layers. Significantly more may indicate missing command consolidation.

## 6. Correct WORKDIR, EXPOSE, and ENTRYPOINT

```bash
docker inspect test-image --format '{{.Config.WorkingDir}}'
docker inspect test-image --format '{{.Config.ExposedPorts}}'
docker inspect test-image --format '{{.Config.Entrypoint}}'
```

Verify:

- `WorkingDir` is set (not empty or `/`)
- `ExposedPorts` documents the expected port
- `Entrypoint` uses exec form (JSON array), not shell form

## 7. .dockerignore exists

Verify a `.dockerignore` file is present alongside the Dockerfile and excludes at minimum:

- `.git/`
- `node_modules/`, `__pycache__/`, or equivalent language artifacts
- `.env` and secret files
- IDE configuration directories

## 8. BuildKit syntax directive

The first line of the Dockerfile must be:

```dockerfile
# syntax=docker/dockerfile:1
```

This enables BuildKit features like cache mounts and `COPY --link`.
