using GameLogic.BattleMoudule.Relic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// 遗器套装效果工厂（解耦套装效果创建，新增套装仅需扩展工厂）
    /// </summary>
    public class RelicSetEffectFactory
    {
        public static IRelicSetEffect Create(int setId)
        {
            return setId switch
            {
                0 => new QuantumRelicSetEffect(),
                // 新增套装时，仅需添加case：nameof(新套装类) => new 新套装效果类()
                _ => null
            };
        }
    }
}
