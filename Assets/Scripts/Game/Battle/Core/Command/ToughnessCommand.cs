using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 韧性相关命令
    /// 目前用于怪物韧性条恢复
    /// </summary>
    public class ToughnessCommand : ICommand
    {
        // 韧性恢复速度
        private float recoverySpeed = 40;

        /// <summary>
        /// 韧性组件
        /// </summary>
        public ToughnessComponent ToughnessComponent { get; private set; }

        public int Priority { get; private set; }

        /// <summary>
        /// 初始化韧性命令
        /// </summary>
        /// <param name="toughnessComponent"></param>
        public void Init(ToughnessComponent toughnessComponent)
        {
            ToughnessComponent = toughnessComponent;
        }

        public IEnumerator Excute(IBattleContext context)
        {
            float currentValue = 0;
            while (ToughnessComponent.CurrentToughnessValue < ToughnessComponent.MaxToughnessVaue)
            {
                currentValue += Time.deltaTime * recoverySpeed;
                ToughnessComponent.SetToughnessValue((int)currentValue, ToughnessComponent.MaxToughnessVaue);
                yield return null;
            }
        }

        void IPoolData.ResetData()
        {
            ToughnessComponent = null;
        }
    }
}
