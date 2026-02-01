using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Service;
using Core.Singleton;
using UnityEngine;

namespace Core.DataPersistence.Json
{
    /// <summary>
    /// Json���ݹ����� ��Ҫ����Json�����л�(�洢)�ͷ����л�(��ȡ)
    /// </summary>
    public class JsonManager : SingletonBase<JsonManager>, IJsonManager
    {
        //�洢����Json����   ����������  ֵ������
        private readonly Dictionary<string, object> _jsonDic = new Dictionary<string, object>();

        private JsonManager()
        {

        }

        /// <summary>
        /// ����Json����
        /// </summary>
        public async Task LoadJsonAsync()
        {
            //LoadJsonAsync<T, K>()
            //...
            await Task.FromResult(true);
        }

        /// <summary>
        /// ����Json����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <typeparam name="K">���ݽṹ����</typeparam>
        private async Task LoadJsonAsync<T, K>() where T : class where K : class
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            T container = null;
            var textAsset = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<TextAsset>(EAssetBundleType.GameConfig, $"{typeof(K).Name}.json");
            //ת��Ϊ����
            container = JsonUtility.FromJson<T>(textAsset.text);
            //�洢
            _jsonDic.Add(typeof(T).Name, container);
#else
            //ͬ����ȡ
            TextAsset textAsset = EditorResManager.Instance.LoadEditorAsset<TextAsset>($"{typeof(K).Name}", ".json");
            T container = JsonUtility.FromJson<T>(textAsset.text);
            _jsonDic.Add(typeof(T).Name, container);
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// ��ȡJson����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <returns></returns>
        public T GetJsonData<T>() where T : class
        {
            if (_jsonDic.ContainsKey(typeof(T).Name))
                return _jsonDic[typeof(T).Name] as T;
            return null;
        }

        /// <summary>
        /// ��Jsonת��Ϊ����
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
                _ => new T()
            };
        }

        /// <summary>
        /// �첽��Jsonת��Ϊ����
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
                _ => new T()
            };
        }

        /// <summary>
        /// �Ӷ���ת��ΪJson
        /// </summary>
        /// <param name="data"></param>
        /// <param name="saveFilePath"></param>
        /// <param name="type"></param>
        public void SaveToJson(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            //���л�
            string jsonStr = "";
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data, true);
                    break;
            }
            File.WriteAllText(saveFilePath, jsonStr);
        }

        /// <summary>
        /// �Ӷ���ת��ΪJson
        /// </summary>
        /// <param name="data"></param>
        /// <param name="saveFilePath">����·��</param>
        /// <param name="type"></param>
        public async Task SaveToJsonAsync(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            //���л�
            string jsonStr = "";
            switch (type)
            {
                case E_JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data, true);
                    break;
            }
            await File.WriteAllTextAsync(saveFilePath, jsonStr);
        }
    }
}
