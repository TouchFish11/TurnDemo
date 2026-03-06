using Core.Components;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Battle
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
