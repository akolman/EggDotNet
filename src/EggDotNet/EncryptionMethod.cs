namespace EggDotNet
{
	/// <summary>
	/// Represents the encryption method used to encrypt an entry.
	/// </summary>
	public enum EncryptionMethod : byte
	{
		/// <summary>
		/// Standard Zip2.0 encryption.
		/// </summary>
		Standard = 0,

		/// <summary>
		/// AES128 encryption.
		/// </summary>
		AES128 = 1,

		/// <summary>
		/// AES256 encryption.
		/// </summary>
		AES256 = 2,

		/// <summary>
		/// LEA128 encryption.
		/// </summary>
		LEA128 = 5,

		/// <summary>
		/// LEA256 encryption.
		/// </summary>
		LEA256 = 6
	}
}
