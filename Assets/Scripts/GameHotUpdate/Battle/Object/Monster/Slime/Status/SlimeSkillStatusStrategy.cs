using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Object.Monster.Slime.Status
{
    /// <summary>
    /// ʷ��ķ����״̬���Ӳ���
    /// </summary>
    public class SlimeSkillStatusStrategy : IStatusAddStrategy
    {
        public void ToAdd(IBattleEntityObject sourcer, List<IBattleEntityObject> targets, params int[] statusIds)
        {
            // foreach (int id in statusIds)
            // {
            //     StatusInfo statusInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            //     IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().GetStatus(id);
            //
            //     foreach (IBattleEntityObject target in targets)
            //     {
            //         // ��ʼ��Buff״̬
            //         status.InitStatus(sourcer, target, id);
            //         target.GetComponent<StatusComponent>().AddStatus(status);
            //     }
            // }
        }
    }
}
