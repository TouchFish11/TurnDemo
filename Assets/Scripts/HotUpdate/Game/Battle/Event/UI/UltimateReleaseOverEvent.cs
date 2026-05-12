using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// �սἼ�ͷŽ����¼�
    /// ��ǰ��ɫ�����ʱ�Żᴦ��
    /// ��ʾ��ǰ��ҽ�ɫ�Ĳ���UI
    /// </summary>
    public class UltimateReleaseOverEvent : BattleEvent
    {
        public IBattleEntityObject CurrentActEntity { get; }

        public UltimateReleaseOverEvent(IBattleContext context, IBattleEntityObject currentEntity) : base(context)
        {
            CurrentActEntity = currentEntity;
        }
    }
}
