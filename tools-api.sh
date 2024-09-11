#!/bin/bash
set -e # Quit on non null output
set -x # echo on

# Run Tools
exec dotnet /app/NovaLab.Servers.API.Tools/NovaLab.Servers.API.Tools.dll
