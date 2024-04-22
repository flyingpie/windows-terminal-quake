# Apps

WTQ can toggle 1 or more **apps**.

Here's an example of WTQ toggling a PowerShell prompt:

```json
{
	"Apps": [
		{
			// A unique name for the app, used for logging.
			"Name":		"PowerShell",

			// Hot keys that toggle this app only.
			"HotKeys":	[{ "Modifiers": "Control", "Key": "D1" }],

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
			"HotKeys":	[{ "Modifiers": "Control", "Key": "D1" }], // Ctrl + 1
			...
		},
		{
			"Name":		"App Name 2",
			"HotKeys":	[{ "Modifiers": "Control", "Key": "D2" }], // Ctrl + 2
			...
		},
		{
			"Name":		"App Name 3",
			"HotKeys":	[{ "Modifiers": "Control", "Key": "D3" }], // Ctrl + 3
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
			"HotKeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"C:/Program Files/WezTerm/wezterm-gui.exe"
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
			"HotKeys":		[{ "Modifiers": "Control", "Key": "D1" }],
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
			"HotKeys":		[{ "Modifiers": "Control", "Key": "D1" }],
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
			"HotKeys":		[{ "Modifiers": "Control", "Key": "D1" }],
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
			"HotKeys":		[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":		"doublecmd"
		}
	]
}
```

[Double Commander home page](https://doublecommander.com/)
