using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    public interface IToughnessCommand : ICommand
    {
        /// <summary>
        /// �������
        /// </summary>
        IToughnessComponent ToughnessComponent { get; }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="toughnessComponent"></param>
        void Init(IToughnessComponent toughnessComponent);
    }
}
