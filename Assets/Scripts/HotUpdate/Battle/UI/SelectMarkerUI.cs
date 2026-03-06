using System.Collections.Generic;
using Core.Mono;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Battle.UI
{
    /// <summary>
    /// 选择标记UI组件
    /// 用于在战斗中显示选中目标的标记，包含跟随目标、旋转和缩放动画效果
    /// </summary>
    public class SelectMarkerUI : UIBehaviourBase
    {
        // 标记旋转速度（每秒旋转角度）
        private const float markerRotationSpeed = 40f;
        // 标记缩放速度
        private const float markerScaleSpeed = 8f;
        // 标记的图片UI集合（用于控制标记颜色）
        private readonly List<Image> images = new();
        // 初始旋转角度（用于重置标记旋转）
        private Quaternion originQuaterion;
        // 初始缩放比例
        private readonly Vector3 originScale = Vector3.one * 1.9f;
        // 最终缩放比例
        private Vector3 endScale;
        // 颜色定义 - 敌方标记红色
        private readonly Color enermyRed = Color.red;
        // 颜色定义 - 友方标记蓝色
        private readonly Color friendBlue = new(0.5686275f,0.937088f,0.943f);

        // 绑定的战斗实体（被标记的目标）
        private IBattleEntityObject battleEntity;
        // 选择标记的父节点（用于UI坐标计算）
        private Transform selectMarkerArea;

        /// <summary>
        /// 初始化组件（Awake生命周期）
        /// 加载标记图片UI、记录初始旋转和缩放值
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            // 加载7个标记图片控件并加入集合
            for (var i = 0; i < 7; i++)
            {
                images.Add(binder.GetControl<Image>($"m{i}"));
            }

            // 记录初始旋转和缩放，用于后续重置
            originQuaterion = transform.rotation;
            endScale = transform.localScale;
        }

        /// <summary>
        /// 组件启用时执行（OnEnable生命周期）
        /// 重置标记状态、注册帧更新监听
        /// </summary>
        protected override void OnEnable()
        {
            // 重置旋转为初始状态
            transform.rotation = originQuaterion;
            // 重置缩放为放大状态
            transform.localScale = originScale;
            // 注册Update监听，驱动标记动画和跟随逻辑
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化选择标记
        /// </summary>
        /// <param name="battleEntity">绑定的战斗实体（目标）</param>
        /// <param name="skillTargetType">技能目标类型（敌方/友方）</param>
        /// <param name="selectMarkerArea">标记的父节点（用于坐标计算）</param>
        public void InitSelectMarker(IBattleEntityObject battleEntity, E_SkillTargetType skillTargetType, Transform selectMarkerArea)
        {
            // 绑定目标实体和父节点
            this.battleEntity = battleEntity;
            this.selectMarkerArea = selectMarkerArea;
            
            // 根据目标类型选择标记颜色（敌方红/友方蓝）
            var color = skillTargetType == E_SkillTargetType.Enemy ? enermyRed : friendBlue;
            // 给所有标记图片设置颜色
            for (var i = 0; i < images.Count; i++)
            {
                if (i == 0)
                {
                    // 由于该UI由多个img拼接而成，第一个img做特殊处理，才能表现好看
                    var m0 = images[i];
                    m0.color = new Color(color.r, color.g, color.b, 0.2f);
                    continue;
                }
                images[i].color = color;      
            }
        }

        /// <summary>
        /// 帧更新逻辑（通过IMonoAdapter注册）
        /// 每帧执行目标跟随和动画更新
        /// </summary>
        private void OnUpdate()
        {
            // 让标记跟随目标位置
            FollowTarget();
            // 更新标记的旋转和缩放动画
            UpdateMarkerAnimation();
        }

        /// <summary>
        /// 跟随目标位置
        /// 将标记UI同步到目标实体的世界位置（带偏移）
        /// </summary>
        private void FollowTarget()
        {
            // 目标为空则不处理
            if (battleEntity == null)
            {
                return;
            }

            // 世界坐标转UI坐标，将标记定位到目标位置上方50像素处
            UIUtility.WorldToLocalPointInRectangle(
                ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera,  // 战斗主相机
                ServiceLocator.Get<IUIManager>().UICamera,                    // UI相机
                selectMarkerArea,                                             // UI父节点
                gameObject,                                                    // 当前标记UI对象
                battleEntity.GameObject.transform.position + Vector3.up * 0.5f                    // 目标世界位置
                //Vector2.up * markerYOffset                               // 向上偏移
            );
        }

        /// <summary>
        /// 更新标记动画
        /// 包含旋转动画和缩放动画
        /// </summary>
        private void UpdateMarkerAnimation()
        {
            // 绕Z轴旋转（每帧旋转速度 * 帧时间）
            transform.Rotate(Vector3.forward, markerRotationSpeed * Time.deltaTime);
            
            // 缩放
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, Time.deltaTime * markerScaleSpeed);
        }

        /// <summary>
        /// 组件禁用时执行（OnDisable生命周期）
        /// 移除帧更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            // 移除Update监听，停止动画和跟随逻辑
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}