namespace EggDotNet.Compression.AZO.Common
{
	internal static class AZOOption
	{
		public const uint AZO_PRIVATE_VERSION = 0x31; //ASCII '1'

		public const uint COMPRESSION_REDUCE_MIN_SIZE = 8;

		public const uint ALPHA_SIZE = 1 << 8;
		public const uint ALPHA_HISTORY_SIZE = 1;

		public const uint MATCH_LENGTH_SGAP = 32;
		public const uint MATCH_LENGTH_GAP = 8;
		public const uint MATCH_DIST_SGAP = 16;
		public const uint MATCH_DIST_GAP = 4;

		public const uint MATCH_MIN_LENGTH = 2;
		public const uint MATCH_MIN_DIST = 1;

		public const uint MATCH_LENGTH_CODE_SIZE = 1 << 7;
		public const uint MATCH_DIST_CODE_SIZE = 1 << 7;

		public const uint MATCH_HASH_LEVEL = 5;
		public const uint MATCH_HASH_BITSIZE  = 22;

		public const uint DISTANCE_HISTORY_SIZE = 1 << 1;

		public const uint DICTIONARY_SIZE = 1 << 7;
		public const uint DICTIONARY_HISTORY_SIZE = 2;

		public const uint ALPHACODE_PREDICT_SHIFT = 5;
		public const uint LENGTHCODE_PREDICT_SHIFT = 4;

		public const int SCALE_FACTOR = 8;
		public const int N = SCALE_FACTOR;

		public const int EXTRACT_CHUNK = 1024 * 4;
	}
}
