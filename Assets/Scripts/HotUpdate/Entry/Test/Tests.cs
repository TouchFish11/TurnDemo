using System;
using UnityEngine;

public class Tests : MonoBehaviour
{
    public class A
    {
        
    }
    
    private Animator animator;
    private CharacterController characterController;

    public Transform leftHandTarget; // 场景中代表左手目标位置的物体（可放在梯子横杆上）
    public Transform rightHandTarget; // 同上

    public Tests(Lazy<A> lazy)
    {
        
    }
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // 通常需要先关闭 Apply Root Motion，避免 Transform 被自动移动
    void Start()
    {
        animator.applyRootMotion = false; // 我们手动控制位移
    }

    private void OnAnimatorMove()
    {
        // 读取根运动增量
        Vector3 deltaPosition = animator.deltaPosition;
        Quaternion deltaRotation = animator.deltaRotation;

        // 使用 CharacterController.Move 移动角色，使其受碰撞检测约束
        if (characterController.enabled)
        {
            characterController.Move(deltaPosition); // 忽略旋转的简化情况
        }
        else
        {
            // 如果不用 CharacterController，直接应用 Transform
            transform.position += deltaPosition;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // 设置左手 IK 位置和旋转
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            // 右手同理
            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }
    }
}
