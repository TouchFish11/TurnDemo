using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗组件
    /// </summary>
    public abstract class BattleComponent : BaseComponent, IBattleComponent
    {
        public IBattleEntityObject BattleEntity { get; private set; }

        protected IBattleContext Context { get; private set; }

        protected sealed override void Awake()
        {
            base.Awake();
            BattleEntity = (IBattleEntityObject)EntityObject;
            // 从战斗实体中获取战斗上下文
            Context = BattleEntity.Context;
        }

        protected sealed override void OnInit()
        {
            OnBattleInit();
        }

        /// <summary>
        /// 战斗初始化逻辑，子类按需实现初始化自身
        /// </summary>
        protected virtual void OnBattleInit()
        {
            
        }
        
        /// <summary>
        /// 战斗销毁逻辑
        /// </summary>
        protected abstract void OnBattleDestroy();

        protected sealed override void OnBaseDestroy()
        {
            // 战斗特有清理
            OnBattleDestroy();
            BattleEntity = null;
            // 通用清理
            base.OnBaseDestroy();
        }
    }
}
