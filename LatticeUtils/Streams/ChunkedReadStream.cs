using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LatticeUtils.Streams
{
    /// <summary>
    /// A stream that reads from a chunked data source.
    /// </summary>
    public class ChunkedReadStream : Stream
    {
        private IEnumerator<byte[]> chunkedDataEnumerator;
        private readonly List<byte> internalBuffer = new List<byte>();

        /// <summary>
        /// Constructs a stream from the chunked data enumerable.
        /// </summary>
        /// <param name="chunkedDataEnumerable">the chunked data; this can be an enumerable that uses deferred execution</param>
        public ChunkedReadStream(IEnumerable<byte[]> chunkedDataEnumerable)
            : this(chunkedDataEnumerable != null ? chunkedDataEnumerable.GetEnumerator() : null) { }

        /// <summary>
        /// Constructs a stream from the chunked data enumerator.
        /// </summary>
        /// <param name="chunkedDataEnumerator">an enumerator over the chunks of data</param>
        public ChunkedReadStream(IEnumerator<byte[]> chunkedDataEnumerator)
        {
            if (chunkedDataEnumerator == null) throw new ArgumentNullException("chunkedDataEnumerator");
            this.chunkedDataEnumerator = chunkedDataEnumerator;
        }

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (chunkedDataEnumerator == null) throw new ObjectDisposedException(GetType().Name);

            int readCount = 0;

            // Fill our internal buffer until we have enough data to satisfy the request (or until we run out of data).
            while (internalBuffer.Count < count && chunkedDataEnumerator.MoveNext())
            {
                var chunk = chunkedDataEnumerator.Current;
                if (chunk == null || chunk.Length == 0) continue;
                
                internalBuffer.AddRange(chunk);
            }

            // Move data from our internal buffer to the output buffer.
            if (internalBuffer.Any())
            {
                var take = Math.Min(internalBuffer.Count, count);
                internalBuffer.CopyTo(0, buffer, offset, take);
                internalBuffer.RemoveRange(0, take);
                readCount += take;
            }

            return readCount;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && chunkedDataEnumerator != null)
            {
                chunkedDataEnumerator.Dispose();
                chunkedDataEnumerator = null;
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

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">always</exception>
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

        #endregion
    }
}
