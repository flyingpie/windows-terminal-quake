namespace Wtq.Docs;

public class WtqDocsSettingsCategory(string name, string description, List<WtqDocsSetting> settings)
{
	public string Name => name;

	public string Description => description;

	public List<WtqDocsSettingsGroup> Groups => settings
		.GroupBy(s => s.GroupName)
		.Select(grp => new WtqDocsSettingsGroup()
		{
			Name = grp.Key,
			Settings = grp
				.OrderBy(s => s.Order)
				.ToList(),
		})
		.ToList();
}