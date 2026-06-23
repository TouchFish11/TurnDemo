using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话回顾UI
    /// </summary>
    public class DialogueReviewUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtDialogueText;

        public RectTransform RectTransform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            RectTransform = transform as RectTransform;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="speakerName"></param>
        /// <param name="dialogueText"></param>
        public void Init(string speakerName, string dialogueText)
        {
            txtDialogueText.text = $"{speakerName}：{dialogueText}";
        }
    }
}
