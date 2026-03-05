namespace HotUpdate.Dialogue
{
    /// <summary>
    /// 可对话接口
    /// </summary>
    public interface IDialable
    {
        /// <summary>
        /// 对话开始
        /// </summary>
        void OnDialogueStart();

        /// <summary>
        /// 对话结束
        /// </summary>
        void OnDialogueEnd();
    }
}
