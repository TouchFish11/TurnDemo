using System;
using System.Collections;
using Core.Service;
using Core.Utility;
using HotUpdate.Core.Battle.Damage;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Projectile
{
    /// <summary>
    /// 抛射物基类（所有子弹/技能弹道等抛射物的抽象基类）
    /// 负责抛射物的基础初始化、粒子系统管理、伤害计算依赖注入等核心基础逻辑
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))] // 强制挂载粒子系统组件，用于抛射物视觉表现
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        /// <summary>
        /// 视觉特效信息（存储抛射物对应的特效配置）
        /// </summary>
        protected VFXInfo vFXInfo;

        /// <summary>
        /// 抛射物核心数据（包含伤害、飞行速度、技能关联等配置）
        /// </summary>
        protected ProjectileData projectileData;

        /// <summary>
        /// 抛射物绑定的粒子系统组件（用于播放弹道/命中特效）
        /// 重命名以覆盖UnityEngine的默认命名，避免歧义
        /// </summary>
        protected new ParticleSystem particleSystem;

        /// <summary>
        /// 伤害计算管理器（用于计算抛射物命中后的伤害数值）
        /// </summary>
        protected IDamageCalcManager damageCalcManager;

        /// <summary>
        /// 触发时间点数组（记录抛射物在哪些时间点触发判定）
        /// </summary>
        protected float[] triggerTimes;

        /// <summary>
        /// 抛射物命中后要附加的Buff/状态ID数组
        /// </summary>
        protected int[] statusIds;

        /// <summary>
        /// 组件唤醒时初始化（Unity生命周期）
        /// 主要完成核心组件和服务的初始化
        /// </summary>
        private void Awake()
        {
            // 获取挂载在当前GameObject上的粒子系统组件
            particleSystem = GetComponent<ParticleSystem>();
            // 从服务定位器获取伤害计算管理器实例（依赖注入）
            damageCalcManager = ServiceLocator.Get<IDamageCalcManager>();
        }

        /// <summary>
        /// 初始化抛射物核心数据
        /// </summary>
        /// <param name="projectileData">抛射物配置数据</param>
        /// <param name="vFXInfo">特效配置信息</param>
        public void Init(ProjectileData projectileData, VFXInfo vFXInfo)
        {
            // 赋值特效配置
            this.vFXInfo = vFXInfo;
            // 赋值抛射物核心数据
            this.projectileData = projectileData;

            // 解析技能关联的状态ID：
            // 判断抛射物是否关联技能
            // 若关联则拆分技能配置的状态ID字符串为int数组
            // 若无关联则初始化为空数组
            statusIds = projectileData.skill != null ? 
                TextUtility.SplitToIntArr(projectileData.skill.SkillInfo.f_statusId, 2) 
                : Array.Empty<int>();

            triggerTimes = projectileData.skill != null
                ? TextUtility.SplitTofloatArr(projectileData.skill.SkillInfo.f_dmgTimes, 2)
                : Array.Empty<float>();
            
            // 播放VFX
            StartCoroutine(PlayingVFX());
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator PlayingVFX();
        
        /// <summary>
        /// 在触发时添加Buff
        /// </summary>
        protected abstract void AddStatusOnTrigger();
        
        /// <summary>
        /// 在触发时应用效果，伤害、回能
        /// </summary>
        protected abstract void ApplyEffectOnTrigger();
        
        /// <summary>
        /// 在触发时创建特效
        /// </summary>
        protected abstract void CreateVFXOnTrigger();

        /// <summary>
        /// 处理计时逻辑
        /// </summary>
        protected abstract void HandleTiming();
    }
}