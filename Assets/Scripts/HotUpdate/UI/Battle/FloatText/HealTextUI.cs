using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public class HealTextUI : FloatTextUI<HealTextUI, HealTextLogic>
    {
        // 治疗数值文本（如"1000"、"500"等）
        [InjectUI] public TextMeshProUGUI txtHealNum;

        // 治疗文字的移动根节点（用于控制位置和缩放）
        [InjectUI(1)] public RectTransform HealTextMover { get; set; }
    }
}
