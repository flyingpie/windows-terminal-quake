# Flatpak

- Tray icon
Not fixed, figure out where to put the file and how to properly reference it.
Got a version working where a totally unrelated icon was shown, and one where a "I don't know what icon you're talking about"-icon was shown.

- wtq.kwin.js
Working, by putting the script in the XDG cache on runtime.

- Launching apps
Use "flatpak-spawn --host dolphin" for that. And "flatpak-spawn --host flatpak run org.bleh.app".

Photino guide
https://github.com/tryphotino/photino.Samples/tree/master/Photino.PublishPhotino/PublishPhotino

XDG Flatpak Conventions
https://docs.flatpak.org/en/latest/conventions.html#xdg-base-directories

```shell
flatpak install org.gnome.Sdk/x86_64/48
flatpak install org.gnome.Platform/x86_64/48

```
