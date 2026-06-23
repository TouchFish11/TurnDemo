using System;
using Core.UI;
using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话选项分支UI
    /// </summary>
    public class DialogueOptUI : UIBehaviourBase
    {
        [InjectUI] private Image imgHightlight;
        [InjectUI] private TextMeshProUGUI txtOptText;

        // 当前分支选择后的对话ID
        private int _dialogueId;
        
        /// <summary>
        /// 选项选择事件
        /// </summary>
        public event Action<int> OnSelectOpt;

        protected override void Awake()
        {
            base.Awake();
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerEnter, OnPointEnter);
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerExit, OnPointExit);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="branchInfo"></param>
        public void Init(BranchInfo branchInfo)
        {
            this._dialogueId = branchInfo.f_dialogueId;
            txtOptText.text = branchInfo.f_optText;
            imgHightlight.gameObject.SetActive(false);
        }

        protected override void OnButtonClick(string btnName)
        {
            // 选择该分支选项
            OnSelectOpt?.Invoke(_dialogueId);
        }
        

        private void OnPointEnter(BaseEventData data)
        {
            imgHightlight.gameObject.SetActive(true);
        }

        private void OnPointExit(BaseEventData data)
        {
            imgHightlight.gameObject.SetActive(false);
        }
    }
}