using Framework;

namespace Game.Battle
{
    /// <summary>
    /// 具体状态：持续回血状态（实现接口，封装自身逻辑）
    /// </summary>
    public class ContinuousHealStatus : IStatus
    {
        public bool IsValid { get; private set; } = true;

        // 剩余持续回合
        private int _remainingTurns;
        // 回血比例（配置表读取）
        private float _healRatio;

        public ContinuousHealStatus(int remainingTurns, float healRatio)
        {
            _remainingTurns = remainingTurns;
            _healRatio = healRatio;
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
    }
}
