using System;
using Core.UI;
using HotUpdate.Game.Dialogue.Datas;
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
        
        // 当前分支数据
        private BranchData _currentBranchData;
        
        /// <summary>
        /// 选项选择事件
        /// </summary>
        public event Action<BranchData> OnSelectOpt;

        protected override void Awake()
        {
            base.Awake();
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerEnter, OnPointEnter);
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerExit, OnPointExit);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="branchData"></param>
        public void Init(BranchData branchData)
        {
            _currentBranchData = branchData;
            txtOptText.text = branchData.BranchInfo.f_optText;
            imgHightlight.gameObject.SetActive(false);
        }

        protected override void OnButtonClick(string btnName)
        {
            // 选择该分支选项
            OnSelectOpt?.Invoke(_currentBranchData);
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