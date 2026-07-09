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

## Running examples without Docker

Both examples can run without LocalStack/Docker using the library's `LocalJsonFallbackPath`
setting, which reads secrets from a local JSON file instead of calling AWS/LocalStack. This is
useful if Docker isn't available in your environment — for example, a Windows VM nested inside
another hypervisor without virtualization passthrough (Docker Desktop's Linux-container support
needs that passthrough, which many nested-VM setups don't expose), or any machine without
Docker installed at all.

#### ConsoleExample

1. Create a fixture JSON file at `src/ConsoleExample/fixtures/console-secrets.json`:
```json
   {
     "dev/ConsoleExample/DbConfig": {
       "Server": "localhost",
       "Database": "ExampleDb",
       "Username": "app_user",
       "Password": "s3cr3t!"
     }
   }
```
This file is copied to the build output directory automatically (see
`ConsoleExample.csproj`), so the relative path below resolves correctly whether you run
via `dotnet run` or the built exe.
2. Add `LocalJsonFallbackPath` to `App.config`'s `appSettings`:
```xml
   <add key="LocalJsonFallbackPath" value="fixtures\console-secrets.json" />
```
3. Run as normal:
```bash
   dotnet run --project src/ConsoleExample/ConsoleExample.csproj
```

### MvcExample

1. Create a fixture JSON file, e.g. `fixtures/mvc-secrets.json`:
```json
   {
     "dev/MvcExample/OpenWeatherMap": {
       "Url": "https://api.openweathermap.org/data/2.5/weather",
       "Key": "YOUR_KEY_HERE"
     }
   }
```

Unlike ConsoleExample, `SecretsManagerAccessor` resolves this path against the site's physical
root (`HostingEnvironment.ApplicationPhysicalPath`, i.e. where `Web.config` lives), not the
process's current working directory — a plain relative path won't resolve correctly under IIS/IIS
Express otherwise, since the worker process's working directory has nothing to do with the site's
content root. No copy-to-output step is needed; the file only needs to exist at this path relative
to the project directory. Verified working against a real IIS Express run.

2. Point `Web.config` at it:
```xml
   <appSettings>
     <add key="LocalJsonFallbackPath" value="fixtures\mvc-secrets.json" />
   </appSettings>
```
3. Run via IIS Express or Visual Studio as normal. MvcExample only hosts on Windows regardless
   of backend, since it requires IIS/IIS Express — see [CLAUDE.md](../CLAUDE.md) for details.

### Limitations

This fallback doesn't exercise the same code path as a real Secrets Manager call (no cache TTL
expiry against a live backend, no rotation handling), so it's not a substitute for the
LocalStack-backed integration coverage the library's test suite and a Docker-enabled ConsoleExample
run already provide. Treat it as "can I boot and see the app work," not "have I verified
Secrets Manager integration."

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