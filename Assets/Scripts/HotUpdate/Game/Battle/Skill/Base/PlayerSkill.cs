// using HotUpdate.Game.Battle.Context;
// using HotUpdate.Game.Battle.Event.UI;
// using HotUpdate.Game.Battle.Property;
//
// namespace HotUpdate.Game.Battle.Skill.Base
// {
//     /// <summary>
//     /// 玩家技能抽象基类
//     /// 所有玩家角色技能需继承此类
//     /// </summary>
//     public abstract class PlayerSkill : Skill
//     {
//         protected PlayerSkill(SkillContext skillContext) : base(skillContext)
//         {
//             
//         }
//
//         /// <summary>
//         /// 玩家技能释放前的预处理逻辑
//         /// 包含战斗点数消耗、技能UI事件触发等核心逻辑
//         /// 不处理终结技相关
//         /// </summary>
//         /// <param name="context">战斗上下文，包含当前战斗的所有环境信息和操作接口</param>
//         protected sealed override void OnPreCast(IBattleContext context)
//         {
//             // 初始化技能的目标对象（如选定的敌人、友方等）
//             //battleCoordinator.InitSkillTarget(this);
//             // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
//             //context.ConsumeSkillPoint(SkillContext.SkillInfo.f_costBP);
//             // 触发UI事件总线，通知前端更新技能释放相关的UI界面
//             if (SkillContext.SkillInfo.f_SkillType != (byte)E_SkillType.UltimateSkill)
//             {
//                 context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
//             }
//             
//             InitProjectile();
//         }
//
//         /// <summary>
//         /// 初始化弹射物
//         /// 子类技能重写，实现弹射物ProjectileData、ProjectileTrans、VFXInfo
//         /// </summary>
//         protected abstract void InitProjectile();
//     }
// }