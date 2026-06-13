// using System.Collections.Generic;
// using HotUpdate.Base;
// using HotUpdate.Game.Battle.Context;
// using HotUpdate.Game.Battle.Object;
//
// namespace HotUpdate.Game.Point
// {
//     public interface IBattlePointProxy
//     {
//         /// <summary>
//         /// 场景战斗点
//         /// </summary>
//         BattlePoint BattlePoint { get; }
//
//         /// <summary>
//         /// 初始化战斗点对象
//         /// </summary>
//         /// <param name="ctx"></param>
//         /// <param name="players"></param>
//         void InitProxy(IBattleContext ctx, List<IBattleEntityObject> players);
//
//         /// <summary>
//         /// 更新相机
//         /// 传入行动的玩家或被攻击的玩家
//         /// </summary>
//         /// <param name="battleEntity">当前操作的玩家对象</param>
//         System.Threading.Tasks.Task UpdateCamera(IBattleEntityObject battleEntity);
//
//         /// <summary>
//         /// 销毁代理
//         /// </summary>
//         void Dispose();
//         
//         /// <summary>
//         /// 更新怪物位置
//         /// </summary>
//         /// <param name="battleEntity"></param>
//         void UpdateMonsterPos(IBattleEntityObject battleEntity);
//     }
// }
