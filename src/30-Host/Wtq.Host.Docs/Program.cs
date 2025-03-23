using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wtq.Configuration;
using Wtq.Utils;

namespace Wtq.Host.Docs;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Console.WriteLine("Hello, World!");

		var path = "/home/marco/workspace/flyingpie/wtq/wtq_3/docs/pages/gen.md";
		File.WriteAllText(path, "");

		// var props = typeof(WtqOptions).GetProperties();
		// var props = typeof(WtqAppOptions).GetProperties();
		var list = new[]
		{
			typeof(WtqOptions), typeof(WtqAppOptions)
		};

		foreach (var item in list)
		{
			File.AppendAllText(path, $"### {item.Name}");
			File.AppendAllText(path, "\n\n");

			WritePropertyListAsync(item.GetProperties());

			File.AppendAllText(path, "\n\n");
		}
	}

	private static async Task WritePropertyListAsync(ICollection<PropertyInfo> props)
	{
		var path = "/home/marco/workspace/flyingpie/wtq/wtq_3/docs/pages/gen.md";

		string? grp = null;

		foreach (var prop in props.OrderBy(p => p.GetCustomAttribute<JsonPropertyOrderAttribute>()?.Order ?? 0))
		{
			Console.WriteLine($"PROP: {prop}");

			var grp2 = prop.GetCustomAttribute<DisplayAttribute>()?.GroupName ?? string.Empty;
			if (grp != grp2)
			{
				File.AppendAllText(path, $"#### {grp2}");
				File.AppendAllText(path, "\n\n");
			}
			grp = grp2;

			var doc = prop.GetMemberDocElement();

			var summ = doc?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
			summ.Name = "div";

			var rem = doc?.Descendants("remarks")?.FirstOrDefault() ?? new XElement("remarks");
			// rem.Name = "div";

			var ex = doc?.Descendants("example")?.FirstOrDefault() ?? new XElement("example");
			ex.Name = "div";

			var disp = prop.GetCustomAttribute<DisplayAttribute>();
			var name = disp?.Name ?? prop.Name;

			File.AppendAllText(path, $"##### {name}");
			File.AppendAllText(path, "\n\n");

			File.AppendAllText(path, summ.ToString());
			File.AppendAllText(path, "\n\n");

			var remstr = rem.ToString().Replace("\n", "").Trim().Replace("    ", "").Replace("<remarks>", "!!! note\n\t").Replace("</remarks>", "");
			File.AppendAllText(path, remstr);
			File.AppendAllText(path, "\n\n");

			var code = ex.ToString().Replace("<code>", "```json").Replace("</code>", "```");
			File.AppendAllText(path, code);
			File.AppendAllText(path, "\n\n");

			File.AppendAllText(path, "---");
			File.AppendAllText(path, "\n\n");

			var x = 2;
		}
	}
}