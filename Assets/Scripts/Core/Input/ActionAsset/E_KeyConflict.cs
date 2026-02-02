namespace Core.Input.ActionAsset
{
    /// <summary>
    /// ��λ��ͻ
    /// </summary>
    public enum E_KeyConflict : byte
    {
        /// <summary>
        /// �����λ��ͻ
        /// </summary>
        SpecialKey,
        /// <summary>
        /// ��ͬ������ͻ
        /// </summary>
        ExistKey,
        /// <summary>
        /// �Ǽ��̰�����ͻ
        /// </summary>
        NotKeyboard,
        /// <summary>
        /// �ļ�����
        /// </summary>
        Over,
    }
}
