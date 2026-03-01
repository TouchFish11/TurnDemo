namespace GameHotUpdate.Dialogue
{
    /// <summary>
    /// �ɶԻ��ӿ�
    /// </summary>
    public interface IDialable
    {
        /// <summary>
        /// �Ի���ʼ
        /// </summary>
        void OnDialogueStart();

        /// <summary>
        /// �Ի�����
        /// </summary>
        void OnDialogueEnd();
    }
}
