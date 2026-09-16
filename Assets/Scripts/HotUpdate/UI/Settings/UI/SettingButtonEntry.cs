using System;
using Core.UI;
using HotUpdate.Base.Settings;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 按钮设置项 UI 条目（对应 ESettingWidget.Button）。
    /// 只有一个按钮，点击触发一次性动作，不绑定 ReactiveProperty、不存值。
    /// </summary>
    public class SettingButtonEntry : UIBehaviourBase, ISettingsEntry
    {
        [InjectUI] public TextMeshProUGUI txtName;
        [InjectUI] public Button btn;

        private Action onButtonPressed;
        
        /// <summary>
        /// 初始化按钮条目：设置名字并绑定点击回调
        /// </summary>
        public void Init(SettingDefinition definition, Action onPress)
        {
            txtName.text = definition.Name;
            onButtonPressed = onPress;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btn))
            {
                onButtonPressed?.Invoke();
            }
        }
        
        protected override void OnDisable()
        {
            onButtonPressed = null;
        }
    }
}
