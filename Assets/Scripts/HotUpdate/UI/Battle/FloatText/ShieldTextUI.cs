using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public class ShieldTextUI : FloatTextUI<ShieldTextUI, ShieldTextLogic>
    {
        // 护盾数值文本（如"1000"、"500"等）
        [InjectUI] public TextMeshProUGUI txtShieldNum;
        // 护盾文字的移动根节点（用于控制位置和缩放）
        [InjectUI(1)] public RectTransform ShieldTextMover { get; set; }
    }
}
