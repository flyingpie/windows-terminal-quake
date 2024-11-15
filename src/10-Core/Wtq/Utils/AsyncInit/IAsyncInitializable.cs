// namespace Wtq.Utils.AsyncInit;
//
// /// <summary>
// /// Flags a class as needing asynchronous initialization after construction by the DI system.
// /// </summary>
// public interface IAsyncInitializable
// {
// 	/// <summary>
// 	/// Optional priority, used for when multiple services have a dependency on each other and order of initialization is important.<br/>
// 	/// Initialization is done from high to low.<br/>
// 	///
// 	/// I would love to replace this with something nicer, but for now it was the simplest option.
// 	/// </summary>
// 	int InitializePriority => 0;
//
// 	/// <summary>
// 	/// Called by <see cref="AsyncServiceInitializer"/>, after construction.
// 	/// </summary>
// 	Task InitializeAsync();
// }