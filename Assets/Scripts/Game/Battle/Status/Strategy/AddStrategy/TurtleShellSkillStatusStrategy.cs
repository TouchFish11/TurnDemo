using Framework;
using Game.Battle;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 甲壳虫技能状态添加策略
/// </summary>
public class TurtleShellSkillStatusStrategy : IStatusAddStrategy
{
    public void ToAdd(IBattleEntityObject sourcer, List<IBattleEntityObject> targets, params int[] statusIds)
    {
        foreach (int id in statusIds)
        {
            StatusInfo statusInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];
            IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<StatusFactory>().GetStatus(id);

            foreach (IBattleEntityObject target in targets)
            {
                // 初始化Buff状态
                status.InitStatus(sourcer, target, id);
                target.GetComponent<StatusComponent>().AddStatus(status);
            }
        }
    }
}
