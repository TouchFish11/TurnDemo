using Core.Components;
using UnityEngine;

namespace HotUpdate.Camera
{
    public interface IOrbitCameraController : IEntityObject
    {
        Transform Transform { get; }

        void SetTarget(Transform target);
    }
}
