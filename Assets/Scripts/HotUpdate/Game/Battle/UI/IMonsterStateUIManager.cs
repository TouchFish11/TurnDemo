using System.Threading.Tasks;
using HotUpdate.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.UI
{
    public interface IMonsterStateUIManager
    {
        /// <summary>
        /// 缓存怪物UI
        /// </summary>
        /// <param name="monsterObject"></param>
        /// <param name="monsterStateArea"></param>
        Task CreateNormalMonsterStateUI(IBattleEntityObject monsterObject, RectTransform monsterStateArea);

        /// <summary>
        /// 移除指定怪物UI
        /// </summary>
        /// <param name="deadMonster"></param>
        void RemoveNormalMonsterStateUI(IBattleEntityObject deadMonster);

        /// <summary>
        /// 激活所有怪物血量UI显示
        /// </summary>
        void ActiveMonsterUIs();

        /// <summary>
        /// 失活所有怪物血量UI显示
        /// </summary>
        void InActiveMonsterUIs();

        /// <summary>
        /// 激活指定怪物血量UI显示
        /// 激活指定怪物UI，其它失活
        /// </summary>
        void ActiveMonsterUI(params IBattleEntityObject[] mosters);

        /// <summary>
        /// 失活指定怪物血量UI显示
        /// 失活指定怪物UI，其它激活
        /// </summary>
        /// <param name="mosters"></param>
        void InActiveMonsterStateUI(params IBattleEntityObject[] mosters);

        /// <summary>
        /// 移除所有血量UI
        /// </summary>
        void RemoveAll();
    }
}
