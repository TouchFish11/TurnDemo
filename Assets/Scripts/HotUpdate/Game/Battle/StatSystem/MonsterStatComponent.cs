using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.StatSystem.Providers;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 怪物属性组件
    /// </summary>
    [ComponentId]
    public class MonsterStatComponent : StatsComponent
    {
        protected override void OnPreInitStats()
        {
            var monsterInfo = ((MonsterObject)BattleEntity).MonsterInfo;
            StatsComponentCore.StatSet = new MonsterStatSet();
            StatsComponentCore.StatSetProvider = new MonsterStatConfigProvider(monsterInfo);
        }

        protected override void OnBattleDestroy()
        {
            
        }
    }
}
