using System;
using Core.UI;
using Core.Utility;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameHotUpdate.Dialogue.UI
{
    /// <summary>
    /// �Ի���֧ѡ��UI
    /// </summary>
    public class DialogueOptUI : UIBehaviourBase
    {
        [Inject] private Image imgHightlight;
        [Inject] private TextMeshProUGUI txtOptText;

        private BranchInfo branchInfo;
        
        /// <summary>
        /// ѡ��ѡ���¼�
        /// </summary>
        public event Action<int> OnSelectOpt;

        protected override void Awake()
        {
            base.Awake();
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerEnter, OnPointEnter);
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerExit, OnPointExit);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="branchInfo"></param>
        public void Init(BranchInfo branchInfo)
        {
            this.branchInfo = branchInfo;
            txtOptText.text = branchInfo.f_optText;
            imgHightlight.gameObject.SetActive(false);
        }

        protected override void OnButtonClick(string btnName)
        {
            // ѡ��÷�֧ѡ��
            OnSelectOpt?.Invoke(branchInfo.f_dialogueId);
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
