using Core.Components;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Core
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
