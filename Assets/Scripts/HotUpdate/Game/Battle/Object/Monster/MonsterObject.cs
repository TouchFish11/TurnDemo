using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.ResponsibilityChain.DamageChain;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Toughness;

namespace HotUpdate.Game.Battle.Object.Monster
{
    /// <summary>
    /// 怪物战斗对象
    /// 继承自BattleObject，封装了怪物的基础属性、战斗行为等核心逻辑
    /// </summary>
    public abstract class MonsterObject : BattleObject, IMonsterObject
    {
        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// 包含怪物ID、技能ID列表、组件名称列表等基础配置
        /// </summary>
        public MonsterInfo MonsterInfo { get; private set; }

        /// <summary>
        /// 战斗初始化方法
        /// 初始化怪物的技能列表和战斗组件，为进入战斗做最终准备
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context">战斗上下文（包含战斗管理器、回合管理器等核心战斗环境）</param>
        /// <param name="factory"></param>
        /// <param name="handler"></param>
        public void MonsterBattleInit(MonsterInfo info, IBattleContext context, Commandfactory factory, IDeathHandler handler)
        {
            BattleInit(info.f_id, context, factory, handler);
            
            MonsterInfo = info;
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetMonsterDamageChain();
            // 根据配置的组件名称列表，为怪物添加对应的战斗组件（如韧性组件、动画组件等）
            AddComponents(TextUtility.Split(MonsterInfo.f_comNames, 2));
            
            OnBattleInit();
        }
        
        /// <summary>
        /// 子类初始化
        /// </summary>
        protected abstract void OnBattleInit();

        public override void ExecuteAction()
        {
            base.ExecuteAction();
            // TODO：可以封装随机选择的策略类，用于玩家/怪物AI
            var skillId = SelectSkill();
            // 释放选中的技能
            CastSkill(skillId);
        }

        /// <summary>
        /// 选择技能
        /// </summary>
        /// <returns></returns>
        public abstract int SelectSkill();

        public override void CastSkill(int skillId)
        {
            var skillComponent = GetComponent<SkillComponent>();
            // 能否释放
            if (!skillComponent.CanCast(skillId))
            {
                return;
            }
            
            // 获取技能数据
            var skill = skillComponent.GetSkill(skillId);
            var toughnessComponent = GetComponent<ToughnessComponent>();
            // 获取怪物行动指令
            var actCommand = commandfactory.GetMonsterActCommand(toughnessComponent, skill);
            // 发送指令
            Context.GetEventBus().TriggerEvent(new InsertCommandEvent(Context, actCommand));
        }
        
        /// <summary>
        /// 怪物对象禁用时的清理逻辑
        /// 触发怪物死亡事件
        /// </summary>
        public override void Destroy()
        {
            Context.GetEventBus().TriggerEvent(new MonsterDeadEvent(Context, this));
            base.Destroy();
        }
    }
}