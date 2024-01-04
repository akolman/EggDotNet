using System.Collections.Generic;
using System.IO;

namespace EggDotNet
{
	/// <summary>
	/// Represents a set of callbacks that can be created by the caller and passed to <see cref="EggArchive"/>.
	/// </summary>
	public static class Callbacks
	{
		/// <summary>
		/// Represents the object passed to <see cref="FileDecryptPasswordCallback"/> and is manipulated by the caller.
		/// </summary>
		public sealed class PasswordCallbackOptions
		{
			/// <summary>
			/// Gets or sets the password to be used for decryption.
			/// </summary>
			public string Password { get; set; } = string.Empty;

			/// <summary>
			/// Gets or sets a flag indicating whether to retry a password request if the password is invalid.
			/// </summary>
			public bool Retry { get; set; }

			internal PasswordCallbackOptions()
			{

			}
		}

		/// <summary>
		/// Callback delegate used to gather stream pieces for a multi-part (split) archive.
		/// Executed when an EGG file with a split header is opened.
		/// For <see cref="FileStream"/> a default callback is used to gather files from the same root folder.
		/// </summary>
		/// <remarks>
		/// Upon receiving streams, pieces will be validated according to their piece ID.  Thus, passing unnecessary streams will not
		/// conflict with processing, and uneeded streams will automatically be disposed.  Used streams will also be disposed once parent
		/// <see cref="EggArchive"/> is disposed.
		/// </remarks>
		/// <param name="sourceStream">The Stream that was opened and found to be part of a split archive.</param>
		/// <returns>A set of Streams, set by the caller, that refer to possible pieces of this archive.</returns>
		public delegate IEnumerable<Stream> SplitFileReceiverCallback(Stream sourceStream);

		/// <summary>
		/// Callback delegate used to fetch password for entry decryption.
		/// </summary>
		/// <param name="entryName">The name of the entry to decrypt.</param>
		/// <param name="passwordOptions">An object which the caller should set with password and whether to retry.</param>
		public delegate void FileDecryptPasswordCallback(string entryName, PasswordCallbackOptions passwordOptions);
	}
}
