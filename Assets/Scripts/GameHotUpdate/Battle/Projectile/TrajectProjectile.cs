using System.Collections;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile
{
    /// <summary>
    /// �켣������
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
