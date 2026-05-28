using Core.UI;
using Core.UI.ViewController;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Tip
{
    /// <summary>
    /// 提示界面基类
    /// </summary>
    public class TipView : UIView
    {
        [InjectUI] public TextMeshProUGUI txtTitle;
        
        [InjectUI(1)] public RectTransform ContentRoot { get; private set; }
        
        
    }
}
