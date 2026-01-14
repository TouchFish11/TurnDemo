using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// µÈ´ýÐÐ¶¯UI
/// </summary>
public class WaitingActUI : BaseUIBehaviour
{
    [Inject] private Image imgIcon;

    public void Init(Sprite icon)
    {
        imgIcon.sprite = icon;
    }
}
