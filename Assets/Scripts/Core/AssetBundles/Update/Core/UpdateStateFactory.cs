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
        public static IEnumerable<IUpdateState> GetStates()
        {
            var list = new List<UpdateStateConfigAttribute>();
            var values = Enum.GetValues(typeof(EUpdatePhase));
            foreach (var phase in values)
            {
                // 将枚举值转为具体的字段
                var phaseEnum = (EUpdatePhase)phase;
                // 获取该枚举字段的FieldInfo
                var fieldInfo = phaseEnum.GetType().GetField(phaseEnum.ToString());
                // 从FieldInfo上获取特性
                var stateConfigAttribute = fieldInfo.GetCustomAttribute<UpdateStateConfigAttribute>();
                // 判断特性是否存在且启用
                if (stateConfigAttribute != null && stateConfigAttribute.IsEnabled)
                {
                    list.Add(stateConfigAttribute);
                }
            }
            
            list.Sort((p1, p2) =>
            {
                if (p1.Order < p2.Order)
                {
                    return -1;
                }

                return 1;
            });
            
            foreach (var updateStateConfigAttribute in list)
            {
                yield return (IUpdateState)DIContainer.Create(updateStateConfigAttribute.StateType);
            }
        }
    }
}
