using System.Collections.Generic;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.MonsterStateUI
{
    /// <summary>
    /// 普通怪物头顶状态UI栏
    /// 负责显示怪物的血条、韧性、弱点等状态，并跟随怪物位置更新UI
    /// </summary>
    public class MonsterStatusBar : UIBehaviourBase, ILogicView<MonsterStatusBar, MonsterStatusBarLogic>
    {
        // 血量渐变遮罩（用于血量变化时的渐变动画效果）
        [InjectUI] public Image imgFade;
        // 血量填充图（实时显示当前血量比例）
        [InjectUI] public Image imgHp;
        // 韧性填充图（显示当前韧性比例）
        [InjectUI] public Image imgToughness;

        // 怪物状态UI的父节点（用于UI的层级管理和坐标转换）
        public Transform monsterStateArea;
        // 血量渐变动画速度（控制fade遮罩的动画速率）
        public float fadeSpeed = 1f;
        private MonsterStatusBarLogic _logic;
        
        /// <summary>
        /// 弱点图标容器
        /// </summary>
        [InjectUI(1)] public RectTransform WeaknessBar { get; set; }

        /// <summary>
        /// 存储当前怪物的所有弱点图标
        /// </summary>
        public List<Image> Weakneses { get; } = new();
        
        public void Init(MonsterStatusBarLogic logic)
        {
            _logic = logic;
        }
    }
}