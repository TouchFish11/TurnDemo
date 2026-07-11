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
        /// 角色场景根对象列表
        /// </summary>
        public List<Transform> RoleTrans => roleTrans;
        
        /// <summary>
        /// 怪物场景根对象列表
        /// </summary>
        public List<Transform> MonsterTrans => monsterTrans;

        /// <summary>
        /// 场景怪物区域的中心点
        /// </summary>
        public Transform MonsterCenter => monsterPointCenter;
        
        /// <summary>
        /// 场景角色相机点
        /// </summary>
        public List<Transform> RoleCamerasTrans => roleCamerasTrans;
    }
}
