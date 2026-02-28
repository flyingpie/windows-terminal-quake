#!/bin/bash

# Install Flatpak Builder
flatpak install --user -y org.flatpak.Builder

# Install WTQ Flatpak dependencies
org.flatpak.Builder build-dir --user --install-deps-from=flathub --download-only nl.flyingpie.wtq.yml

# Generate nuget-sources.json
wget https://raw.githubusercontent.com/flatpak/flatpak-builder-tools/refs/heads/master/dotnet/flatpak-dotnet-generator.py
python3 flatpak-dotnet-generator.py --dotnet 9 --freedesktop 24.08 nuget-sources.json ../../src/30-Host/Wtq.Host.Linux/Wtq.Host.Linux.csproj