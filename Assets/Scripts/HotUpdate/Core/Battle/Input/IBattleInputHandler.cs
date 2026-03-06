using System;
using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Core.Battle.Input
{
    public interface IBattleInputHandler
    {
        GameObject GameObject { get; }    
        
        /// <summary>
        /// 选中战斗实体对象的事件（如选中玩家/怪物作为技能目标）
        /// 事件参数：选中的战斗实体对象接口
        /// </summary>
        event Action<IBattleEntityObject> OnSelectedObject;

        /// <summary>
        /// 向左拖拽的事件（用于切换目标等逻辑）
        /// </summary>
        event Action OnLeftDrag;

        /// <summary>
        /// 向右拖拽的事件（用于切换目标等逻辑）
        /// </summary>
        event Action OnRightDrag;

        /// <summary>
        /// 拖拽过程中的事件（传递拖拽X轴偏移量）
        /// 事件参数：拖拽X轴方向的偏移量（像素）
        /// </summary>
        event Action<float> OnDrag;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        void Init(IBattleContext context);

        event Action<bool> OnRebound;

        /// <summary>
        /// 设置输入状态
        /// </summary>
        /// <param name="activeInput"></param>
        void SetInputState(bool activeInput);
    }
}
