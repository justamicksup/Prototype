using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;

    public bool ikActive = true;
    public Transform gunObj = null;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal.
            if (ikActive)
            {

                // Set the hand target position and rotation, if one has been assigned
                if (gunObj != null)
                {
                    //right hand
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, gunObj.position);
                    //animator.SetIKRotation(AvatarIKGoal.RightHand, gunObj.rotation);

                    //left hand
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, gunObj.position);
                    //animator.SetIKRotation(AvatarIKGoal.LeftHand, gunObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}
