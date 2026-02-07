using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Service;
using UnityEngine;

namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// Excel配置加载器
    /// </summary>
    public class ExcelConfigLoader : ConfigLoader
    {
        // 存储所有表数据的字典，键：容器名  值：容器
        private readonly Dictionary<string, object> _tableDic = new Dictionary<string, object>();

        public override async Task LoadConfig()
        {
            //await LoadTableAsync<xxxContainer, xxxInfo>();
            //...
            await LoadTableAsync<RoleInfoContainer, RoleInfo>();
            await LoadTableAsync<MonsterInfoContainer, MonsterInfo>();
            await LoadTableAsync<SkillInfoContainer, SkillInfo>();
            await LoadTableAsync<StatusInfoContainer, StatusInfo>();
            await LoadTableAsync<DialogueInfoContainer, DialogueInfo>();
            await LoadTableAsync<BranchInfoContainer, BranchInfo>();
            await LoadTableAsync<TaskInfoContainer, TaskInfo>();
            await LoadTableAsync<TaskConditionInfoContainer, TaskConditionInfo>();
            await LoadTableAsync<NpcInfoContainer, NpcInfo>();
            
            await LoadTableAsync<ActivityInfoContainer,ActivityInfo>();
            await LoadTableAsync<ItemInfoContainer,ItemInfo>();
            await Task.FromResult(true);
        }

        public override T GetConfig<T>() where T : class
        {
            if (_tableDic.ContainsKey(typeof(T).Name))
            {
                return _tableDic[typeof(T).Name] as T;
            }
            return null;
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
        // 异步加载数据，资源名不需要后缀。.tInfo的后缀是txt
        var tInfo = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<TextAsset>(EAssetBundleType.GameConfig, $"{typeof(K).Name}");
#else
            // 加载编辑器数据
            TextAsset tInfo = EditorResManager.Instance.LoadEditorAsset<TextAsset>($"{typeof(K).Name}");
            await Task.CompletedTask;
#endif
            // 转换二进制到数据类
            ConvertFrom<T, K>(tInfo);
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

                // 将dataObj存储进containerObj中
                // 获取containerObj的字典变量
                var dicObj = containerType.GetField("dataDic").GetValue(containerObj);
                // 获取该变量的Add方法信息
                var methodInfo = dicObj.GetType().GetMethod("Add");
                // 得到数据结构类对象中指定主键字段的值
                var keyValue = dataObj.GetType().GetField(keyName).GetValue(dataObj);
                methodInfo?.Invoke(dicObj, new[] { keyValue, dataObj });
            }
            //把读取完的表记录下来
            _tableDic.Add(typeof(T).Name, containerObj);
        }
    }
}
