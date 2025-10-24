# WTQ v2
### Enable Quake-style dropdown for (almost) any application.
For **Windows 10** & **11**, and **KDE Plasma 5** & **6** (Wayland only).

[Installation](#installation) - [Configuration](#configuration) - [Documentation](https://wtq.flyingpie.nl)

[![WTQ CI](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml/badge.svg)](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/ci.yml)

![Preview](./assets/logo-01.webp)

# Showcase

### Windows 11
With [Windows Terminal](https://github.com/microsoft/terminal), [Double Commander](https://github.com/doublecmd/doublecmd), [Process Hacker](https://processhacker.sourceforge.io/) and [KeePassXC](https://keepassxc.org/).

https://github.com/user-attachments/assets/57372a68-ab69-4cb1-b70d-acf440e5368c

### KDE Plasma 6
With [WezTerm](https://wezfurlong.org/wezterm/index.html), [Dolphin](https://apps.kde.org/dolphin/), [System Monitor](https://apps.kde.org/plasma-systemmonitor/) and [KeePassXC](https://keepassxc.org/).

https://github.com/user-attachments/assets/c1b386fc-9026-48d9-87e8-081a26b0ff45

# Installation

- [Scoop (Windows)](#scoop-windows)
- [WinGet (Windows)](#winget-windows)
- [Arch AUR (Linux)](#arch-aur-linux)
- [Flatpak (Linux)](#flatpak-linux)
- [Direct Download Script (Linux)](#direct-download-script-linux)
- [Direct Download (Windows/Linux)](#direct-download-windowslinux)
- [Build From Source (Windows/Linux)](#build-from-source-windowslinux)

*Also see the [documentation](https://wtq.flyingpie.nl)*

### Scoop (Windows)

> [!NOTE]
> The WTQ Scoop packages has moved to the Scoop [extras bucket](https://github.com/ScoopInstaller/Extras).

```pwsh
scoop bucket add extras
scoop install extras/wtq
```

A shortcut is then available named **WTQ - Windows Terminal Quake**, or you can just run ```wtq``` from a command line or Win+R.

### WinGet (Windows)
```
winget install windows-terminal-quake
```
You can then call ```wtq``` from the command line.

After having done that **at least once**, a shortcut will appear in the start menu, called **WTQ - Main Window**.

![image](https://github.com/user-attachments/assets/aebaf70c-76d3-4d51-9c28-1f6a7ad4b78f)

### Arch AUR (Linux)
Multiple versions are published to the Arch User Repository (AUR):

#### [wtq-bin](https://aur.archlinux.org/packages/wtq-bin) (Recommended)
- Latest stable release, pre-built;
- Downloads from GitHub Releases;
- Quicker to install and minimal dependencies.

```bash
yay -S wtq-bin
```
or
```bash
paru -S wtq-bin
```

#### [wtq](https://aur.archlinux.org/packages/wtq)
- Latest stable release, built from source;
- Purist open source, but takes a bit longer to install and has a bit more (build-time) dependencies.

```bash
yay -S wtq
```
or
```bash
paru -S wtq
```

### Flatpak (Linux)
Since WTQ only supports KDE Plasma on Linux, it's not a great fit for Flathub.

As an alternative, you can use the Flatpak remote hosted on the [sister repository](https://github.com/flyingpie/flatpak).
It uses the [Flatter](https://github.com/andyholmes/flatter) GitHub Action for building the Flatpak itself, and everything is hosted on GitHub Pages.

The app itself and the Flatpaks are [built entirely from source, using GitHub Actions](https://github.com/flyingpie/flatpak/actions/workflows/flatpak-repo.yml), in the open.

#### Per-User
```bash
flatpak --user remote-add flyingpie https://flatpak.flyingpie.nl/index.flatpakrepo
flatpak --user install nl.flyingpie.wtq
```

#### System-Wide
```bash
flatpak remote-add flyingpie https://flatpak.flyingpie.nl/index.flatpakrepo
flatpak install nl.flyingpie.wtq
```

### Direct Download Script (Linux)

> [!NOTE]
> Requires webkit2gtk-4.1 to be installed

See the [/linux/install-or-upgrade-wtq.sh script](https://github.com/flyingpie/windows-terminal-quake/blob/master/pkg/linux/install-or-upgrade-wtq.sh) that downloads the latest version of WTQ, installs it to ```~/.local/share/wtq```, and creates a **wtq.desktop** file.

As a 1-liner:
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/pkg/linux/install-or-upgrade-wtq.sh)
```

And the [/linux/uninstall-wtq.sh uninstall script](https://github.com/flyingpie/windows-terminal-quake/blob/master/pkg/linux/uninstall-wtq.sh).
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/pkg/linux/uninstall-wtq.sh)
```

> [!NOTE]
> The WTQ configuration is not removed by this script. These are usually located at ```~/.config/wtq```.

### Direct Download (Windows/Linux)

> [!NOTE]
> [Linux Only] Requires webkit2gtk-4.1 to be installed

See [the latest release](https://github.com/flyingpie/windows-terminal-quake/releases/latest), and pick a zip.

### Build From Source (Windows/Linux)

> [!NOTE]
> Requires the [.Net 9 SDK](https://dotnet.microsoft.com/en-us/download) to be installed

> [!NOTE]
> [Linux Only] Requires webkit2gtk-4.1 to be installed

You can also clone the repo and run the "Install" build target, which will build and install WTQ:
- Windows: At ```~/AppData/Local/wtq```
- Linux: At ```~/.local/share/wtq (respects XDG spec)```

```shell
git clone https://github.com/flyingpie/windows-terminal-quake.git
cd windows-terminal-quake

./build.ps1 Install
# OR
./build.sh Install
```

Uninstall:
```shell
./build.ps1 Uninstall
# OR
./build.sh Uninstall
```

You can also take a look at the build options, do see more options for building, including without actually installing:
```shell
./build.ps1|sh --help

./build.ps1|sh BuildWindows
./build.ps1|sh BuildLinux
```

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