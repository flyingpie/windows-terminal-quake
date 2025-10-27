#!/bin/bash

# Make a folder for the WTQ binaries to live, and go there.
WTQ_DIR=~/.local/share/wtq

echo "Removing WTQ app directory '$WTQ_DIR'..."
rm -rf ~/.local/share/wtq

echo "Removing WTQ .desktop file"
rm -f ~/.local/share/applications/wtq.desktop

echo "Uninstall complete!"
