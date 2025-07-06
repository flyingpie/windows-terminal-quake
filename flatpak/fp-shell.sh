#!/bin/bash

flatpak uninstall -y nl.flyingpie.wtq

flatpak-builder --force-clean --install --user build-dir nl.flyingpie.wtq.yaml

flatpak run --command=sh nl.flyingpie.wtq