#!/bin/bash

flatpak uninstall -y nl.flyingpie.wtq

rm -rf ./.flatpak-build-dir
rm -rf ./.flatpak-repo
rm -rf ./.flatpak-state-dir
