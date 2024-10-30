# Apps

WTQ can toggle 1 or more **apps**.

Here's an example of WTQ toggling a PowerShell prompt:

```json
{
	"Apps": [
		{
			// A unique name for the app, used for logging.
			"Name":		"PowerShell",

			// Hotkeys that toggle this app only.
			"Hotkeys":	[{ "Modifiers": "Control", "Key": "D1" }],

			// Used to find an existing instance, or start a new one.
			"FileName":	"powershell"
		}
	]
}
```

You can define more than 1 app:

```json
{
	"Apps": [
		{
			"Name":		"App Name 1",
			"Hotkeys":	[{ "Modifiers": "Control", "Key": "D1" }], // Ctrl + 1
			...
		},
		{
			"Name":		"App Name 2",
			"Hotkeys":	[{ "Modifiers": "Control", "Key": "D2" }], // Ctrl + 2
			...
		},
		{
			"Name":		"App Name 3",
			"Hotkeys":	[{ "Modifiers": "Control", "Key": "D3" }], // Ctrl + 3
			...
		},
		...
	]
}
```

Absolute file paths work too, if the app is not on the PATH:

```json
{
	"Apps": [
		{
			"Name":			"WezTerm",
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"C:/Program Files/WezTerm/wezterm-gui.exe"
		}
	]
}
```

Here are some more app-specific properties:

```json
{
	"Apps": [
		{
			// For use in logging and error messages.
			"Name":			"Name Of The App",

			// A list of hotkeys that toggle this app, and this app only.
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],

			// The file name of the app to attach to. This is used to both find an existing instance of an already running app, and for starting a new one.
			"FileName":		"C:/Program Files/WezTerm/wezterm-gui.exe",

			// Command line arguments can be passed, only used when spawning new app instances.
			"Arguments":	"--argument1=value1",

			// For some apps, the initially started process does not equal the ultimately running process name.
			// Such as for Windows Terminal, which is started using "wt.exe", but then runs using "WindowsTerminal.exe".
			// In such cases, the "FileName" can't be used to find existing processes.
			// The "ProcessName" property can be used to override the process name that is used when looking for existing processes.
			// By default, the "FileName" property is used for this, and "ProcessName" remains null.
			"ProcessName":	null,
		}
	]
}
```

## Examples

### Windows Terminal

*Assuming **wt.exe** to be on the PATH, eg. can be run through Win+R.*

```json
{
	"Apps": [
		{
			"Name":			"Windows Terminal",
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"wt",
			"ProcessName":	"WindowsTerminal"
		}
	]
}
```

Windows Terminal has a particular behavior where the initial "wt.exe" starts another process, called "WindowsTerminal". Currently, we can't use the same filter to both start a new instance, and find an existing one, so we need to define both separately.

[Windows Terminal home page](https://learn.microsoft.com/en-us/windows/terminal/install)

### WezTerm

```json
{
	"Apps": [
		{
			"Name":			"WezTerm",
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"wezterm-gui"
		}
	]
}
```

[WezTerm home page](https://wezfurlong.org/wezterm/index.html)

### Alacritty

```json
{
	"Apps": [
		{
			"Name":			"Alacritty",
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"alacritty"
		}
	]
}
```

[Alacritty home page](https://alacritty.org/)

### Double Commander

```json
{
	"Apps": [
		{
			"Name":			"Double Commander",
			"Hotkeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"doublecmd"
		}
	]
}
```

[Double Commander home page](https://doublecommander.com/)
