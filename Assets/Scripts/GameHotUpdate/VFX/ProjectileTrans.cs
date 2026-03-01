using UnityEngine;

namespace GameHotUpdate.VFX
{
    /// <summary>
    /// 弹射物变换信息
    /// </summary>
    public readonly struct ProjectileTrans
    {
        public Transform Parent { get; }
        public Vector3 WorldPos { get; }
        public Vector3 LocalPos { get; }
        public Quaternion Rotation { get; }
        public bool WorldPositionStays { get; }

        public ProjectileTrans(Transform parent, bool worldPositionStays) : this()
        {
            Parent = parent;
            WorldPositionStays = worldPositionStays;
        }

        public ProjectileTrans(Vector3 worldPos, Quaternion rotation) : this()
        {
            WorldPos = worldPos;
            Rotation = rotation;
        }

        public ProjectileTrans(Transform parent, Vector3 localPos, Quaternion rotation, bool worldPositionStays) : this()
        {
            Parent = parent;
            LocalPos = localPos;
            Rotation = rotation;
            WorldPositionStays = worldPositionStays;
        }
    }
}
