using Microsoft.AspNetCore.Components;
using Wtq.Configuration;

namespace Wtq.Services.UI.Components;

public partial class ProcessArg : ComponentBase
{
	[EditorRequired]
	[Parameter]
	public ProcessArgument Argument { get; set; } = null!;

	[EditorRequired]
	[Parameter]
	public Action OnRemove { get; set; } = null!;
}