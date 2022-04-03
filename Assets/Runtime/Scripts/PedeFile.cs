using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal class PedeFile
    {
        
        internal static void SetFile<T>(string key, T value)
        {
            Directory.CreateDirectory(Constants.Consts.PedeFileRootFolderName);
            
            var binaryFormatter = new BinaryFormatter();
            var file = File.Create(GetFullPath(key));
            var strigValue = JsonUtility.ToJson(value);
        
            binaryFormatter.Serialize(file, strigValue);
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
                var binaryFormatter = new BinaryFormatter();
                var file = File.Open(filePath, FileMode.Open);
                var obj = JsonUtility.FromJson<T>((string)binaryFormatter.Deserialize(file));
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
        
        private static string GetFullPath(string key) =>
            String.Format(Constants.Consts.PedeFilePethFormat, Constants.Consts.PedeFileRootFolderName, key);

    }
}