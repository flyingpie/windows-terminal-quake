# WTQ KWin Support

Current version is mostly a proof-of-concept, and since I couldn't wait for a better implementation.

## General KWin Interaction
Look into refactoring the KWin JS interop with a more async event-driven system, so we don't need to write
a JS file for each and every call we make. Preferably write one, or just a couple JS files for the whole app lifetime.

Cleanup shortcuts on app start/stop, and make the shortcut system generic (is currently hardcoded to register Ctrl+1 through Ctrl+4).
