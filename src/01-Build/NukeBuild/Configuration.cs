using Nuke.Common.Tooling;
using System.ComponentModel;

[TypeConverter(typeof(TypeConverter<Configuration>))]
#pragma warning disable S3903 // Types should be defined in named namespaces // MvdO: NukeBuild convention.
public class Configuration : Enumeration
#pragma warning restore S3903 // Types should be defined in named namespaces
{
	public static readonly Configuration Debug = new Configuration { Value = nameof(Debug) };
	public static readonly Configuration Release = new Configuration { Value = nameof(Release) };

	public static implicit operator string(Configuration configuration)
	{
		return configuration.Value;
	}
}