# Middleware Order - 02 Modern Hosting

## Observed Legacy Order

1. Environment CORS policy: `Development` or `Production`.
2. Environment exception handler route: `/error-local-development` or `/error`.
3. Serilog request logging.
4. Custom Serilog middleware.
5. Custom error handling middleware.
6. HSTS.
7. Status code pages with JSON `CustomResult`.
8. Current IP rate limiting middleware.
9. Security headers middleware.
10. Authentication.
11. HTTPS redirection.
12. Static files.
13. Routing.
14. Cookie policy.
15. Authorization.
16. Endpoint route mapping.
17. Response compression.
18. Swagger and Swagger UI.
19. Health check endpoint `/hc`.

## Desired Order Implemented

1. Environment CORS policy: `Development` or `Production`.
2. Environment exception handler route: `/error-local-development` or `/error`.
3. Serilog request logging.
4. Custom Serilog middleware.
5. Custom error handling middleware.
6. HSTS.
7. Status code pages with JSON `CustomResult`.
8. Current IP rate limiting middleware.
9. Security headers middleware.
10. Response compression.
11. HTTPS redirection.
12. Static files.
13. Routing.
14. Cookie policy.
15. Authentication.
16. Authorization.
17. Controller endpoint mapping and conventional default route.
18. Swagger and Swagger UI.
19. Health check endpoint `/hc`.

## Necessary Changes

- Authentication now runs after routing and before authorization.
- Controller endpoint mapping is explicit through `MapControllers`.
- Response compression moved before endpoint execution.

## Deferred Changes

- Replace current rate limiting package with native ASP.NET Core rate limiting in issue `#6`.
- Replace legacy Swagger/OpenAPI/versioning package shape in the OpenAPI/versioning prompt.
- Adopt final Problem Details in the Problem Details prompt.
- Revisit `/hc-ui` only when a .NET 10-compatible strategy is selected.
