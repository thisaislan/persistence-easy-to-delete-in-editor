using System;
using System.Linq;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal class PedePlayerPrefs
    {
        private const int DefaultBooleanTrueValue = 1;
        private const int DefaultBooleanFalseValue = 0;
        
        internal void DeletePlayerPrefsByKey(string key, bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteKey(key);

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }
        
        internal void DeleteAllPlayerPrefs( bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteAll();

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }

        internal void HasPlayerPrefsKey(string key, Action<bool> actionWithResult) => 
            actionWithResult.Invoke(PlayerPrefs.HasKey(key));

        internal void SavePlayerPrefs() =>
            PlayerPrefs.Save();

        internal void SetPlayerPrefs<T>(string key, T value, Pede.PlayerPrefsSetMode playerPrefsSetMode)
        {
            SetPlayerPrefs(key, value);

            if (playerPrefsSetMode != Pede.PlayerPrefsSetMode.Normal)
            {
                SavePlayerPrefs();
            }
        }
        
        internal void GetPlayerPrefs<T>(
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
        
        private void SetPlayerPrefs<T>(string key, T value)
        {
            //string
            if (typeof(T) == typeof(String)) { SetStringPlayerPrefs(key, Convert.ToString(value)); }
            //int
            else if (typeof(T) == typeof(Int32)) { SetIntPlayerPrefs(key, Convert.ToInt32(value)); }
            //bool
            else if (typeof(T) == typeof(Boolean)) { SetBooleanPlayerPrefs(key, Convert.ToBoolean(value)); }
            //char
            else if (typeof(T) == typeof(Char)) { SetCharPlayerPrefs(key, Convert.ToString(value)); }
            //float
            else if (typeof(T) == typeof(Single)) { SetFloatPlayerPrefs(key, Convert.ToSingle(value)); }
            //others
            else { SetObjectPlayerPrefs(key, JsonUtility.ToJson(value)); }
        }
        
        private void GetPlayerPrefs<T>(string key, Action<T> actionWithResult)
        {
            //string    
            if (typeof(T) == typeof(String)) { GetStringPlayerPrefs(key, actionWithResult as Action<string>); }
            //int
            else if (typeof(T) == typeof(Int32)) { GetIntPlayerPrefs(key, actionWithResult as Action<int>); }
            //bool
            else if (typeof(T) == typeof(Boolean)) { GetBooleanPlayerPrefs(key, actionWithResult as Action<bool>); }
            //char
            else if (typeof(T) == typeof(Char)) { GetCharPlayerPrefs(key, actionWithResult as Action<char>); }
            //float
            else if (typeof(T) == typeof(Single)) { GetFloatPlayerPrefs(key, actionWithResult as Action<float>); }
            //others
            else { GetObjectPlayerPrefs(key, actionWithResult); }
        }

        private void GetStringPlayerPrefs(string key, Action<string> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.GetString(key, default));

        private void GetIntPlayerPrefs(string key, Action<int> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.GetInt(key, default));

        private void GetBooleanPlayerPrefs(string key, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.GetInt(key, DefaultBooleanFalseValue) == DefaultBooleanTrueValue);
        private void GetCharPlayerPrefs(string key, Action<char> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.GetString(key, default).First());

        private void GetFloatPlayerPrefs(string key, Action<float> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.GetFloat(key, default));

        private void GetObjectPlayerPrefs<T>(string key, Action<T> actionIfHasResult)
        {
            var obj = JsonUtility.FromJson<T>(PlayerPrefs.GetString(key, default));
            
            actionIfHasResult.Invoke(obj);
        }

        private void SetStringPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);

        private void SetIntPlayerPrefs(string key, int value) =>
            PlayerPrefs.SetInt(key, value);

        private void SetBooleanPlayerPrefs(string key, bool value) =>
            PlayerPrefs.SetInt(key, value? DefaultBooleanTrueValue: DefaultBooleanFalseValue);

        private void SetCharPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);

        private void SetFloatPlayerPrefs(string key, float value) =>
            PlayerPrefs.SetFloat(key, value);

        private void SetObjectPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);
        
    }
}