using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Pool
{
    /// <summary>
    /// 对象池工具类
    /// </summary>
    internal static class PoolUtil
    {
        /// <summary>
        /// 根据游戏对象身上的组件转换为对象类型，若对象身上同时有多个符合条件的组件，则按照优先级获取
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static EObjectType ConvertFrom(Object obj)
        {
            if (obj is not Component component) 
                return EObjectType.GameObject;
            
            if (component.TryGetComponent<ParticleSystem>(out _))
            {
                return EObjectType.VFX;
            }
            
            if (component.TryGetComponent<UIBehaviour>(out _))
            {
                return EObjectType.UI;
            }

            if (component.TryGetComponent<AudioSource>(out _))
            {
                return EObjectType.SFX;
            }
            
            // TODO：这里可能要换成自定义组件，避免误获取Transform
            if(component.TryGetComponent<Component>(out _))
            {
                return EObjectType.Component;
            }

            // 都不符合上述条件，则返回游戏对象类型
            return EObjectType.GameObject;
        }
    }
}
