using Core.Components;
using UnityEngine;

namespace GameHotUpdate.Camera
{
    public interface IOrbitCameraController : IEntityObject
    {
        Transform Transform { get; }

        void SetTarget(Transform target);
    }
}
