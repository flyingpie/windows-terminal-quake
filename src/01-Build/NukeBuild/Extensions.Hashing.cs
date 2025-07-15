using Nuke.Common.IO;
using System;
using System.IO;
using System.Security.Cryptography;

public static partial class Extensions
{
	public static string GetFileHashSha256(this AbsolutePath path) =>
		Convert.ToHexString(SHA256.HashData(File.OpenRead(path))).ToLowerInvariant();
}