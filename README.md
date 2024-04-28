# WTQ v2

[![WTQ CI](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml/badge.svg)](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml)

Windows Terminal Quake v2 is underway, featuring support for other terminals, other apps in general, and multiple apps simultaneously.

Join the [discussion](https://github.com/flyingpie/windows-terminal-quake/discussions/119) to give us feedback, or take a look at a [prerelease](https://github.com/flyingpie/windows-terminal-quake/releases/tag/v2.0.0-pre7).

## Manual Download
See [the latest release](https://github.com/flyingpie/windows-terminal-quake/releases/latest), and pick a zip.

## Scoop
```
scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/wtq-latest.json
```

## WinGet
*[Pending WinGet Manifest PR](https://github.com/microsoft/winget-pkgs/pull/151384)*
```
winget install windows-terminal-quake
```

https://github.com/flyingpie/windows-terminal-quake/assets/1295673/04360031-424e-49c4-b453-47e4f55822b4

# WTQ v1
If you're missing a feature from v2, feel free to use v1 instead.

Companion program for the new Windows Terminal that enables Quake-style drop down.

![Preview](./assets/example.gif)

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
