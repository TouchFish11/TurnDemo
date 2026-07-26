using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object.Monster;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 怪物属性组件
    /// </summary>
    [ComponentId]
    public class MonsterPropertyComponent : PropertyComponent
    {
        
        protected override void OnBattleInit()
        {
            battleProperty = new MonsterProperty();
            ((MonsterProperty)battleProperty).InitProperty(((IMonsterObject)BattleEntity).MonsterInfo);
        }

        protected override void OnBattleDestroy()
        {
            
        }
    }
}
