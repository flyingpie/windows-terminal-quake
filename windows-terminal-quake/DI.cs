using System;
using System.Collections.Generic;

namespace WindowsTerminalQuake
{
	/// <summary>
	/// Poor man's DI, to have some inversion-of-control, but not all the ceremony involved with setting up a proper one.
	/// </summary>
	public static class DI
	{
		private static Dictionary<Type, object> _registrations = new Dictionary<Type, object>();

		public static T Get<T>()
		{
			return _registrations.TryGetValue(typeof(T), out var service)
				? (T)service
				: throw new InvalidOperationException($"No service found with type '{typeof(T)}'.")
			;
		}

		public static void RegisterInstance<T>(T instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));

			_registrations[typeof(T)] = instance;
		}
	}
}