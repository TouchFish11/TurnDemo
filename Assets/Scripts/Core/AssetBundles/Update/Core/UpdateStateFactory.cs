using System;
using System.Collections.Generic;
using System.Reflection;
using Core.AssetBundles.Update.State;
using Core.Collection;
using Core.Pool;
using Core.Serialize.Json;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新状态工厂
    /// </summary>
    public class UpdateStateFactory
    {
        private readonly IAssetBundleUpdater _updater;
        private readonly IPoolManager _poolManager;
        private readonly IJsonManager _jsonManager;

        // 工厂接收通用依赖，传给所有状态
        public UpdateStateFactory(IAssetBundleUpdater updater, IPoolManager poolManager, IJsonManager jsonManager)
        {
            _updater = updater;
            _poolManager = poolManager;
            _jsonManager = jsonManager;
        }

        /// <summary>
        /// 根据枚举创建状态实例
        /// </summary>
        /// <param name="updatePhase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private IUpdateState CreateState(EUpdatePhase updatePhase)
        {
            return updatePhase switch
            {
                EUpdatePhase.DownLoadRemoteListFile => new DownloadListFileState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.GetLocalCompareFile => new GetLocalListFileState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.CompareContrast => new CompareContrastState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.CheckDeviceStorage => new CheckDeviceStorageState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.DownLoadAssets => new DownLoadAssetState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.CheckAssetsIntegrity => new CheckAssetIntegrityState(_updater, _poolManager, _jsonManager),
                EUpdatePhase.Finished => new FinishState(_updater, _poolManager, _jsonManager),
                _ => throw new ArgumentOutOfRangeException(nameof(updatePhase), "未知的更新状态")
            };
        }

        public IEnumerable<IUpdateState> GetStates()
        {
            var uniList = ListUtility.GetUniList<EUpdatePhase>();
            var values = System.Enum.GetValues(typeof(EUpdatePhase));
            foreach (var phase in values)
            {
                // 将枚举值转为具体的字段
                var phaseEnum = (EUpdatePhase)phase;
                // 获取该枚举字段的FieldInfo
                var fieldInfo = phaseEnum.GetType().GetField(phaseEnum.ToString());
                // 从FieldInfo上获取特性
                var stateConfigAttribute = fieldInfo.GetCustomAttribute<StateConfigAttribute>();
                // 判断特性是否存在且启用
                if (stateConfigAttribute != null && stateConfigAttribute.IsEnabled)
                {
                    uniList.List.Add(phaseEnum);
                }
            }
            
            uniList.List.Sort((p1, p2) =>
            {
                var stateConfigAttribute1 = p1.GetType().GetField(p1.ToString()).GetCustomAttribute<StateConfigAttribute>();
                var stateConfigAttribute2 = p2.GetType().GetField(p2.ToString()).GetCustomAttribute<StateConfigAttribute>();
                
                if (stateConfigAttribute1.Order < stateConfigAttribute2.Order)
                {
                    return -1;
                }

                return 1;
            });
            
            foreach (var updatePhase in uniList.List)
            {
                yield return CreateState(updatePhase);
            }
            ListUtility.CollectUniList(uniList);
        }
    }
}
