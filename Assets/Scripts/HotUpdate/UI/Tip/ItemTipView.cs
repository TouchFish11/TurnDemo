using Core.UI;
using Core.UI.ViewController;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Tip
{
    public class ItemTipView : UIView
    {
        [InjectUI] public Button btnClose;
        [InjectUI] public Image imgIcon;
        [InjectUI] public TextMeshProUGUI txtHoldNumInfo;
        [InjectUI] public TextMeshProUGUI txtItemName;
        [InjectUI] public Button btnBoxClose;
        [InjectUI] public TextMeshProUGUI txtDescription;
    }
}
