using Wtq.Services;

namespace Wtq.Utils;

/// <summary>
/// Returns a path to a tray icon, based on both the <see cref="WtqOptions.TrayIconStyle"/>
/// and <see cref="IPlatformService.OsColorMode"/>.
/// </summary>
public class TrayIconUtil(
	IOptions<WtqOptions> opts,
	IPlatformService platform)
{
	private readonly IOptions<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IPlatformService _platform = Guard.Against.Null(platform);

	public string TrayIconPath
	{
		get
		{
			switch (_opts.Value.TrayIconStyle)
			{
				case TrayIconStyle.Dark:
					return _platform.PathToTrayIconDark;
				case TrayIconStyle.Light:
					return _platform.PathToTrayIconLight;

				case TrayIconStyle.Auto:
				case TrayIconStyle.None:
				default:
				{
					switch (_platform.OsColorMode)
					{
						case OsColorMode.Dark:
							return _platform.PathToTrayIconLight;

						case OsColorMode.Light:
							return _platform.PathToTrayIconDark;

						case OsColorMode.None:
						case OsColorMode.Unknown:
						default:
							return _platform.PathToTrayIconLight;
					}
				}
			}
		}
	}
}