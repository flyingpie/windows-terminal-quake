// namespace Wtq.Host.Linux.E2ETest;
//
// /// <summary>
// /// Only runs the test when E2E tests are explicitly enabled (WTQ_E2E=1) and we're in a KDE Plasma session.<br/>
// /// Otherwise the test is reported as skipped, so a regular solution-wide "dotnet test" (e.g. in CI) is unaffected.
// /// </summary>
// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
// public sealed class E2EConditionAttribute : ConditionBaseAttribute
// {
// 	public E2EConditionAttribute()
// 		: base(ConditionMode.Include)
// 	{
// 		IgnoreMessage =
// 			"E2E tests are opt-in (they move real windows around), set WTQ_E2E=1 in a KDE Plasma session to run them.";
// 	}
//
// 	public override string GroupName => "E2E";
//
// 	public override bool IsConditionMet =>
// 		Environment.GetEnvironmentVariable("WTQ_E2E") == "1"
// 		&& (Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ?? string.Empty).Contains(
// 			"KDE",
// 			StringComparison.OrdinalIgnoreCase
// 		);
// }
