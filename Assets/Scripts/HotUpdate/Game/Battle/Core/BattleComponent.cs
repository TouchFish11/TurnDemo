using HotUpdate.Base.Component;
using HotUpdate.Base.Object;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗组件
    /// </summary>
    public abstract class BattleComponent : BaseComponent, IBattleComponent
    {
        public IBattleEntityObject BattleEntity { get; private set; }

        public void BattleInit(IBattleEntityObject battleEntity)
        {
            Init(battleEntity);
            BattleEntity = battleEntity;
        }

        public sealed override void Init(IEntityObject entityObject)
        {
            
        }

        public void DestroyBattle(IBattleEntityObject battleEntity)
        {
            OnBattleDestroy();
            BattleEntity = null;
        }
        
        /// <summary>
        /// 战斗销毁逻辑
        /// </summary>
        protected abstract void OnBattleDestroy();

        protected sealed override void OnDestroyBase()
        {
            
        }
    }
}
