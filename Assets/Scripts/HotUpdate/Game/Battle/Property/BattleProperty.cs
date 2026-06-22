using UnityEngine;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 战斗属性
    /// </summary>
    public abstract class BattleProperty
    {
        // 战斗实体ID
        protected int battleId;
        
        // 基础属性
        protected int baseHp;   // 生命值
        protected int baseAtk;  // 攻击力
        protected int baseDef;  // 防御力

        // 进阶属性
        protected int baseSpeed;    // 速度
        protected int baseCrit;     // 暴击率
        protected int baseCritDmg;  // 暴击伤害
        // ...

        // 动态属性
        protected int currentHp;    // 当前生命值
        protected int maxHp;    // 最大生命值
        protected int totalAtk;   // 总攻击力
        protected int totalDef;   // 总防御力
        protected int currentSpeed;     // 当前速度
        protected int totalCrit;    // 总暴击率
        protected int totalCritDmg;     // 总暴击伤害
        protected int currentShield;    // 当前护盾量

        // 静态属性
        public int BattleId => battleId;
        public int BaseHp => baseHp;
        public int BaseAtk => baseAtk;
        public int BaseDef => baseDef;
        public int BaseSpeed => baseSpeed;
        public int BaseCrit => baseCrit;
        public int BaseCritDmg => baseCritDmg;

        // 动态属性
        public int CurrentHp { get => currentHp; set => currentHp = Mathf.Clamp(value, 0, MaxHp); }
        public int MaxHp { get => maxHp; set => maxHp = value; }
        public int TotalAtk { get => totalAtk; set => totalAtk = value; }
        public int TotalDef { get => totalDef; set => totalDef = value; }
        public int CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
        public int TotalCrit { get => totalCrit; set => totalCrit = value; }
        public int TotalCritDmg { get => totalCritDmg; set => totalCritDmg = Mathf.Clamp(value, 0, value); }
        public int CurrentShield { get => currentShield; set => currentShield = Mathf.Clamp(value, 0, value); }
    }
}
