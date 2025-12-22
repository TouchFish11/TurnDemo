using Framework;

namespace Game.Battle
{
    /// <summary>
    /// 破盾后追加穿刺
    /// </summary>
    public class BreakToughnessAdditionalAttack : IAdditionalAttack
    {
        public bool CanTrigger(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target)
        {
            // 触发条件：目标已破盾 + 攻击者是破盾者
            return target.GetComponent<ToughnessComponent>().IsToughnessBroken();
        }

        public void Execute(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target)
        {
            // 计算追加攻击伤害（配置表读取系数）
            int additionalDamage = (int)(attacker.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().MaxAtk * 0.8f);
            target.TakeDamage(additionalDamage, E_ElementType.Physical);
            LogManager.Log($"{attacker.Name}触发破盾追加攻击！{target.Name}额外受到{additionalDamage}点伤害");
        }
    }
}
