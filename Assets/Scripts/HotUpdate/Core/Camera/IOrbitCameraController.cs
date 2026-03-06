using Core.Components;
using UnityEngine;

namespace HotUpdate.Core.Camera
{
    public interface IOrbitCameraController : IEntityObject
    {
        Transform Transform { get; }

        void SetTarget(Transform target);
    }
}
