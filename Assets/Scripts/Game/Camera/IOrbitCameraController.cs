using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机控制器接口
/// </summary>
public interface IOrbitCameraController
{
    Transform Transform { get; }

    void SetTarget(Transform target);
}
