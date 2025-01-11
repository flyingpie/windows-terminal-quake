using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Wtq.Services;

public class WtqOptionsSaveService
{
	private static ConcurrentDictionary<Type, object> _defs = new();

	private readonly JsonSerializerOptions _opts;

	public WtqOptionsSaveService()
	{
		_opts = new()
		{
			Converters =
			{
				new JsonStringEnumConverter(),
			},
//			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,

			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,

			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
			{
				Modifiers =
				{
					DefaultValuesModifier,
				},
			},
		};
	}

	public async Task SaveAsync(WtqOptions options)
	{
		options.PrepareForSave();

		var res = JsonSerializer.Serialize(options, _opts);

		await File.WriteAllTextAsync(WtqOptionsPath.Instance.Path, res).NoCtx();
	}

	private static void DefaultValuesModifier(JsonTypeInfo typeInfo)
	{
		foreach (var propertyInfo in typeInfo.Properties)
		{
			propertyInfo.ShouldSerialize = (_, val) =>
			{
				var xx2 = typeInfo.Type;
				var xx3 = _defs.GetOrAdd(typeInfo.Type, t => Activator.CreateInstance(t)!);
				var xx4 = propertyInfo.Get(xx3);

				// Null
				if (val == null)
				{
					return false;
				}

				// Default values for value types.
				if (val.GetType().IsValueType)
				{
					// var def = Activator.CreateInstance(val.GetType());
					if (val.Equals(xx4))
					{
						return false;
					}
				}

				// [JsonDefaultValue]
				var defAttr = propertyInfo
					?.AttributeProvider
					?.GetCustomAttributes(true)
					.OfType<JsonDefaultValueAttribute>()
					.FirstOrDefault();

				if (defAttr != null)
				{
					if (defAttr.DefaultValue == null && val == null)
					{
						return false;
					}

					if (defAttr.DefaultValue?.Equals(val) ?? false)
					{
						return false;
					}

					return false;
				}

				// Lists.
				if (val is not string && val is IEnumerable asEnum && xx4 is IEnumerable origAsEnumr)
				{
					var actual = ToList(asEnum).ToList();
					var orig = ToList(origAsEnumr).ToList();

					if (actual.Count != orig.Count)
					{
						return true;
					}

					var areEqual = true;
					for (var i = 0; i < actual.Count; i++)
					{
						var a = actual[i];
						var b = orig[i];

						if (a != null && b != null && !a.Equals(b))
						{
							areEqual = false;
							break;
						}
					}

					if (areEqual)
					{
						return false;
					}
				}

				return true;
			};
		}
	}

	public static IEnumerable<object> ToList(IEnumerable asEnum)
	{
		var v1 = asEnum.GetEnumerator();
		using var v1a = v1 as IDisposable;

		while (v1.MoveNext())
		{
			yield return v1.Current;
		}
	}
}