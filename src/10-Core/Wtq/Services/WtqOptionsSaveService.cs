using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Wtq.Services;

public sealed class DontSaveWhenEmptyAttribute : Attribute
{
}

public class JsonDefaultValueJsonConverterFactory : JsonConverterFactory
{
	/// <summary>
	///
	/// </summary>
	/// <param name="typeToConvert"></param>
	/// <returns></returns>
	public override bool CanConvert(Type typeToConvert)
	{
		// return typeToConvert.GetCustomAttribute<JsonDefaultValueAttribute>() != null;
		return typeToConvert == typeof(WtqOptions) || typeToConvert == typeof(WtqAppOptions);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="typeToConvert"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		JsonConverter converter = (JsonConverter)Activator.CreateInstance(
			typeof(JsonDefaultValueJsonConverter).MakeGenericType(
				new Type[]
				{
					typeToConvert
				}),
			BindingFlags.Instance | BindingFlags.Public,
			binder: null,
			args: null,
			culture: null)!;

		return converter;
	}
}

public class JsonDefaultValueJsonConverter : JsonConverter<object>
{


	// public override bool CanConvert(Type typeToConvert) => true;



	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<int>(ref reader, options);
	}

	public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		foreach (var property in value.GetType().GetProperties())
		{
			var propValue = property.GetValue(value);

			var attr = property.GetCustomAttribute<JsonDefaultValueAttribute>();
			if (attr != null && propValue != null && propValue.Equals(attr.DefaultValue))
			{
				continue;
			}

			switch (true)
			{
				case true when propValue is not null:
				case true when propValue is null && options.DefaultIgnoreCondition == JsonIgnoreCondition.Never:
					writer.WritePropertyName(property.Name);
					JsonSerializer.Serialize(writer, propValue, options);
					break;
			}
		}

		writer.WriteEndObject();
	}
}

public class JsonDefaultValueAttribute(object? defaultValue) : Attribute
{
	public object? DefaultValue { get; } = defaultValue;
}

public class WtqOptionsSaveService
{
	private readonly JsonSerializerOptions _opts;

	public WtqOptionsSaveService()
	{
		_opts = new()
		{
			Converters =
			{
				// new JsonDefaultValueJsonConverterFactory(),
				new JsonStringEnumConverter(),
			},
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			//PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
			IndentCharacter = '\t',
			IndentSize = 1,
			WriteIndented = true,

			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
			{
				// This modifier will suppress empty lists
				Modifiers =
				{
					DefaultValues,
					IgnoreEmptyLists,
				},
			},
		};

		// _opts.Converters.Add(new JsonDefaultValueJsonConverterFactory());
	}

	public async Task SaveAsync(WtqOptions options)
	{
		var res = JsonSerializer.Serialize(options, _opts);

		await File.WriteAllTextAsync(WtqOptionsPath.Instance.Path, res).NoCtx();
	}

	private static ConcurrentDictionary<Type, object> _defs = new();

	private static void IgnoreEmptyLists(JsonTypeInfo typeInfo)
	{
		// var def1 = typeInfo.CreateObject();

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
					var actual = ToList1(asEnum).ToList();
					var orig = ToList1(origAsEnumr).ToList();

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



					// if (!hasItems)
					// {
					// 	return false;
					// }
				}

				return true;
			};
		}
	}

	public static IEnumerable<object> ToList1(IEnumerable asEnum)
	{
		var v1 = asEnum.GetEnumerator();
		using var v1a = v1 as IDisposable;

		while (v1.MoveNext())
		{
			yield return v1.Current;
		}
	}

	private static void DefaultValues(JsonTypeInfo typeInfo)
	{
		foreach (var propertyInfo in typeInfo.Properties)
		{
			var def = propertyInfo.PropertyType.GetCustomAttribute<JsonDefaultValueAttribute>();

			propertyInfo.ShouldSerialize = (_, val) =>
			{
				if (def == null)
				{
					return true;
				}

				var xx = 2;

				// var enumr = asEnum.GetEnumerator();
				// using var enumr1 = enumr as IDisposable;
				// var hasItems = enumr.MoveNext();
				//
				// return hasItems;
				return true;
			};
		}
	}
}