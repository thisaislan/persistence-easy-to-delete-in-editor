using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal class PedeFile
    {

        internal static void Serialize<T>(T value, Action<byte[]> actionAfterSerialize)
        {
            var compressedValue = GetCompressedStringValue(value);
            var bytes = SerializeBytes(compressedValue);

            actionAfterSerialize(bytes);
        }

        public static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize)
        {
            var decompressedValue = StringCompressor.DecompressString(DeserializeBytes(value));
            var obj = JsonUtility.FromJson<T>(decompressedValue);

            actionAfterDeserialize(obj);
        }

        internal static void SetFile<T>(string key, T value)
        {
            var filePath = GetFullPath(key);
            
            Directory.CreateDirectory(Constants.Consts.PedeFileRootFolderName);
            
            File.Create(filePath).Close();
            
            var compressedValue = GetCompressedStringValue(value);
            var bytes = SerializeBytes(compressedValue);
            
            File.WriteAllBytes(filePath, bytes);
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
                File.Open(filePath, FileMode.Open).Close();
                
                var decompressedValue = StringCompressor.DecompressString(DeserializeBytes(File.ReadAllBytes(filePath)));
                var obj = JsonUtility.FromJson<T>(decompressedValue);

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

        private static byte[] SerializeBytes(string value) =>
            Encoding.UTF8.GetBytes(value);

        private static string DeserializeBytes(byte[]  bytes) =>
            Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        private static string GetFullPath(string key) =>
            String.Format(Constants.Consts.PedeFilePethFormat, Constants.Consts.PedeFileRootFolderName, key);

    }
}