using System.Collections.Generic;
using System.Threading.Tasks;
using Core.UI;
using Core.UI.ViewController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话界面视图类
    /// 负责对话界面的UI显示、状态控制，实现IDialogueView接口，对接对话业务逻辑层
    /// </summary>
    public class DialogueView : UIView
    {
        #region 注入的UI组件
        // 剧情回顾滚动视图组件
        [InjectUI] private ScrollRect svReview;
        // 说话人名称文本组件
        [InjectUI] private TextMeshProUGUI txtSpeakerName;
        // 提示文本组件（用于显示对话相关提示信息）
        [InjectUI] private TextMeshProUGUI txtTip;
        // 对话内容文本组件
        [InjectUI] private TextMeshProUGUI txtDialogue;
        // 自动播放状态文本组件（显示"自动"等状态提示）
        [InjectUI] private Text txtAuto;
        // 对话主体容器（承载对话核心UI的根节点）
        [InjectUI(1)] public RectTransform DialogueBox { get; private set; }
        // 对话选项框容器（承载对话选择项的根节点）
        [InjectUI(1)] public RectTransform DialogueOptBox { get; private set; }
        // 剧情回顾子界面容器（剧情回顾界面的根节点）
        [InjectUI(1)] public RectTransform ReviewRoot { get; private set; }
        #endregion
        
        #region 公开属性
        // 对话选项UI的缓存列表，用于管理当前显示的所有对话分支选项
        public List<DialogueOptUI> DialogueOptUIs { get; } = new();
        // 对话回顾内容容器（供外部访问，用于挂载回顾项UI）
        public RectTransform ReviewContent => svReview.content;
        // 剧情回顾视图接口实例（对接剧情回顾的视图逻辑）
        public StoryReviewView StoryReviewView { get; private set; }
        #endregion

        #region 公开方法
        /// <summary>
        /// 设置对话回顾界面对象缓存
        /// </summary>
        /// <param name="storyReviewView"></param>
        public void SetStoryReviewView(StoryReviewView storyReviewView)
        {
            StoryReviewView = storyReviewView;
        }
        
        /// <summary>
        /// 设置对话主体容器的激活状态
        /// </summary>
        /// <param name="isActive">是否激活：true显示，false隐藏</param>
        public void SetDialogueBoxActive(bool isActive)
        {
            DialogueBox.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 设置提示文本内容
        /// </summary>
        /// <param name="text">要显示的提示文本</param>
        public void SetTip(string text)
        {
            txtTip.text = text;
        }

        /// <summary>
        /// 更新说话人名称和对话内容
        /// </summary>
        /// <param name="speakerName">说话人名称</param>
        /// <param name="dialogueText">对话内容</param>
        public void UpdateNameAndText(string speakerName, string dialogueText)
        {
            txtSpeakerName.text = speakerName;
            txtDialogue.text = dialogueText;
        }
        
        /// <summary>
        /// 设置剧情回顾子界面的激活状态
        /// </summary>
        /// <param name="isActive">是否激活：true显示，false隐藏</param>
        public void SetActiveReview(bool isActive)
        {
            ReviewRoot.gameObject.SetActive(isActive);
        }
        #endregion

        public override Task Destroy()
        {
            DialogueOptUIs.Clear();
            StoryReviewView = null;
            return base.Destroy();
        }
    }
}