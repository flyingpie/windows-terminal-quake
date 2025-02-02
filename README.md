# WTQ v2
### Enable Quake-style dropdown for (almost) any application.
For **Windows 10** & **11**, and **KDE Plasma 5** & **6** (Wayland only).

[![WTQ CI](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml/badge.svg)](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml)

# Showcase

![Preview](./assets/logo-01.webp)

On **Windows 10**, with [Windows Terminal](https://github.com/microsoft/terminal), [Double Commander](https://github.com/doublecmd/doublecmd) and [Process Hacker](https://processhacker.sourceforge.io/).

https://github.com/flyingpie/windows-terminal-quake/assets/1295673/04360031-424e-49c4-b453-47e4f55822b4

On **CachyOS**, with [WezTerm](https://wezfurlong.org/wezterm/index.html), [Dolphin](https://apps.kde.org/dolphin/), [System Monitor](https://apps.kde.org/plasma-systemmonitor/), [KeePassXC](https://keepassxc.org/) and [Spotify](https://www.spotify.com/us/download/).

https://github.com/user-attachments/assets/5a668737-1147-4861-93b6-be637c0d3eaf

On **CachyOS**, combined with [Kando](https://github.com/kando-menu/kando).
<video src="https://github.com/user-attachments/assets/532964f3-2e3b-4c60-9b08-7ffbb3662096" />

# Installation

## Manual Download (Windows & Linux)
See [the latest release](https://github.com/flyingpie/windows-terminal-quake/releases/latest), and pick a zip.

## Scoop (Windows)
```
scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/wtq-latest.json
```
A shortcut is then available named 'WTQ - Windows Terminal Quake', or you can just run 'wtq' from a command line or Win+R.

## WinGet (Windows)
```
winget install windows-terminal-quake
```
A shortcut is then available named 'wtq', or you can just run 'wtq' from a command line or Win+R.

## Flatpak (Linux)
```
TODO
```

# Configuration
After starting WTQ, an icon will appear in the tray, which has some useful buttons in the context menu:
![wtq-context-menu](https://github.com/user-attachments/assets/d9045c85-b3a3-4c57-8d2f-53ab55fc4e38)

From there, the settings file can quickly be opened using "Open Settings File".

A JSON schema file should be generated next to the settings file, enabling intellisense-like features in supporting editors:
![image](https://github.com/user-attachments/assets/c7ec61f2-4e98-41d5-8fc3-4a082a7d6a97)

## Up Next

The main thing for the next release (**2.12.0**) is a GUI to assist with configuring WTQ.
![image](https://github.com/user-attachments/assets/511f167e-a0b9-4882-bcb3-0d4a4fe0fb26)
![image](https://github.com/user-attachments/assets/9d172f29-53eb-47b6-9fc3-0f5b71d7cff9)

# WTQ v1
WTQ started as a companion app to the new Microsoft's [Windows Terminal](https://github.com/microsoft/terminal), before a rewrite to v2 that supported other apps as well.

If you're missing a feature from v2, feel free to use v1 instead.

Companion program for the new Windows Terminal that enables Quake-style drop down.

![image](https://github.com/user-attachments/assets/26eb74ab-ff96-4f0b-a2e3-28bcba5cd7de)

- Runs alongside the new [Windows Terminal](https://github.com/microsoft/terminal)
- Toggle using CTRL+~ or CTRL+Q (configurable, see below)
- Shows up on the screen where the mouse is (eg. multi-monitor and multi-workspace)
- Transparency
- Configurable as fullscreen, or partial screen

## Usage
There are a couple of options:

- Download the latest release from the [releases page](https://github.com/flyingpie/windows-terminal-quake/releases).
- Clone/download the source and run **build.ps1** (uses [Cakebuild](https://cakebuild.net/)).
- Clone/download the source and build using Visual Studio.
- Via [scoop](https://scoop.sh): `scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/windows-terminal-quake.json`

See [the documentation](https://flyingpie.github.io/windows-terminal-quake) for more settings and information.
