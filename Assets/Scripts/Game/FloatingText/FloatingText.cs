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
    private Transform followTarget;
    // 头顶偏移量
    [SerializeField] private Vector3 offset;
    // 主相机
    private Camera mainCamera;

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
    /// <param name="target"></param>
    /// <param name="name"></param>
    /// <param name="tip"></param>
    public void Init(Transform target, string name, string tip)
    {
        this.followTarget = target;
        txtName.text = name;
        txtTip.text = tip;
    }

    private void OnUpdate()
    {
        if (followTarget == null || mainCamera == null)
        {
            return;
        }

        // 面向相机
        this.transform.forward = mainCamera.transform.forward;
        // 跟随目标
        this.transform.position = followTarget.position + offset;
        // 缩放
    }

    private void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
