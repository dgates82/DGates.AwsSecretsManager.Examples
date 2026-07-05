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

This starts LocalStack and runs the init container which seeds all secrets automatically. You should see `Seeding complete.` in the logs before starting an example project.

To run in the background:

```bash
docker compose up -d
```

## Run ConsoleExample

```bash
dotnet run --project src/ConsoleExample/ConsoleExample.csproj
```

Or open `DGates.AwsSecretsManager.Examples.sln` in Visual Studio and run `ConsoleExample`.


## Tearing down

```sh
docker compose down -v
```


## Secrets seeded by default

| Secret name | Purpose |
|---|---|
| `dev/ConsoleExample/DbConfig` | Fake DB credentials used by ConsoleExample |
| `dev/DGates.AwsSecretsManager.Examples/OpenWeatherMap` | Placeholder API key used by MvcExample |

## Using a real OpenWeatherMap API key (MvcExample)

The seeded OpenWeatherMap secret contains a placeholder key. The MvcExample frontend displays a graceful message when the key is a placeholder — no errors.

To use live weather data, get a free key at https://openweathermap.org/api and update the secret:

```bash
aws --endpoint-url=http://localhost:4566 secretsmanager put-secret-value \
  --secret-id dev/DGates.AwsSecretsManager.Examples/OpenWeatherMap \
  --secret-string '{"Url":"https://api.openweathermap.org/data/2.5/weather","Key":"YOUR_REAL_KEY"}'
```

> This updates the secret in the running LocalStack instance only. It resets to the placeholder on the next `docker-compose up`.

## Re-seeding manually

If LocalStack is already running and you need to re-seed:

```bash
docker exec aws-sm-netframework-localstack /bin/bash /etc/localstack/init/ready.d/seed-secrets.sh
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
