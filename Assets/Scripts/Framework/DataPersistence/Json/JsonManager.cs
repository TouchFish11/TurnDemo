using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        public async Task LoadJsonAsync()
        {
            //LoadJsonAsync<T, K>()
            //...
            await Task.FromResult(true);
        }

        /// <summary>
        /// 加载Json数据
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <typeparam name="K">数据结构类名</typeparam>
        private async Task LoadJsonAsync<T, K>() where T : class where K : class
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            T container = null;
            TextAsset textAsset = await AssetBundleManager.Instance.LoadAssetAsync<TextAsset>(E_AssetBundleType.Json, $"{typeof(K).Name}.json");
            //转换为对象
            container = JsonUtility.FromJson<T>(textAsset.text);
            //存储
            _jsonDic.Add(typeof(T).Name, container);
#else
            //同步读取
            TextAsset textAsset = EditorResMgr.Instance.LoadEditorAsset<TextAsset>($"{typeof(K).Name}", ".json");
            T container = JsonUtility.FromJson<T>(textAsset.text);
            _jsonDic.Add(typeof(T).Name, container);
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 获取Json数据
        /// </summary>
        /// <typeparam name="T">容器类型</typeparam>
        /// <returns></returns>
        public T GetJsonData<T>() where T : class
        {
            if (_jsonDic.ContainsKey(typeof(T).Name))
                return _jsonDic[typeof(T).Name] as T;
            return null;
        }

        /// <summary>
        /// 从Json转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonType"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public T FromJson<T>(string json, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new()
        {
            if (string.IsNullOrEmpty(json))
            {
                return new T();
            }

            return jsonType switch
            {
                E_JsonType.JsonUtlity => JsonUtility.FromJson<T>(json),
                E_JsonType.LitJson => JsonMapper.ToObject<T>(json),
                _ => new T()
            };
        }

        /// <summary>
        /// 异步从Json转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="jsonType"></param>
        /// <returns></returns>
        public async Task<T> FromJsonAsync<T>(string path, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new()
        {
            if (!File.Exists(path))
            {
                return new T();
            }

            string json = await File.ReadAllTextAsync(path);
            if (string.IsNullOrEmpty(json))
            {
                return new T();
            }

            return jsonType switch
            {
                E_JsonType.JsonUtlity => JsonUtility.FromJson<T>(json),
                E_JsonType.LitJson => JsonMapper.ToObject<T>(json),
                _ => new T()
            };
        }

        /// <summary>
        /// 从对象转换为Json
        /// </summary>
        /// <param name="data"></param>
        /// <param name="saveFilePath"></param>
        /// <param name="type"></param>
        public void ToJson(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            //序列化
            string jsonStr = "";
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data, true);
                    break;
                case E_JsonType.LitJson:
                    jsonStr = JsonMapper.ToJson(data);
                    break;
            }
            File.WriteAllText(saveFilePath, jsonStr);
        }

        /// <summary>
        /// 从对象转换为Json
        /// </summary>
        /// <param name="data"></param>
        /// <param name="saveFilePath">绝对路径</param>
        /// <param name="type"></param>
        public async Task ToJsonAsync(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            //序列化
            string jsonStr = "";
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data, true);
                    break;
                case E_JsonType.LitJson:
                    jsonStr = JsonMapper.ToJson(data);
                    break;
            }
            await File.WriteAllTextAsync(saveFilePath, jsonStr);
        }
    }
}
