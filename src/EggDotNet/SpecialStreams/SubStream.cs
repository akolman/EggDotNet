using System;
using System.IO;

namespace EggDotNet.SpecialStreams
{
#pragma warning disable CA2215

	internal sealed class SubStream : Stream
	{
		private readonly Stream _superStream;
		private readonly long _endPosition;
		private readonly long _startPosition;
		private long _expectedSuperPosition;
		private bool _isDisposed;

		public override bool CanRead => _superStream.CanRead && !_isDisposed;

		public override bool CanSeek => _superStream.CanSeek && !_isDisposed;

		public override bool CanWrite => false;

		public override long Length => _endPosition - _startPosition;

		public override long Position
		{
			get => _superStream.Position - _startPosition;
			set
			{
				Seek(value, SeekOrigin.Begin);
			}
		}

		public SubStream(Stream superStream, long startPosition)
			: this(superStream, startPosition, superStream.Length)
		{

		}

		public SubStream(Stream superStream, long startPosition, long endPosition)
		{
			_superStream = superStream;
			_startPosition = startPosition;
			_endPosition = endPosition;
			_expectedSuperPosition = startPosition;
			_superStream.Seek(startPosition, SeekOrigin.Begin);
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//check if stream moved outside and adjust if so
			if (_superStream.Position != _expectedSuperPosition)
				Seek(_expectedSuperPosition, SeekOrigin.Begin);

			if (_expectedSuperPosition + count > _endPosition)
				count = (int)(_endPosition - _expectedSuperPosition);

#if NETSTANDARD2_0
			int readCount = _superStream.Read(buffer, offset, count);
#elif NETSTANDARD2_1_OR_GREATER
			int readCount = _superStream.Read(new Span<byte>(buffer, 0, count));
#endif

			_expectedSuperPosition += readCount;

			return readCount;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
			{
				if (_startPosition + offset > _endPosition)
				{
					_superStream.Seek(_endPosition, SeekOrigin.Begin);
				}
				else if (_startPosition + offset < _startPosition)
				{
					_superStream.Seek(_startPosition, SeekOrigin.Begin);
				}
				else
				{
					_superStream.Seek(_startPosition + offset, SeekOrigin.Begin);
				}

			}
			else if (origin == SeekOrigin.Current)
			{
				if (_superStream.Position != _expectedSuperPosition)
					Seek(_expectedSuperPosition, SeekOrigin.Begin);

				if (Position + offset > _endPosition)
				{
					_superStream.Seek(_endPosition, SeekOrigin.Begin);
				}
				else if (Position + offset < _startPosition)
				{
					_superStream.Seek(_startPosition, SeekOrigin.Begin);
				}
				else
				{
					_superStream.Seek(offset, SeekOrigin.Current);
				}
			}
			else
			{
				throw new NotImplementedException("Seek from end not currently supported");
			}
			_expectedSuperPosition = _superStream.Position;
			return Position;
		}

		public override void SetLength(long value)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!_isDisposed)
				{
					_isDisposed = true;
				}
			}
		}
	}
}
