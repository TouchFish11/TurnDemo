using System.Collections;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 韧性相关命令
    /// 目前用于怪物韧性条恢复
    /// </summary>
    public class ToughnessCommand : Command
    {
        // 韧性恢复速度
        private float recoverySpeed = 40;

        /// <summary>
        /// 韧性组件
        /// </summary>
        public ToughnessComponent ToughnessComponent { get; private set; }

        public override int Priority { get; protected set; }

        /// <summary>
        /// 初始化韧性命令
        /// </summary>
        /// <param name="toughnessComponent"></param>
        public void Init(ToughnessComponent toughnessComponent)
        {
            this.Sender = toughnessComponent.BattleEntity;
            ToughnessComponent = toughnessComponent;
        }

        public override IEnumerator Excute(IBattleContext context)
        {
            float currentValue = 0;
            while (ToughnessComponent.CurrentToughnessValue < ToughnessComponent.MaxToughnessVaue)
            {
                currentValue += Time.deltaTime * recoverySpeed;
                ToughnessComponent.SetToughnessValue((int)currentValue, ToughnessComponent.MaxToughnessVaue);
                yield return null;
            }
        }

        public override void ResetData()
        {
            base.ResetData();
            ToughnessComponent = null;
        }
    }
}
