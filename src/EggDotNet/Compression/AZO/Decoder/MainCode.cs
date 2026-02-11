using EggDotNet.Compression.AZO.Common;
using EggDotNet.Exceptions;
using EggDotNet.InternalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class MainCode
	{
		const uint MAIN_HEAD_SIZE = 2;
		const uint BLOCK_SIZE_SIZE = 4;
		const uint BLOCK_HEAD_SIZE = BLOCK_SIZE_SIZE * 3;

		private bool _init;
		private bool _setSizeInfo;
		private bool _finish;

		private uint blockSize;
		private uint compressSize;

		#region bufferedifwanted

		/*
		public int Code(byte[] inBuf, int inAvailable, byte[] outBuf, int outAvailable)
		{
			int totalRead = 0;
			int pos = 0;
			while (true)
			{
				if (!_init)
				{
					_init = true;
					if (inBuf.Length < MAIN_HEAD_SIZE)
					{
						throw new Exception("Missing AZO header");
					}
					if (inBuf[0] != AZOOption.AZO_PRIVATE_VERSION)
					{
						throw new Exception("Data error");
					}
					//TODO: check filter
					pos += (int)MAIN_HEAD_SIZE;
				}

				if (!_setSizeInfo)
				{
					if (inBuf.Length < BLOCK_SIZE_SIZE * 3 + MAIN_HEAD_SIZE)
					{
						throw new Exception("Missing AZO block header");
					}

					_setSizeInfo = true;
					var blockSizeBuffer = inBuf.Skip(pos).Take((int)BLOCK_SIZE_SIZE).ToArray();
					pos += (int)BLOCK_SIZE_SIZE;
					var compressSizeBuffer = inBuf.Skip(pos).Take((int)BLOCK_SIZE_SIZE).ToArray();
					pos += (int)BLOCK_SIZE_SIZE;
					var checkSizeBuffer = inBuf.Skip(pos).Take((int)BLOCK_SIZE_SIZE).ToArray();
					pos += (int)BLOCK_SIZE_SIZE;
					Array.Reverse(blockSizeBuffer);
					blockSize = (uint)BitConverter.ToInt32(blockSizeBuffer, 0);
					Array.Reverse(compressSizeBuffer);
					compressSize = (uint)BitConverter.ToInt32(compressSizeBuffer, 0);
					Array.Reverse(checkSizeBuffer);
					var checkSize = (uint)BitConverter.ToInt32(checkSizeBuffer, 0);
					if (blockSize < compressSize || (blockSize ^ compressSize) != checkSize)
					{
						throw new Exception("Data error");
					}
					//inStream.Position -= 12;
					//inStream.Read(checkSizeBuffer);
				}

				if (_finish)
				{
					break;
				}

				if (blockSize > 0 && compressSize > 0)
				{
					var buf = inBuf.Skip(pos).Take((int)compressSize).ToArray();
					pos += (int)compressSize;
					var obuf = new byte[blockSize];
					

					int ret = ReadBlock(buf, compressSize, obuf, blockSize);
					
					if (ret == 0)
					{
						Array.Copy(obuf, 0, outBuf, totalRead, blockSize);
						_setSizeInfo = false;
						//outSTream.Write(obuf, 0, ret);
					}
					else
					{
						break;
					}
					totalRead += (int)blockSize;

					if (totalRead >= outAvailable)
					{
						return totalRead;
					}
					if (pos >= inAvailable)
					{
						return totalRead;
					}
				}
				else
				{
					_finish = true;
				}

			}
			return totalRead;
		}*/
		#endregion

		public int Code(Stream inStream, DecompressedBufferCache outBuff, int readSize = 4096)
		{
			int totalRead = 0;
			while (true)
			{
				if (!_init)
				{
					_init = true;

					var headBuf = new byte[MAIN_HEAD_SIZE];
					var readCount = inStream.Read(headBuf);

					if (readCount != MAIN_HEAD_SIZE)
					{
						throw new DecompressionDataException("Missing AZO header");
					}
					if (headBuf[0] != AZOOption.AZO_PRIVATE_VERSION)
					{
						throw new DecompressionDataException("Invalid AZO header");
					}
				}

				if (!_setSizeInfo)
				{
					var blockHeadBuf = new byte[BLOCK_SIZE_SIZE * 3];
					var blockHeadRead = inStream.Read(blockHeadBuf);

					if (blockHeadRead < BLOCK_SIZE_SIZE * 3)
					{
						throw new DecompressionDataException("Failed reading AZO block");
					}

					_setSizeInfo = true; //TODO: optimize below
					var blockSizeBuffer = blockHeadBuf.Take((int)BLOCK_SIZE_SIZE).ToArray();
					var compressSizeBuffer = blockHeadBuf.Skip((int)BLOCK_SIZE_SIZE).Take((int)BLOCK_SIZE_SIZE).ToArray();
					var checkSizeBuffer = blockHeadBuf.Skip((int)BLOCK_SIZE_SIZE*2).Take((int)BLOCK_SIZE_SIZE).ToArray();
					Array.Reverse(blockSizeBuffer);
					Array.Reverse(compressSizeBuffer);
					Array.Reverse(checkSizeBuffer);
					blockSize = (uint)BitConverter.ToInt32(blockSizeBuffer, 0);
					compressSize = (uint)BitConverter.ToInt32(compressSizeBuffer, 0);
					var checkSize = (uint)BitConverter.ToInt32(checkSizeBuffer, 0);
					if (blockSize < compressSize || (blockSize ^ compressSize) != checkSize)
					{
						throw new DecompressionDataException("AZO checksum failure");
					}
				}

				if (_finish)
				{
					break;
				}

				if (blockSize > 0 && compressSize > 0)
				{
					var buf = new byte[compressSize];
					var readCount = inStream.Read(buf, 0, (int)compressSize);
					_ = readCount;
					var obuf = new byte[blockSize];
					int ret = ReadBlock(buf, compressSize, obuf, blockSize);
					totalRead += (int)blockSize;

					if (ret == 0)
					{
						outBuff.Push(obuf);
						_setSizeInfo = false;
					}
					else
					{
						break;
					}

					if (totalRead >= readSize)
					{
						return totalRead;
					}
				}
				else
				{
					_finish = true;
				}

			}
			return totalRead;
		}

		private static int ReadBlock(byte[] inBuf, uint insize, byte[] outBuf, uint outsize)
		{
			if (insize + AZOOption.COMPRESSION_REDUCE_MIN_SIZE > outsize)
			{
				if (insize == outsize)
				{
					Array.Copy(inBuf, outBuf, insize);
					return 0;
				}
				else
				{
					return -4; //TODO: find and define codes
				}
			}
			else
			{
				EntropyCode entropy = new EntropyCode(inBuf, insize);
				BlockCode block = new BlockCode(outBuf, (int)outsize);
				int ret = block.Code(entropy);
				return ret;
			}
		}
	}
}
