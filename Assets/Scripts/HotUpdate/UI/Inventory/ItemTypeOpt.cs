using System;
using Core.Log;
using Core.UI;
using HotUpdate.Common.Config.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 物品类型选项
    /// </summary>
    public class ItemTypeOpt : UIBehaviourBase
    {
        [InjectUI] private Image imgIcon;
        [InjectUI] private Toggle togOpt;
        
        /// <summary>
        /// 选项类型切换事件
        /// </summary>
        public event Action<EItemType> OnItemTypeOptChange;
        
        public EItemType ItemType { get; private set; }
        
        /// <summary>
        /// 初始化选项
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="icon"></param>
        /// <param name="group"></param>
        public void InitOption(EItemType itemType, Sprite icon, ToggleGroup group)
        {
            ItemType = itemType;
            if(icon)
                imgIcon.sprite = icon;
            togOpt.group = group;
        }

        public void Select()
        {
            togOpt.isOn = true;
        }

        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            try
            {
                if (togName == nameof(togOpt) && isOn)
                {
                    OnItemTypeOptChange?.Invoke(ItemType);
                }
            }
            catch (OperationCanceledException canceledException)
            {
                Logger.LogDebug(ELogTags.Item, $"[{nameof(ItemTypeOpt)}]: Item type switch operator cancel, {canceledException.Message}");
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"[{nameof(ItemTypeOpt)}]: Item type switch fail, {e.Message}");
            }
        }

        protected override void OnDisable()
        {
            OnItemTypeOptChange = null;
        }
    }
}
