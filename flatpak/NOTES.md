# Flatpak

- wtq.kwin.js
Working, by putting the script in the XDG cache on runtime.

- Launching apps
Use "flatpak-spawn --host dolphin" for that. And "flatpak-spawn --host flatpak run org.bleh.app".

- Unix socket (and TCP socket?)
Ability to expose unix pipe/tcp socket

- Events

Photino guide
https://github.com/tryphotino/photino.Samples/tree/master/Photino.PublishPhotino/PublishPhotino

XDG Flatpak Conventions
https://docs.flatpak.org/en/latest/conventions.html#xdg-base-directories

```shell
flatpak install org.gnome.Sdk/x86_64/48
flatpak install org.gnome.Platform/x86_64/48
```
