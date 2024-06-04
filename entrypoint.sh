#!/bin/bash
set -e # Quit on non null output

# Start main application
echo "Running server..."
exec dotnet /app/NovaLab/NovaLab.dll