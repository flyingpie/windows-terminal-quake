namespace Wtq.Configuration;

/// <summary>
/// Sometimes functionality is added or changed that carries more risk of introducing bugs.<br/>
/// <br/>
/// For these cases, such functionality can be put behind a "feature flag", which makes them opt-in or opt-out.<br/>
/// That way, we can still merge to master, and make it part of the stable release version (reducing branches and dev builds and what not),
/// but still have a way back should things go awry.
/// </summary>
public class FeatureFlags { }
