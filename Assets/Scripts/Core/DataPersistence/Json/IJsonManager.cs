using System.Threading.Tasks;

namespace Core.DataPersistence.Json
{
    /// <summary>
    /// Json�������ӿ�
    /// </summary>
    public interface IJsonManager
    {
        T FromJson<T>(string json, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new();
        Task<T> FromJsonAsync<T>(string path, E_JsonType jsonType = E_JsonType.JsonUtlity) where T : new();
        T GetJsonData<T>() where T : class;
        Task LoadJsonAsync();
        void SaveToJson(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity);
        Task SaveToJsonAsync(object data, string saveFilePath, E_JsonType type = E_JsonType.JsonUtlity);
    }
}
