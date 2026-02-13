using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Exception indicating a split volume is required but not provided.
	/// </summary>
	public sealed class MissingVolumeException : Exception
	{
		internal MissingVolumeException(int volumeId)
			: base($"Missing volume with ID {volumeId}") { }
	}
}
