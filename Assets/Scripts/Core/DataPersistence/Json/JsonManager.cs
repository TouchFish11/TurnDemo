using System.IO;
using System.Threading.Tasks;
using Core.Singleton;
using UnityEngine;

namespace Core.DataPersistence.Json
{
    /// <summary>
    /// JSON数据管理单例类
    /// 负责JSON数据的序列化（保存）和反序列化（读取）操作
    /// 实现IJsonManager接口，基于Unity的JsonUtility封装
    /// </summary>
    public class JsonManager : SingletonBase<JsonManager>, IJsonManager
    {
        /// <summary>
        /// 私有构造函数（单例模式）
        /// 防止外部实例化，保证全局唯一实例
        /// </summary>
        private JsonManager()
        {
        }
        
        public T FromJson<T>(string json, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new()
        {
            // 空值校验：JSON字符串为空时返回默认实例
            if (string.IsNullOrEmpty(json))
            {
                return new T();
            }

            // 根据解析器类型执行反序列化
            return jsonType switch
            {
                E_JsonType.JsonUtlity => JsonUtility.FromJson<T>(json),
                _ => new T() // 未知解析器类型时返回默认实例
            };
        }
        
        public async Task<T> FromJsonAsync<T>(string path, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new()
        {
            // 文件存在性校验：文件不存在时返回默认实例
            if (!File.Exists(path))
            {
                return new T();
            }

            // 异步读取文件内容（非阻塞）
            var json = await File.ReadAllTextAsync(path);
            // JSON字符串空值校验
            if (string.IsNullOrEmpty(json))
            {
                return new T();
            }

            // 根据解析器类型执行反序列化
            return jsonType switch
            {
                E_JsonType.JsonUtlity => JsonUtility.FromJson<T>(json),
                _ => new T() // 未知解析器类型时返回默认实例
            };
        }
        
        public void SaveToJson(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            // 根据序列化器类型执行序列化（格式化输出）
            var jsonStr = type switch
            {
                E_JsonType.JsonUtlity => JsonUtility.ToJson(data, true),
                _ => ""
            };

            // 同步写入文件
            File.WriteAllText(saveFilePath, jsonStr);
        }
        
        public async Task SaveToJsonAsync(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity)
        {
            // 根据序列化器类型执行序列化（格式化输出）
            var jsonStr = type switch
            {
                E_JsonType.JsonUtlity => JsonUtility.ToJson(data, true),
                _ => ""
            };

            // 异步写入文件
            await File.WriteAllTextAsync(saveFilePath, jsonStr);
        }
    }
}