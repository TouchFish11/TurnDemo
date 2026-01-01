using Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Game.Battle
{
    /// <summary>
    /// 韧性组件
    /// 管理目标的韧性系统
    /// </summary>
    [ComponentId(nameof(ToughnessComponent))]
    public class ToughnessComponent : BattleComponent, IToughnessComponent
    {
        // 当前韧性状态
        private Toughness _toughness;

        void IToughnessComponent.Init(IBattleEntityObject owner, int[] elementTypes , int initialToughness)
        {
            List<E_ElementType> weakPropertys = new List<E_ElementType>(elementTypes.Length);
            foreach (var type in elementTypes)
            {
                weakPropertys.Add(type.ToElementType());
            }
            _toughness = new Toughness(weakPropertys, initialToughness);
        }

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            MonsterInfo monsterInfo = battleEntity.GetComponent<MonsterObject>().MonsterInfo;
            (this as IToughnessComponent).Init(battleEntity, TextUtility.SplitToIntArr(monsterInfo.f_weaknesses, 2), monsterInfo.f_baseToughness);

            // 订阅“技能释放事件”（监听所有技能释放，计算韧性）
            BattleEntity.Context.GetEventBus().AddListener<SkillCastEvent>(OnSkillCastHandler);
        }

        /// <summary>
        /// 削减韧性
        /// </summary>
        /// <param name="reducer"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        public void ReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, int value)
        {
            // 能否削减韧性
            if (!CanReduceToughness(propertyType))
            {
                return;
            }

            _toughness.ReduceToughness(value);
            // 触发韧性削减事件
            this.BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessChangedEvent(this.BattleEntity.Context, this.BattleEntity, _toughness.CurrentToughnessValue, _toughness.MaxToughnessVaue));
            // 判断是否破韧
            if (IsToughnessBroken())
            {
                this.BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessBrokenEvent(this.BattleEntity.Context, reducer, this.BattleEntity));
            }
        }

        /// <summary>
        /// 能否削减韧性
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private bool CanReduceToughness(E_ElementType propertyType)
        {
            // TODO：判断逻辑抽象为接口，便于拓展
            if (_toughness.WeakPropertys.Contains(propertyType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 事件回调：技能释放后，计算韧性伤害
        /// </summary>
        /// <param name="evt"></param>
        private void OnSkillCastHandler(SkillCastEvent skillCastEvent)
        {
            // 只处理当前组件所属角色的韧性（避免处理其他角色）
            if (!skillCastEvent.Contain(BattleEntity))
            {
                return;
            }

            // 技能对韧性造成削减（调用韧性API）
            //_toughness.ReduceToughness(skillCastEvent.PropertyType, 25);

            // 若韧性为0且未触发过破盾（防止重复触发）
            if (_toughness.IsBroken)
            {
                LogManager.Log($"\n{BattleEntity.GameObject.name}被击破！");

                // 广播“破盾事件”（通知其他模块“目标已破盾”）
                BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessBrokenEvent(skillCastEvent.Context, skillCastEvent.Skill.Caster, BattleEntity));
            }
        }

        /// <summary>
        /// 获取当前韧性状态
        /// </summary>
        /// <returns></returns>
        public bool IsToughnessBroken() => _toughness.IsBroken;

        /// <summary>
        /// 最大韧性值
        /// </summary>
        public int CurrentToughnessValue => _toughness.CurrentToughnessValue;

        /// <summary>
        /// 当前韧性值
        /// </summary>
        public int MaxToughnessVaue => _toughness.MaxToughnessVaue;

        /// <summary>
        /// 弱点属性列表
        /// </summary>
        public List<E_ElementType> WeakPropertys => _toughness.WeakPropertys;

    }
}
