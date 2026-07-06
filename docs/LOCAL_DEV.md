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

## Tearing down
```bash
docker compose down -v
```

## Secrets seeded by default
| Secret name | Purpose |
|---|---|
| `dev/ConsoleExample/DbConfig` | Fake DB credentials used by ConsoleExample |

## Re-seeding manually
If LocalStack is already running and you need to re-seed:
```bash
./docker/seed-secrets.sh
```
> The seed script is idempotent — re-running it is safe.

## Inspecting secrets
```bash
# List all secrets
aws --endpoint-url=http://localhost:4566 secretsmanager list-secrets

# Read a specific secret value
aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value \
  --secret-id dev/ConsoleExample/DbConfig
```