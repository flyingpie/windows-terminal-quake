#!/bin/bash

# Create settings metadata file, which is used to build the docs upon.
dotnet run --project ./src/30-Host/Wtq.Host.Docs/Wtq.Host.Docs.csproj ./docs/wtqsettings.yml

pushd ./docs
uv run mkdocs build
uv run mkdocs serve