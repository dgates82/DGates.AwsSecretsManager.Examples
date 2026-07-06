# Local Development

## Prerequisites

| Tool | Purpose |
|---|---|
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | Runs LocalStack and seeds secrets |
| [.NET SDK 8+](https://dotnet.microsoft.com/download) | Builds and runs example projects |
| `aws` CLI _(optional)_ | Manually inspect or update secrets in LocalStack |

> The examples target **net48** but build with any recent .NET SDK.

## Start LocalStack

```bash
docker compose up
```

This starts LocalStack and runs the init container which seeds all secrets
automatically. You should see `Seeding complete.` in the logs before starting
an example project.

To run in the background:

```bash
docker compose up -d
```

## Run ConsoleExample

```bash
dotnet run --project src/ConsoleExample/ConsoleExample.csproj
```

Or open `DGates.AwsSecretsManager.Examples.sln` in Visual Studio and run
`ConsoleExample`.

## Running MvcExample without Docker

MvcExample targets classic ASP.NET MVC 5 (`System.Web.Mvc`), which needs IIS/IIS Express to
host — so it only runs on Windows regardless of backend. That's usually fine, since Docker
Desktop's Linux containers (which LocalStack requires) work normally on native Windows via the
WSL2 backend.

The gap shows up specifically if you're developing inside a **Windows VM nested inside another
hypervisor** (rather than native Windows hardware) — Docker Desktop's Linux-container support
needs virtualization extensions passed through to the guest, which many nested-VM setups don't
expose. In that case, Docker isn't available at all on the Windows side, and LocalStack can't
be reached over the network from the VM either.

For this case, use the library's `LocalJsonFallbackPath` setting instead of pointing at
LocalStack:

1. Create a fixture JSON file, e.g. `fixtures/mvc-secrets.json`:
```json
   {
     "dev/MvcExample/OpenWeatherMap": {
       "Url": "https://api.openweathermap.org/data/2.5/weather",
       "Key": "placeholder-key-for-local-dev"
     }
   }
```
2. Point `Web.config` at it:
```xml
   <appSettings>
     <add key="LocalJsonFallbackPath" value="fixtures\mvc-secrets.json" />
   </appSettings>
```
3. Run MvcExample via IIS Express or Visual Studio as normal. The library reads the secret
   from the JSON file instead of calling AWS/LocalStack.

This is a fallback for environments where Docker isn't reachable — it doesn't exercise the
same code path as a real Secrets Manager call (no cache TTL expiry against a live backend, no
rotation handling), so it's not a substitute for the LocalStack-backed integration coverage
ConsoleExample and the library's test suite already provide. Treat it as "can I boot and see
the app work," not "have I verified Secrets Manager integration."

## Tearing down

```bash
docker compose down -v
```

## Secrets seeded by default

| Secret name | Purpose |
|---|---|
| `dev/ConsoleExample/DbConfig` | Fake DB credentials used by ConsoleExample |
| `dev/MvcExample/OpenWeatherMap` | Placeholder OpenWeatherMap API URL/key used by MvcExample |

## Re-seeding manually

If LocalStack is already running and you need to re-seed:

```bash
docker/seed-secrets.sh
```

> The seed script is idempotent — re-running it is safe.

## Testing with a real OpenWeatherMap key

`docker/seed-secrets.sh` seeds a placeholder key by default. If you want to test against the
real OpenWeatherMap API, create `docker/seed-secrets.local.sh` with your real key (this file is
gitignored and never committed) and run it after `docker compose up` instead of the default
seed script.

## Inspecting secrets

```bash
# List all secrets
aws --endpoint-url=http://localhost:4566 secretsmanager list-secrets

# Read a specific secret value
aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value \
  --secret-id dev/ConsoleExample/DbConfig
```