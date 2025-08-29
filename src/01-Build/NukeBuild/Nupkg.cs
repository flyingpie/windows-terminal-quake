using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Nupkg
{
	public static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		IndentCharacter = '\t',
		IndentSize = 1,
		WriteIndented = true,
	};

	[JsonPropertyName("type")]
	public string Type { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; }

	[JsonPropertyName("sha512")]
	public string Sha512 { get; set; }

	[JsonPropertyName("dest")]
	public string Dest { get; set; }

	[JsonPropertyName("dest-filename")]
	public string DestFilename { get; set; }

	public static ICollection<Nupkg> FromNupkgDir(string pathToNupkgs) =>
		Directory
			.GetFiles(pathToNupkgs, "*.nupkg.sha512", SearchOption.AllDirectories)
			.Select(FromSha512Path)
			.OrderBy(p => p.DestFilename)
			.ToList();

	public static Nupkg FromSha512Path(string pathToNupkgSha512)
	{
		var name = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(pathToNupkgSha512)));
		var version = Path.GetFileName(Path.GetDirectoryName(pathToNupkgSha512));
		var filename = $"{name}.{version}.nupkg";
		var url = $"https://api.nuget.org/v3-flatcontainer/{name}/{version}/{filename}";

		// Convert Base64-encoded hash to hex string.
		var sha512 = Convert.ToHexString(Convert.FromBase64String(File.ReadAllText(pathToNupkgSha512))).ToLowerInvariant();

		return new Nupkg()
		{
			Type = "file",
			Url = url,
			Sha512 = sha512,
			Dest = "nuget-sources",
			DestFilename = filename,
		};
	}
}