using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


class EntityCustomCar : EntityCustomBike
{
    Transform handlebar_joint = null;
    Transform frontWheel_joint_yaw = null;
    Transform frontWheel_joint = null;
    Transform right_frontWheel_joint_yaw = null;
    Transform right_frontWheel_joint = null;

    Transform handlebar_joint2 = null;
    Transform frontWheel_joint_yaw2 = null;
    Transform frontWheel_joint2 = null;
    Transform right_frontWheel_joint_yaw2 = null;
    Transform right_frontWheel_joint2 = null;

    bool allBonesSet1Found;
    bool allBonesSet2Found;

    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    protected override void Start()
    {
        base.Start();

        List<Transform> childrenList = new List<Transform>();
        List<int> childrenInstanceIds = new List<int>();
        childrenList.Add(this.transform);
        childrenInstanceIds.Add(this.transform.GetInstanceID());
        CustomVehiclesUtils.GetAllChildTransforms(this.transform, ref childrenList, ref childrenInstanceIds);

        foreach (Transform child in childrenList)
        {
            switch (child.name)
            {
                case "handlebar_joint":
                    if (handlebar_joint == null)
                        handlebar_joint = child;
                    else
                        handlebar_joint2 = child;
                    break;
                case "frontWheel_joint_yaw":
                    if (frontWheel_joint_yaw == null)
                        frontWheel_joint_yaw = child;
                    else
                        frontWheel_joint_yaw2 = child;
                    break;
                case "frontWheel_joint":
                    if (frontWheel_joint == null)
                        frontWheel_joint = child;
                    else
                        frontWheel_joint2 = child;
                    break;
                case "right_frontWheel_joint_yaw":
                    if (right_frontWheel_joint_yaw == null)
                        right_frontWheel_joint_yaw = child;
                    else
                        right_frontWheel_joint_yaw2 = child;
                    break;
                case "right_frontWheel_joint":
                    if (right_frontWheel_joint == null)
                        right_frontWheel_joint = child;
                    else
                        right_frontWheel_joint2 = child;
                    break;
            }
        }

        if (handlebar_joint == null || frontWheel_joint_yaw == null || frontWheel_joint == null || right_frontWheel_joint_yaw == null || right_frontWheel_joint == null)
        {
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            allBonesSet1Found = true;
            DebugMsg(this.ToString() + " : All bones set 1 found.");
        }

        if (handlebar_joint2 == null || frontWheel_joint_yaw2 == null || frontWheel_joint2 == null || right_frontWheel_joint_yaw2 == null || right_frontWheel_joint2 == null)
        {
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            allBonesSet2Found = true;
            DebugMsg(this.ToString() + " : All bones set 2 found.");
        }
    }


    public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (!allBonesSet1Found || !(this.AttachedEntities is global::EntityPlayer))
            return;

        // There is sometimes 2 versions of the car prefab in the game, we need to use the second one when it exists.
        if (allBonesSet2Found)
        {
            frontWheel_joint_yaw2.localRotation = handlebar_joint2.localRotation;
            right_frontWheel_joint_yaw2.localRotation = handlebar_joint2.localRotation;
            right_frontWheel_joint2.localRotation = frontWheel_joint2.localRotation;
        }
        else
        {
            frontWheel_joint_yaw.localRotation = handlebar_joint.localRotation;
            right_frontWheel_joint_yaw.localRotation = handlebar_joint.localRotation;
            right_frontWheel_joint.localRotation = frontWheel_joint.localRotation;
        }
    }
}

