using System;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Inputs
{
    public interface IBattleInputHandler
    {
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
        /// 相机回弹事件
        /// 参数是否回弹
        /// </summary>
        event Action<bool> OnRebound;

        /// <summary>
        /// 设置输入状态
        /// </summary>
        /// <param name="activeInput"></param>
        void SetInputState(bool activeInput);

        event Action OnClick;
        
        void Init(IBattleContext context);
        void Reset();
    }
}
