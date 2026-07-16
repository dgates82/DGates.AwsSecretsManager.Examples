# Changelog
All notable changes to this repository will be documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
---
## [Unreleased]
### Added
- Logging demonstration in both example apps, showing `DGates.AwsSecretsManager`'s `ILogger`
  integration works with arbitrary logging frameworks:
  - ConsoleExample — custom synchronous `ILoggerProvider`/`ILogger` (`Logging/SyncConsoleLoggerProvider.cs`),
    writing directly to `Console.Out` to keep log output correctly interleaved with the demo's
    step-by-step narration (`Microsoft.Extensions.Logging.Console`'s async queue caused
    out-of-order output when mixed with synchronous console writes)
  - MvcExample — NLog, bridged into the library via `NLogLoggerProvider` in
    `SecretsManagerAccessor.Initialize()`, with `WeatherController` and `Global.asax.cs` also
    converted to NLog directly (`LogManager.GetCurrentClassLogger()`), replacing
    `System.Diagnostics.Trace` — one unified logging pipeline for both app and library output
  - MvcExample logs to `App_Data/logs/mvcexample.log` and the VS debugger output via
    `nlog.config`, `minlevel="Debug"` by default

### Changed
- ConsoleExample's demo sequence restructured to insert a fetch between `InvalidateCache` and
  `RefreshSecretAsync`, so each step's log output proves its narration (invalidate actually
  clearing the cache, refresh bypassing a warm cache) rather than asserting it
- Removed ConsoleExample's hardcoded `"[cache hit]"` console message, now redundant with the
  library's real `Debug`-level cache-hit log
- `DGates.AwsSecretsManager` dependency bumped to `1.0.0-beta.3`
---
## [0.2.0] - 2026-07-09
### Added
- MvcExample (ASP.NET MVC 5, .NET Framework 4.8) — demo app retrieving an OpenWeatherMap API
  key from Secrets Manager via `SecretsManagerServiceFactory`, geocoding and fetching live
  weather for a user-entered city
- MvcExample.Core — pure logic layer (view-model construction, response parsing),
  multi-targeted net48/net8.0, tested via MvcExample.Tests (xUnit)
- MvcExample.Infrastructure — isolates the AWS SDK dependency chain via ProjectReference,
  resolving the full transitive closure correctly in the legacy web project
- Local JSON fallback support (`LocalJsonFallbackPath`) for both ConsoleExample and MvcExample,
  documented in docs/LOCAL_DEV.md, for environments without Docker
- "How this worked" panel on the weather demo page — shows secret name, retrieved URL, and
  backend source (AWS/LocalStack vs. JSON fallback)
- Graceful handling of unconfigured/placeholder API keys and Secrets Manager connection
  failures (previously crashed the app at startup)

### Changed
- MvcExample's UI/MVC package references migrated from packages.config to PackageReference,
  fixing clean-checkout build failures on Linux
- Upgraded Bootstrap to 5.x, jQuery to 3.7.1, Newtonsoft.Json, and other vulnerable
  dependencies
- Renamed OpenWeatherMap secret from `dev/DGates.AwsSecretsManager.Examples/OpenWeatherMap` to
  `dev/MvcExample/OpenWeatherMap`, matching ConsoleExample's per-project naming convention

---
## [0.1.0] - 2025-07-05
### Added
- ConsoleExample (.NET Framework 4.8) — demonstrates typed secret retrieval,
  raw string retrieval, cache invalidation, force refresh, and cache hit logging
  against LocalStack via DGates.AwsSecretsManager
- Docker Compose setup with LocalStack 3 and automated secret seeding via
  ready.d init hook
- Secrets seeded: `dev/ConsoleExample/DbConfig` (fake DB credentials) and
  `dev/DGates.AwsSecretsManager.Examples/OpenWeatherMap` (placeholder API key)
- GitHub Actions CI workflow building the solution on push and PR against
  ubuntu-latest
- README, LOCAL_DEV.md covering prerequisites, quickstart, manual re-seed,
  and OpenWeatherMap key override steps