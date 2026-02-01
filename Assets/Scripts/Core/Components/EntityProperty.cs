namespace Core.Components
{
    /// <summary>
    /// ʵ������
    /// </summary>
    public abstract class EntityProperty
    {
        protected int id;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public abstract void InitProperty(int id);

        public int Id => id;
    }
}
