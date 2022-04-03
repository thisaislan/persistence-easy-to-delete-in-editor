using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Settings;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo(Metadata.PedeInternalsVisibleToAssemblyName)]
namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor
{
    internal static class PedeEditor
    {
        private static PedeSettings pedeSettings;
        private static PedeData pedeData => pedeSettings.pedeData;

        static PedeEditor()
        {
            CheckFileSettings();
            CheckFileData();
        }

        #region StartRegion
        
        private static void CheckFileSettings()
        {
            var settingsPath = $"{Metadata.PedeSettingFolderPath}/{Metadata.PedeSettingFileName}";

            pedeSettings = AssetDatabase.LoadAssetAtPath<PedeSettings>(settingsPath);
            
            if (pedeSettings == null)
            {
                pedeSettings = CreateSettingsScriptableObjectAsset(Metadata.PedeSettingFolderPath, settingsPath);
            }
        }

        private static PedeSettings CreateSettingsScriptableObjectAsset(string folderPath, string settingsPath) =>
            CreateScriptableObjectAsset<PedeSettings>(folderPath, settingsPath);
        
        private static T CreateScriptableObjectAsset<T>(string directoryPath, string filePath) 
            where T : ScriptableObject
        {
            var scriptableObject = ScriptableObject.CreateInstance<T>();
            
            Directory.CreateDirectory(directoryPath);
            
            CreatAsset(scriptableObject, filePath);
            PersistAsset(scriptableObject);

            return scriptableObject;
        }
        
        private static void CreatAsset(Object asset, string path) =>
            AssetDatabase.CreateAsset(asset, path);

        private static void PersistAsset(UnityEngine.Object scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CheckFileData()
        {
            if (pedeSettings.pedeData == null)
            {
                var dataPath = $"{Metadata.PedeDataFolderPath}/{Metadata.PedeDataFileName}";
                var pedeData = AssetDatabase.LoadAssetAtPath<PedeData>(dataPath);
                
                if (pedeData == null)
                {
                    pedeData = CreateDataScriptableObjectAsset(Metadata.PedeDataFolderPath, dataPath);
                }
                
                pedeSettings.pedeData = pedeData;
                
                PersistAsset(pedeSettings);
            }
        }
        
        private static PedeData CreateDataScriptableObjectAsset(string directoryPath, string filePath) =>
            CreateScriptableObjectAsset<PedeData>(directoryPath, filePath);
        
        #endregion //StartRegion
        
        #region PlayerPrefsRegion
        
        internal static void SetPlayerPrefs<T>(string key, T value) =>
            pedeData.SetPlayerPrefs(key, value);

        internal static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            bool destroyAfter) =>
            pedeData.GetPlayerPrefs(key, actionIfHasResult, actionIfHasNotResult, destroyAfter);

        internal static void DeletePlayerPrefs<T>(string key) =>
            pedeData.DeletePlayerPrefs<T>(key);

        internal static void DeleteAllPlayerPrefs() =>
            pedeData.DeleteAllPlayerPrefs();

        internal static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult) =>
            pedeData.HasPlayerPrefsKey<T>(key, actionWithResult);

        #endregion //PlayerPrefsRegion
        
        #region FileRegion

        internal static void SetFile<T>(string key, T value) =>
            pedeData.SetFile(key, value);

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            bool destroyAfter) =>
            pedeData.GetFile(key, actionIfHasResult, actionIfHasNotResult, destroyAfter);

        internal static void DeleteFile<T>(string key) =>
            pedeData.DeleteFile<T>(key);

        internal static void DeleteAllFiles() =>
            pedeData.DeleteAllFiles();

        internal static void HasFileKey<T>(string key, Action<bool> actionWithResult) =>
            pedeData.HasFileKey<T>(key, actionWithResult);

        #endregion //FileRegion

    }
}