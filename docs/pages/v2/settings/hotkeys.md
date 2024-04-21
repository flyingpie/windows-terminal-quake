# Hotkeys

Configures the keys used to open and close an app.

Hot keys are supported on a global basis, and per app.
Multiple hot keys are supported, with an optional modifier.

## Global
```json
{
	"HotKeys": [
		// Tilde, without modifiers
		{ "Key": "OemTilde" },

		// Ctrl + Q
		{ "Modifiers": "Control", "Key": "Q" },

		// Multiple modifiers
		{ "Modifiers": "Control,Shift", "Key": "W" }
	]
}
```

## Per App
```json
{
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

## Windows Key + Tilde as a Hot Key

When using the Windows key + tilde as a hot key, while toggling Windows Terminal, the default shortcut for Windows Terminal's own Quake mode may interfere.

You can turn off the built-in Quake mode, by unmapping the key:

![WT Hot Key](../../assets/img/wt-hotkey.png)

## F12 as a Hot Key

When using "F12" as a hotkey, it may be necessary to disable the key as a shortcut for the system-wide debugger first.

Since F12 is the default for the debugger, it won't respond when used in another app, such as Windows Terminal Quake, or ConEmu.

This can be done by changing the registry key at:

```
Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AeDebug\UserDebuggerHotKey
```.

This key defaults to ```0```, which means ```F12```.

As pointed out by the ConEmu docs, it can for example be changed to the "Pause"-key (value ```13```).

A reboot may also be required, after which F12 can be used as a hotkey for toggling.

Thanks to [Luca](https://github.com/lpuglia) for pointing this out on GitHub.

See also:

- [MSDN docs](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey?redirectedfrom=MSDN).

- [ConEmu explanation](https://conemu.github.io/en/GlobalHotKeys.html#Using_F12_as_global_hotkey)
