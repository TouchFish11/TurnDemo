using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    /// <summary>
    /// 伤害文字UI组件
    /// 负责显示伤害数值、伤害类型，控制文字的上浮、缩放、销毁逻辑
    /// </summary>
    public class DamageTextUI : FloatTextUI<DamageTextUI, DamageTextLogic>
    {
        // 伤害类型文本（如"暴击"、"普通攻击"等）
        [InjectUI] public TextMeshProUGUI txtDamageTip;
        // 伤害数值文本（如"1000"、"500"等）
        [InjectUI] public TextMeshProUGUI txtDamageNum;

        // 伤害文字的移动根节点（用于控制位置和缩放）
        [InjectUI(1)] public RectTransform DamageTextMover { get; set; }
    }
}