using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wtq.Utils;

namespace Wtq.Services.KWin.Dto;

public class KWinSupportInformation
{
	private static readonly ILogger _log = Log.For<KWinSupportInformation>();

	public required ICollection<KWinScreenInfo> Screens { get; init; }

	public static KWinSupportInformation Parse(string supportInfoStr)
	{
		Guard.Against.Null(supportInfoStr);

		using var reader = new StringReader(supportInfoStr);

		var headerRegex = new Regex("^screen (?<index>[0-9]+):$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		var nameRegex = new Regex("^name: (?<name>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		var enabledRegex = new Regex("^enabled: (?<enabled>[0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		var geometryRegex = new Regex("^geometry: (?<x>[0-9]+),(?<y>[0-9]+),(?<w>[0-9]+)x(?<h>[0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		KWinScreenInfo scr = null;
		List<KWinScreenInfo> scrs = [];

		//var inScreens = false;

		while (true)
		{
			// If we hit a 'null', we're at the end of the stream.
			var line = reader.ReadLine()?.Trim();
			if (line == null)
			{
				break;
			}

			if (string.IsNullOrWhiteSpace(line))
			{
				// break;
			}

			var headerMatch = headerRegex.Match(line);
			if (headerMatch.Success)
			{
				scr = new KWinScreenInfo();
				scrs.Add(scr);
			}

			var nameMatch = nameRegex.Match(line);
			if (nameMatch.Success)
			{
				if (scr != null)
				{
					scr.Name = nameMatch.Groups["name"].Value;
				}
			}

			var geometryMatch = geometryRegex.Match(line);
			if (geometryMatch.Success)
			{
				var xStr = geometryMatch.Groups["x"].ValueSpan;
				int.TryParse(xStr, out var xInt);

				var yStr = geometryMatch.Groups["y"].ValueSpan;
				int.TryParse(yStr, out var yInt);

				var wStr = geometryMatch.Groups["w"].ValueSpan;
				int.TryParse(wStr, out var wInt);

				var hStr = geometryMatch.Groups["h"].ValueSpan;
				int.TryParse(hStr, out var hInt);

				if (scr != null)
				{
					scr.Geometry = new Rectangle(xInt, yInt, wInt, hInt);
				}
			}

			var dbg = 2;

			// line = reader.ReadLine()?.Trim();
		}

		var res = new KWinSupportInformation()
		{
			Screens = scrs,
		};

		return res;
	}
}

public class KWinScreenInfo
{
	public string? Name { get; set; }

	public bool IsEnabled { get; set; }

	public Rectangle Geometry { get; set; }
}