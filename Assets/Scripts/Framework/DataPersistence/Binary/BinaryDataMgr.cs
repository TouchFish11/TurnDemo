using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 二进制数据管理器
    /// </summary>
    public class BinaryDataMgr : SingletonBase<BinaryDataMgr>
    {
        private readonly Dictionary<E_ConfigLoadType, IConfigLoader> typeToLoaderMap = new Dictionary<E_ConfigLoadType, IConfigLoader>();

        private BinaryDataMgr()
        {
            typeToLoaderMap.Add(E_ConfigLoadType.Excel, new ExcelConfigLoader());
            typeToLoaderMap.Add(E_ConfigLoadType.Editor, new EditorConfigLoader());
        }

        /// <summary>
        /// 加载本地配置
        /// </summary>
        /// <returns></returns>
        public async Task LoadConfig()
        {
            foreach (var loader in typeToLoaderMap.Values)
            {
                await loader.LoadConfig();
            }
        }

        /// <summary>
        /// 获取配置数据数据
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <param name="loadType"></param>
        /// <returns></returns>
        public T GetConfig<T>(E_ConfigLoadType loadType) where T : class
        {
            return typeToLoaderMap[loadType].GetConfig<T>();
        }

        /// <summary>
        /// 以二进制存储数据
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="fileName">文件名</param>
        public static void Save(string fileName, object obj)
        {
            using FileStream fs = new FileStream(PathManager.GetUserDataLocalSavePath(fileName), FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }

        /// <summary>
        /// 加载二进制数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static T Load<T>(string fileName) where T : new()
        {
            if (!File.Exists(PathManager.GetUserDataLocalSavePath(fileName)))
            {
                LogManager.Log($"未找到该路径的二进制数据文件：{fileName}，已返回默认值");
                return new();
            }

            T dataObj;
            using (FileStream fs = File.Open(PathManager.GetUserDataLocalSavePath(fileName), FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                dataObj = (T)bf.Deserialize(fs);
                fs.Close();
            }

            if (dataObj == null)
            {
                LogManager.Log($"二进制数据文件反序列化失败：{fileName}，已返回默认值");
                return new();
            }

            return dataObj;
        }
    }
}
