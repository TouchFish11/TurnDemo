using System;
using Core.UI;
using HotUpdate.Base.Settings;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 设置页签（侧边栏的一项）。
    /// prefab 根上挂着一个 Toggle（选中态高亮），子节点有 txtSetting 文本标签。
    /// 点击时通过 onSelect 回调把所属分类回传给 SettingsController 去切换条目。
    /// </summary>
    public class SettingOpt : UIBehaviourBase
    {
        [InjectUI] public TextMeshProUGUI txtSetting;
        [InjectUI] private Toggle togSelect;
        
        private ESettingCategory _category;
        private Action<ESettingCategory> _onSelect;

        /// <summary>
        /// 初始化页签：设置显示名并监听选中事件
        /// </summary>
        public void Init(ESettingCategory category, Action<ESettingCategory> onSelect)
        {
            _category = category;
            _onSelect = onSelect;
            txtSetting.text = GetCategoryName(category);
        }

        /// <summary>
        /// 设置选中态（不触发 onValueChanged，避免切换时重复回调）
        /// </summary>
        public void SetSelected(bool selected)
        {
            togSelect.SetIsOnWithoutNotify(selected);
        }

        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            // 只有从「未选中 → 选中」时才触发切换，避免取消选中也触发
            if (togName == nameof(togSelect))
            {
                if (isOn)
                {
                    _onSelect?.Invoke(_category);
                }
            }
        }

        /// <summary>
        /// 分类 → 中文显示名
        /// </summary>
        private static string GetCategoryName(ESettingCategory category) => category switch
        {
            ESettingCategory.Graphics => "画面",
            ESettingCategory.Audio => "音频",
            ESettingCategory.Gameplay => "玩法",
            _ => "其他",
        };

        protected override void OnDisable()
        {
            _onSelect = null;
        }
    }
}
