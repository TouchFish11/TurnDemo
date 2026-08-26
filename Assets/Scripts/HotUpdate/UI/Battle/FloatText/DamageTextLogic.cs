using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public class DamageTextLogic : FloatTextLogic<DamageTextUI, DamageTextLogic>
    {
        protected override RectTransform TextMover => View.DamageTextMover;

        public override void SetTextMover()
        {
            // 重置计时
            currentTime = 0;
            // 重置文本颜色（恢复初始颜色）
            View.txtDamageTip.color = originColor;
            View.txtDamageNum.color = originColor;
            base.SetTextMover();
        }

        /// <summary>
        /// 初始化伤害文字的显示内容和样式
        /// </summary>
        /// <param name="damageTextUI"></param>
        /// <param name="textColor">文字颜色（如伤害类型对应的颜色）</param>
        /// <param name="damageTypeText">伤害类型文本（如"暴击"、"法术伤害"）</param>
        /// <param name="damage">伤害数值（需要显示的具体伤害值）</param>
        public void InitDamageText(DamageTextUI damageTextUI, Color textColor, string damageTypeText, int damage)
        {
            View = damageTextUI;
            // 设置伤害类型和数值的文字颜色
            View.txtDamageTip.color = textColor;
            View.txtDamageNum.color = textColor;

            // 设置伤害类型文本内容
            View.txtDamageTip.text = damageTypeText;
            // 设置伤害数值文本内容（转为字符串）
            View.txtDamageNum.text = damage.ToString();

            // 记录初始颜色（用于后续透明度过渡）
            originColor = View.txtDamageTip.color;

            StartUpdate();
        }
    }
}
