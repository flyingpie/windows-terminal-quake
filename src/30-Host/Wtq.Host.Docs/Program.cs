using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

		// var path = "/home/marco/workspace/flyingpie/wtq/wtq_3/docs/pages/gen.md";
		var tpl = "/home/marco/workspace/flyingpie/wtq/wtq_2/docs/pages/index.tpl.md";
		var tplStr = File.ReadAllText(tpl);
		var target = "/home/marco/workspace/flyingpie/wtq/wtq_2/docs/pages/index.md";

		// File.WriteAllText(path, "");

		// await using var wr = new StreamWriter(File.OpenWrite(path));
		var wr = new StringWriter();

		// var props = typeof(WtqOptions).GetProperties();
		// var props = typeof(WtqAppOptions).GetProperties();
		var list = new[]
		{
			typeof(WtqOptions), typeof(WtqAppOptions), typeof(WtqSharedOptions),
		};

		await NewMethod(typeof(WtqOptions), wr, SettingPropertyInfo.Scope.Global);
		await NewMethod(typeof(WtqAppOptions), wr, SettingPropertyInfo.Scope.App);
		await NewMethod(typeof(WtqSharedOptions), wr, SettingPropertyInfo.Scope.App | SettingPropertyInfo.Scope.Global);

		File.WriteAllText(target, tplStr.Replace("{{TEMPLATE__SETTINGS}}", wr.ToString()));
	}

	private static async Task NewMethod(Type item, StringWriter wr, SettingPropertyInfo.Scope scope)
	{
		var disp = item.GetCustomAttribute<DisplayAttribute>();
		var doc = XmlDocsExtensions.GetXmlDocsSummary(item);
		// var doc = AttrUtils.GetMemberDocElement()

		// File.AppendAllText(path, $"### {item.Name}");
		// File.AppendAllText(path, "\n\n");
		await wr.WriteLineAsync($"### {disp?.Name ?? item.Name}");
		await wr.WriteLineAsync(doc);

		var props = item
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
			.Select(p => new SettingPropertyInfo(p, scope))
			.Where(p => p.IsVisible)
			.ToList();

		await WritePropertyListAsync(props, wr);

		// File.AppendAllText(path, "\n\n");
		await wr.WriteLineAsync();
	}

	private static async Task WritePropertyListAsync(ICollection<SettingPropertyInfo> props, StringWriter wr)
	{
		// var path = "/home/marco/workspace/flyingpie/wtq/wtq_3/docs/pages/gen.md";

		// string? grp = null;

		foreach (var group in props.GroupBy(p => p.GroupName))
		{
			// File.AppendAllText(path, $"#### {grp2}");
			await wr.WriteLineAsync($"#### {group.Key}");


			foreach (var prop in group.OrderBy(p => p.Order))
			{
				// Console.WriteLine($"PROP: {prop}");

				// var grp2 = prop.GetCustomAttribute<DisplayAttribute>()?.GroupName ?? string.Empty;
				// if (grp != grp2)
				// {
				// 	File.AppendAllText(path, $"#### {grp2}");
				// 	File.AppendAllText(path, "\n\n");
				// }

				// var doc = prop.GetMemberDocElement();

				// var summ = doc?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
				// summ.Name = "div";

				// var rem = doc?.Descendants("remarks")?.FirstOrDefault() ?? new XElement("remarks");
				// rem.Name = "div";

				// var ex = doc?.Descendants("example")?.FirstOrDefault() ?? new XElement("example");
				// ex.Name = "div";

				// var disp = prop.GetCustomAttribute<DisplayAttribute>();
				// var name = disp?.Name ?? prop.Name;

				await wr.WriteLineAsync($"##### {prop.Name}");
				await wr.WriteLineAsync();
				// File.AppendAllText(path, "\n\n");

				if (prop.IsRequired)
				{
					await wr.WriteLineAsync("**REQUIRED**");
					await wr.WriteLineAsync();
				}

				await wr.WriteLineAsync(prop.Summary);
				await wr.WriteLineAsync();
				// File.AppendAllText(path, "\n\n");

				if (prop.DefaultAttr?.Value != null)
				{
					await wr.WriteLineAsync($"Defaults to ```{prop.DefaultAttr?.Value}```");
					await wr.WriteLineAsync();
				}

				// Enum values ///////////////////
				var propType = Nullable.GetUnderlyingType(prop.Property.PropertyType);
				if (propType != null && propType.IsEnum)
				{
					var enumVals = EnumUtils.GetValues(propType);

					foreach (var enumVal in enumVals)
					{
						// var summ = enumVal.GetMemberDocEnumElement(enumVal.Value.GetType())?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
						// summ.Name = "div";
						var summ = enumVal.Doc;
						var xxx = enumVal.DocElement?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
						xxx.Name = "div";

						await wr.WriteLineAsync($"- **{enumVal.DisplayName}** {xxx.ToString()}");
						await wr.WriteLineAsync();
					}

					var dbg = 2;
				}

				// var remstr = rem.ToString().Replace("\n", "").Trim().Replace("    ", "").Replace("<remarks>", "!!! note\n\t").Replace("</remarks>", "");
				await wr.WriteLineAsync(prop.Remarks);
				await wr.WriteLineAsync();
				// File.AppendAllText(path, "\n\n");


				// Example ////////////////////////////
				// var code = ex.ToString().Replace("<code>", "```json").Replace("</code>", "```");
				await wr.WriteLineAsync(prop.Example);
				await wr.WriteLineAsync();
				// File.AppendAllText(path, "\n\n");

				// Ruler //////////////////////////////
				await wr.WriteLineAsync("---");
				await wr.WriteLineAsync();
				// File.AppendAllText(path, "\n\n");

				var x = 2;
			}

			// await wr.WriteLineAsync("---");
			// await wr.WriteLineAsync("---");
		}
	}
}

