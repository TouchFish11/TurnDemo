using Core.Components;
using Game.Battle.Objects;

namespace Game.Battle.Component
{
    /// <summary>
    /// ս������ӿ�
    /// </summary>
    public interface IBattleComponent : IComponent
    {
        /// <summary>
        /// ս��ʵ��
        /// </summary>
        IBattleEntityObject BattleEntity { get; }

        /// <summary>
        /// ս����ʼ��
        /// </summary>
        public void BattleInit(IBattleEntityObject battleEntity);
    }
}
