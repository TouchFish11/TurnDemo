using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.Time;
using Core.UI;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.UI.Battle.ActionLine
{
    public class ActionGridLogic : IUILogic<ActionGridUI, ActionGridLogic>, IPoolData
    {
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IPoolManager _poolManager;
        
        // 起始Y坐标
        private float _startY;
        // 基础的Y坐标偏移
        private float _baseOffsetY;
        // 滑动到的目标Y
        private float _targetY;
        // 当前索引
        private int _currentIndex;
        // 滑动插值时间
        private float _slidingTime;
        // 是否正在滑动
        private bool _isSliding;
        // 闪烁图片当前的透明度
        private float currentAlpha = 1f;
        // 闪烁动画的计时变量
        private float time;
        
        public ActionGridUI View { get; private set; }
        
        /// <summary>
        /// 绑定的战斗实体对象
        /// </summary>
        public IBattleEntityObject BattleEntity { get; private set; }
        
        /// <summary>
        /// 只读属性：当前格子是否处于选中状态
        /// </summary>
        public bool IsSelect { get; private set; }
        
        public void Init(ActionGridUI view, Sprite icon, float startX, float startY, int targetIndex, IBattleEntityObject battleEntity)
        {
            View = view;
            BattleEntity = battleEntity;
            View.imgIcon.sprite = icon;
            View.RectTransform.SetSiblingIndex(targetIndex);
            var initY = startY + targetIndex * -(View.RectTransform.rect.height + View.space);
            View.RectTransform.anchoredPosition = new Vector2(startX, initY);
            _baseOffsetY = startY;
        }
        
        public void OnEnable()
        {
            _monoAdapter.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 检查并更新选中状态
        /// </summary>
        /// <param name="battleEntity">当前选中的战斗实体</param>
        public void CheckSelect(IBattleEntityObject battleEntity)
        {
            // 判断当前格子绑定的实体是否为选中实体
            IsSelect = BattleEntity == battleEntity;
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
            View.Flashing.gameObject.SetActive(IsSelect);
            // 重置所有闪烁图片的颜色为白色（初始状态）
            foreach (var image in View.Images)
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
            View.imgSelect.gameObject.SetActive(IsSelect);
            View.ImgSelectRect.transform.localPosition = View.InitLocalPos;
        }
        
        /// <summary>
        /// 设置UI格子滑动到的目标位置索引
        /// </summary>
        /// <param name="targetIndex"></param>
        public void SetSlideTarget(int targetIndex)
        {
            _targetY = _baseOffsetY + targetIndex * -(View.RectTransform.rect.height + View.space);
            _slidingTime = 0;
            _isSliding = true;
            _startY = View.RectTransform.anchoredPosition.y;
            _currentIndex = targetIndex;
        }
        
        /// <summary>
        /// 帧更新逻辑（仅在选中状态下执行）
        /// 处理选中框的水平位移动画和闪烁特效的透明度动画
        /// </summary>
        private void OnUpdate()
        {
            SlideToTarget();
            SelectAnim();
            FlashAnim();
        }
        
        /// <summary>
        /// 滑动到目标位置
        /// </summary>
        private void SlideToTarget()
        {
            if(!_isSliding)
                return;
            
            _slidingTime += TimeUtil.DeltaTime * View.slidingSpeed;
            var currentY = Mathf.Lerp(_startY, _targetY, _slidingTime);
            View.RectTransform.anchoredPosition = new Vector2(View.RectTransform.anchoredPosition.x, currentY);
            
            if (Mathf.Approximately(View.RectTransform.anchoredPosition.y, _targetY))
            {
                _isSliding = false;
            }
        }
        
        /// <summary>
        /// 选中动画
        /// </summary>
        private void SelectAnim()
        {
            if (!IsSelect)
            {
                return;
            }
            
            // 选中框水平位移计算（基于正弦曲线的平滑往复运动）
            var xOffset = Mathf.Sin(Time.time * View.moveSpeed) * View.moveRange;
            // 应用位移（保持初始Y/Z轴位置不变）
            View.ImgSelectRect.localPosition = new Vector3(View.InitLocalPos.x + xOffset, View.InitLocalPos.y, View.InitLocalPos.z);
        }
        
        /// <summary>
        /// 闪烁动画
        /// </summary>
        private void FlashAnim()
        {
            if (!IsSelect)
            {
                return;
            }
            
            // 闪烁特效透明度计算（PingPong实现0-1之间的往复变化）
            time += Time.deltaTime * View.falshSpeed;
            currentAlpha = 1 - Mathf.PingPong(time, 1f);

            // 应用透明度到所有闪烁图片
            var color = new Color(1, 1, 1, currentAlpha);
            foreach (var image in View.Images)
            {
                image.color = color;
            }
        }
        
        public void OnDisable()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
            BattleEntity = null;
        }

        void IPoolData.ResetData()
        {
            
        }

        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
