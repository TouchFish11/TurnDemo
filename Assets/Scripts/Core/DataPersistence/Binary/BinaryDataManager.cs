using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Core.Log;
using Core.Singleton;
using Core.Utility;

namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// 二进制数据管理器
    /// </summary>
    public class BinaryDataManager : SingletonBase<BinaryDataManager>, IBinaryDataManager
    {
        // 配置加载类型到加载器的映射
        private readonly Dictionary<EConfigLoadType, IConfigLoader> typeToLoaderMap = new();

        private BinaryDataManager()
        {
            typeToLoaderMap.Add(EConfigLoadType.Excel, new ExcelConfigLoader());
            typeToLoaderMap.Add(EConfigLoadType.Editor, new EditorConfigLoader());
        }
        
        public async Task LoadConfig(string abName)
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
        
        public void Save(string fileName, object obj)
        {
            using var fs = new FileStream(PathUtility.GetUserDataLocalSavePath(fileName), FileMode.OpenOrCreate, FileAccess.Write);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }

        public T Load<T>(string fileName) where T : new()
        {
            if (!File.Exists(PathUtility.GetUserDataLocalSavePath(fileName)))
            {
                LogManager.Log($"未找到该文件:{fileName},已返回默认值ֵ");
                return new T();
            }

            using var fs = File.Open(PathUtility.GetUserDataLocalSavePath(fileName), FileMode.Open, FileAccess.Read);
            var bf = new BinaryFormatter();
            var dataObj = (T)bf.Deserialize(fs);
            fs.Close();

            return dataObj;
        }
    }
}
