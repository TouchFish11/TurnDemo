using System;

namespace Game.Test.Animation
{
    /// <summary>
    /// 动画类型（位标记枚举，支持组合使用）
    /// </summary>
    [Flags]
    public enum EAnimationType
    {
        Null = 0,          // 无
        Idle = 1 << 0,     // 待机
        Walk = 1 << 1,     // 行走
        Run = 1 << 2,      // 奔跑
        Jump = 1 << 3,     // 跳跃
        Fall = 1 << 4,     // 下落
        Dash = 1 << 5,     // 冲刺
        Die = 1 << 6,      // 死亡
        Interact = 1 << 7, // 交互
        Attack = 1 << 8,   // 攻击
        All = Null | Idle | Walk | Run | Jump | Fall | Dash | Die | Interact | Attack  // 所有类型
    }
}
