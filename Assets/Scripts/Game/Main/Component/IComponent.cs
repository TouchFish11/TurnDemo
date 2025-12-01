using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameLogic.BattleMoudule
{
    /// <summary>
    /// 自定义组件接口
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        IEntityObject EntityObject { get; }

        /// <summary>
        /// 初始化组件
        /// </summary>
        void Init(IEntityObject entityObject);
    }
}