public class SettingPropertyInfo
{
	private readonly Scope _scope;

	[Flags]
	public enum Scope
	{
		None = 0,
		Global = 2,
		App = 4,
	}

	public SettingPropertyInfo(PropertyInfo prop, Scope scope)
	{
		_scope = scope;
		Property = prop;

		DisplayAttr = prop.GetCustomAttribute<DisplayAttribute>();
		OrderProp = prop.GetCustomAttribute<JsonPropertyOrderAttribute>();
		Doc = prop.GetMemberDocElement();
		FlagsAttr = prop.GetCustomAttribute<DisplayFlagsAttribute>();
		DefaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
		ExampleAttr = prop.GetCustomAttribute<ExampleValueAttribute>();
		RequiredAttribute = prop.GetCustomAttribute<RequiredAttribute>();
	}

	public PropertyInfo Property { get; set; }

	public DisplayAttribute? DisplayAttr { get; set; }

	public DisplayFlagsAttribute? FlagsAttr { get; set; }

	public JsonPropertyOrderAttribute? OrderProp { get; set; }

	public DefaultValueAttribute? DefaultAttr { get; set; }

	public ExampleValueAttribute? ExampleAttr { get; set; }

	public RequiredAttribute? RequiredAttribute { get; set; }

	public bool IsRequired => RequiredAttribute != null;

	public XElement? Doc { get; set; }

	public int Order => OrderProp?.Order ?? 0;

	public string? GroupName => DisplayAttr?.GroupName?.EmptyOrWhiteSpaceToNull();

	public string Name => DisplayAttr?.Name ?? Property.Name;

	public bool IsVisible => FlagsAttr?.IsVisible ?? true;

	public IEnumerable<EnumValue>? EnumValues
	{
		get
		{
			var propType = Nullable.GetUnderlyingType(Property.PropertyType);
			if (propType != null && propType.IsEnum)
			{
				return EnumUtils.GetValues(propType);
			}

			return null;
		}
	}

	public string Example
	{
		get
		{
			var ex = Doc?.Descendants("example")?.FirstOrDefault();
			if (ex != null)
			{
				ex.Name = "div";
				var code = ex.ToString().Replace("<code>", "```json").Replace("</code>", "```").Replace("    ", "");

				return code;
			}

			var values = DefaultAttr?.Value?.ToString() ?? string.Empty;

			var enumVals = EnumValues;
			if (enumVals != null)
			{
				values = string.Join(" | ", enumVals.Select(v => v.Value));
			}

			if (ExampleAttr != null)
			{
				values = ExampleAttr.Value;
			}

			var example1 = new StringBuilder();

			if (_scope.HasFlag(Scope.Global))
			{
				example1.AppendLine($$"""
					Globally:
					```json
					{
						"{{Property.Name}}": "{{values}}",
						// ...
					}
					```
					""");
			}

			if (_scope.HasFlag(Scope.App))
			{
				example1.AppendLine($$"""
					For a single app:
					```json
					{
						"Apps": [
							{
								"{{Property.Name}}": "{{values}}",
								// ...
							}
						]
					}
					```
					""");
			}

			return example1.ToString();
		}
	}

	public string? Remarks
	{
		get
		{
			var rem = Doc?.Descendants("remarks")?.FirstOrDefault() ?? new XElement("remarks");
			var remstr = rem.ToString().Replace("\n", "").Trim().Replace("    ", "").Replace("<remarks>", "!!! note\n\t").Replace("</remarks>", "");
			return remstr;
		}
	}

	public string? Summary
	{
		get
		{
			var summ = Doc?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
			summ.Name = "div";

			return summ.ToString().Replace("<div>", "").Replace("</div>", "").Replace("    ", "").Trim();
		}
	}
}