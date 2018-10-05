using System.Collections.Generic;
using UnityEngine;


class EntityCustomCar : EntityMinibike
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

    Vector3 cameraOffset = new Vector3(0.5f, 0.1f, 0.75f);

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        if (entityClass.Properties.Values.ContainsKey("CameraOffset"))
        {
            Vector3 newVector3;
            if (StringVectorToVector3(entityClass.Properties.Values["CameraOffset"], out newVector3))
            {
                cameraOffset = newVector3;
            }
        }
    }


    protected override void Start()
    {
        base.Start();

        List<Transform> childrenList = new List<Transform>();
        List<int> childrenInstanceIds = new List<int>();
        childrenList.Add(this.transform);
        childrenInstanceIds.Add(this.transform.GetInstanceID());
        GetAllChildTransforms(this.transform, ref childrenList, ref childrenInstanceIds);

        foreach (Transform child in childrenList)
        {
            switch (child.name)
            {
                case "handlebar_joint":
                    if(handlebar_joint == null)
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
            Debug.LogError("EntityCustomCar.Start: Some bones could not be found for set 1.");
        }
        else
        {
            allBonesSet1Found = true;
            Debug.Log("EntityCustomCar.Start: All bones set 1 found.");
        }

        if (handlebar_joint2 == null || frontWheel_joint_yaw2 == null || frontWheel_joint2 == null || right_frontWheel_joint_yaw2 == null || right_frontWheel_joint2 == null)
        {
            Debug.Log("EntityCustomCar.Start: Some bones could not be found for set 2.");
        }
        else
        {
            allBonesSet2Found = true;
            Debug.Log("EntityCustomCar.Start: All bones set 2 found.");
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

        EntityPlayerLocal entityPlayerLocal = this.AttachedEntities as EntityPlayerLocal;
        if (entityPlayerLocal != null)
        {
            entityPlayerLocal.vp_FPCamera.Position3rdPersonOffset = cameraOffset;
        }
    }


    public void GetAllChildTransforms(Transform root, ref List<Transform> childrenList, ref List<int> childrenInstanceIds)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (!childrenInstanceIds.Contains(child.GetInstanceID()))
            {
                childrenList.Add(child);
                childrenInstanceIds.Add(child.GetInstanceID());
            }
            GetAllChildTransforms(child, ref childrenList, ref childrenInstanceIds);
        }
    }

    public static bool StringVectorToVector3(string stringVec, out Vector3 newVector3)
    {
        string[] stringVector;
        stringVector = stringVec.Split(',');
        if (stringVector.Length == 3)
        {
            float x;
            float.TryParse(stringVector[0], out x);
            float y;
            float.TryParse(stringVector[1], out y);
            float z;
            float.TryParse(stringVector[2], out z);
            newVector3 = new Vector3(x, y, z);
            return true;
        }
        else
        {
            Debug.Log("Xml Vector is invalid");
        }
        newVector3 = new Vector3();
        return false;
    }
}

