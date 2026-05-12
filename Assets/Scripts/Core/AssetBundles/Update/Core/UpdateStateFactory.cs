using System;
using System.Collections.Generic;
using System.Reflection;
using Core.AssetBundles.Update.State;
using Core.DI;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新状态工厂
    /// </summary>
    public class UpdateStateFactory
    {
        /// <summary>
        /// 根据枚举创建状态实例
        /// </summary>
        /// <param name="updatePhase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static IUpdateState CreateState(EUpdatePhase updatePhase)
        {
            return updatePhase switch
            {
                EUpdatePhase.DownLoadRemoteListFile => DIContainer.Create<DownloadListFileState>(),
                EUpdatePhase.GetLocalCompareFile => DIContainer.Create<GetLocalListFileState>(),
                EUpdatePhase.CompareContrast => DIContainer.Create<CompareContrastState>(),
                EUpdatePhase.CheckDeviceStorage => DIContainer.Create<CheckDeviceStorageState>(),
                EUpdatePhase.DownLoadAssets => DIContainer.Create<DownLoadAssetState>(),
                EUpdatePhase.CheckAssetsIntegrity => DIContainer.Create<CheckAssetIntegrityState>(),
                EUpdatePhase.Finished => DIContainer.Create<FinishState>(),
                EUpdatePhase.None or _ => throw new ArgumentOutOfRangeException(nameof(updatePhase), "未知的更新状态")
            };
        }

        public static IEnumerable<IUpdateState> GetStates()
        {
            var list = new List<EUpdatePhase>();
            var values = Enum.GetValues(typeof(EUpdatePhase));
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
                    list.Add(phaseEnum);
                }
            }
            
            list.Sort((p1, p2) =>
            {
                var stateConfigAttribute1 = p1.GetType().GetField(p1.ToString()).GetCustomAttribute<StateConfigAttribute>();
                var stateConfigAttribute2 = p2.GetType().GetField(p2.ToString()).GetCustomAttribute<StateConfigAttribute>();
                
                if (stateConfigAttribute1.Order < stateConfigAttribute2.Order)
                {
                    return -1;
                }

                return 1;
            });
            
            foreach (var updatePhase in list)
            {
                yield return CreateState(updatePhase);
            }
        }
    }
}
