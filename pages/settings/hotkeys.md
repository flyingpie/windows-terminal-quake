# Hotkeys

Multiple hot keys are supported, with an optional modifier.

```json
{
	// The keys that can be used to toggle the terminal
	"HotKeys": [

		// Tilde, without modifiers
		{
			"Key": "OemTilde"
		},

		// Ctrl + Q
		{
			"Modifiers": "Control",
			"Key": "Q"
		}

	]
}
```

## F12 As a Hotkey

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
