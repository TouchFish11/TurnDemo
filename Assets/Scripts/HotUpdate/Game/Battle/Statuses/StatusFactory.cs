using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Core.Exceptions;
using Core.HotUpdate;
using Core.Serialize.Binary;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 战斗状态工厂类，实现IStatusFactory接口
    /// 负责扫描并缓存所有战斗状态类与状态ID的映射关系，提供根据状态ID创建状态实例的能力
    /// </summary>
    public class StatusFactory : IStatusFactory
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        /// <summary>
        /// 状态ID到状态类类型的映射字典
        /// Key：状态唯一标识ID（由StatusTypeIdAttribute标记）
        /// Value：对应状态ID的具体状态类Type
        /// </summary>
        private readonly Dictionary<int, Type> idToTypeMap = new();
        
        // 状态缓存栈
        private readonly Stack<IStatus> _stack = new();

        private StatusFactory(IHotUpdateManager hotUpdateManager)
        {
            ScanAllStatu(idToTypeMap, hotUpdateManager);
        }

        /// <summary>
        /// 根据状态ID创建对应的状态实例
        /// </summary>
        /// <param name="sorucer">提供者</param>
        /// <param name="owner">拥有者</param>
        /// <param name="statusId">状态唯一标识ID</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">未找到<see cref="statusId"/>的状态类时抛出</exception>
        public IStatus GetStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId)
        {
            var statusInfo = _binaryDataManager.GetConfig<StatusInfoContainer>(EConfigLoadType.Excel).dataDic[statusId];
            if (_stack.TryPop(out var result))
            {
                result.InitStatus(sorucer, owner, statusInfo);
                return result;
            }
            
            // 从映射字典中查找状态ID对应的状态类Type
            if (!idToTypeMap.TryGetValue(statusId, out var statusType)) 
                throw ExceptionHelper.Throw<KeyNotFoundException>($"Status id '{statusId}' not found");
            
            // 通过反射创建状态类实例，并转换为IStatus接口返回
            var status = (IStatus)DIContainer.Create(statusType);
            status.InitStatus(sorucer, owner, statusInfo);
            return status;
        }

        /// <summary>
        /// 释放对象，调用其<see cref="StatusBase.ResetData"/>方法
        /// </summary>
        /// <param name="status"></param>
        public void Release(IStatus status)
        {
            ((StatusBase)status).ResetData();
            _stack.Push(status);
        }

        /// <summary>
        /// 扫描所有热更新程序集中的状态类，构建状态ID与状态类的映射关系
        /// </summary>
        /// <param name="dic">用于存储映射关系的字典（Key：状态ID，Value：状态类Type）</param>
        /// <param name="hotUpdateManager"></param>
        private static void ScanAllStatu(Dictionary<int, Type> dic, IHotUpdateManager hotUpdateManager)
        {
            // 遍历所有热更新程序集（通过AssemblyUtility工具类获取）
            foreach (var assembly in hotUpdateManager.GetHotAssemblies())
            {
                // 遍历当前程序集中的所有类型
                foreach (var type in assembly.GetTypes())
                {
                    // 获取类型上标记的StatusTypeIdAttribute特性（用于标记状态ID）
                    var attribute = type.GetCustomAttribute<StatusTypeIdAttribute>();
                    // 若类型未标记该特性，跳过当前类型
                    if (attribute == null)
                    {
                        continue;
                    }
            
                    // 校验类型：必须实现IStatus接口，且不是抽象类（确保是可实例化的具体状态类）
                    if (typeof(IStatus).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        // 将状态ID和对应的状态类Type添加到映射字典中
                        dic.Add(attribute.StatusId, type);
                    }
                }
            }
        }
    }
}