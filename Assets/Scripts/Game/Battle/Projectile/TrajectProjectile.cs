using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// πÏº£µØ…‰ŒÔ
    /// </summary>
    public abstract class TrajectProjectile : Projectile
    {

        protected virtual IEnumerator PlayingVFX()
        {
            float nowTime = 0;
            int index = 0;
            while (particleSystem.IsAlive() && index < dmgTimes.Length)
            {
                nowTime += Time.deltaTime;
                if (nowTime >= dmgTimes[index])
                {
                    Trigger();
                    index++;
                }
                yield return null;
            }
        }
    }
}
