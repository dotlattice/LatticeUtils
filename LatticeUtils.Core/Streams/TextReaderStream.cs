using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LatticeUtils.Streams
{
    /// <summary>
    /// A read-only stream backed by a <c>TextReader</c>.
    /// </summary>
    public class TextReaderStream : Stream
    {
        private readonly TextReader textReader;
        private readonly Encoding encoding;
        private readonly List<byte> textReaderBuffer = new List<byte>();

        /// <summary>
        /// Constructs a stream that reads from the specified <c>TextReader</c> encoded to bytes using the <c>UTF8</c> encoding.
        /// </summary>
        /// <param name="textReader">the backing <c>TextReader</c></param>
        /// <exception cref="System.ArgumentNullException">the textReader is null</exception>
        public TextReaderStream(TextReader textReader)
            : this(textReader, encoding: null) { }

        /// <summary>
        /// Constructs a stream that reads from the specified <c>TextReader</c> using the specified encoding.
        /// </summary>
        /// <param name="textReader">the backing <c>TextReader</c></param>
        /// <param name="encoding">the encoding to use or null to use a default encoding (<c>UTF8</c>)</param>
        /// <exception cref="System.ArgumentNullException">the textReader is null</exception>
        public TextReaderStream(TextReader textReader, Encoding encoding)
        {
            if (textReader == null) throw new ArgumentNullException("textReader");
            if (encoding == null) encoding = Encoding.UTF8;

            this.textReader = textReader;
            this.encoding = encoding;
        }

        /// <summary>
        /// The encoding used to encode the characters from the backing <c>TextReader</c> as bytes.
        /// </summary>
        public Encoding Encoding { get { return encoding; } }

        #region Read

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool CanRead { get { return true; } }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ReadToTextReaderBufferIfNecessary(count);
            if (!textReaderBuffer.Any())
            {
                return 0;
            }

            int readCount = Math.Min(textReaderBuffer.Count, count);
            textReaderBuffer.CopyTo(0, buffer, offset, readCount);
            textReaderBuffer.RemoveRange(0, readCount);
            return readCount;
        }

        public override int ReadByte()
        {
            ReadToTextReaderBufferIfNecessary(1);
            if (!textReaderBuffer.Any())
            {
                return -1;
            }

            int result = (int)textReaderBuffer[0];
            textReaderBuffer.RemoveAt(0);
            return result;
        }

        private void ReadToTextReaderBufferIfNecessary(int count)
        {
            if (textReaderBuffer.Count >= count)
            {
                return;
            }

            int requestedByteCount = count - textReaderBuffer.Count;

            // We're assuming that each character is at least one byte in the encoding.
            // It's fine if a character is more than one byte, that just means we'll get more 
            // data than we need in the buffer.
            int requestedCharCount = requestedByteCount;

            var charBuffer = new char[requestedCharCount];
            int charReadCount = textReader.Read(charBuffer, index: 0, count: charBuffer.Length);
            if (charReadCount > 0)
            {
                var charBytes = encoding.GetBytes(charBuffer, index: 0, count: charReadCount);
                textReaderBuffer.AddRange(charBytes);
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

        #endregion

        #region Seek

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override long Length { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override void SetLength(long value) { throw new NotSupportedException(); }

        #endregion

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                textReader.Dispose();
                textReaderBuffer.Clear();
            }
        }
    }
}
