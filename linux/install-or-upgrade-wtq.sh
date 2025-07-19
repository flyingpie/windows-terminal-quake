#!/bin/bash

# Where WTQ will be installed (I've picked a common place for this, can be changed to whatever you like).
WTQ_DIR=$(realpath ~/.local/share/wtq)

# Path to where a .desktop file will be created (follows the XDG spec, so DE's can pick it up. See https://wiki.archlinux.org/title/Desktop_entries).
APP_DIR=$(realpath ~/.local/share/applications)
WTQ_DESKTOP_FILE="$APP_DIR/wtq.desktop"

echo "Installing WTQ to directory '$WTQ_DIR'..."
echo "Creating .desktop file at '$WTQ_DESKTOP_FILE'..."

# Download WTQ.
# mkdir -p $WTQ_DIR && curl -L https://github.com/flyingpie/windows-terminal-quake/releases/download/vNext/linux-x64_self-contained.tar.gz | tar -zx -C $WTQ_DIR && chmod +x $WTQ_DIR/wtq
mkdir -p $WTQ_DIR && curl -L https://github.com/flyingpie/windows-terminal-quake/releases/latest/download/linux-x64_self-contained.tar.gz | tar -zx -C $WTQ_DIR && chmod +x $WTQ_DIR/wtq

# Create .desktop file.
mkdir -p $APP_DIR

echo "
[Desktop Entry]
Name=WTQ
Exec=env WEBKIT_DISABLE_DMABUF_RENDERER=1 $WTQ_DIR/wtq
Version=1.0
Type=Application
Categories=
Terminal=false
Icon=$WTQ_DIR/assets/icon-v2-64.png
Comment=Enable Quake-mode for (almost) any app
StartupNotify=true
" > $WTQ_DESKTOP_FILE

echo "Install complete! You should have a 'WTQ' app available from your launcher now."