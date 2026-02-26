using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.DataPersistence.Json
{
    /// <summary>
    /// Json�������ӿ�
    /// </summary>
    public interface IJsonManager
    {
        /// <summary>
        /// 将JSON字符串反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">要反序列化的目标类型（必须有无参构造函数）</typeparam>
        /// <param name="json">待反序列化的JSON字符串</param>
        /// <param name="jsonType">JSON解析器类型（默认使用Unity的JsonUtility）</param>
        /// <param name="settings">使用非Newtonsoft.Json时忽略；使用Newtonsoft.Json忽略，则默认使用全局设置</param>
        /// <returns>反序列化后的T类型对象；若JSON为空/无效，返回T的新实例</returns>
        T FromJson<T>(string json, E_JsonType jsonType = E_JsonType.Newtonsoft, JsonSerializerSettings settings = null) where T : new();

        /// <summary>
        /// 异步从指定文件读取JSON字符串，并反序列化为指定类型的对象
        /// （非阻塞IO，适合大文件/频繁读取场景）
        /// </summary>
        /// <typeparam name="T">要反序列化的目标类型（必须有无参构造函数）</typeparam>
        /// <param name="path">JSON文件的完整路径</param>
        /// <param name="jsonType">JSON解析器类型（默认使用Unity的JsonUtility）</param>
        /// <param name="settings">使用非Newtonsoft.Json时忽略；使用Newtonsoft.Json忽略，则默认使用全局设置</param>
        /// <returns>
        /// 异步任务结果：反序列化后的T类型对象；
        /// 若文件不存在/JSON为空/无效，返回T的新实例
        /// </returns>
        /// <exception cref="IOException">文件读取时可能抛出IO异常（如权限不足）</exception>
        Task<T> FromJsonAsync<T>(string path, E_JsonType jsonType = E_JsonType.Newtonsoft, JsonSerializerSettings settings = null) where T : new();

        /// <summary>
        /// 将对象序列化为JSON字符串并同步保存到指定文件
        /// （阻塞IO，适合小文件/低频保存场景）
        /// </summary>
        /// <param name="data">待序列化的对象（需符合JsonUtility序列化规则）</param>
        /// <param name="saveFilePath">保存JSON文件的完整路径（包含文件名和扩展名）</param>
        /// <param name="type">JSON序列化器类型（默认使用Unity的JsonUtility）</param>
        /// <param name="settings">使用非Newtonsoft.Json时忽略；使用Newtonsoft.Json忽略，则默认使用全局设置</param>
        /// <exception cref="IOException">文件写入时可能抛出IO异常（如路径不存在/权限不足）</exception>
        void SaveToJson(object data, string saveFilePath, E_JsonType type = E_JsonType.Newtonsoft, JsonSerializerSettings settings = null);

        /// <summary>
        /// 将对象序列化为JSON字符串并异步保存到指定文件
        /// （非阻塞IO，适合大文件/频繁保存场景）
        /// </summary>
        /// <param name="data">待序列化的对象（需符合JsonUtility序列化规则）</param>
        /// <param name="saveFilePath">保存JSON文件的完整路径（包含文件名和扩展名）</param>
        /// <param name="type">JSON序列化器类型（默认使用Unity的JsonUtility）</param>
        /// <param name="settings">使用非Newtonsoft.Json时忽略；使用Newtonsoft.Json忽略，则默认使用全局设置</param>
        /// <returns>异步任务（无返回值）</returns>
        /// <exception cref="IOException">文件异步写入时可能抛出IO异常（如路径不存在/权限不足）</exception>
        Task SaveToJsonAsync(object data, string saveFilePath, E_JsonType type = E_JsonType.Newtonsoft, JsonSerializerSettings settings = null);

        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="settings">使用非Newtonsoft.Json时忽略；使用Newtonsoft.Json忽略，则默认使用全局设置</param>
        /// <returns>格式化后的Json字符串</returns>
        string ToJson(object data, E_JsonType type = E_JsonType.Newtonsoft, JsonSerializerSettings settings = null);
    }
}
