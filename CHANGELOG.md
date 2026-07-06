# Changelog

All notable changes to this repository will be documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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