﻿using Wtq.Core.Data;
using Wtq.Services;

namespace Wtq.Core.Services;

public class WtqEvent : IWtqEvent
{
	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }

	public override string ToString() => $"[WtqEvent] ActionType:{ActionType} App:{App}";
}