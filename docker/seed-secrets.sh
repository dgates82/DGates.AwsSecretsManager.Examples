#!/usr/bin/env bash
# Seeds LocalStack Secrets Manager with secrets for local dev.
# Runs automatically via LocalStack's ready.d hook on `docker compose up`.
#
# Usage: ./docker/seed-secrets.sh

set -euo pipefail

ENDPOINT="http://localhost:4566"
REGION="us-west-2"

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION="$REGION"

create_or_update_secret() {
  local name="$1"
  local value="$2"

  if aws --endpoint-url="$ENDPOINT" secretsmanager describe-secret --secret-id "$name" >/dev/null 2>&1; then
    echo "Updating existing secret: $name"
    aws --endpoint-url="$ENDPOINT" secretsmanager put-secret-value \
      --secret-id "$name" --secret-string "$value" >/dev/null
  else
    echo "Creating secret: $name"
    aws --endpoint-url="$ENDPOINT" secretsmanager create-secret \
      --name "$name" --secret-string "$value" >/dev/null
  fi
}

# ConsoleExample — fake DB credentials
create_or_update_secret "dev/ConsoleExample/DbConfig" \
  '{"Server":"localhost","Database":"ExampleDb","Username":"app_user","Password":"s3cr3t!"}'

# MvcExample — placeholder OpenWeatherMap key (replace with real key for live data)
create_or_update_secret "dev/DGates.AwsSecretsManager.Examples/OpenWeatherMap" \
  '{"Url":"https://api.openweathermap.org/data/2.5/weather","Key":"YOUR_KEY_HERE"}'

echo "Done. Seeded secrets are available at $ENDPOINT (region $REGION)."
