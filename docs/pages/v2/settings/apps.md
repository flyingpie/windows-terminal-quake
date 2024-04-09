# Apps

WTQ supports toggling more than 1 program at a time. The programs to toggle, are referred to as "apps".

Here's (a part of) the default configuration, that's shipped with WTQ:

```json
{
	"Apps": [
		{
			"Name": "PowerShell",
			"HotKeys": [{ "Modifiers": "Control", "Key": "D1" }],
			"FileName": "powershell"
		}
	]
}
```

The default setup uses PowerShell, to prevent requiring a dependency on Windows Terminal.

Here's a similar example, for Windows Terminal:

```json
{
	"Apps": [
		{
			"Name": "PowerShell",
			"HotKeys": [{ "Modifiers": "Control", "Key": "D1" }],
			"FileName": "powershell"
		}

		// Example with Windows Terminal
		//{
		//	"Name": "Terminal",
		//	"HotKeys": [{ "Modifiers": "Control", "Key": "D1" }],
		//	"FileName": "wt",
		//	"ProcessName": "WindowsTerminal"
		//}
	],

	// Hot keys that toggle the most recent app (or the first one if none has been active yet).
	"HotKeys": [ { "Modifiers": "Control", "Key": "Q" } ],
	"Apps": [
		{
			"Name": "Terminal",

			"HotKeys": [
				// Ctrl + 1
				{ "Modifiers": "Control", "Key": "1" }
			]
		},
		{
			"Name": "File Browser",

			"HotKeys": [
				// Ctrl + 2
				{ "Modifiers": "Control", "Key": "2" }
			]
		}
	]
}
```
