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

namespace LatticeUtils.UnitTests.Streams
{
    public class TestChunkedReadStream
    {
        [Test]
        public void Read_EmptyAndNullByteArrays()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0], null, new byte[0] }))
            {
                var buffer = new byte[1];
                for (var i = 0; i < 10; i++)
                {
                    Assert.AreEqual(0, stream.Read(buffer, 0, buffer.Length));
                }
            }
        }

        [Test]
        public void Read_SimpleString_TwoChunks()
        {
            var expected = "Hello world!";
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var chunkEnumerator = new[] { expectedBytes.Take(5).ToArray(), expectedBytes.Skip(5).ToArray() }.AsEnumerable().GetEnumerator();
            using (var stream = new ChunkedReadStream(chunkEnumerator))
            {
                byte[] actualBytes;
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    actualBytes = memoryStream.ToArray();
                }

                var actual = Encoding.UTF8.GetString(actualBytes);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Read_ChunkSizesMismatchReadCount()
        {
            var chunks = new[] 
            {
                new byte[] { 1, 2, 3 },
                new byte[] { 4 },
                new byte[] { 5, 6, 7 },
            };
            var chunkEnumerator = chunks.AsEnumerable().GetEnumerator();
            using (var stream = new ChunkedReadStream(chunkEnumerator))
            {
                byte[] tempBuffer = new byte[8];
                Assert.AreEqual(2, stream.Read(tempBuffer, offset: 0, count: 2));
                Assert.AreEqual(3, stream.Read(tempBuffer, offset: 2, count: 3));
                Assert.AreEqual(2, stream.Read(tempBuffer, offset: 5, count: 3));
                Assert.AreEqual(0, stream.Read(tempBuffer, offset: 7, count: 1));
                CollectionAssert.AreEqual(chunks.SelectMany(c => c), tempBuffer.Take(7));
            }
        }

        [Test]
        public void Construct_NullEnumerable()
        {
            Assert.Throws<ArgumentNullException>(() => new ChunkedReadStream(default(IEnumerable<byte[]>)));
        }

        [Test]
        public void Construct_NullEnumerator()
        {
            Assert.Throws<ArgumentNullException>(() => new ChunkedReadStream(default(IEnumerator<byte[]>)));
        }

        [Test]
        public void Dispose_AlsoDisposesEnumerator()
        {
            var enumeratorMock = new Mock<IEnumerator<byte[]>>();
            enumeratorMock.Setup(x => x.Dispose()).Verifiable();

            var stream = new ChunkedReadStream(enumeratorMock.Object);
            stream.Dispose();

            enumeratorMock.Verify();
        }

        [Test]
        public void Dispose_ThenReadThrowsObjectDisposedException()
        {
            var stream = new ChunkedReadStream(new[] { new byte[0] });
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Read(new byte[10], 0, 10));
        }

        [Test]
        public void CanRead_ReturnsTrue()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.IsTrue(stream.CanRead);
            }
        }

        [Test]
        public void CanWrite_ReturnsFalse()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.IsFalse(stream.CanWrite);
            }
        }

        [Test]
        public void Write_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => stream.Write(new byte[1], 0, 1));
            }
        }

        [Test]
        public void CanSeek_ReturnsFalse()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.IsFalse(stream.CanSeek);
            }
        }

        [Test]
        public void Seek_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            }
        }

        [Test]
        public void PositionGet_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Position; });
            }
        }

        [Test]
        public void PositionSet_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => { stream.Position = 0; });
            }
        }


        [Test]
        public void LengthnGet_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Length; });
            }
        }

        [Test]
        public void SetLength_ThrowsNotSupportedException()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                Assert.Throws<NotSupportedException>(() => { stream.SetLength(0); });
            }
        }

        [Test]
        public void Flush_DoesNothing()
        {
            using (var stream = new ChunkedReadStream(new[] { new byte[0] }))
            {
                // Just to make sure this doesn't throw an exception or anything like that.
                stream.Flush();
            }
        }
    }
}
