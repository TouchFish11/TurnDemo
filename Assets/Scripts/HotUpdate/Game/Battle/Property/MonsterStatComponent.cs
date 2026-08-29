using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 怪物属性组件
    /// </summary>
    [ComponentId]
    public class MonsterStatComponent : StatsComponent
    {
        
        protected override void OnBattleInit()
        {
            var monsterInfo = ((MonsterObject)BattleEntity).MonsterInfo;
            StatsComponentCore.StatSet = new MonsterStatSet();
            StatsComponentCore.StatSetProvider = new MonsterStatConfigProvider(monsterInfo);
            base.OnBattleInit();
        }

        protected override void OnBattleDestroy()
        {
            
        }
    }
}
