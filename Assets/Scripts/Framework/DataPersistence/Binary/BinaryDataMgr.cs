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
        // 存储所有表数据的字典，键：容器名  值：容器
        private readonly Dictionary<string, object> _tableDic = new Dictionary<string, object>();

        private BinaryDataMgr()
        {
            QuitHandler.Instance.OnAppQuit += OnApplicationQuit;
        }

        /// <summary>
        /// 初始化表数据
        /// </summary>
        public async Task InitTableAsync()
        {
            //yield return LoadTableAsync<T, K>()
            //...
            await LoadTableAsync<DialogueInfoContainer, DialogueInfo>();
            await LoadTableAsync<BranchInfoContainer, BranchInfo>();
            await Task.FromResult(true);
        }

        /// <summary>
        /// 加载表配置信息
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <typeparam name="K">数据结构类名</typeparam>
        /// <returns></returns>
        private async Task LoadTableAsync<T, K>()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 异步加载数据
            TextAsset tInfo = await AssetBundleManager.Instance.LoadAssetAsync<TextAsset>(E_AssetBundleType.TableInfo, typeof(K).Name + ".tInfo.txt");
#else
            // 加载编辑器数据
            TextAsset tInfo = EditorResMgr.Instance.LoadEditorAsset<TextAsset>(typeof(K).Name + ".tInfo");
#endif
            // 转换二进制到数据类
            ConvertFrom<T, K>(tInfo);
        }

        /// <summary>
        /// 获取单张表的数据
        /// </summary>
        /// <typeparam name="T">容器类名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            if (_tableDic.ContainsKey(typeof(T).Name))
                return _tableDic[typeof(T).Name] as T;
            return null;
        }

        /// <summary>
        /// 以二进制存储数据
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="fileName">文件名</param>
        public void Save(string fileName, object obj)
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
        public T Load<T>(string fileName) where T : new()
        {
            if (!File.Exists(PathManager.GetUserDataLocalSavePath(fileName)))
            {
                LogMgr.Log($"没有找到该路径的二进制数据文件：{fileName}，已返回默认值");
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
                LogMgr.Log("没有找到该路径的二进制数据文件，已返回默认值");
                return new();
            }

            return dataObj;
        }

        /// <summary>
        /// 从二进制中转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="textAsset"></param>
        private void ConvertFrom<T, K>(TextAsset textAsset)
        {
            //读取excel表对于的二进制文件
            byte[] bytes = textAsset.bytes;

            int index = 0;
            //读取有效数据内容行数量
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;

            //读取主键名字
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;

            //获取容器类Type
            Type containerType = typeof(T);
            //实例化容器类
            object containerObj = Activator.CreateInstance(containerType);
            //获取数据结构类Type
            Type dataType = typeof(K);
            //获取数据结构类所有字段
            FieldInfo[] fieldInfos = dataType.GetFields();
            //遍历所有行
            for (int i = 0; i < count; i++)
            {
                //实例化数据结构类
                object dataObj = Activator.CreateInstance(dataType);

                //遍历所有字段信息
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.FieldType == typeof(int))
                    {
                        fieldInfo.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        fieldInfo.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (fieldInfo.FieldType == typeof(bool))
                    {
                        fieldInfo.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (fieldInfo.FieldType == typeof(string))
                    {
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        fieldInfo.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }
                }

                //将dataObj存储进containerObj中
                //获取containerObj的字典变量
                object dicObj = containerType.GetField("dataDic").GetValue(containerObj);
                //获取该变量的Add方法信息
                MethodInfo methodInfo = dicObj.GetType().GetMethod("Add");
                //得到数据结构类对象中指定主键字段的值
                object keyValue = dataObj.GetType().GetField(keyName).GetValue(dataObj);
                methodInfo.Invoke(dicObj, new object[] { keyValue, dataObj });
            }
            //把读取完的表记录下来
            _tableDic.Add(typeof(T).Name, containerObj);
        }

        private Task OnApplicationQuit()
        {
            // 保存音乐数据
            if (GameDataMgr.Instance.MusicData != null)
            {
                Save(FileUtility.LocalMusicDataFileName, GameDataMgr.Instance.MusicData);
            }

            // 保存改键数据
            if (GameDataMgr.Instance.InputActionContainer != null)
            {
                Save(FileUtility.LocalInputDataFileName, GameDataMgr.Instance.InputActionContainer);
            }

            return Task.CompletedTask;
        }
    }
}
