using System;
using System.IO;

public static class InstallConstants
{
	public static class Linux
	{
		/// <summary>
		/// Defaults to "/home/username/.local/share".
		/// </summary>
		public static string XdgDataHome
		{
			get
			{
				var envVar = Environment.GetEnvironmentVariable("XDG_DATA_HOME");

				return !string.IsNullOrWhiteSpace(envVar)
					? envVar
					: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
			}
		}

		public static string PathToInstall =>
			Path.Combine(XdgDataHome, "wtq");

		public static string PathToDesktopFile =>
			Path.Combine(XdgDataHome, "applications", "wtq.desktop");

		public static string DesktopFile =>
			$"""
			[Desktop Entry]
			Name=WTQ
			Exec=env WEBKIT_DISABLE_DMABUF_RENDERER=1 {PathToInstall}/wtq
			Version=1.0
			Type=Application
			Categories=
			Terminal=false
			Icon={PathToInstall}/assets/icon-v2-64.png
			Comment=Enable Quake-mode for (almost) any app
			StartupNotify=true
			""";
	}

	public static class Windows
	{
		public static string PathToInstall =>
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wtq");

		public static string PathToShortcut =>
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "WTQ.lnk");
	}
}