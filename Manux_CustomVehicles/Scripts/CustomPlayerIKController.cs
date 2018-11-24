using System;
using UnityEngine;


public class CustomPlayerIKController : MonoBehaviour
{
    public Animator m_animator;
    public Transform rightFootTarget;
    public Transform leftFootTarget;
    public Transform headTarget;


    public void Start()
    {
        this.m_animator = base.GetComponent<Animator>();
    }

    public void OnAnimatorIK()
    {
        if (this.m_animator)
        {
            if (this.headTarget != null)
            {
                this.m_animator.SetLookAtPosition(this.headTarget.position);
                this.m_animator.SetLookAtWeight(1f, 0f);
            }
            else
            {
                this.m_animator.SetLookAtWeight(0f, 0f);
            }

            if (this.rightFootTarget != null)
            {
                this.m_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
                this.m_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
                this.m_animator.SetIKPosition(AvatarIKGoal.RightFoot, this.rightFootTarget.position);
                this.m_animator.SetIKRotation(AvatarIKGoal.RightFoot, this.rightFootTarget.rotation);
            }
            else
            {
                this.m_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
                this.m_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
            }
            if (this.leftFootTarget != null)
            {
                this.m_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                this.m_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
                this.m_animator.SetIKPosition(AvatarIKGoal.LeftFoot, this.leftFootTarget.position);
                this.m_animator.SetIKRotation(AvatarIKGoal.LeftFoot, this.leftFootTarget.rotation);
            }
            else
            {
                this.m_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                this.m_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            }
        }
    }
}
