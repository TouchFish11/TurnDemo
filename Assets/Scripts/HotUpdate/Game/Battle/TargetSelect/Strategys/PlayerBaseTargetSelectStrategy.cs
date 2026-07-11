using System.Collections.Generic;
using Core.Log;

using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;

namespace HotUpdate.Game.Battle.TargetSelect.Strategys
{
    /// <summary>
    /// 玩家基础目标选择策略类
    /// 实现ITargetSelectStrategy接口，提供基于施法者类型和技能目标类型的默认目标选择逻辑
    /// </summary>
    public class PlayerBaseTargetSelectStrategy : ITargetSelectStrategy
    {
        /// <summary>
        /// 策略优先级
        /// 优先级为0，代表基础默认策略，可被更高优先级的策略覆盖
        /// </summary>
        public int Priority => 0;

        /// <summary>
        /// 选择技能主要目标的核心方法
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="caster">施法者实体（玩家/怪物）</param>
        /// <param name="skillInfo">技能信息，包含技能目标类型配置</param>
        /// <returns>选中的主要目标实体，无有效目标时返回null</returns>
        public IBattleEntityObject SelectMainTarget(List<IBattleEntityObject> targets, IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 声明选中的主目标变量
            IBattleEntityObject currentMainTarget;
            // 获取筛选后的有效目标数量
            var targetNum = targets.Count;
            // 根据有效目标数量选择最终目标
            switch (targetNum)
            {
                // 无有效目标：返回null，技能无法释放
                case 0:
                    Logger.LogError(ELogTags.Battle, $"无有效目标:{targetNum}，返回null。技能目标类型：{(E_SkillTargetType)skillInfo.f_SkillTargetType}，技能信息：{skillInfo.f_id}");
                    return null;
                // 仅有1个有效目标：直接选中该目标
                case 1:
                    currentMainTarget = targets[0];
                    break;
                // 多个有效目标：默认选中列表中间位置的目标
                // 逻辑说明：奇数个目标选正中间（如3个选索引1），偶数个选偏后位置（如4个选索引2）
                default:
                    currentMainTarget = targets[targetNum / 2];
                    break;
            }
            
            // 返回最终选中的主目标
            return currentMainTarget;
        }
    }
}