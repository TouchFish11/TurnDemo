using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Pool;
using Core.Service;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Layer;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Point;
using UnityEngine;

namespace HotUpdate.Battle.Turn
{
    /// <summary>
    /// 回合创建器
    /// </summary>
    public class TurnCreator : IPoolData
    {
        // 战斗上下文
        private IBattleContext _context;
        // 一回合共有多少波次，每波次创建的怪物ID
        private List<List<int>> _waves;
        // 当前波次
        private int _waveIndex;
        // 总回合数
        private int _totalTurnNum;
        // 当前回合
        private int _turnIndex;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        /// <param name="totalTurnNum"></param>
        /// <param name="waves"></param>
        public void Init(IBattleContext context, int totalTurnNum, List<List<int>> waves)
        {
            _context = context;
            _waves = waves;
            _totalTurnNum = totalTurnNum;
        }

        /// <summary>
        /// 检查战斗是否结束
        /// true为结束
        /// </summary>
        /// <returns></returns>
        public bool CheckBattleOver()
        {
            // 存在剩余回合
            while (_turnIndex < _totalTurnNum)
            {
                if (_waveIndex < _waves.Count)
                {
                    return false;
                }

                // 当前回合的所有波次已经处理完毕，进入下一回合
                ++_turnIndex;
                // 重置波次索引
                _waveIndex = 0;
            }
            
            return true;
        }

        /// <summary>
        /// 创建当前波次
        /// </summary>
        public async Task<List<IBattleEntityObject>> CreateWave()
        {
            // 创建当前波次的怪物
            return await CreateMonsters(_waves[_waveIndex++].ToArray());
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<List<IBattleEntityObject>> CreateRoles(params int[] roleIds)
        {
            var roles = new List<IBattleEntityObject>(roleIds.Length);
            var playerTrans = new List<Transform>(ServiceLocator.Get<IBattlePointProxy>().BattlePoint.GetRoleTransforms());
            for (var i = 0; i < roleIds.Length; i++)
            {
                var roleId = roleIds[i];
                var transform = playerTrans[i];
                // 创建角色对象
                var playerObject = await RoleBuilder.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                playerObject.BattleInit(roleId, _context);
                // 记录角色所在的位置索引
                playerObject.EntityPosIndex = i;
                // 设置角色层级
                LayerUtility.SetLayerRecursively(playerObject.GameObject, LayerGeter.GetRoleLayerByIndex(i));
                roles.Add(playerObject);
            }

            return roles;
        }
        
        /// <summary>
        /// 创建怪物
        /// </summary>
        /// <returns></returns>
        private async Task<List<IBattleEntityObject>> CreateMonsters(int[] monsterIds)
        {
            var monsters = new List<IBattleEntityObject>(monsterIds.Length);
            var monsterTrans = new List<Transform>(ServiceLocator.Get<IBattlePointProxy>().BattlePoint.GetMonsterTransforms());
            // 批量创建怪物
            if (monsterIds.Length == monsterTrans.Count)
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var monsterId = monsterIds[i];
                    var transform = monsterTrans[i];
                    // 创建怪物对象
                    var monsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                    // 设置名称
                    monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{i}";
                    // 注入上下文，供角色内部组件使用
                    monsterObject.BattleInit(monsterId, _context);
                    // 记录怪物所在的位置索引
                    monsterObject.EntityPosIndex = i;
                    // 设置怪物层级
                    LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(i));
                    // 缓存对象
                    monsters.Add(monsterObject);
                }
            }
            else if (monsterIds.Length == 1)
            {
                var monsterId = monsterIds[0];
                var transform = monsterTrans[2];
                // 创建怪物对象
                var monsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                // 设置名称
                monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{2}";
                // 注入上下文，供角色内部组件使用
                monsterObject.BattleInit(monsterId, _context);
                // 记录怪物所在的位置索引
                monsterObject.EntityPosIndex = 2;
                // 设置怪物层级
                LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(2));
                // 缓存对象
                monsters.Add(monsterObject);
            }
            else
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var monsterId = monsterIds[i];
                    var transform = monsterTrans[i + 1];
                    // 创建怪物对象
                    var monsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                    // 设置名称
                    monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{i + 1}";
                    // 注入上下文，供角色内部组件使用
                    monsterObject.BattleInit(monsterId, _context);
                    // 记录怪物所在的位置索引
                    monsterObject.EntityPosIndex = i + 1;
                    // 设置怪物层级
                    LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(i + 1));
                    // 缓存对象
                    monsters.Add(monsterObject);
                }
            }
            
            return monsters;
        }

        public void ResetData()
        {
            _context = null;
            _waves.Clear();
            _waves = null;

            _totalTurnNum = 0;
            _waveIndex = 0;
            _turnIndex = 0;
        }
    }
}
