using Core.DataPersistence.Binary;
using Core.Service;

namespace Game.Battle.Property
{
    /// <summary>
    /// ��������
    /// </summary>
    public class MonsterProperty : BattleProperty
    {
        public override void InitProperty(int id)
        {
            base.InitProperty(id);
            MonsterInfo monsterInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Editor).dataDic[id];

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
