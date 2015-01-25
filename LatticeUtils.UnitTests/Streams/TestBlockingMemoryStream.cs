using LatticeUtils.Streams;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LatticeUtils.UnitTests.Streams
{
    public class TestBlockingMemoryStream
    {
        [Test]
        public void WriteThenRead_Timeout()
        {
            var buffer = new byte[] { 1, 2, 3 };
            using (var stream = new BlockingMemoryStream())
            {
                stream.Write(buffer, 0, 3);

                // If we try to read too much, then it should stop blocking and return once we hit the timout.
                var readBuffer = new byte[4];
                var readTask = new TaskFactory().StartNew(() => 
                    stream.Read(readBuffer, offset: 0, count: 4, timeout: TimeSpan.FromMilliseconds(10), cancellationToken: CancellationToken.None)
                );
                Assert.IsTrue(readTask.Wait(millisecondsTimeout: 100));
                var readCount = readTask.Result;

                Assert.AreEqual(3, readCount);
                CollectionAssert.AreEqual(buffer, readBuffer.Take(3));
            }
        }

        [Test]
        public void WriteThenReadToEnd_StopsBlockingWhenCompleteWriting()
        {
            var buffer = new byte[] { 1, 2, 3 };
            using (var stream = new BlockingMemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);

                // We can read a byte without hitting any blocking
                var readByteTask = new TaskFactory().StartNew(() => stream.ReadByte());
                Assert.IsTrue(readByteTask.Wait(millisecondsTimeout: 1000));
                Assert.AreEqual(1, readByteTask.Result);

                // If we try to read to the end, then we should block until we call CompleteWriting
                var readToEndTask = new TaskFactory().StartNew(() => StreamUtils.ReadToEnd(stream));
                Assert.IsFalse(readToEndTask.Wait(millisecondsTimeout: 100));
                
                stream.CompleteWriting();

                Assert.IsTrue(readToEndTask.Wait(millisecondsTimeout: 1000));
                CollectionAssert.AreEqual(buffer.Skip(1), readToEndTask.Result);
            }
        }

        [Test]
        public void WriteThenGetConsumingEnumerable_StopsBlockingWhenCompleteWriting()
        {
            var buffer = new byte[] { 1, 2, 3 };
            using (var stream = new BlockingMemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);

                var readToListTask = new TaskFactory().StartNew(() => stream.GetConsumingEnumerable().ToList());
                Assert.IsFalse(readToListTask.Wait(millisecondsTimeout: 100));

                stream.CompleteWriting();

                Assert.IsTrue(readToListTask.Wait(millisecondsTimeout: 1000));
                CollectionAssert.AreEqual(buffer, readToListTask.Result);
            }
        }

        [Test]
        public void Write_TimeoutUntilRead()
        {
            var buffer = new byte[] { 1, 2, 3 };
            using (var stream = new BlockingMemoryStream(boundedCapacity: 2))
            {
                // Write one byte more than the capacity, should cause the write to block.
                var writeTask = new TaskFactory().StartNew(() =>
                    stream.Write(buffer, 0, 3)
                );
                Assert.IsFalse(writeTask.Wait(millisecondsTimeout: 100));

                // Read one byte
                var readByteTask = new TaskFactory().StartNew(() =>
                    stream.ReadByte()
                );
                Assert.IsTrue(readByteTask.Wait(millisecondsTimeout: 1000));
                Assert.AreEqual(1, readByteTask.Result);

                // Now there should be enough buffer space to finish the write task.
                Assert.IsTrue(writeTask.Wait(millisecondsTimeout: 1000));
            }
        }

        [Test]
        public void DisposeThenWrite_ThrowsObjectDisposedException()
        {
            var stream = new BlockingMemoryStream();
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Write(new byte[1], 0, 1));
        }

        [Test]
        public void CanWrite_ReturnsTrue()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.IsTrue(stream.CanWrite);
            }
        }

        [Test]
        public void CanWriteAfterCompleteWriting_ReturnsFalse()
        {
            using (var stream = new BlockingMemoryStream())
            {
                stream.CompleteWriting();
                Assert.IsFalse(stream.CanWrite);
            }
        }

        [Test]
        public void WriteAfterCompleteWriting_ThrowsInvalidOperationException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                stream.CompleteWriting();
                Assert.Throws<InvalidOperationException>(() => stream.Write(new byte[1], 0, 1));
            }
        }

        [Test]
        public void CanRead_ReturnsTrue()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.IsTrue(stream.CanRead);
            }
        }

        [Test]
        public void CanSeek_ReturnsFalse()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.IsFalse(stream.CanSeek);
            }
        }

        [Test]
        public void Seek_ThrowsNotSupportedException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            }
        }

        [Test]
        public void PositionGet_ThrowsNotSupportedException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Position; });
            }
        }

        [Test]
        public void PositionSet_ThrowsNotSupportedException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.Throws<NotSupportedException>(() => { stream.Position = 0; });
            }
        }


        [Test]
        public void LengthnGet_ThrowsNotSupportedException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.Throws<NotSupportedException>(() => { var temp = stream.Length; });
            }
        }

        [Test]
        public void SetLength_ThrowsNotSupportedException()
        {
            using (var stream = new BlockingMemoryStream())
            {
                Assert.Throws<NotSupportedException>(() => { stream.SetLength(0); });
            }
        }

        [Test]
        public void Flush_DoesNothing()
        {
            using (var stream = new BlockingMemoryStream())
            {
                // Just to make sure this doesn't throw an exception or anything like that.
                stream.Flush();
            }
        }
    }
}
