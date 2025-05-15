using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Minicop.Game.GravityRave
{
    public static class JSONController
    {
        public static void Save(object item, string name)
        {
#if PLATFORM_ANDROID
            PlayerPrefs.SetString(name, ToJson(item));
#else
            Directory.CreateDirectory(Application.dataPath);
            File.WriteAllText(Application.dataPath + $"/{name}.json", ToJson(item));
#endif
        }
        public static void Load<T>(ref T item, string name)
        {
            try
            {
#if PLATFORM_ANDROID
                object obj = (object)item;
                obj = PlayerPrefs.GetString(name);
                item = (T)obj;
#else
                object obj = (object)item;
                FromJson(ref obj, File.ReadAllText(Application.dataPath + $"/{name}.json")); 
                item = (T)obj;
#endif
            }
            catch { Save(item, name); }
        }

        public static string ToJson(object item)
        {
            return JsonUtility.ToJson(item);
        }

        public static void FromJson<T>(ref T item, string json)
        {
            object obj = (object)item;
            JsonUtility.FromJsonOverwrite(json, obj);
            item = (T)obj;
        }
    }
}