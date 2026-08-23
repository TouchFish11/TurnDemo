using Core.UI;

namespace HotUpdate.UI.Battle.FloatText
{
    /// <summary>
    /// 伤害文字UI组件
    /// 负责显示伤害数值、伤害类型，控制文字的上浮、缩放、销毁逻辑
    /// </summary>
    public abstract class FloatTextUI<TFloatUI, TLogic> : UIBehaviourBase, ILogicView<TFloatUI, TLogic>
    where TFloatUI : ILogicView<TFloatUI, TLogic> where TLogic : IUILogic<TFloatUI, TLogic>
    {
        protected TLogic _logic;
        
        /// <summary>
        /// 组件启用时初始化
        /// 注册更新监听、重置位置/缩放/计时/透明度
        /// </summary>
        protected sealed override void OnEnable()
        {
            _logic?.OnEnable();
        }
        
        public void Init(TLogic logic)
        {
            _logic = logic;
        }

        protected sealed override void OnDisable()
        {
            _logic?.OnDisable();
        }
    }
}