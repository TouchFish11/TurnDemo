using Core.Components;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// �����������
    /// </summary>
    [ComponentId(typeof(MonsterPropertyComponent))]
    public class MonsterPropertyComponent : PropertyComponent
    {
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            battleProperty = new MonsterProperty();
            battleProperty.InitProperty(battleEntity.BattleEntityId);
        }
    }
}
