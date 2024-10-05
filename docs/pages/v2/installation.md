# Getting Started

## Direct Download (Recommended)

!!! note "Recommended"
	Direct download is currently the recommended way of using WTQ v2, due to it receiving updates the soonest.

- Go to the [GitHub Releases page](https://github.com/flyingpie/windows-terminal-quake/releases) for a recent build;
- Download the desired version:
	- **Self Contained** builds are larger, but don't require any dependencies;
	- **Framework Dependent** builds are smaller, but require the [.Net 8 runtime](https://get.dot.net) to be installed.
- Extract the archive;
- Launch **wtq.exe** to start the program;
- Edit **wtq.jsonc**, which contains the configuration.

## Get From WinGet

Download from [WinGet](https://learn.microsoft.com/en-us/windows/package-manager/):

```cmd
winget install windows-terminal-quake
```

The manifests can be found [here](https://github.com/microsoft/winget-pkgs/tree/master/manifests/f/flyingpie/windows-terminal-quake).

## Get From Scoop

Download from [scoop](https://scoop.sh/):

```cmd
scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/wtq-latest.json
```

## Build (From Command Line)

!!! note "Requirements"
	Requires .Net 8 SDK, which can be downloaded from [https://get.dot.net](https://get.dot.net)

Clone the repo:

```cmd
git clone https://github.com/flyingpie/windows-terminal-quake.git
```

Go to the "src" directory:
```cmd
cd windows-terminal-quake/src
```

Run the Nuke build script:
```cmd
./build.ps1
```

The build output is under **~/_output/artifacts**.

## Build (Using Visual Studio)

Clone the repo:

```cmd
git clone https://github.com/flyingpie/windows-terminal-quake.git
```

Open the solution file:

```cmd
src/Wtq.sln
```

Build the solution (by default Ctrl + Shift + B).

The build output is under **~/_output/build/bin/Wtq.Windows/net8.0-windows**.
