using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal static class StringCompressor
    {
        
        private const int Offset = 0;
        private const int DistOffset = 4;
        
        internal static string CompressString(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, Offset, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, Offset, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + DistOffset];
            
            Buffer.BlockCopy(compressedData, Offset, gZipBuffer, DistOffset, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), Offset, gZipBuffer, Offset, DistOffset);
            
            return Convert.ToBase64String(gZipBuffer);
        }
        
        internal static string DecompressString(string compressedText)
        {
            var gZipBuffer = Convert.FromBase64String(compressedText);
            
            using (var memoryStream = new MemoryStream())
            {
                var dataLength = BitConverter.ToInt32(gZipBuffer, Offset);
                memoryStream.Write(gZipBuffer, DistOffset, gZipBuffer.Length - DistOffset);

                var buffer = new byte[dataLength];
                memoryStream.Position = Offset;
                
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, Offset, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        
    }
}