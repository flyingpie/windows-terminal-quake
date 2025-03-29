using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Wtq.Services;
using YamlDotNet.Serialization;

namespace Wtq.Docs;

public class WtqDocsSetting
{
	private readonly WtqDocsSettingScope _scope;

	public WtqDocsSetting(PropertyInfo prop, WtqDocsSettingScope scope)
	{
		_scope = scope;
		Property = prop;

		DisplayAttr = prop.GetCustomAttribute<DisplayAttribute>();
		OrderProp = prop.GetCustomAttribute<JsonPropertyOrderAttribute>();
		_doc = prop.GetMemberDocElement();
		FlagsAttr = prop.GetCustomAttribute<DisplayFlagsAttribute>();
		DefaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
		ExampleAttr = prop.GetCustomAttribute<ExampleValueAttribute>();
		RequiredAttribute = prop.GetCustomAttribute<RequiredAttribute>();
	}

	[YamlIgnore]
	public PropertyInfo Property { get; set; }

	[YamlIgnore]
	public DisplayAttribute? DisplayAttr { get; set; }

	[YamlIgnore]
	public DisplayFlagsAttribute? FlagsAttr { get; set; }

	[YamlIgnore]
	public JsonPropertyOrderAttribute? OrderProp { get; set; }

	[YamlIgnore]
	public DefaultValueAttribute? DefaultAttr { get; set; }

	[YamlIgnore]
	public ExampleValueAttribute? ExampleAttr { get; set; }

	[YamlIgnore]
	public RequiredAttribute? RequiredAttribute { get; set; }

	public bool IsRequired => RequiredAttribute != null;

	private readonly XElement? _doc;

	public bool IsGlobal => _scope.HasFlag(WtqDocsSettingScope.Global);

	public bool IsApp => _scope.HasFlag(WtqDocsSettingScope.App);

	public WtqDocsSettingScope Scope => _scope;

	[YamlIgnore]
	public int Order => OrderProp?.Order ?? 0;

	[YamlIgnore]
	public string? GroupName => DisplayAttr?.GroupName?.EmptyOrWhiteSpaceToNull();

	public string DisplayName => DisplayAttr?.Name ?? Property.Name;

	public string Name => Property.Name;

	[YamlIgnore]
	public bool IsVisible => FlagsAttr?.IsVisible ?? true;

	private string? _descr;

	public bool IsEnum => EnumValues?.Any() ?? false;

	public object? ExampleValue
	{
		get
		{
			if (IsEnum)
			{
				return string.Join(" | ", EnumValues?.Select(e => e.Value) ?? []);
			}

			return ExampleAttr?.Value ?? DefaultValue;
		}
	}

	public object? DefaultValue => DefaultAttr?.Value;

	public string? Description
	{
		get
		{
			if (_descr == null)
			{
				var summ = _doc?.Descendants("summary")?.FirstOrDefault() ?? new XElement("summary");
				summ.Name = "div";

				_descr = summ.ToString().Replace("<div>", "").Replace("</div>", "").Replace("    ", "").Trim();
			}

			return _descr;
		}
	}

	public IEnumerable<WtqDocsEnumValue>? EnumValues
	{
		get
		{
			var propType = Nullable.GetUnderlyingType(Property.PropertyType);
			if (propType != null && propType.IsEnum)
			{
				return EnumUtils.GetValues(propType).Select(v => new WtqDocsEnumValue(v)).ToList();
			}

			return null;
		}
	}

	public bool HasExample => !string.IsNullOrWhiteSpace(Example);

	public string? Example
	{
		get
		{
			var rem = _doc?.Descendants("code")?.FirstOrDefault() ?? new XElement("code");
			var remstr = rem
					.ToString()
					.Replace("    ", "")
					.Replace("<code>", "")
					.Replace("</code>", "")
					.Replace("<code />", "") // Jesus
					.Trim()
				;

			return remstr;
		}
	}

	public string? Remarks
	{
		get
		{
			var rem = _doc?.Descendants("remarks")?.FirstOrDefault() ?? new XElement("remarks");
			var remstr = rem
					.ToString()
					.Replace("\n", "")
					.Trim()
					.Replace("    ", "")
					.Replace("<remarks>", "")
					.Replace("</remarks>", "")
					.Replace("<remarks />", "")
				;

			return remstr;
		}
	}
}