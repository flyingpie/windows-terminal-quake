using System.Collections;
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
			typeof(JsonDefaultValueJsonConverter<>).MakeGenericType(
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

public class JsonDefaultValueJsonConverter<T> : JsonConverter<T>
{
	// public override bool CanConvert(Type typeToConvert) => true;

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<T>(ref reader, options);
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		Console.WriteLine($"WRITE:{value}");

		// if (value is not null)
		{
			writer.WriteStartObject();

			foreach (var property in value.GetType().GetProperties())
			{

				var propValue = property.GetValue(value);
				Console.WriteLine($"WRITE:{value}:{property.Name}.{propValue}");

				var attr = property.GetCustomAttribute<JsonDefaultValueAttribute>();
				if (attr != null && propValue != null && propValue.Equals(attr.Default))
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
}

public class JsonDefaultValueAttribute : Attribute
{
	public object? Default { get; set; }
}

public class WtqOptionsSaveService
{
	private static readonly JsonSerializerOptions _opts = new()
	{
		Converters =
		{
			new JsonDefaultValueJsonConverterFactory(), new JsonStringEnumConverter(),
		},
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		IndentCharacter = '\t',
		IndentSize = 1,
		WriteIndented = true,
		TypeInfoResolver = new DefaultJsonTypeInfoResolver()
		{
			// This modifier will suppress empty lists
			Modifiers =
			{
				IgnoreEpmptyListOfStrings,
			},
		},
	};

	public async Task SaveAsync(WtqOptions options)
	{
		var res = JsonSerializer.Serialize(options, _opts);

		await File.WriteAllTextAsync(WtqOptionsPath.Instance.Path, res).NoCtx();
	}

	private static void IgnoreEpmptyListOfStrings(JsonTypeInfo typeInfo)
	{
		var listOfStringProperties = typeInfo.Properties

			//.Where(p => p.PropertyType.IsAssignableTo(typeof(IEnumerable)))
			;

		foreach (var propertyInfo in listOfStringProperties)
		{
			propertyInfo.ShouldSerialize = (_, val) =>
			{
				if (val is not IEnumerable asEnum)
				{
					return false;
				}

				var enumr = asEnum.GetEnumerator();
				using var enumr1 = enumr as IDisposable;
				var hasItems = enumr.MoveNext();

				return hasItems;
			};
		}
	}
}