using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public class ShieldTextLogic : FloatTextLogic<ShieldTextUI, ShieldTextLogic>
    {
        protected override RectTransform TextMover => View.ShieldTextMover;

        /// <summary>
        /// 初始化护盾文字的显示内容和样式
        /// </summary>
        /// <param name="shieldTextUI"></param>
        /// <param name="shieldAmount">护盾量</param>
        public void InitshieldText(ShieldTextUI shieldTextUI , float shieldAmount)
        {
            View = shieldTextUI;
            if (shieldAmount > 0)
            {
                // 设置护盾数值文本内容（转为字符串）
                View.txtShieldNum.text = ((int)shieldAmount).ToString();
            }
            else if(shieldAmount < 0)
            {
                // 设置护盾数值文本内容（转为字符串）
                View.txtShieldNum.text = $"{(int)shieldAmount}";
            }
            StartUpdate();
        }
        
        public override void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
