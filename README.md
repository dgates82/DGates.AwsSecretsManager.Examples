# DGates.AwsSecretsManager.Examples

[![CI](https://github.com/dgates82/DGates.AwsSecretsManager.Examples/actions/workflows/ci.yml/badge.svg)](https://github.com/dgates82/DGates.AwsSecretsManager.Examples/actions/workflows/ci.yml)

Example applications demonstrating integration of the [DGates.AwsSecretsManager](https://github.com/dgates82/DGates.AwsSecretsManager) library into .NET Framework 4.8 apps.

## NuGet

```
Install-Package DGates.AwsSecretsManager
```

> Package page: https://www.nuget.org/packages/DGates.AwsSecretsManager _(coming soon — pending NuGet publication)_

## Examples

### ConsoleExample
Minimal wiring demo — reads a typed secret from LocalStack, demonstrates cache invalidation, force refresh, and cache hit logging.

### MvcExample _(coming soon)_
ASP.NET MVC app using `SecretsManagerServiceFactory` for DI. Retrieves an OpenWeatherMap API key from Secrets Manager and displays live weather data.

## Local Development

See [docs/LOCAL_DEV.md](docs/LOCAL_DEV.md) for prerequisites and setup steps.

Quick start:

```bash
docker-compose up
```

Then run ConsoleExample from Visual Studio or the `dotnet` CLI. _(MvcExample coming soon.)_

## Library

- Library repo: https://github.com/dgates82/DGates.AwsSecretsManager
- Examples repo: https://github.com/dgates82/DGates.AwsSecretsManager.Examples

## License

MIT
