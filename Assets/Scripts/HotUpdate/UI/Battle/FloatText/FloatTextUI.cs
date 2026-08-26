using Core.UI;

namespace HotUpdate.UI.Battle.FloatText
{
    /// <summary>
    /// 伤害文字UI组件
    /// 负责显示伤害数值、伤害类型，控制文字的上浮、缩放、销毁逻辑
    /// </summary>
    public abstract class FloatTextUI<TFloatUI, TLogic> : UIBehaviourBase, ILogicView<TFloatUI, TLogic>
    where TFloatUI : FloatTextUI<TFloatUI, TLogic> where TLogic : FloatTextLogic<TFloatUI, TLogic>
    {
        protected TLogic _logic;
        
        public void Init(TLogic logic)
        {
            logic.SetTextMover();
            _logic = logic;
        }

        protected sealed override void OnDisable()
        {
            _logic.Dispose();
        }
    }
}