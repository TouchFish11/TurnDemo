using UnityEngine;

namespace Game.Controller
{
    /// <summary>
    /// IK目标
    /// </summary>
    public class UpperBodyLookAtIK : MonoBehaviour
    {
        private Transform lookTarget;
        // 上半身权重：0~1
        [Range(0, 1)] public float bodyWeight = 0.25f;    // 身体( spine )强度
        [Range(0, 1)] public float headWeight = 1.0f;     // 头部强度
        [Range(0, 1)] public float eyesWeight = 1.0f;     // 眼睛强度
        public float scaleFactor = 20f;

        private Animator _animator;
        private float currentRealY;
    
        private void Awake()
        {
            lookTarget = new GameObject("LookAtIK").transform;
            _animator = GetComponent<Animator>();
            currentRealY = 1.5f;
        }

        public void MoveTarget(float y)
        {
            currentRealY += -y / scaleFactor;
            currentRealY = Mathf.Clamp(currentRealY, -3.5f, 6.5f);
        }

        private void Update()
        {
            var pos = this.transform.position + this.transform.forward;
            pos = new Vector3(pos.x, currentRealY, pos.z);
            lookTarget.position = pos;
        }

        // IK 必须写在这个函数里
        private void OnAnimatorIK(int layerIndex)
        {
            if (!lookTarget || !_animator) return;

            // 设置 IK 目标
            _animator.SetLookAtPosition(lookTarget.position);

            // 设置上半身权重（关键！）
            _animator.SetLookAtWeight(1, bodyWeight, headWeight, eyesWeight);
        }
    }
}
