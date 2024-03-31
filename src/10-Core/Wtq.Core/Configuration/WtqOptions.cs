namespace Wtq.Core.Configuration;

public sealed class WtqOptions
{
	private readonly ILogger _log = Log.For<WtqOptions>();

	[Required]
	public IList<WtqAppOptions> Apps { get; set; } = [];

	public AttachMode AttachMode { get; set; } = AttachMode.FindOrStart;

	[Required]
	public IEnumerable<HotKeyOptions> HotKeys { get; set; } = [];

	/// <summary>
	/// <para>Gets or sets if "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
	/// <para>Zero based, eg. 0, 1, etc.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int MonitorIndex { get; set; }

	/// <summary>
	/// <para>Gets or sets what monitor to preferrably drop the terminal.</para>
	/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
	/// </summary>
	public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;

	#region Sizes

	public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Center;

	/// <summary>
	/// <para>How much room to leave between the top of the terminal and the top of the screen, in pixels.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int VerticalOffset { get; set; } = 0;

	/// <summary>
	/// <para>Vertical screen coverage as a percentage (0-100).</para>
	/// <para>Defaults to "95".</para>
	/// </summary>
	public float VerticalScreenCoverage { get; set; } = 95f;

	/// <summary>
	/// <para>Horizontal screen coverage, as a percentage.</para>
	/// <para>Defaults to "95".</para>
	/// </summary>
	public float HorizontalScreenCoverage { get; set; } = 95f;

	public HorizontalAlign GetHorizontalAlignForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		return opts.HorizontalAlign ?? HorizontalAlign;
	}

	public float GetVerticalOffsetForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		return opts.VerticalOffset ?? VerticalOffset;
	}

	public float GetHorizontalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		return opts.HorizontalScreenCoverage ?? HorizontalScreenCoverage;
	}

	public float GetVerticalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		if (opts.VerticalScreenCoverage.HasValue)
		{
			_log.LogTrace("[{Property}] Using app-specific value '{Value}' ('{App}')", nameof(VerticalScreenCoverage), opts.VerticalScreenCoverage.Value, opts);
			return opts.VerticalScreenCoverage.Value;
		}

		_log.LogTrace("[{Property}] Using global value '{Value}'", nameof(VerticalScreenCoverage), VerticalScreenCoverage);

		return VerticalScreenCoverage;
	}

	/// <summary>
	/// Horizontal screen coverage as an index (0 - 1).
	/// </summary>
	internal float HorizontalScreenCoverageIndexForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		//var cov = opts.HorizontalScreenCoverage ?? HorizontalScreenCoverage;
		var cov = GetHorizontalScreenCoverageForApp(opts);

		return cov / 100f;
	}

	/// <summary>
	/// Vertical screen coverage as an index (0 - 1).
	/// </summary>
	internal float VerticalScreenCoverageIndexForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts, nameof(opts));

		//var cov = opts.VerticalScreenCoverage ?? VerticalScreenCoverage;
		var cov = GetVerticalScreenCoverageForApp(opts);

		return cov / 100f;
	}

	#endregion Sizes
}