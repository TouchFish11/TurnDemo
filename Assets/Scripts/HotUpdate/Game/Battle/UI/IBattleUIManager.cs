using System;
using System.Collections;
using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.UI
{
    public interface IBattleUIManager : IDisposable
    {
        /// <summary>
        /// 显示战斗结束界面
        /// 包含协程逻辑，控制界面显示时长后触发退出战斗事件
        /// </summary>
        /// <param name="context">战斗上下文，用于触发退出战斗事件</param>
        void ShowBattleOver(IBattleContext context);

        /// <summary>
        /// 显示战斗开始界面
        /// </summary>
        void ShowBattleStart();

        /// <summary>
        /// 显示战斗提示信息
        /// 异步创建提示UI并初始化文本内容
        /// </summary>
        /// <param name="msg">要显示的提示文本内容</param>
        void ShowBattleMessage(string msg);

        /// <summary>
        /// 显示伤害文本（飘字）
        /// 包含伤害文本位置计算、UI初始化、累计伤害更新逻辑
        /// </summary>
        /// <param name="damageResult">伤害结算结果数据</param>
        void ShowDamageText(DamageResult damageResult);

        /// <summary>
        /// 显示护盾文本（飘字）
        /// 包含护盾文本位置计算、UI初始化
        /// </summary>
        /// <param name="target">目标战斗实体</param>
        /// <param name="sheilAmount">护盾量</param>
        void ShowShieldText(IBattleEntityObject target, int sheilAmount);

        /// <summary>
        /// 显示治疗文本（飘字）
        /// 逻辑与伤害文本类似
        /// </summary>
        /// <param name="target">治疗目标战斗实体</param>
        /// <param name="healAmount">治疗量</param>
        void ShowHealText(IBattleEntityObject target, int healAmount);

        /// <summary>
        /// 显示状态效果文本（Buff/Debuff飘字）
        /// 状态添加时显示对应的状态名称文本
        /// </summary>
        /// <param name="newStatus">新增的状态实例</param>
        void ShowStatusText(IStatus newStatus);

        /// <summary>
        /// 更新累计伤害UI显示
        /// 控制累计伤害区域的激活状态，并更新数值
        /// </summary>
        /// <param name="isShow">是否显示累计伤害UI</param>
        /// <param name="dmg">本次新增伤害值</param>
        void UpdateCumulativeDamage(bool isShow, int dmg);

        /// <summary>
        /// 更新等待行动队列UI
        /// 为每个等待行动的战斗实体创建对应的UI并初始化
        /// </summary>
        /// <param name="battleEntities">等待行动的战斗实体列表</param>
        void UpdateWaitingCommmand(List<IBattleEntityObject> battleEntities);

        /// <summary>
        /// 更新行动条（ActionBar）UI
        /// 为每个战斗实体创建行动格子UI，第一个实体的格子会特殊放大
        /// </summary>
        /// <param name="battleEntities">需要显示在行动条的战斗实体列表</param>
        void UpdateActionBar(IEnumerable<IBattleEntityObject> battleEntities);

        /// <summary>
        /// 设置行动格子高亮状态
        /// 根据选中的目标列表，高亮对应的行动格子
        /// </summary>
        /// <param name="selectedTargets">选中的目标实体列表</param>
        void SetActionGridHighlights(List<IBattleEntityObject> selectedTargets);

        /// <summary>
        /// 清理选中目标标记UI
        /// 重置所有选中目标的视觉标记
        /// </summary>
        void ClearSelectMarker();

        /// <summary>
        /// 设置技能操作区UI
        /// 传入null则清空操作区
        /// </summary>
        void ClearOperator();

        /// <summary>
        /// 设置行动提示的激活状态
        /// 控制"当前行动方"提示文本的显示/隐藏及内容
        /// </summary>
        /// <param name="actTipType">行动提示类型</param>
        void SetActTipActive(EActTipType actTipType);

        /// <summary>
        /// 更新技能操作区UI
        /// 根据当前行动实体和技能数据提供器，创建并初始化技能按键UI
        /// 需先调用SetActTipActive设置行动提示
        /// </summary>
        /// <param name="currentObject">当前行动的战斗实体</param>
        /// <param name="dataProvider">技能按键UI数据提供器</param>
        void UpdateOperator(IBattleEntityObject currentObject, ISkillKeyUIDataProvider dataProvider);

        /// <summary>
        /// 设置目标标记UI
        /// 为选中的目标实体创建视觉标记，传入null则清空标记
        /// </summary>
        /// <param name="selectedTargets">选中的目标实体列表</param>
        /// <param name="skillTargetType"></param>
        void SetTargetMarkers(List<IBattleEntityObject> selectedTargets, E_SkillTargetType skillTargetType);

        /// <summary>
        /// 更新战斗点数（能量/怒气）UI
        /// 动态创建点数UI并设置激活状态（已解锁/未解锁）
        /// </summary>
        /// <param name="current">当前可用点数</param>
        /// <param name="max">总点数上限</param>
        /// <returns>异步任务</returns>
        System.Threading.Tasks.Task UpdateBattlePointCount(int current, int max);

        /// <summary>
        /// 更新玩家状态条UI
        /// 刷新指定实体的血量、能量等状态数值显示
        /// </summary>
        /// <param name="currentBattleEntity">需要更新的战斗实体</param>
        void UpdatePlayerStatuebar(IBattleEntityObject currentBattleEntity);

        /// <summary>
        /// 显示角色立绘（必杀技）提示
        /// 短暂显示立绘图标和名称后隐藏
        /// </summary>
        /// <param name="roleInfo">角色信息</param>
        /// <param name="skillInfo">技能信息</param>
        IEnumerator ShowPaiting(RoleInfo roleInfo, SkillInfo skillInfo);
    }
}
