using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 悬浮文本
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro txtName;
    [SerializeField] private TextMeshPro txtTip;

    // 跟随的NPCTransform
    private Transform followNpcTarget;
    // 头顶偏移量
    [SerializeField] private Vector3 offset;
    // 最小缩放
    [SerializeField] private Vector3 minScale = Vector3.one * 0.2f;
    // 最大缩放
    [SerializeField] private Vector3 maxScale = Vector3.one * 1.75f;
    // 缩放速度
    [SerializeField] private float scaleSpeed = 1.1f;
    // 主相机
    private Camera mainCamera;
    // 主玩家
    private Transform mainPlayer;
    // 上次距离
    private float lastDis;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="mainTarget"></param>
    /// <param name="name"></param>
    /// <param name="tip"></param>
    public void Init(Transform npcTarget, Transform player, string name, string tip)
    {
        this.followNpcTarget = npcTarget;
        this.mainPlayer = player;
        txtName.text = name;
        txtTip.text = tip;
    }

    private void OnUpdate()
    {
        if (followNpcTarget == null || mainCamera == null)
        {
            return;
        }

        // 面向相机
        this.transform.forward = mainCamera.transform.forward;
        // 跟随目标
        this.transform.position = followNpcTarget.position + offset;
        // 离目标越近，文本越小；反之越大
        UpdateScale();
    }

    private void UpdateScale()
    {
        float currentDis = Vector3.Distance(this.transform.position, mainPlayer.position);
        if (currentDis < lastDis)
        {
            // 缩小
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, minScale, Time.deltaTime * scaleSpeed);
            lastDis = currentDis;
        }
        else if(currentDis > lastDis)
        {
            // 放大
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, maxScale, Time.deltaTime * scaleSpeed);
            lastDis = currentDis;
        }
    }

    private void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
