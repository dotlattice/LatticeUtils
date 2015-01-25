using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LatticeUtils.Streams
{
    /// <summary>
    /// A memory stream that only keeps data in memory until it is read.
    /// </summary>
    /// <remarks>
    /// This stream only supports the write operations.  
    /// The only way to get data out of the stream is through the <c>ToArrayAndClear</c> method.
    /// </remarks>
    public class TransientMemoryWriteStream : Stream
    {
        private readonly List<byte> transientBuffer = new List<byte>();
        private bool isDisposed = false;

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", offset, "offset cannot be negative");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "count cannot be negative");
            if (buffer.Length - offset < count) throw new ArgumentException(string.Format("buffer cannot hold {0} bytes (capacity is {1} bytes)", count, buffer.Length));
            if (isDisposed) throw new ObjectDisposedException(GetType().FullName);

            transientBuffer.AddRange(buffer.Skip(offset).Take(count));
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.isDisposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Clears the current data in the stream and returns the data that was cleared.
        /// </summary>
        /// <returns>the content of the stream before it was cleared</returns>
        public byte[] ToArrayAndClear()
        {
            byte[] result = transientBuffer.ToArray();
            transientBuffer.Clear();

            // We may want to reduce the capacity of our buffer if it got very large.
            var maximumEmptyCapacity = isDisposed ? 0 : 8192;
            if (transientBuffer.Capacity > maximumEmptyCapacity)
            {
                transientBuffer.Capacity = maximumEmptyCapacity;
            }

            return result;
        }

        #region Not Supported

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanRead { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">always</exception>
        public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

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
