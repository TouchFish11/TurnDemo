using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.Dialogue.UI
{
    /// <summary>
    /// �Ի��ع�UI
    /// </summary>
    public class DialogueReviewUI : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtDialogueText;

        public RectTransform RectTransform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            RectTransform = transform as RectTransform;
        }

        public void Init(string speakerName, string dialogueText)
        {
            txtDialogueText.text = $"{speakerName}：{dialogueText}";
        }
    }
}
