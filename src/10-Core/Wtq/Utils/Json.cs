using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Wtq.Utils;

public static class Json
{
	private static ConcurrentDictionary<Type, object> _defs = new();

	public static JsonSerializerOptions Options { get; } = new()
	{
		Converters =
		{
			new JsonStringEnumConverter(),
		},
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Causes UTF8 characters to be serialized as-is, instead of (less readable) escape codes.
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

	public static string Serialize(object obj)
	{
		return JsonSerializer.Serialize(obj, Options);
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
					if (val.Equals(xx4))
					{
						return false;
					}
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