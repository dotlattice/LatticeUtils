using LatticeUtils.Streams;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests.Streams
{
    public class TestTextReaderStream
    {
        [Test]
        public void ReadAllBytes_SimpleString_AllAvailableEncodings()
        {
            const string inputString = "Hello world!";
            foreach (var encodingInfo in Encoding.GetEncodings())
            {
                var encoding = encodingInfo.GetEncoding();
                using (var stream = new TextReaderStream(new StringReader(inputString), encoding: encoding))
                {
                    Assert.AreEqual(encoding, stream.Encoding);

                    byte[] streamBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        streamBytes = memoryStream.ToArray();
                    }
                    var streamText = encoding.GetString(streamBytes);
                    Assert.AreEqual(inputString, streamText, encodingInfo.DisplayName);
                }
            }
        }

        [Test]
        public void ReadBytes_FillBufferWithPartialCharacter()
        {
            // Reading a multi-byte character into a buffer that is too small requires us to buffer 
            // the result from the text reader, since we're only returning part of a character.
            // So this test is to make sure we're doing that.

            // This character should be encoded into three bytes for UTF8.
            const string inputString = "☃";
            Assert.AreEqual(3, Encoding.UTF8.GetBytes(inputString).Length);

            using (var stream = new TextReaderStream(new StringReader(inputString)))
            {
                var buffer = new byte[3];

                int readCount = stream.Read(buffer, 0, 2);
                Assert.AreEqual(2, readCount);

                int secondReadCount = stream.Read(buffer, 2, 999);
                Assert.AreEqual(1, secondReadCount);

                var streamText = Encoding.UTF8.GetString(buffer);
                Assert.AreEqual(inputString, streamText);
            }
        }

        [Test]
        public void ReadByte_UnicodeMix()
        {
            const string inputString = "☀a☁b☂☃c☄d☇☈☉";
            using (var stream = new TextReaderStream(new StringReader(inputString)))
            {
                var streamBytes = ReadUsingReadByte(stream).ToArray();
                var streamText = Encoding.UTF8.GetString(streamBytes);
                Assert.AreEqual(inputString, streamText);
            }
        }

        private static IEnumerable<byte> ReadUsingReadByte(Stream stream)
        {
            int i;
            while ((i = stream.ReadByte()) != -1)
            {
                yield return (byte)i;
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(27)]
        [TestCase(28)]
        [TestCase(29)]
        public void Read_UnicodeMix(int bufferSize)
        {
            const string inputString = "☀a☁b☂☃c☄d☇☈☉";
            Assert.AreEqual(28, Encoding.UTF8.GetBytes(inputString).Length);

            using (var stream = new TextReaderStream(new StringReader(inputString)))
            {
                var streamBytes = ReadUsingBufferSize(stream, bufferSize).ToArray();
                var streamText = Encoding.UTF8.GetString(streamBytes);
                Assert.AreEqual(inputString, streamText);
            }
        }

        private static IEnumerable<byte> ReadUsingBufferSize(Stream stream, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];

            int readCount;
            while ((readCount = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (var i = 0; i < readCount; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        [Test]
        public void Constructor_NullTextReader()
        {
            var expectedException = Assert.Throws<ArgumentNullException>(() => new TextReaderStream(null));
            StringAssert.Contains("textReader", expectedException.Message);
        }

        [Test]
        public void Constructor_NullTextEncoding()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty), encoding: null))
            {
                Assert.AreEqual(Encoding.UTF8, stream.Encoding);
            }
        }

        [Test]
        public void Dispose_AlsoDisposesTextReader()
        {
            var readerMock = new Mock<TextReader>();
            readerMock.Protected().Setup("Dispose", true).Verifiable();

            var stream = new TextReaderStream(readerMock.Object);
            stream.Dispose();

            readerMock.Verify();
        }

        [Test]
        public void Dispose_ThenReadByteThrowsObjectDisposedException()
        {
            var stream = new TextReaderStream(new StringReader(string.Empty));
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
        }

        [Test]
        public void Dispose_ThenReadThrowsObjectDisposedException()
        {
            var stream = new TextReaderStream(new StringReader(string.Empty));
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Read(new byte[10], 0, 10));
        }

        [Test]
        public void CanRead_ReturnsTrue()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.IsTrue(stream.CanRead);
            }
        }

        [Test]
        public void CanWrite_ReturnsFalse()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.IsFalse(stream.CanWrite);
            }
        }

        [Test]
        public void Write_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => stream.Write(new byte[1], 0, 1));
            }
        }

        [Test]
        public void CanSeek_ReturnsFalse()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.IsFalse(stream.CanSeek);
            }
        }

        [Test]
        public void Seek_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            }
        }

        [Test]
        public void PositionGet_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Position; });
            }
        }

        [Test]
        public void PositionSet_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => { stream.Position = 0; });
            }
        }


        [Test]
        public void LengthnGet_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Length; });
            }
        }

        [Test]
        public void SetLength_ThrowsNotSupportedException()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                Assert.Throws<NotSupportedException>(() => { stream.SetLength(0); });
            }
        }

        [Test]
        public void Flush_DoesNothing()
        {
            using (var stream = new TextReaderStream(new StringReader(string.Empty)))
            {
                // Just to make sure this doesn't throw an exception or anything like that.
                stream.Flush();
            }
        }
    }
}
