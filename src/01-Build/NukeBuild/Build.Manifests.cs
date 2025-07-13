#pragma warning disable
// ReSharper disable All

using Nuke.Common;
using System;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Manifests for upstream repositories.
/// </summary>
public sealed partial class Build
{
	/// <summary>
	/// Scoop manifest.
	/// </summary>
	private Target CreateScoopManifest => _ => _
		.Executes(async () =>
		{
			var tpl = await File.ReadAllTextAsync(RootDirectory / "scoop" / "_template.json");
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			var manifest = tpl
				.Replace("$GH_RELEASE_VERSION$", GitHubRelease, StringComparison.OrdinalIgnoreCase)
				.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
				.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-latest.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-nightly.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / $"wtq-{SemVerVersion}.json", manifest);
		});

	/// <summary>
	/// WinGet manifest.
	/// </summary>
	private Target CreateWinGetManifest => _ => _
		.Executes(async () =>
		{
			var templateRoot = RootDirectory / "winget" / "_template";
			var manifestRoot = RootDirectory / "winget" / SemVerVersion;
			var prefix = "flyingpie.windows-terminal-quake";
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			if (Directory.Exists(manifestRoot)) { Directory.Delete(manifestRoot, true); }

			Directory.CreateDirectory(manifestRoot);

			var installerFn = $"{prefix}.installer.yaml";
			var localeFn = $"{prefix}.locale.en-US.yaml";
			var mainFn = $"{prefix}.yaml";

			var fns = new string[]
			{
				installerFn, localeFn, mainFn
			};

			foreach (var fn in fns)
			{
				var tpl = await File.ReadAllTextAsync(templateRoot / fn);
				var target = manifestRoot / fn;

				var manifest = tpl
					.Replace("$GH_RELEASE_VERSION$", GitHubRelease, StringComparison.OrdinalIgnoreCase)
					.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$RELEASE_DATE$", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"), StringComparison.OrdinalIgnoreCase)
					.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

				await File.WriteAllTextAsync(target, manifest);
			}
		});
}