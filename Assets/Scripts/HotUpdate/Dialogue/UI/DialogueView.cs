using Core.UI;
using Core.UI.MVC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Dialogue.UI
{
    /// <summary>
    /// 对话界面视图类
    /// 负责对话界面的UI显示、状态控制，实现IDialogueView接口，对接对话业务逻辑层
    /// </summary>
    public class DialogueView : UIView
    {
        #region 注入的UI组件
        // 剧情回顾滚动视图组件
        [Inject] private ScrollRect svReview;
        // 说话人名称文本组件
        [Inject] private TextMeshProUGUI txtSpeakerName;
        // 提示文本组件（用于显示对话相关提示信息）
        [Inject] private TextMeshProUGUI txtTip;
        // 对话内容文本组件
        [Inject] private TextMeshProUGUI txtDialogue;
        // 自动播放状态文本组件（显示"自动"等状态提示）
        [Inject] private Text txtAuto;

        // 对话主体容器（承载对话核心UI的根节点）
        [Inject(1)] public RectTransform DialogueBox { get; private set; }
        // 对话选项框容器（承载对话选择项的根节点）
        [Inject(1)] public RectTransform DialogueOptBox { get; private set; }
        // 剧情回顾子界面容器（剧情回顾功能的根节点）
        [Inject(1)] public RectTransform StoryReviewSubView { get; private set; }
        #endregion

        #region 公开属性
        // 对话回顾内容容器（供外部访问，用于挂载回顾项UI）
        public RectTransform ReviewContent => svReview.content;
        // 剧情回顾视图接口实例（对接剧情回顾的视图逻辑）
        public StoryReviewView StoryReviewView { get; private set; }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 初始化方法（在Awake阶段执行）
        /// 完成子视图的初始化，获取剧情回顾子视图实例
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // 从子物体中获取剧情回顾视图接口实例，关联剧情回顾功能
            StoryReviewView = GetComponentInChildren<StoryReviewView>();
        }
        #endregion

        #region 公开方法
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
            StoryReviewSubView.gameObject.SetActive(isActive);
        }
        #endregion
    }
}