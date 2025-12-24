using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择标记UI
/// </summary>
public class SelectMarkerUI : BaseUIBehaviour
{
    private readonly List<Image> images = new List<Image>();

    private Color enermyRed;
    private Color friendBlue;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 5; i++)
        {
            images.Add(binder.GetControl<Image>($"m{i + 1}"));
        }
    }

    /// <summary>
    /// 初始化选择标记
    /// </summary>
    /// <param name="skillTargetType"></param>
    public void InitSelectMarker(E_SkillTargetType skillTargetType)
    {
        Color color = skillTargetType == E_SkillTargetType.Enemy ? enermyRed : friendBlue;
        foreach (Image image in images)
        {
            image.color = color;
        }
    }

}
