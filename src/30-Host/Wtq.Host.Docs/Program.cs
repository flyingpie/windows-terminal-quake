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
		var target2 = "/home/marco/workspace/flyingpie/wtq/wtq_2/docs/wtqsettings.yml";

		var root = new WtqDocsDoc()
		{
			WtqSettings = await GetDocsRootAsync(),
		};

		var s = new SerializerBuilder()
			.WithQuotingNecessaryStrings()
			.Build();

		await File.WriteAllTextAsync(target2, s.Serialize(root));
	}

	private static async Task<WtqDocsSettingsRoot> GetDocsRootAsync()
	{
		var root = new WtqDocsSettingsRoot()
		{
			Categories =
			[
				await GetDocsSettingsAsync2(typeof(WtqOptions), WtqDocsSettingScope.Global),
				await GetDocsSettingsAsync2(typeof(WtqAppOptions), WtqDocsSettingScope.App),
				await GetDocsSettingsAsync2(typeof(WtqSharedOptions), WtqDocsSettingScope.App | WtqDocsSettingScope.Global),
			],
		};

		return root;
	}

	private static async Task<WtqDocsSettingsCategory> GetDocsSettingsAsync2(Type item, WtqDocsSettingScope scope)
	{
		var disp = item.GetCustomAttribute<DisplayAttribute>();
		var doc = XmlDocsExtensions.GetXmlDocsSummary(item);

		var props = item
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
			.Select(p => new WtqDocsSetting(p, scope))
			.Where(p => p.IsVisible)
			.ToList();

		return new WtqDocsSettingsCategory(disp.Name, doc, props);
	}
}