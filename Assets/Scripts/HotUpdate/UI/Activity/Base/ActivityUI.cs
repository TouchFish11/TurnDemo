using System;
using Core.DI;
using Core.UI;
using HotUpdate.Common.Config.ExcelInfo.Info;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动选项UI
    /// </summary>
    public class ActivityUI : UIBehaviourBase
    {
        [Inject] private Image imgIcon;
        [Inject] private TextMeshProUGUI txtName;
        [Inject] private Toggle togActivity;
        
        private ActivityInfo _activityInfo;
        private ActivityController _activityController;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="activityInfo"></param>
        /// <param name="toggleGroup"></param>
        /// <param name="activityController"></param>
        public void Init(Sprite icon, ActivityInfo activityInfo, ToggleGroup toggleGroup, ActivityController activityController)
        {
            _activityInfo = activityInfo;
            _activityController = activityController;
            togActivity.group =  toggleGroup;
            imgIcon.sprite = icon;
            txtName.text = activityInfo.f_name;
        }

        /// <summary>
        /// 选中活动
        /// </summary>
        public void SelectActivity()
        {
            togActivity.isOn = true;
        }

        protected override async void OnToggleValueChanged(string togName, bool isOn)
        {
            try
            {
                if (isOn)
                {
                    await _activityController.UpdateDetailActivity(_activityInfo);
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }
    }
}