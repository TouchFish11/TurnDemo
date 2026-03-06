using Core.Serialize.Binary;
using Core.Service;
using HotUpdate.Core.Battle.Property;

namespace HotUpdate.Battle.Property
{
    /// <summary>
    /// ��������
    /// </summary>
    public class MonsterProperty : BattleProperty
    {
        public override void InitProperty(int id)
        {
            base.InitProperty(id);
            MonsterInfo monsterInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[id];

            baseHp = monsterInfo.f_baseHp;
            baseAtk = monsterInfo.f_baseAtk;
            baseDef = monsterInfo.f_baseDef;
            baseSpeed = monsterInfo.f_baseSpeed;

            currentHp = maxHp = baseHp;
            currentSpeed = baseSpeed;
            totalAtk = baseAtk;
            totalDef = baseDef;
            totalCrit = baseCrit;
            totalCritDmg = baseCritDmg;
        }
    }
}
