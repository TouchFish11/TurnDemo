using Core.Components;
using UnityEngine;

namespace Game.Battle.Camera
{
    /// <summary>
    /// ����������ӿ�
    /// </summary>
    public interface IOrbitCameraController : IEntityObject
    {
        Transform Transform { get; }

        void SetTarget(Transform target);
    }
}
