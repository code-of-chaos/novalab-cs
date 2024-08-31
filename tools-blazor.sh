#!/bin/bash
set -e # Quit on non null output
set -x # echo on

# Run Tools
exec dotnet /app/NovaLab.Servers.Blazor.Tools/NovaLab.Servers.Blazor.Tools.dll
