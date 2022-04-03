using System;
using System.Collections.Generic;
using System.Linq;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data
{
    /// <summary> ScriptableObject containing data used in editor by Pede and methods to handle them. </summary>
    [
        CreateAssetMenu(
            fileName = Metadata.PedeDataScriptableObjectFileName,
            menuName = Metadata.PedeDataScriptableObjectMenuName,
            order = Metadata.PedeDataScriptableObjectOrder
        )
    ]
    public class PedeData : ScriptableObject
    {
        /// <summary> Struct that stores information to be saved and used in editor by Pede. </summary>
        [Serializable]
        public struct Data
        {
            /// <summary>
            ///   Key used to identify
            ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data.PedeData.Data" />.
            /// </summary>
            public string key; 
            
            /// <summary>
            ///   The
            ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data.PedeData.Data" />.
            ///   type.
            /// </summary>
            public string type;
            
            /// <summary>
            ///   Serialized
            ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data.PedeData.Data" />
            ///   value.
            /// </summary>
            [TextArea(Metadata.PedeDataValueMinLines, Metadata.PedeDataValueMaxLines)]
            public string value;
            
            /// <summary>
            ///   Checks if
            ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data.PedeData.Data" />
            ///   key is null.
            /// </summary>
            public bool IsKeyNull() =>
                key == null;
            
            /// <summary> Checks if
            ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data.PedeData.Data" />
            ///   key and type are equals to <paramref name="key"/> and <paramref name="type"/> passed.
            /// </summary>
            /// <param name="key"> Key used to the comparison. </param>
            /// <param name="tyoe"> Type used to the comparison. </param>
            public bool IsSameValue(string key, string type) =>
                this.key.Equals(key) && this.type.Equals(type);
        }
        
        /// <summary> Stores all PlayerPrefs data used in editor. </summary>
        public List<Data> playerPrefData = new List<Data>();
        
        /// <summary> Stores all File data used in editor. </summary>
        public List<Data>  fileData = new List<Data>();

        #region PlayerPrefsRegion
        
        /// <summary> Saves PlayerPrefs data with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the PlayerPrefs. It cannot be null. </param>
        /// <param name="value"> Value to be saved. It cannot be null. </param>
        /// <remarks> It cannot save engine type objects</remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and value cannot be null.
        /// </exception>
        public void SetPlayerPrefs<T>(string key, T value)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
            var data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                var index = playerPrefData.IndexOf(data);
                
                playerPrefData.Remove(data);
                playerPrefData.Insert(index, CreatePlayerPrefsData(key, value));
            }
            else
            {
                playerPrefData.Add(CreatePlayerPrefsData(key, value));
            }

            PersistAsset();
        }

        /// <summary> Loads PlayerPrefs data with a specific key and type. </summary>
        /// <param name="key"> Key used to get the PlayerPrefs. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the PlayerPrefs exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the PlayerPrefs does not exist. Default is null
        /// </param>
        /// <param name="destroyAfter"> If true, deletes file after. Default is false </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionIfHasResult cannot be null.
        /// </exception>
        public void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
            var data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetPlayerPrefsData(data.value,  actionIfHasResult);
                
                if (destroyAfter) { DeletePlayerPrefs<T>(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        /// <summary> Deletes PlayerPrefs data with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the PlayerPrefs. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public void DeletePlayerPrefs<T>(string key)
        {
            RemovePlayerPrefsData<T>(key);
            PersistAsset();
        }

        /// <summary> Deletes all PlayerPrefs key and values by calling PlayerPrefs.DeleteAll() </summary>
        public void DeleteAllPlayerPrefs()
        {
            playerPrefData.Clear();
            PersistAsset();
        }

        /// <summary> Returns true if the given key exists in PlayerPrefs data, otherwise returns false. </summary>
        /// <param name="key"> Key used to check the PlayerPrefs. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
            actionWithResult.Invoke(ExistsData(playerPrefData, key, GetTypeName(typeof(T))));
        }

        private Data CreatePlayerPrefsData<T>(string key, T value) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value = GetPlayerPrefsValue(value)
            };
        
        private string GetPlayerPrefsValue<T>(T value)
        {
            //string
            if (typeof(T) == typeof(String)) { return Convert.ToString(value); }
            //int
            else if (typeof(T) == typeof(Int32)) { return Convert.ToString(value); }
            //bool
            else if (typeof(T) == typeof(Boolean)) { return Convert.ToString(value); }
            //char
            else if (typeof(T) == typeof(Char)) { return Convert.ToString(value); }
            //float
            else if (typeof(T) == typeof(Single)) { return Convert.ToString(value); }
            //others
            else { return JsonUtility.ToJson(value, true); }
        }

        private void GetPlayerPrefsData<T>(string value, Action<T> actionWithResult)
        {
            //string
            if (typeof(T) == typeof(String))
            {
                GetStringPlayerPrefs(value, actionWithResult as Action<string>);
            }
            //int
            else if (typeof(T) == typeof(Int32))
            {
                GetIntPlayerPrefs(Convert.ToInt32(value), actionWithResult as Action<int>);
            }
            //bool
            else if (typeof(T) == typeof(Boolean))
            {
                GetBooleanPlayerPrefs(Convert.ToBoolean(value), actionWithResult as Action<bool>);
            }
            //char
            else if (typeof(T) == typeof(Char))
            {
                GetCharPlayerPrefs(value, actionWithResult as Action<char>);
            }
            //float
            else if (typeof(T) == typeof(Single))
            {
                GetFloatPlayerPrefs(Convert.ToSingle(value), actionWithResult as Action<Single>);
            }
            //others
            else
            {
                GetObject(JsonUtility.FromJson<T>(value), actionWithResult);
            }
        }
        
        private void GetStringPlayerPrefs(string value, Action<string> actionWithResult) =>
            actionWithResult.Invoke(value);

        private void GetIntPlayerPrefs(int value, Action<int> actionWithResult) =>
            actionWithResult.Invoke(value);

        private void GetBooleanPlayerPrefs(bool value, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(value);
        private void GetCharPlayerPrefs(string value, Action<char> actionWithResult) =>
            actionWithResult.Invoke(value.First());

        private void GetFloatPlayerPrefs(float value, Action<float> actionWithResult) =>
            actionWithResult.Invoke(value);

        private Data GetFirstPlayerPrefsDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(playerPrefData, key, GetTypeName(typeof(T)));

        private void RemovePlayerPrefsData<T>(string key) =>
            RemoveData(playerPrefData, key, GetTypeName(typeof(T)));
        
        #endregion //PlayerPrefsRegion
        
        #region FileRegion
        
        /// <summary> Saves file data with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the file. </param>
        /// <param name="value"> Object to be saved or a engine type. </param>
        /// <remarks> It cannot save engine type objects. </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and value cannot be null.
        /// </exception>
        public void SetFile<T>(string key, T value)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
            var data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                var index = fileData.IndexOf(data);
                
                fileData.Remove(data);
                fileData.Insert(index, CreateFileData(key, value));
            }
            else
            {
                fileData.Add(CreateFileData(key, value));
            }

            PersistAsset();
        }
        
        /// <summary> Loads file data with a specific key and type. </summary>
        /// <param name="key"> Key used to get the file. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the file exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the file does not exist. Default is null
        /// </param>
        /// <param name="destroyAfter"> If true, deletes file after. Default is false </param>
        /// <remarks> It cannot save engine type objects</remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionIfHasResult cannot be null.
        /// </exception>
        public void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
            var data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetFileData(data.value,  actionIfHasResult);
                
                if (destroyAfter) { DeleteFile<T>(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }
        
        /// <summary> Deletes file data with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the file. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public void DeleteFile<T>(string key)
        {
            CheckKeyAsNull(key);
            RemoveFile<T>(key);
            PersistAsset();
        }
        
        /// <summary> Deletes all files data saved. </summary>
        public void DeleteAllFiles()
        {
            fileData.Clear();
            PersistAsset();
        }

        /// <summary> Checks if exists a file data with a specific key and type. </summary>
        /// <param name="key"> Key used to save the file. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public void  HasFileKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
            actionWithResult.Invoke(ExistsData(fileData, key, GetTypeName(typeof(T))));
        }

        private void RemoveFile<T>(string key) =>
            RemoveData(fileData, key, GetTypeName(typeof(T)));
        
        private void GetFileData<T>(string value, Action<T> actionWithResult) =>
            GetObject(JsonUtility.FromJson<T>(value), actionWithResult);

        private Data GetFirstFileDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(fileData, key, GetTypeName(typeof(T)));
        
        private Data CreateFileData<T>(string key, T value) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value =  JsonUtility.ToJson(value, true)
            };
        
        #endregion //FileRegion
        
        /// <summary> Deletes all saved data. </summary>
        public void DeleteAll()
        {
            DeleteAllPlayerPrefs();
            DeleteAllFiles();
            PersistAsset();
        }
        
        private void GetObject<T>(T value, Action<T> actionIfHasResult) =>
            actionIfHasResult.Invoke(value);
        
        private bool ExistsData(List<Data> dataList, string key, string typeName) =>
            dataList.Exists(data => data.IsSameValue(key, typeName));

        private void RemoveData(List<Data> dataList, string key, string typeName) =>
            dataList.RemoveAll(data => data.IsSameValue(key, typeName));

        private Data GetFirstDataOrDefault(List<Data> daraList, string key, string typeName) =>
            daraList.FirstOrDefault(data => data.IsSameValue(key, typeName));

        private string GetTypeName(Type type) =>
            type.ToString();
        
        private void CheckKeyAsNull(string key) =>
            CheckArgumentAsNull(key, nameof(key));
        
        private void CheckValueAsNull<T>(T value) =>
            CheckArgumentAsNull(value, nameof(value));
        
        private void CheckActionAsNull<T>(Action<T> actionIfHasResult) =>
            CheckArgumentAsNull(actionIfHasResult, nameof(actionIfHasResult));
        
        private void CheckArgumentAsNull<T>(T argument, string argumentName)
        {
            if (argument == null) { throw new ArgumentNullException(nameof(argumentName)); }
        }

        private void PersistAsset()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}