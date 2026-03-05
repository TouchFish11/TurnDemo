using Core.Mono;
using Core.Service;
using Core.UI;
using HotUpdate.Battle.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Battle.UI.ActionLine
{
    /// <summary>
    /// 行动格子UI组件
    /// 负责战斗场景中行动格子的视觉表现、选中状态、闪烁动画、位移动画等逻辑
    /// </summary>
    public class ActionGridUI : UIBehaviourBase
    {
        // 选中状态的背景图片
        [Inject] private Image imgSelect;
        // 行动格子的图标图片
        [Inject] private Image imgIcon;
        // 行动值显示文本
        [Inject] private TextMeshProUGUI txtActionValue;
        
        // 闪烁特效的根节点
        [Inject(1)] private RectTransform Flashing { get; set; }
        
        // 选中框水平移动的范围
        [SerializeField] private float moveRange = 3f;
        // 选中框移动的速度
        [SerializeField] private float moveSpeed = 5f;
        // 闪烁动画的速度
        [SerializeField] private float falshSpeed = 1.5f;
        
        // 选中框的矩形变换组件
        private RectTransform imgSelectRect;
        // 闪烁特效下的所有图片组件
        private Image[] images;
        // 闪烁图片当前的透明度
        private float currentAlpha = 1f;
        // 闪烁动画的计时变量
        private float time;
        // 选中框的初始本地位置（用于位移动画复位）
        private Vector3 initLocalPos; 
        // 绑定的战斗实体对象
        private IBattleEntityObject battleEntity;
        // 是否处于选中状态
        // 是否为第一个行动格子（用于区分缩放）
        private bool isFirstGrid;
        // 第一个格子的缩放系数
        private readonly float scaleFactor = 1.1f;
        // 当前格子的行动值
        private float actionValue;
        
        /// <summary>
        /// 只读属性：当前格子是否处于选中状态
        /// </summary>
        public bool IsSelect { get; private set; }

        /// <summary>
        /// 初始化函数（生命周期）
        /// 初始化组件引用、初始位置、默认状态等
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // 获取选中框的矩形变换组件并记录初始位置
            imgSelectRect = imgSelect.rectTransform;
            initLocalPos = imgSelectRect.localPosition;

            // 初始状态隐藏选中框和闪烁特效
            imgSelect.gameObject.SetActive(false);
            images = Flashing.GetComponentsInChildren<Image>();
            Flashing.gameObject.SetActive(false);
        }

        /// <summary>
        /// 组件启用时调用（生命周期）
        /// 注册帧更新监听，用于处理动画逻辑
        /// </summary>
        protected override void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化UI数据
        /// </summary>
        /// <param name="icon">格子显示的图标</param>
        /// <param name="actionValue">行动值</param>
        /// <param name="battleEntity">绑定的战斗实体</param>
        /// <param name="isFirst">是否为第一个行动格子</param>
        public void Init(Sprite icon, float actionValue, IBattleEntityObject battleEntity, bool isFirst)
        {
            this.battleEntity = battleEntity;
            isFirstGrid = isFirst;
            imgIcon.sprite = icon;
            this.actionValue = actionValue;
            txtActionValue.text = ((int)actionValue).ToString();

            // 根据是否为第一个格子更新缩放
            UpdateScale();
        }

        /// <summary>
        /// 更新格子缩放比例
        /// 第一个格子使用放大系数，其余格子为原始大小
        /// </summary>
        private void UpdateScale()
        {
            if (isFirstGrid)
            {
                transform.localScale = Vector3.one * scaleFactor;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 检查并更新选中状态
        /// </summary>
        /// <param name="battleEntity">当前选中的战斗实体</param>
        public void CheckSelect(IBattleEntityObject battleEntity)
        {
            // 判断当前格子绑定的实体是否为选中实体
            IsSelect = this.battleEntity == battleEntity;
            // 设置闪烁特效状态
            SetFlashing();
            // 设置选中框状态
            SetSelecting();
        }

        /// <summary>
        /// 设置闪烁特效状态
        /// 选中时启用闪烁特效并重置动画参数，未选中时关闭
        /// </summary>
        private void SetFlashing()
        {
            time = 0;
            Flashing.gameObject.SetActive(IsSelect);
            // 重置所有闪烁图片的颜色为白色（初始状态）
            foreach (var image in images)
            {
                image.color = Color.white;
            }
        }

        /// <summary>
        /// 设置选中框显示状态
        /// 选中时显示选中框并复位到初始位置，未选中时隐藏
        /// </summary>
        private void SetSelecting()
        {
            imgSelect.gameObject.SetActive(IsSelect);
            imgSelectRect.transform.localPosition = initLocalPos;
        }

        /// <summary>
        /// 帧更新逻辑（仅在选中状态下执行）
        /// 处理选中框的水平位移动画和闪烁特效的透明度动画
        /// </summary>
        private void OnUpdate()
        {
            if (!IsSelect)
            {
                return;
            }

            // 选中框水平位移计算（基于正弦曲线的平滑往复运动）
            var xOffset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
            // 应用位移（保持初始Y/Z轴位置不变）
            imgSelectRect.localPosition = new Vector3(initLocalPos.x + xOffset, initLocalPos.y, initLocalPos.z);

            // 闪烁特效透明度计算（PingPong实现0-1之间的往复变化）
            time += Time.deltaTime * falshSpeed;
            currentAlpha = 1 - Mathf.PingPong(time, 1f);

            // 应用透明度到所有闪烁图片
            var color = new Color(1, 1, 1, currentAlpha);
            foreach (var image in images)
            {
                image.color = color;
            }
        }

        /// <summary>
        /// 组件禁用时调用（生命周期）
        /// 移除帧更新监听，避免无效计算
        /// </summary>
        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}