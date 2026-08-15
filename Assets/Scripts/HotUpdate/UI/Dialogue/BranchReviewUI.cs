using Core.UI;
using TMPro;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 分支回顾UI
    /// </summary>
    public class BranchReviewUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtDialogueText;

        private const string Prefix = "选项：";
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="branchText"></param>
        public void Init(string branchText)
        {
            txtDialogueText.text = $"{Prefix}{branchText}";
        }
    }
}
