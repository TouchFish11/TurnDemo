

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 怪物属性
    /// </summary>
    public class MonsterProperty : BattleProperty
    {
        public void InitProperty(MonsterInfo monsterInfo)
        {
            battleId = monsterInfo.f_id;
            
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
