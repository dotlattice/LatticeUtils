using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LatticeUtils.Streams
{
    /// <summary>
    /// Provides blocking and bounding capabilities for a memory stream.
    /// </summary>
    public class BlockingMemoryStream : Stream
    {
        private readonly BlockingCollection<byte> blockingBuffer;

        /// <summary>
        /// Constructs a stream with no upper bound on the amount of data that can be stored.
        /// </summary>
        public BlockingMemoryStream()
        {
            this.blockingBuffer = new BlockingCollection<byte>();
        }

        /// <summary>
        /// Constructs a stream with the specified upper bound on the amount of data that can be stored.
        /// </summary>
        /// <remarks>
        /// If the bounded capacity is reached, then write will block until some data is read from the stream or the stream is disposed.
        /// </remarks>
        /// <param name="boundedCapacity">the maximum amount of data that can be stored in this stream at once</param>
        public BlockingMemoryStream(int boundedCapacity)
        {
            this.blockingBuffer = new BlockingCollection<byte>(boundedCapacity);
        }

        /// <summary>
        /// True unless <c>CompleteWriting</c> has been called on this stream.
        /// </summary>
        public override bool CanWrite { get { return !blockingBuffer.IsAddingCompleted; } }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(buffer, offset, count, millisecondsTimeout: Timeout.Infinite, cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Writes bytes to this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to write</param>
        /// <param name="offset">the zero-based offset of the first byte in the buffer to be written to this stream</param>
        /// <param name="count">the number of bytes to write</param>
        /// <param name="cancellationToken">a cancellation token for the write operation</param>
        public void Write(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Write(buffer, offset, count, millisecondsTimeout: Timeout.Infinite, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Writes bytes to this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to write</param>
        /// <param name="offset">the zero-based offset of the first byte in the buffer to be written to this stream</param>
        /// <param name="count">the number of bytes to write</param>
        /// <param name="timeout">the maximum amount of time to wait for writing each byte, or a timespan of <c>Timeout.Infinite</c> milliseonds to wait forever</param>
        /// <param name="cancellationToken">a cancellation token for the write operation</param>
        public void Write(byte[] buffer, int offset, int count, TimeSpan timeout, CancellationToken cancellationToken)
        {
            Write(buffer, offset, count, millisecondsTimeout: (int)timeout.TotalMilliseconds, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Writes bytes to this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to write</param>
        /// <param name="offset">the zero-based offset of the first byte in the buffer to be written to this stream</param>
        /// <param name="count">the number of bytes to write</param>
        /// <param name="millisecondsTimeout">the maximum number of milliseconds to wait for reading each byte, or <c>Timeout.Infinite</c> to wait forever</param>
        /// <param name="cancellationToken">a cancellation token for the write operation</param>
        private void Write(byte[] buffer, int offset, int count, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", offset, "offset cannot be negative");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "count cannot be negative");
            if (buffer.Length - offset < count) throw new ArgumentException(string.Format("buffer cannot hold {0} bytes (capacity is {1} bytes)", count, buffer.Length));

            foreach (var b in buffer.Skip(offset).Take(count))
            {
                blockingBuffer.TryAdd(b, millisecondsTimeout: millisecondsTimeout, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Marks this stream as not accepting any more writes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">if this stream has been disposed</exception>
        public void CompleteWriting()
        {
            blockingBuffer.CompleteAdding();
        }

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer, offset, count, millisecondsTimeout: Timeout.Infinite, cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Reads bytes from this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to be written</param>
        /// <param name="offset">the zero-based offset in the buffer to where the first byte should be copied</param>
        /// <param name="count">the number of bytes to read into the buffer</param>
        /// <param name="cancellationToken">a cancellation token for the read operation</param>
        public int Read(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Read(buffer, offset, count, millisecondsTimeout: Timeout.Infinite, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Reads bytes from this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to be written</param>
        /// <param name="offset">the zero-based offset in the buffer to where the first byte should be copied</param>
        /// <param name="count">the number of bytes to read into the buffer</param>
        /// <param name="timeout">the maximum amount of time to wait for reading each byte, or a timespan of <c>Timeout.Infinite</c> milliseonds to wait forever</param>
        /// <param name="cancellationToken">a cancellation token for the read operation</param>
        public int Read(byte[] buffer, int offset, int count, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Read(buffer, offset, count, millisecondsTimeout: (int)timeout.TotalMilliseconds, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Reads bytes from this stream.
        /// </summary>
        /// <param name="buffer">a buffer containing the bytes to be written</param>
        /// <param name="offset">the zero-based offset in the buffer to where the first byte should be copied</param>
        /// <param name="count">the number of bytes to read into the buffer</param>
        /// <param name="millisecondsTimeout">the maximum number of milliseconds to wait for reading each byte, or <c>Timeout.Infinite</c> to wait forever</param>
        /// <param name="cancellationToken">a cancellation token for the read operation</param>
        private int Read(byte[] buffer, int offset, int count, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int bytesRead = 0;
            for (var i = offset; i < count; i++)
            {
                byte b;
                if (!blockingBuffer.TryTake(out b, millisecondsTimeout: millisecondsTimeout, cancellationToken: cancellationToken))
                {
                    break;
                }
                buffer[i] = b;
                bytesRead++;
            }
            return bytesRead;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// Returns a consuming enumerable for the bytes in this stream.
        /// </summary>
        /// <returns>an enumerator that reads and removes bytes from this stream</returns>
        /// <exception cref="ObjectDisposedException">if this stream is disposed</exception>
        public IEnumerable<byte> GetConsumingEnumerable()
        {
            return blockingBuffer.GetConsumingEnumerable();
        }

        /// <summary>
        /// Returns a consuming enumerable for the bytes in this stream.
        /// </summary>
        /// <returns>an enumerator that reads and removes bytes from this stream</returns>
        /// <exception cref="ObjectDisposedException">if this stream is disposed</exception>
        public IEnumerable<byte> GetConsumingEnumerable(CancellationToken cancellationToken)
        {
            return blockingBuffer.GetConsumingEnumerable(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                blockingBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Not Supported

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">always</exception>
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override long Length { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">always</exception>
        public override void SetLength(long value) { throw new NotSupportedException(); }

        #endregion
    }

}
