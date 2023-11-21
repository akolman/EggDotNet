using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EggDotNet.SpecialStreams
{
#pragma warning disable CA2215

	internal sealed class CollectiveStream : Stream
	{
		private readonly List<SubStream> subStreams;
		private readonly long totalLength;
		private long _position;
		private int _currentStreamIndex = -1;
		private readonly long[] positions;
		private bool _isDisposed;

		public override bool CanRead => !_isDisposed;

		public override bool CanSeek => !_isDisposed;

		public override bool CanWrite => false;

		public override long Length => totalLength;

		public override long Position
		{
			get => _position;
			set
			{
				Seek(value, SeekOrigin.Begin);
			}
		}

		public CollectiveStream(List<Stream> streams)
		{
			var posTemps = new List<long>(streams.Count);
			subStreams = new List<SubStream>(streams.Count);
			foreach (var stream in streams)
			{
				subStreams.Add(new SubStream(stream, 0, stream.Length));
			}

			var len = 0L;
			for (var i = 0; i < subStreams.Count; i++)
			{
				len += subStreams[i].Length;
				posTemps.Add(len);
			}
			positions = posTemps.ToArray();
			totalLength = len;
			_currentStreamIndex = 0;

		}

		public CollectiveStream(List<SubStream> streams)
		{
			var posTemps = new List<long>(streams.Count);
			var len = 0L;
			subStreams = streams;
			for (var i = 0; i < subStreams.Count; i++)
			{
				len += subStreams[i].Length;
				posTemps.Add(len);
			}
			positions = posTemps.ToArray();
			totalLength = len;
			_currentStreamIndex = 0;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (subStreams == null || subStreams.Count == 0)
				return 0;

			if (Position + count > totalLength)
			{
				count = (int)(totalLength - Position);
			}

			var amtToRead = count;
			var bufPos = 0;
			var currentStream = subStreams[_currentStreamIndex];

			SyncStream();

			while (amtToRead > 0)
			{
				var read = currentStream.Read(buffer, offset + bufPos, amtToRead);

				if (read == 0)
				{
					break; //currentStream should always be the stream we need, and since they're in order, if we get here it means we'er at the EOF
				}
				if (read < amtToRead)
				{
					if (_currentStreamIndex + 1 == subStreams.Count) break; //already at the last stream, can't go any further.
					currentStream = subStreams[++_currentStreamIndex]; //if we can, then advance to next stream.
					currentStream.Seek(0, SeekOrigin.Begin); //and set position to start of next stream
				}

				amtToRead -= read; //we've read some, so take it off the balance
				bufPos += read; //advance the current buffer position tracker
			}

			_position += count - amtToRead; //advance the overall stream position by the amount total read

			return count - amtToRead; //then return that amount read.
		}

		private int GetStreamIndexFromPosition(long position)
		{
			var smallest = 0;
			for(var i=0; i<positions.Length; i++)
			{
				if (position > positions[i])
				{
					smallest = i+1;
				}
			}

			return smallest;
		}

		//Stream position may have moved outside of this context, so need to sync the position during calls.
		private void SyncStream()
		{
			if (_currentStreamIndex > 0)
			{
				subStreams[_currentStreamIndex].Seek(_position - positions[_currentStreamIndex - 1], SeekOrigin.Begin);
			}
			else
			{
				subStreams[0].Seek(_position, SeekOrigin.Begin);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
			{
				_currentStreamIndex = GetStreamIndexFromPosition(offset);
				_position = offset;
			}
			else if (origin == SeekOrigin.Current)
			{
				_position += offset;
				_currentStreamIndex = GetStreamIndexFromPosition(_position);
				SyncStream();
			}
			else
			{
				throw new NotImplementedException("From end not implemented");
			}

			return _position;
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
