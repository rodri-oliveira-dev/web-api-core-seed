# Threat Assessment

| Risk | Mitigation |
| --- | --- |
| SQL password committed | Removed tracked legacy password and moved local value to User Secrets or `.env.local`. |
| JWT secret committed | Removed tracked placeholder value and made secret required externally. |
| User Secrets copied to image | Dockerfile does not copy user profile secret paths; `.dockerignore` excludes local secret files. |
| Secret in Docker build layers | No secret is passed as `ARG`, `ENV` or copied file during image build. |
| API runs as root | Final stage uses `USER app`. |
| Stale DB schema | One-shot migrations service runs before API. |
| Health check leaks password | SQL health command uses container env variable expansion and does not echo values. |
| File logging permission issue | Compose sets `SeqSettings__FilePath` to empty for the non-root API container. |
| Confusing legacy Dockerfiles | Legacy Dockerfiles with fixed password/floating tags were removed. |
| Compose mistaken for production | Documentation states Compose is local development only. |
