using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Battle.Property;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// ��������
    /// </summary>
    public class MonsterProperty : BattleProperty
    {
        public override void InitProperty(int id)
        {
            base.InitProperty(id);
            MonsterInfo monsterInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[id];

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
