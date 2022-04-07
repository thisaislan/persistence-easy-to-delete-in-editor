using System;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal static class PedePlayerPrefs
    {
        
        internal static void DeletePlayerPrefsByKey(string key, bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteKey(key);

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }
        
        internal static void DeleteAllPlayerPrefs( bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteAll();

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }

        internal static void HasPlayerPrefsKey(string key, Action<bool> actionWithResult) => 
            actionWithResult.Invoke(PlayerPrefs.HasKey(key));

        internal static void SavePlayerPrefs() =>
            PlayerPrefs.Save();

        internal static void SetPlayerPrefs<T>(string key, T value, Pede.PlayerPrefsSetMode playerPrefsSetMode)
        {
            SetPlayerPrefs(key, value);

            if (playerPrefsSetMode != Pede.PlayerPrefsSetMode.Normal)
            {
                SavePlayerPrefs();
            }
        }
        
        internal static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Pede.PlayerPrefsGetMode playerPrefsGetMode)
        {
            HasPlayerPrefsKey(key, (result) =>
            {
                if (!result) { actionIfHasNotResult?.Invoke(); }
                else { GetPlayerPrefs(key, actionIfHasResult); }
            });

            if (playerPrefsGetMode != Pede.PlayerPrefsGetMode.Normal)
            {
                DeletePlayerPrefsByKey(key, playerPrefsGetMode == Pede.PlayerPrefsGetMode.DestructiveAndPersistent);
            }
        }
        
        private static void SetPlayerPrefs<T>(string key, T value)
        {
            //string
            if (typeof(T) == typeof(String) || 
                // Int
                typeof(T) == typeof(Int32) ||
                //bool
                typeof(T) == typeof(Boolean) ||
                //char
                typeof(T) == typeof(Char) ||
                //float
                typeof(T) == typeof(Single))
            {
                SetPlayerPrefsStringValue(key, Convert.ToString(value));   
            }
            //others
            else
            {
                SetPlayerPrefsStringValue(key, JsonUtility.ToJson(value));
            }
        }
        
        private static void GetPlayerPrefs<T>(string key, Action<T> actionWithResult)
        {
            var value = PlayerPrefs.GetString(key, default);
            var decompressedValue = StringCompressor.DecompressString(value);
            
            //string
            if (typeof(T) == typeof(String) || 
                // Int
                typeof(T) == typeof(Int32) ||
                //bool
                typeof(T) == typeof(Boolean) ||
                //char
                typeof(T) == typeof(Char) ||
                //float
                typeof(T) == typeof(Single))
            {
                GetPlayerPrefsValue(decompressedValue, actionWithResult);   
            }
            //others
            else
            {
                GetPlayerPrefsObject(decompressedValue, actionWithResult);
            }
        }

        private static void GetPlayerPrefsObject<T>(string decompressedValue, Action<T> actionWithResult) =>
            actionWithResult.Invoke(JsonUtility.FromJson<T>(decompressedValue));

        private static void GetPlayerPrefsValue<T>(string decompressedValue, Action<T> actionWithResult) =>
            actionWithResult.Invoke((T)Convert.ChangeType(decompressedValue, typeof(T)));

        private static void SetPlayerPrefsStringValue(string key, string value) =>
            SetCompressedPlayerPrefs(key, StringCompressor.CompressString(value));
        
        private static void SetCompressedPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);
        
    }
}