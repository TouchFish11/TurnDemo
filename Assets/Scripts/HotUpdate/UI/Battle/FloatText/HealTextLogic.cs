using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public class HealTextLogic : FloatTextLogic<HealTextUI, HealTextLogic>
    {
        protected override RectTransform TextMover => View.HealTextMover;
        
        /// <summary>
        /// 初始化治疗文字的显示内容和样式
        /// </summary>
        /// <param name="healTextUI"></param>
        /// <param name="healText"></param>
        public void InitHealText(HealTextUI healTextUI, int healText)
        {
            View = healTextUI;
            // 设置治疗数值文本内容（转为字符串）
            View.txtHealNum.text = healText.ToString();
            StartUpdate();
        }
        
        public override void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
