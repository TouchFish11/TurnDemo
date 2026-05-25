using Core.Serialize.Binary;
using HotUpdate.Common.Config.ExcelInfo.Container;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 怪物属性
    /// </summary>
    public class MonsterProperty : BattleProperty
    {
        public override void InitProperty(int id)
        {
            base.InitProperty(id);
            var monsterInfo = binaryDataManager.GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[id];

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
