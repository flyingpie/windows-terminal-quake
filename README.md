# WTQ v2
### Enable Quake-style dropdown for (almost) any application.
For **Windows 10** & **11**, and **KDE Plasma 5** & **6** (Wayland only).

[Documentation](https://wtq.flyingpie.nl)

[![WTQ CI](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml/badge.svg)](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml)

![Preview](./assets/logo-01.webp)

# Showcase

### Windows 10
With [Windows Terminal](https://github.com/microsoft/terminal), [Double Commander](https://github.com/doublecmd/doublecmd) and [Process Hacker](https://processhacker.sourceforge.io/).

https://github.com/user-attachments/assets/57372a68-ab69-4cb1-b70d-acf440e5368c

### CachyOS - KDE Plasma 6
With [WezTerm](https://wezfurlong.org/wezterm/index.html), [Dolphin](https://apps.kde.org/dolphin/), [System Monitor](https://apps.kde.org/plasma-systemmonitor/), [KeePassXC](https://keepassxc.org/) and [Spotify](https://www.spotify.com/us/download/).

https://github.com/user-attachments/assets/c1b386fc-9026-48d9-87e8-081a26b0ff45

# Installation

*Also see the [documentation](https://wtq.flyingpie.nl)*

### Manual Download (Windows)
See [the latest release](https://github.com/flyingpie/windows-terminal-quake/releases/latest), and pick a zip.

### Scoop (Windows)
```
scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/wtq-latest.json
```
A shortcut is then available named **WTQ - Windows Terminal Quake**, or you can just run ```wtq``` from a command line or Win+R.

### WinGet (Windows)
```
winget install windows-terminal-quake
```
You can then call ```wtq``` from the command line.

After having done that at least once, a shortcut will appear in the start menu, called **WTQ - Main Window**.

![image](https://github.com/user-attachments/assets/aebaf70c-76d3-4d51-9c28-1f6a7ad4b78f)

### Flatpak (Linux)
```
TODO
```

### Manual (Linux)
See the [/linux/install-or-upgrade-wtq.sh script](https://github.com/flyingpie/windows-terminal-quake/blob/master/linux/install-or-upgrade-wtq.sh) that downloads the latest version of WTQ, installs it to ```~/.local/share/wtq```, and creates a **wtq.desktop** file.

As a 1-liner:
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/linux/install-or-upgrade-wtq.sh)
```

And the [/linux/uninstall-wtq.sh uninstall script](https://github.com/flyingpie/windows-terminal-quake/blob/master/linux/uninstall-wtq.sh).
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/linux/uninstall-wtq.sh)
```

> [!NOTE]
> The WTQ configuration is not removed by this script. These are usually located at ```~/.config/wtq```.

# Configuration

*Also see the [documentation](https://wtq.flyingpie.nl)*

After starting WTQ, an icon will appear in the tray, which has some useful buttons in the context menu:
![wtq-context-menu](https://github.com/user-attachments/assets/d9045c85-b3a3-4c57-8d2f-53ab55fc4e38)

From there, the settings file can quickly be opened using "Open Settings File".

A JSON schema file should be generated next to the settings file, enabling intellisense-like features in supporting editors:

![image](https://github.com/user-attachments/assets/c7ec61f2-4e98-41d5-8fc3-4a082a7d6a97)

There's also an GUI available to configure WTQ, through the same context menu - "Open Main Window".

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
