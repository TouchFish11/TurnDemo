using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 波次创建器
    /// </summary>
    public class WaveCreator : IWaveCreator
    {
        [Inject] private IBattleManager _battleManager;
        
        private WaveHandler _waveHandler;
        // 波次数据列表，长度代表总波次，每波次可独立配置
        private List<WaveData> _waveDatas;
        // 当前波次在列表的位置索引
        private int _waveIndex;
        
        public void Init(IBattleContext _battleContext, List<WaveData> waveDatas)
        {
            _waveHandler = DIContainer.Create<WaveHandler>(parameterValues: _battleContext);
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
            return await _battleManager.GetBattleService().CreateMonsters(_waveDatas[_waveIndex].MonsterIds.ToArray());
        }
    }
}
