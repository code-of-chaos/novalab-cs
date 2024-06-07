#!/bin/bash
set -e # Quit on non null output
set -x # echo on

# Run Tools
exec dotnet /app/NovaLab.Server.Tools/NovaLab.Server.Tools.dll
