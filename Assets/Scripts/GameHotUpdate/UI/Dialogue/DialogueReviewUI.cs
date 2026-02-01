using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.UI.Dialogue
{
    /// <summary>
    /// �Ի��ع�UI
    /// </summary>
    public class DialogueReviewUI : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtDialogueText;

        public RectTransform RectTransform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            RectTransform = this.transform as RectTransform;
        }

        public void Init(string speakerName, string dialogueText)
        {
            txtDialogueText.text = $"{speakerName}：{dialogueText}";
        }
    }
}
