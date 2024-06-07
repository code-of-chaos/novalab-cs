#!/bin/bash
set -e # Quit on non null output
set -x # echo on

# Run Tools
exec dotnet /app/NovaLab.API.Tools/NovaLab.API.Tools.dll
