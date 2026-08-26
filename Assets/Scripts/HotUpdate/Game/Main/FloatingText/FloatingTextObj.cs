using Core.DI;
using Core.Mono;
using TMPro;
using UnityEngine;

namespace HotUpdate.Game.Main.FloatingText
{
    /// <summary>
    /// 浮动文本对象
    /// </summary>
    public class FloatingTextObj : MonoBehaviour
    {
        private TextMeshPro txtName;
        private TextMeshPro txtTip;

        // 跟随NPC的Transform
        private Transform followNpcTarget;
        // 头顶偏移量
        private readonly Vector3 offset = new(0, 2, 0);
        // 最小缩放
        private readonly Vector3 minScale = Vector3.one * 0.2f;
        // 最大缩放
        private readonly Vector3 maxScale = Vector3.one * 1.35f;
        // 缩放速度
        private const float scaleSpeed = 1.1f;
        // 主摄像机
        private Camera mainCamera;
        // 主玩家
        private Transform mainPlayer;
        // 上次距离
        private float lastDis;

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get => this.gameObject.activeInHierarchy;
            set => this.gameObject.SetActive(value);
        }

        private void Awake()
        {
            txtName = transform.Find($"{nameof(txtName)}")?.GetComponent<TextMeshPro>();
            txtTip = transform.Find($"{nameof(txtTip)}")?.GetComponent<TextMeshPro>();
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="tip"></param>
        /// <param name="npcTarget"></param>
        public void Init(Transform npcTarget, Transform player, string name, string tip)
        {
            followNpcTarget = npcTarget;
            mainPlayer = player;
            txtName.text = name;
            txtTip.text = tip;
        }

        private void OnUpdate()
        {
            if(!IsShow)
                return;
                
            if (!followNpcTarget || !mainCamera)
                return;

            // 面向摄像机
            transform.forward = mainCamera.transform.forward;
            // 跟随目标
            transform.position = followNpcTarget.position + offset;
            // 离目标越近文字越小，反之越大
            UpdateScale();
        }

        private void UpdateScale()
        {
            var currentDis = Vector3.Distance(transform.position, mainPlayer.position);
            if (currentDis < lastDis)
            {
                // 变小
                transform.localScale = Vector3.Lerp(transform.localScale, minScale, Time.deltaTime * scaleSpeed);
            }
            else if(currentDis > lastDis)
            {
                // 变大
                transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime * scaleSpeed);
            }
            lastDis = currentDis;
        }
        

        private void OnDisable()
        {
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
