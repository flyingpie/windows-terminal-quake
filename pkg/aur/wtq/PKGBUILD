# Maintainer: Marco van den Oever <arch@flyingpie.nl>
pkgname=wtq
pkgver=2.0.18
pkgrel=1
pkgdesc="Enable Quake-style dropdown for (almost) any application."
arch=("x86_64")
url="https://github.com/flyingpie/windows-terminal-quake"
license=("MIT")
depends=(
	"libappindicator-gtk3"
	"webkit2gtk-4.1"
)
makedepends=("dotnet-sdk>=9")
provides=()
conflicts=()
source=(
	"git+https://github.com/flyingpie/windows-terminal-quake.git#tag=v2.0.18"
)
noextract=()
sha256sums=(
	"SKIP"
)
validpgpkeys=()

build() {
	pushd windows-terminal-quake

	# disable dotnet telemetry
	export DOTNET_CLI_TELEMETRY_OPTOUT=1
	export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
	export DOTNET_NOLOGO=1

	./build.sh BuildLinuxSelfContained

	popd
}

package() {
	# Set target paths
	app_dir="$pkgdir/opt/wtq"
	app_bin="$pkgdir/opt/wtq/wtq"

	install -Ddm755 $app_dir # Create install dir
	cp -R $srcdir/windows-terminal-quake/_output/staging/linux-x64_self-contained/* $app_dir # Copy app files to install dir
	chmod -R 755 $app_dir

	# Create symlink in /usr/bin
	install -Ddm755 "$pkgdir/usr/bin"
	ln -s "/opt/wtq/wtq" "$pkgdir/usr/bin/wtq"

	# Create .desktop file
	install -d "$pkgdir/usr/share/applications"
	cat >"$pkgdir/usr/share/applications/wtq.desktop" <<EOF
[Desktop Entry]
Name=WTQ
Exec=env WEBKIT_DISABLE_DMABUF_RENDERER=1 /opt/wtq/wtq
Version=1.0
Type=Application
Categories=
Terminal=false
Icon=/opt/wtq/assets/icon-v2-64.png
Comment=Enable Quake-mode for (almost) any app
StartupNotify=true
EOF
}