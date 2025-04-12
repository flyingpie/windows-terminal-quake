using System.Reflection;
using YamlDotNet.Serialization;

namespace Wtq.Docs;

public class WtqDocsSetting(PropertyInfo prop, WtqDocsSettingScope scope)
{
	private readonly DisplayAttribute? _displayAttr
		= prop.GetCustomAttribute<DisplayAttribute>();

	private readonly DisplayFlagsAttribute? _dispFlagsAttr
		= prop.GetCustomAttribute<DisplayFlagsAttribute>();

	private readonly JsonPropertyOrderAttribute? _orderAttr
		= prop.GetCustomAttribute<JsonPropertyOrderAttribute>();

	private readonly DefaultValueAttribute? _defaultValAttr
		= prop.GetCustomAttribute<DefaultValueAttribute>();

	private readonly ExampleValueAttribute? _exampleValAttr
		= prop.GetCustomAttribute<ExampleValueAttribute>();

	private readonly RequiredAttribute? _reqAttr
		= prop.GetCustomAttribute<RequiredAttribute>();

	#region Only used during preperation

	[YamlIgnore]
	public int Order => _orderAttr?.Order ?? 0;

	[YamlIgnore]
	public string? GroupName => _displayAttr?.GroupName?.EmptyOrWhiteSpaceToNull();

	[YamlIgnore]
	public bool IsVisible => _dispFlagsAttr?.IsVisible ?? true;

	#endregion

	public string DisplayName => _displayAttr?.Name ?? prop.Name;

	public string SettingName => prop.Name;

	public string? Description => XmlDocUtils.GetSummary(prop);

	public bool IsGlobal => scope.HasFlag(WtqDocsSettingScope.Global);

	public bool IsApp => scope.HasFlag(WtqDocsSettingScope.App);

	public bool IsEnum => EnumValues?.Any() ?? false;

	public bool IsRequired => _reqAttr != null;

	public object? DefaultValue => _defaultValAttr?.Value;

	public object? ExampleValue => _exampleValAttr?.Value ?? DefaultValue;

	public IEnumerable<WtqDocsEnumValue>? EnumValues
	{
		get
		{
			var propType = Nullable.GetUnderlyingType(prop.PropertyType);
			if (propType != null && propType.IsEnum)
			{
				return EnumUtils.GetValues(propType).Select(v => new WtqDocsEnumValue(v)).ToList();
			}

			return null;
		}
	}

	public bool HasExample => !string.IsNullOrWhiteSpace(Example);

	public string? Example => XmlDocUtils.GetExample(prop);

	public string? Remarks => XmlDocUtils.GetRemarks(prop);
}