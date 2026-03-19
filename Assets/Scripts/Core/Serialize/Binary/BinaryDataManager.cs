using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Singleton;
using Core.Utility;
using Newtonsoft.Json;

namespace Core.Serialize.Binary
{
    /// <summary>
    /// 二进制数据管理器
    /// </summary>
    public class BinaryDataManager : SingletonBase<BinaryDataManager>, IBinaryDataManager
    {
        public override int InitPriority => 0;
        // 配置加载类型到加载器的映射
        private readonly Dictionary<EConfigLoadType, IConfigLoader> typeToLoaderMap = new();

        private BinaryDataManager()
        {

        }

        public override Task InitAsync()
        {
            typeToLoaderMap.Add(EConfigLoadType.Excel, new ExcelConfigLoader());
            typeToLoaderMap.Add(EConfigLoadType.Editor, new EditorConfigLoader());
            return Task.CompletedTask;
        }

        public async Task LoadConfigAsync(string abName)
        {
            foreach (var loader in typeToLoaderMap.Values)
            {
                await loader.LoadConfig(abName);
            }
        }
        
        public T GetConfig<T>(EConfigLoadType loadType) where T : class
        {
            return typeToLoaderMap[loadType].GetConfig<T>();
        }

        public void AddConfig(EConfigLoadType loadType, Func<IConfigLoader, Task> onConfigLoaded)
        {
            var loader = typeToLoaderMap[loadType];
            loader.OnConfigLoaded += onConfigLoaded;
        }
        
        public Task SaveAsync(string fileName, object obj)
        {
            var jsonStr = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);
            return File.WriteAllBytesAsync(PathUtility.GetUserDataLocalSavePath(fileName), bytes);
        }
        
        public void Save(string fileName, object obj)
        {
            var jsonStr = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);
            File.WriteAllBytes(PathUtility.GetUserDataLocalSavePath(fileName), bytes);
        }

        public async Task<T> LoadAsync<T>(string fileName) where T : new()
        {
            if (!File.Exists(PathUtility.GetUserDataLocalSavePath(fileName))) return new T();
            var bytes = await File.ReadAllBytesAsync(PathUtility.GetUserDataLocalSavePath(fileName));
            var jsonStr = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        
        public T Load<T>(string fileName) where T : new()
        {
            if (!File.Exists(PathUtility.GetUserDataLocalSavePath(fileName))) return new T();
            var bytes = File.ReadAllBytes(PathUtility.GetUserDataLocalSavePath(fileName));
            var jsonStr = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
