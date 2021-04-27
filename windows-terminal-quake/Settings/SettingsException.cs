using System;
using System.Runtime.Serialization;

namespace WindowsTerminalQuake.Settings
{
	public class SettingsException : Exception
	{
		public SettingsException()
		{
		}

		public SettingsException(string message) : base(message)
		{
		}

		public SettingsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}