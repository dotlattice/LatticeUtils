using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.UnitTests
{
    public class TestStreamUtils
    {
        [Test]
        public void FromTextReader_StreamReader()
        {
            const string inputString = "Hello world!";
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            using (var stream = StreamUtils.FromTextReader(streamReader))
            {
                Assert.AreEqual(typeof(MemoryStream), stream.GetType());
                Assert.AreSame(memoryStream, stream);
                Assert.AreEqual(inputString, Encoding.UTF8.GetString(StreamUtils.ReadAllBytes(stream)));
            }
        }

        [Test]
        public void FromTextReader_StringReader()
        {
            const string inputString = "Hello world!";
            using (var stringReader = new StringReader(inputString))
            using (var stream = StreamUtils.FromTextReader(stringReader))
            {
                Assert.AreEqual(typeof(LatticeUtils.Streams.TextReaderStream), stream.GetType());
                Assert.AreEqual(inputString, Encoding.UTF8.GetString(StreamUtils.ReadAllBytes(stream)));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(6)]
        [TestCase(12)]
        public void ReadToEnd_MemoryStream(int offset)
        {
            const string inputString = "Hello world!";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
            {
                if (offset > 0) stream.Read(new byte[offset], 0, offset);

                var bytes = StreamUtils.ReadToEnd(stream);
                Assert.AreEqual(inputString.Substring(offset), Encoding.UTF8.GetString(bytes));
            }
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(1, true)]
        [TestCase(6, true)]
        [TestCase(12, true)]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(6, false)]
        [TestCase(12, false)]
        public void ReadToEnd_MemoryStreamAdapter(int offset, bool canSeek)
        {
            const string inputString = "Hello world!";
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
            using (var stream = new MemoryStreamAdapter(memoryStream, canSeek: canSeek))
            {
                if (offset > 0) stream.Read(new byte[offset], 0, offset);

                var bytes = StreamUtils.ReadToEnd(stream);
                Assert.AreEqual(inputString.Substring(offset), Encoding.UTF8.GetString(bytes));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(6)]
        [TestCase(12)]
        public void ReadAllBytes_MemoryStream(int offset)
        {
            const string inputString = "Hello world!";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
            {
                if (offset > 0) stream.Read(new byte[offset], 0, offset);

                var bytes = StreamUtils.ReadAllBytes(stream);
                Assert.AreEqual(inputString, Encoding.UTF8.GetString(bytes));
            }
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(1, true)]
        [TestCase(6, true)]
        [TestCase(12, true)]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(6, false)]
        [TestCase(12, false)]
        public void ReadAllBytes_MemoryStreamAdapter(int offset, bool canSeek)
        {
            const string inputString = "Hello world!";

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
            using (var stream = new MemoryStreamAdapter(memoryStream, canSeek: canSeek))
            {
                if (offset > 0) stream.Read(new byte[offset], 0, offset);

                var bytes = StreamUtils.ReadAllBytes(stream);
                var expected = canSeek ? inputString : inputString.Substring(offset);
                Assert.AreEqual(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        private class MemoryStreamAdapter : Stream
        {
            private readonly MemoryStream memoryStream;
            private readonly bool canSeek;

    
            public MemoryStreamAdapter(MemoryStream memoryStream, bool canSeek)
            {
                if (memoryStream == null) throw new ArgumentNullException("memoryStream");
                this.memoryStream = memoryStream;
                this.canSeek = canSeek;
            }

            public override bool CanRead { get { return true; } }
            public override int Read(byte[] buffer, int offset, int count) { return memoryStream.Read(buffer, offset, count); } 

            public override bool CanWrite { get { return false; } }
            public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

            public override bool CanSeek { get { return canSeek; } }
            public override long Seek(long offset, SeekOrigin origin)
            {
                if (!canSeek) throw new NotSupportedException();
                return memoryStream.Seek(offset, origin);
            }
            public override long Position
            {
                get 
                { 
                    if (!canSeek)  throw new NotSupportedException();
                    return memoryStream.Position;
                }
                set 
                { 
                    if (!canSeek) throw new NotSupportedException(); 
                    memoryStream.Position = value;
                }
            }
            public override long Length 
            { 
                get 
                { 
                    if (!canSeek) throw new NotSupportedException(); 
                    return memoryStream.Length;
                } 
            }
            public override void SetLength(long value) 
            { 
                if (!canSeek) throw new NotSupportedException(); 
                memoryStream.SetLength(value);
            }

            public override void Flush() { }
            protected override void Dispose(bool disposing) { if (disposing) memoryStream.Dispose(); }
        }
    }
}