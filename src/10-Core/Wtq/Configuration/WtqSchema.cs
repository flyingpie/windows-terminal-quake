using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using System.Text.Json.Serialization;

namespace Wtq.Configuration;

public static class WtqSchema
{
	public static void WriteFor(string pathToWtqJson)
	{
		var dir = Path.GetDirectoryName(pathToWtqJson)!;
		var dst = Path.Combine(dir, "wtq.schema.json");

		var schema = JsonSchema.FromType<WtqOptions>(
			new SystemTextJsonSchemaGeneratorSettings()
			{
				SerializerOptions =
				{
					Converters =
					{
						new JsonStringEnumConverter(),
					},
				},
			});

		var schemaData = schema.ToJson(Formatting.Indented);

		File.WriteAllText(dst, schemaData);
	}
}