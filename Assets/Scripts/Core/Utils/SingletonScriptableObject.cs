using UnityEngine;

namespace Core.Utils
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private const string DefaultPath = "SingletonScriptableObjects/";

        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<T>(GetDefaultResourcesPath());

#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        // make sure directory exists
                        string folderPath = "Assets/Resources/" + DefaultPath;
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                            UnityEditor.AssetDatabase.Refresh();
                        }
                        
                        _instance = CreateInstance<T>();
                        UnityEditor.AssetDatabase.CreateAsset(_instance, GetAssetPath());
                        UnityEditor.AssetDatabase.SaveAssets();
                        Debug.Log("Created " + typeof(T).Name + " at " + GetAssetPath());
                    }
#endif
                }

                return _instance;
            }
        }

        public static string GetDefaultResourcesPath()
        {
            return $"{DefaultPath}{typeof(T).Name}";
        }

        private static string GetAssetPath()
        {
            return $"Assets/Resources/{DefaultPath}{typeof(T).Name}.asset";
        }
    }
}
