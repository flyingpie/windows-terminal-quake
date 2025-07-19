# Flatpak

## TODO
- Unix socket (and TCP socket?)
Ability to expose unix pipe/tcp socket

- Events?

- Re-implement "IsCallable" for Flatpak builds.

## Download nuget-sources.json generator
wget https://raw.githubusercontent.com/flatpak/flatpak-builder-tools/master/dotnet/flatpak-dotnet-generator.py

## Generate nuget-sources.json
python3 flatpak-dotnet-generator.py --dotnet 9 --freedesktop 24.08 nuget-sources.json ../src/30-Host/Wtq.Host.Linux/Wtq.Host.Linux.csproj

## Install deps from Flathub
flatpak-builder build-dir --user --install-deps-from=flathub --download-only <app-id>.yml

flatpak-builder build-dir --user --force-clean --install --repo=repo nl.flyingpie.wtq.yml

## Docs
- Permission '--filesystem=home' is not required, but useful if the settings file exists there (either directly, or as a symlink)

## Photino guide
https://github.com/tryphotino/photino.Samples/tree/master/Photino.PublishPhotino/PublishPhotino

XDG Flatpak Conventions
https://docs.flatpak.org/en/latest/conventions.html#xdg-base-directories