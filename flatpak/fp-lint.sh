#!/bin/bash

source vars.sh

flatpak run --command=flatpak-builder-lint org.flatpak.Builder appstream $FLATPAK_APP.metainfo.xml