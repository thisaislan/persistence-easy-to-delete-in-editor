using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal class PedeFile
    {

        internal static void Serialize<T>(T value, Action<byte[]> actionAfterSerialize)
        {
            var memoryStream = new MemoryStream();
            var compressedValue = GetCompressedStringValue(value);

            BinaryFormatterSerialize(memoryStream, compressedValue);
            
            actionAfterSerialize(memoryStream.ToArray());
        }

        public static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize)
        {
            var memoryStream = new MemoryStream();
            
            memoryStream.Write(value, 0, value.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
                
            var decompressedValue = StringCompressor.DecompressString(BinaryFormatterDeserialize(memoryStream));
            var obj = JsonUtility.FromJson<T>(decompressedValue);

            actionAfterDeserialize(obj);
        }

        internal static void SetFile<T>(string key, T value)
        {
            Directory.CreateDirectory(Constants.Consts.PedeFileRootFolderName);
            
            var file = File.Create(GetFullPath(key));
            var compressedValue = GetCompressedStringValue(value);

            BinaryFormatterSerialize(file, compressedValue);
            
            file.Close();
        }

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            bool destroyAfter)
        {
            var filePath = GetFullPath(key);
            
            if (File.Exists(filePath))
            {
                var file = File.Open(filePath, FileMode.Open);
                var decompressedValue = StringCompressor.DecompressString(BinaryFormatterDeserialize(file));
                var obj = JsonUtility.FromJson<T>(decompressedValue);
                
                file.Close();

                if (obj != null) { actionIfHasResult.Invoke(obj); }
                
                if (destroyAfter) { DeleteFile(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        internal static void DeleteFile(string key)
        {
            var filePath = GetFullPath(key);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        internal static void DeleteAllFiles()
        {
            if (Directory.Exists(Constants.Consts.PedeFileRootFolderName))
            {
                Directory.Delete(Constants.Consts.PedeFileRootFolderName, true);
            }
        }

        internal static void HasFileKey(string key, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(File.Exists(GetFullPath(key)));

        private static string GetCompressedStringValue<T>(T value)
        {
            var strigValue = JsonUtility.ToJson(value);
            
            return StringCompressor.CompressString(strigValue);
        }

        private static void BinaryFormatterSerialize(Stream serializationStream, string value)
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(serializationStream, value);
        }

        private static string BinaryFormatterDeserialize(Stream serializationStream)
        {
            var binaryFormatter = new BinaryFormatter();
            return (string)binaryFormatter.Deserialize(serializationStream);
        }

        private static string GetFullPath(string key) =>
            String.Format(Constants.Consts.PedeFilePethFormat, Constants.Consts.PedeFileRootFolderName, key);

    }
}