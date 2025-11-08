using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Json数据管理类 主要用于Json的序列化(存储)和反序列化(读取)
    /// </summary>
    public class JsonManager : SingletonBase<JsonManager>
    {
        //存储所有Json数据   键：容器名  值：容器
        private readonly Dictionary<string, object> _jsonDic = new Dictionary<string, object>();

        private JsonManager() { }

        /// <summary>
        /// 加载Json数据
        /// </summary>
        public IEnumerator LoadJsonData()
        {
            //LoadJson<T, K>()
            //...
            yield return null;
        }

        /// <summary>
        /// 加载Json数据
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <typeparam name="K">数据结构类名</typeparam>
        private IEnumerator LoadJson<T, K>() where T : class where K : class
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            T container = null;
            AssetBundleLoadManager.Instance.LoadAssetAsync<TextAsset>(E_AssetBundleType.Json, $"{typeof(K).Name}.json", (textAsset) =>
            {
                //转换为对象
                container = JsonUtility.FromJson<T>(textAsset.text);
                //存储
                _jsonDic.Add(typeof(T).Name, container);
            });
            yield return new WaitUntil(() => container != null);
#else
            //同步读取
            TextAsset textAsset = EditorResMgr.Instance.LoadEditorAsset<TextAsset>($"{typeof(K).Name}", ".json");
            T container = JsonUtility.FromJson<T>(textAsset.text);
            _jsonDic.Add(typeof(T).Name, container);
            yield return null;
#endif
        }

        /// <summary>
        /// 获取Json数据
        /// </summary>
        /// <typeparam name="T">容器类型</typeparam>
        /// <returns></returns>
        public T GetJsonData<T>() where T : class
        {
            if(_jsonDic.ContainsKey(typeof(T).Name))
                return _jsonDic[typeof(T).Name] as T;
            return null;
        }

        /// <summary>
        /// 保存为Json数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        public void SaveToJson(object data, string fileName, E_JsonType type = E_JsonType.LitJson)
        {
            //确定存储路径
            string path = Application.persistentDataPath + "/" + fileName + ".json";
            //序列化
            string jsonStr = "";
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data);
                    break;
                case E_JsonType.LitJson:
                    jsonStr = JsonMapper.ToJson(data);
                    break;
            }
            File.WriteAllText(path, jsonStr);
        }

        /// <summary>
        /// 读取Json数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名称</param>
        /// <param name="type">使用的Json库</param>
        /// <returns></returns>
        public T LoadJson<T>(string fileName, E_JsonType type = E_JsonType.JsonUtlity) where T : new()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            string path = PathManager.GetJsonRuntimeLoadPath($"{fileName}.json");
            if (!File.Exists(path))
            {
                return new T();
            }

            string jsonStr = File.ReadAllText(path);
            T data = default;
            //序列化
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    data = JsonUtility.FromJson<T>(jsonStr);
                    break;
                case E_JsonType.LitJson:
                    data = JsonMapper.ToObject<T>(jsonStr);
                    break;
            }
            return data;
#else
            string path = PathManager.GetJsonDebugLoadPath($"{fileName}.json");
            if (!File.Exists(path))
            {
                Debug.LogError($"路径不存在：{path}");
                return new T();
            }
            //读取文件
            string jsonStr = File.ReadAllText(path);
            //序列化
            return type switch
            {
                E_JsonType.JsonUtlity => JsonUtility.FromJson<T>(jsonStr),
                E_JsonType.LitJson => JsonMapper.ToObject<T>(jsonStr),
                _ => new T(),
            };
#endif
        }
    }
}
