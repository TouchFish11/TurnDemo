using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.UI
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
