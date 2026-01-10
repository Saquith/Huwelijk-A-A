#!/bin/sh
# Simple entrypoint that validates required env vars then starts the app.
# Location: /app in the runtime image.

set -e

missing=false
check_var() {
  var_name="$1"
  if [ -z "$(printenv "$var_name")" ]; then
    echo "ERROR: Required environment variable $var_name is not set"
    missing=true
  fi
}

# Required settings: Host, Port, To
check_var "Smtp__Host"
check_var "Smtp__Port"
check_var "Smtp__To"

if [ "$missing" = "true" ]; then
  echo "One or more required environment variables are missing. Exiting."
  exit 1
fi

# If SMTP user is provided but not password, warn
if [ -n "$Smtp__User" ] && [ -z "$Smtp__Password" ]; then
  echo "WARNING: Smtp__User is set but Smtp__Password is empty. Authentication will likely fail."
fi

echo "Starting WeddingBackend with the following SMTP settings:"
echo "  Host: ${Smtp__Host}"
echo "  Port: ${Smtp__Port}"
echo "  UseSsl: ${Smtp__UseSsl:-true}"
echo "  To: ${Smtp__To}"
echo ""

# Execute the main process
exec dotnet WeddingBackend.dll