using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Íæ¼Ò½ÇÉ«
    /// </summary>
    public class PlayerObject : BattleObject
    {
        public override void BaseInit(int id)
        {
            base.BaseInit(id);
        }

        public override IEnumerator ExecuteAction()
        {
            throw new System.NotImplementedException();
        }

        public override int GetSpeed()
        {
            throw new System.NotImplementedException();
        }
    }
}
