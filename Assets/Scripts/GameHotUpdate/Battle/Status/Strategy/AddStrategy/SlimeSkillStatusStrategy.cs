using System.Collections.Generic;
using Core.DataPersistence.Binary;
using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Status;
using GameHotUpdate.Status;

namespace GameHotUpdate.Battle.Status.Strategy.AddStrategy
{
    /// <summary>
    /// ʷ��ķ����״̬���Ӳ���
    /// </summary>
    public class SlimeSkillStatusStrategy : IStatusAddStrategy
    {
        public void ToAdd(IBattleEntityObject sourcer, List<IBattleEntityObject> targets, params int[] statusIds)
        {
            foreach (int id in statusIds)
            {
                StatusInfo statusInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(EConfigLoadType.Excel).dataDic[id];
                IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().GetStatus(id);

                foreach (IBattleEntityObject target in targets)
                {
                    // ��ʼ��Buff״̬
                    status.InitStatus(sourcer, target, id);
                    target.GetComponent<StatusComponent>().AddStatus(status);
                }
            }
        }
    }
}
