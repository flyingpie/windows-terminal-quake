#!/bin/bash

flatpak uninstall -y $FLATPAK_APP

rm -rf $FLATPAK_BUILD_ROOT