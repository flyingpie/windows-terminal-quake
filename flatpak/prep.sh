#!/bin/bash

flatpak remote-add --if-not-exists --user flathub https://dl.flathub.org/repo/flathub.flatpakrepo

flatpak install --user -y flathub org.flatpak.Builder
flatpak install --user -y flathub org.gnome.Platform//48
flatpak install --user -y flathub org.gnome.Sdk//48