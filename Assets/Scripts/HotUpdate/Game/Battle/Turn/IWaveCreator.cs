using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Turn
{
    public interface IWaveCreator
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_battleContext"></param>
        /// <param name="waveDatas"></param>
        void Init(IBattleContext _battleContext, List<WaveData> waveDatas);
        
        /// <summary>
        /// 创建当前波次
        /// </summary>
        Task<List<IBattleEntityObject>> CreateWave();
        
        /// <summary>
        /// 当前回合的所有波次已经处理完毕，进入下一回合
        /// </summary>
        /// <returns>若为true，则存在下一波次；否则返回false，代表战斗结束</returns>
        bool MoveWave();

        /// <summary>
        /// 检查当前波次是否结束
        /// </summary>
        /// <returns>true为结束；false为未结束</returns>
        bool CheckOver();
    }
}
