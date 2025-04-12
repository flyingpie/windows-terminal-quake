using Namotion.Reflection;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Docs;
using YamlDotNet.Serialization;

namespace Wtq.Host.Docs;

public static class Program
{
	public static async Task Main(string[] args)
	{
		// First argument should be a file path to where we should write the metadata.
		var pathToWtqSettingsYaml = args.FirstOrDefault();
		if (string.IsNullOrWhiteSpace(pathToWtqSettingsYaml))
		{
			Console.WriteLine("Usage: wtq.host.docs (path to wtqsettings.yml)");
			return;
		}

		// Make sure the path directory exists, the prevent accidental invalid use.
		var dir = Path.GetDirectoryName(pathToWtqSettingsYaml);
		if (!Directory.Exists(dir))
		{
			Console.WriteLine($"Directory '' does not exist, are you sure the path to the yml file ({pathToWtqSettingsYaml}) is correct?");
			return;
		}

		// Build a metadata tree.
		var root = new WtqDocsDoc()
		{
			WtqSettings = CreateDocsSettingsRoot(),
		};

		// Serialize to YAML.
		var s = new SerializerBuilder()
			.WithQuotingNecessaryStrings()
			.Build();

		// Write the file.
		await File.WriteAllTextAsync(pathToWtqSettingsYaml, s.Serialize(root));
	}

	private static WtqDocsSettingsRoot CreateDocsSettingsRoot() =>
		new()
		{
			Categories =
			[
				CreateDocsSettingsCategory(typeof(WtqOptions), WtqDocsSettingScope.Global),
				CreateDocsSettingsCategory(typeof(WtqAppOptions), WtqDocsSettingScope.App),
				CreateDocsSettingsCategory(typeof(WtqSharedOptions), WtqDocsSettingScope.App | WtqDocsSettingScope.Global),
			],
		};

	private static WtqDocsSettingsCategory CreateDocsSettingsCategory(Type item, WtqDocsSettingScope scope)
	{
		var disp = item.GetCustomAttribute<DisplayAttribute>() ?? throw new InvalidOperationException($"Type '{item}' is missing a [Display] attribute.");
		if (string.IsNullOrWhiteSpace(disp.Name))
		{
			throw new InvalidOperationException($"Type '{item}' [Display] attribute is missing a 'Name' property.");
		}

		var doc = XmlDocsExtensions.GetXmlDocsSummary(item);

		var props = item
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
			.Select(p => new WtqDocsSetting(p, scope))
			.Where(p => p.IsVisible)
			.ToList();

		return new WtqDocsSettingsCategory(disp.Name, doc, props);
	}
}