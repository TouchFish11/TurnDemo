using System;
using Core.Exceptions;
using Core.UI;
using HotUpdate.Game.Battle.Statuses;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 行动状态UI容器
    /// </summary>
    public class ActionStatusUI : UIBehaviourBase
    {
        [InjectUI] public TextMeshProUGUI txtDescription;
        
        [InjectUI(1)] public RectTransform StatusContent { get; private set; }

        public void InitText(EStatusType statusType, int num)
        {
            txtDescription.text = statusType switch
            {
                EStatusType.Positive => $"增益效果{num}",
                EStatusType.Negative => $"减益效果{num}",
                EStatusType.Other => $"其它效果{num}",
                _ => throw ExceptionHelper.Throw<ArgumentOutOfRangeException>($"{statusType}"),
            };
        }
    }
}
