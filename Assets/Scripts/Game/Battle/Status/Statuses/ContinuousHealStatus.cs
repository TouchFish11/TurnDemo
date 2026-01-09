using Framework;

namespace Game.Battle
{
    /// <summary>
    /// 持续回血状态
    /// </summary>
    public class ContinuousHealStatus : Status
    {
        // 剩余持续回合
        private int _remainingTurns;
        // 回血比例（配置表读取）
        private float _healRatio;

        public ContinuousHealStatus()
        {

        }

        public void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            if (!IsValid)
            {
                return;
            }

            // 调用角色的“回血API”执行具体逻辑（模块内部/依赖模块API调用）
            int healValue = (int)(owner.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().MaxHp * _healRatio);
            owner.Heal(healValue);
            LogManager.Log($"{owner.GameObject.name}触发持续回血，恢复{healValue}点HP");

            // 减少持续回合，过期则失效
            _remainingTurns--;
            if (_remainingTurns <= 0)
            {
                IsValid = false;
            }
        }

        public void OnTurnEnd(IBattleEntityObject owner, IBattleContext context)
        {
            /* 本状态无需回合结束逻辑 */
        }

        protected override void OnAdd()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRemove()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPineChanged()
        {
            throw new System.NotImplementedException();
        }
    }
}
