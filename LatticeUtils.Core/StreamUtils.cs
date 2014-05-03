using LatticeUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods for working with streams.
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>
        /// Returns a stream that reads from the specified <c>TextReader</c>.
        /// </summary>
        /// <remarks>
        /// Depending on the type of <c>TextReader</c> provided, the resulting stream may be read-only 
        /// (with <c>Stream.CanSeek</c> and <c>Stream.CanWrite</c> both false).
        /// </remarks>
        /// <param name="textReader">the <c>TextReader</c> to convert to a stream</param>
        /// <returns>the stream</returns>
        public static Stream FromTextReader(TextReader textReader)
        {
            return FromTextReader(textReader, preferredEncoding: null);
        }

        /// <summary>
        /// Returns a stream that reads from the specified text reader, which will use the specified 
        /// encoding if the stream needs to convert characters to bytes.
        /// </summary>
        /// <remarks>
        /// Depending on the type of <c>TextReader</c>, the resulting stream may be read-only 
        /// (with <c>Stream.CanSeek</c> and <c>Stream.CanWrite</c> both false).
        /// The specified encoding will only be used if a backing stream for the text reader 
        /// cannot be found (for example, if it is not a <c>StreamReader</c>).
        /// </remarks>
        /// <param name="textReader">the <c>TextReader</c> to convert to a stream</param>
        /// <param name="preferredEncoding">the encoding to use (if necessary)</param>
        /// <returns>the stream</returns>
        public static Stream FromTextReader(TextReader textReader, Encoding preferredEncoding)
        {
            if (textReader is StreamReader)
            {
                return ((StreamReader)textReader).BaseStream;
            }
            else
            {
                return new TextReaderStream(textReader, encoding: preferredEncoding);
            }
        }

        /// <summary>
        /// Reads and returns the bytes from the current position to the end of the stream.
        /// </summary>
        /// <param name="stream">the stream from which to read</param>
        /// <returns>the remaining bytes from the stream</returns>
        /// <exception cref="System.OutOfMemoryException">there is insufficient memory to allocate the byte array</exception>
        /// <exception cref="System.ObjectDisposedException">the stream has been disposed</exception>
        public static byte[] ReadToEnd(Stream stream)
        {
            // If the stream is seekable, we can use the Length and Position properties 
            // to calculate the size of the byte array and read directly into that.
            if (stream.CanSeek)
            {
                var streamLength = stream.Length;
                var streamPosition = stream.Position;

                var resultLength = (int)(streamLength - streamPosition);
                var result = new byte[resultLength];

                int bytesRead;
                int resultOffset = 0;
                do
                {
                    bytesRead = stream.Read(result, resultOffset, resultLength - resultOffset);
                    resultOffset += bytesRead;
                } while (bytesRead > 0);
                return result;
            }
            
            MemoryStream memoryStream;
            using (memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
            }
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Reads and returns all of the bytes from the specified stream, regardless of the <c>Position</c> property if the stream is seekable.
        /// </summary>
        /// <param name="stream">the stream from which to read</param>
        /// <returns>the bytes from the stream</returns>
        /// <exception cref="System.OutOfMemoryException">there is insufficient memory to allocate the byte array</exception>
        /// <exception cref="System.ObjectDisposedException">the stream has been disposed</exception>
        public static byte[] ReadAllBytes(Stream stream)
        {
            var memoryStream = stream as MemoryStream;
            if (memoryStream != null)
            {
                return memoryStream.ToArray();
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            return ReadToEnd(stream);
        }
    }
}
