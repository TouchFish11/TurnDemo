using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Point;
using UnityEngine;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 波次创建器
    /// </summary>
    public class WaveCreator : IWaveCreator
    {
        [Inject] private RoleFactory _roleFactory;
        [Inject] private MonsterFactory _monsterFactory;
        [Inject] private IBattlePointProxy _battlePointProxy;
        [Inject] private WaveHandler _waveHandler;
        
        // 战斗上下文
        private IBattleContext _context;
        // 波次数据列表，长度代表总波次，每波次可独立配置
        private List<WaveData> _waveDatas;
        // 当前波次在列表的位置索引
        private int _waveIndex;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        /// <param name="waveDatas"></param>
        public void Init(IBattleContext context, List<WaveData> waveDatas)
        {
            _context = context;
            _waveDatas = waveDatas;
            _waveIndex = 0;
        }

        /// <summary>
        /// 检查当前波次是否结束
        /// </summary>
        /// <returns>true为结束；false为未结束</returns>
        public bool CheckOver()
        {
            return _waveHandler.CheckOver();
        }
        
        /// <summary>
        /// 推进到下一波次
        /// </summary>
        /// <returns>若为true，则存在下一波次并推进；否则返回false，代表所有波次结束</returns>
        public bool MoveWave()
        {
            if (_waveIndex < _waveDatas.Count)
            {
                ++_waveIndex;
                _waveHandler.UpdateCondition(_waveDatas[_waveIndex].WaveVictoryConditionType);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建当前波次
        /// </summary>
        public async Task<List<IBattleEntityObject>> CreateWave()
        {
            // 创建当前波次的怪物
            return await CreateMonsters(_waveDatas[_waveIndex].MonsterIds.ToArray());
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<List<IBattleEntityObject>> CreateRoles(params int[] roleIds)
        {
            var roles = new List<IBattleEntityObject>(roleIds.Length);
            var playerTrans = new List<Transform>(_battlePointProxy.BattlePoint.GetRoleTransforms());
            for (var i = 0; i < roleIds.Length; i++)
            {
                var roleId = roleIds[i];
                var transform = playerTrans[i];
                // 创建角色对象
                var playerObject = await _roleFactory.CreateRole(roleId, transform);
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
            var monsterTrans = new List<Transform>(_battlePointProxy.BattlePoint.GetMonsterTransforms());
            // 批量创建怪物
            if (monsterIds.Length == monsterTrans.Count)
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var monsterId = monsterIds[i];
                    var transform = monsterTrans[i];
                    // 创建怪物对象
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
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
                var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
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
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
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
    }
}
