using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate.Game.Point
{
    /// <summary>
    /// 场景战斗点
    /// </summary>
    public class BattlePoint : MonoBehaviour
    {
        [SerializeField] private List<Transform> roleTrans;

        [SerializeField] private List<Transform> monsterTrans;

        [SerializeField] private Transform monsterPointCenter;

        [SerializeField] private List<Transform> roleCamerasTrans;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Transform> GetRoleTransforms()
        {
            foreach (var playerTran in roleTrans)
            {
                yield return playerTran;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Transform> GetMonsterTransforms()
        {
            foreach (var monsterTran in monsterTrans)
            {
                yield return monsterTran;
            }
        }

        public Transform GetRoleTransByIndex(int index)
        {
            return roleTrans[index];
        }

        public Transform GetMonsterTransByIndex(int index)
        {
            return monsterTrans[index];
        }
        
        public Transform GetRoleCameraTransByIndex(int index)
        {
            return roleCamerasTrans[index];
        }

        public Transform MonsterCenter => monsterPointCenter;
    }
}
