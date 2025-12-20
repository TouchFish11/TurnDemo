using Game.Battle;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : UIView
{
    private ScrollRect svActionbar;
    private ScrollRect svPoint;

    private TextMeshProUGUI txtCount;

    private Transform operatorArea;
    private Transform playerArea;


    protected override void Awake()
    {
        base.Awake();

        svActionbar = binder.GetControl<ScrollRect>(nameof(svActionbar));
        svPoint = binder.GetControl<ScrollRect>(nameof(svPoint));

        txtCount = binder.GetControl<TextMeshProUGUI>(nameof(txtCount));

        operatorArea = this.transform.Find(nameof(operatorArea));
        playerArea = this.transform.Find(nameof(playerArea));
    }


    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "actions":
                UpdateActionbar(value as List<ActionGridUI>);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 更新行动栏
    /// </summary>
    private void UpdateActionbar(List<ActionGridUI> actionGridUIs)
    {
        svActionbar.content.DetachChildren();
        foreach (ActionGridUI actionGridUI in actionGridUIs)
        {
            actionGridUI.transform.SetParent(svActionbar.content, false);
        }
    }

    internal void BattleOver(Action value)
    {
        throw new NotImplementedException();
    }

    internal object InitMonsterUI(object value)
    {
        throw new NotImplementedException();
    }

    internal object InitPlayerObjUI(object value)
    {
        throw new NotImplementedException();
    }

    internal object InitUI(List<IBattleEntityObject> actionList)
    {
        throw new NotImplementedException();
    }
}
