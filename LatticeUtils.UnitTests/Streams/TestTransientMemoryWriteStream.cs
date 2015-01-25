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
    public class TestTransientMemoryWriteStream
    {
        [Test]
        public void Write_ToArrayAndClear()
        {
            var buffer = new byte[] { 1, 2, 3 };
            using (var stream = new TransientMemoryWriteStream())
            {
                stream.Write(buffer, offset: 0, count: buffer.Length);

                var actual1 = stream.ToArrayAndClear();
                CollectionAssert.AreEqual(buffer, actual1);

                var actual2 = stream.ToArrayAndClear();
                Assert.AreEqual(0, actual2.Length);
            }
        }

        [Test]
        public void Write_Dispose_ToArrayAndClear()
        {
            var buffer = new byte[] { 1, 2, 3 };
            TransientMemoryWriteStream stream;
            using (stream = new TransientMemoryWriteStream())
            {
                stream.Write(buffer, offset: 0, count: buffer.Length);
            }

            var actual1 = stream.ToArrayAndClear();
            CollectionAssert.AreEqual(buffer, actual1);

            var actual2 = stream.ToArrayAndClear();
            Assert.AreEqual(0, actual2.Length);
        }

        [Test]
        public void DisposeThenWrite_ThrowsObjectDisposedException()
        {
            var stream = new TransientMemoryWriteStream();
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Write(new byte[1], 0, 1));
        }

        [Test]
        public void CanWrite_ReturnsTrue()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.IsTrue(stream.CanWrite);
            }
        }

        [Test]
        public void CanRead_ReturnsFalse()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.IsFalse(stream.CanRead);
            }
        }

        [Test]
        public void Read_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => stream.Read(new byte[1], 0, 1));
            }
        }

        [Test]
        public void CanSeek_ReturnsFalse()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.IsFalse(stream.CanSeek);
            }
        }

        [Test]
        public void Seek_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            }
        }

        [Test]
        public void PositionGet_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Position; });
            }
        }

        [Test]
        public void PositionSet_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => { stream.Position = 0; });
            }
        }


        [Test]
        public void LengthnGet_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Length; });
            }
        }

        [Test]
        public void SetLength_ThrowsNotSupportedException()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                Assert.Throws<NotSupportedException>(() => { stream.SetLength(0); });
            }
        }

        [Test]
        public void Flush_DoesNothing()
        {
            using (var stream = new TransientMemoryWriteStream())
            {
                // Just to make sure this doesn't throw an exception or anything like that.
                stream.Flush();
            }
        }
    }
}
