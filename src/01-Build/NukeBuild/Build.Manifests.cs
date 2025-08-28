#pragma warning disable
// ReSharper disable All

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

/// <summary>
/// Manifests for upstream repositories.
/// </summary>
public sealed partial class Build
{
	/// <summary>
	/// Scoop manifest.
	/// </summary>
	private Target CreateAurManifest => _ => _
		.Executes(async () =>
		{
			var aur = PkgDirectory / "aur";
			var aurWtq = aur / "wtq";
			var aurWtqBin = aur / "wtq-bin";

			foreach (var dir in new[] { aurWtq, aurWtqBin })
			{
				var tpl = await File.ReadAllTextAsync(dir / "_template");
				var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

				var pkgbuild = tpl
					.Replace("$GH_RELEASE_VERSION$", GitHubRelease, StringComparison.OrdinalIgnoreCase)
					.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

				await File.WriteAllTextAsync(dir / "PKGBUILD", pkgbuild);
			}
		});

	/// <summary>
	/// Flatpak manifest.
	/// </summary>
	private Target CreateFlatpakManifest => _ => _
		.Executes(async () =>
		{
			var templateRoot = PkgDirectory / "flatpak" / "_template";
			var manifestRoot = PkgDirectory / "flatpak" / SemVerVersion;
			var prefix = "nl.flyingpie.wtq";

			if (Directory.Exists(manifestRoot)) { Directory.Delete(manifestRoot, true); }

			Directory.CreateDirectory(manifestRoot);

			var installerFn = $"{prefix}.desktop";
			var localeFn = $"{prefix}.metainfo.xml";
			var mainFn = $"{prefix}.yml";

			var fns = new string[] { installerFn, localeFn, mainFn };

			foreach (var fn in fns)
			{
				var tpl = await File.ReadAllTextAsync(templateRoot / fn);
				var target = manifestRoot / fn;

				var manifest = tpl
					.Replace("$GH_RELEASE_VERSION$", GitHubRelease, StringComparison.OrdinalIgnoreCase)
					.Replace("$GIT_COMMIT$", GitRepository.Commit, StringComparison.OrdinalIgnoreCase)
					.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$RELEASE_DATE$", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"), StringComparison.OrdinalIgnoreCase);

				await File.WriteAllTextAsync(target, manifest);
			}

			var nugetSources = Nupkg.FromNupkgDir(NupkgsDirectory);
			var nugetSourcesJson = manifestRoot / "nuget-sources.json";

			await File.WriteAllTextAsync(nugetSourcesJson, JsonSerializer.Serialize(nugetSources, Nupkg.JsonSerializerOptions));
		});

	/// <summary>
	/// Scoop manifest.
	/// </summary>
	private Target CreateScoopManifest => _ => _
		.Executes(async () =>
		{
			var scoop = PkgDirectory / "scoop";

			var tpl = await File.ReadAllTextAsync(scoop / "_template.json");
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			var manifest = tpl
				.Replace("$GH_RELEASE_VERSION$", GitHubRelease, StringComparison.OrdinalIgnoreCase)
				.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
				.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

			await File.WriteAllTextAsync(scoop / "wtq-latest.json", manifest);
			await File.WriteAllTextAsync(scoop / "wtq-nightly.json", manifest);
			await File.WriteAllTextAsync(scoop / $"wtq-{SemVerVersion}.json", manifest);

			// Keep old scoop dir on root around for now, as it's referenced by local Scoop clients.
			scoop.CopyToDirectory(RootDirectory);
		});

	/// <summary>
	/// WinGet manifest.
	/// </summary>
	private Target CreateWinGetManifest => _ => _
		.Executes(async () =>
		{
			var templateRoot = PkgDirectory / "winget" / "_template";
			var manifestRoot = PkgDirectory / "winget" / SemVerVersion;
			var prefix = "flyingpie.windows-terminal-quake";
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			if (Directory.Exists(manifestRoot)) { Directory.Delete(manifestRoot, true); }

			Directory.CreateDirectory(manifestRoot);

			var installerFn = $"{prefix}.installer.yaml";
			var localeFn = $"{prefix}.locale.en-US.yaml";
			var mainFn = $"{prefix}.yaml";

			var fns = new string[] { installerFn, localeFn, mainFn };

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