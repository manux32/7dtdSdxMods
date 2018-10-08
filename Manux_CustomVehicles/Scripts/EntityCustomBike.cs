using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


class EntityCustomBike : EntityMinibike
{
    Vector3 cameraOffset = new Vector3(0.5f, 0.1f, 0.75f);
    bool hasCamOffset;
    bool thirdPersonModelVisible = true;
    Vector3 playerOffsetPos;
    Vector3 playerOffsetRot;
    bool hasPlayerOffsetPos;
    bool hasPlayerOffsetRot;

    Vector3 colliderCenter;
    float colliderRadius;
    float colliderHeight;
    float controllerSlopeLimit;
    float controllerStepOffset;
    bool hasColliderCenter;
    bool hasColliderRadius;
    bool hasColliderHeight;
    bool hasControllerSlopeLimit;
    bool hasControllerStepOffset;

    bool camAndPlayerOffsetsDone;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void Init(int _entityClass)
    {
        // Cam offset from xml
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        if (entityClass.Properties.Values.ContainsKey("CameraOffset"))
        {
            Vector3 newVector3;
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["CameraOffset"], out newVector3))
            {
                cameraOffset = newVector3;
                hasCamOffset = true;
            }
        }

        // Player settings from xml
        if (entityClass.Properties.Values.ContainsKey("3rdPersonModelVisible"))
        {
            bool playerVisible;
            if (bool.TryParse(entityClass.Properties.Values["3rdPersonModelVisible"], out playerVisible))
            {
                thirdPersonModelVisible = playerVisible;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("PlayerPositionOffset"))
        {
            Vector3 newVector3;
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["PlayerPositionOffset"], out newVector3))
            {
                playerOffsetPos = newVector3;
                hasPlayerOffsetPos = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("PlayerRotationOffset"))
        {
            Vector3 newVector3;
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["PlayerRotationOffset"], out newVector3))
            {
                playerOffsetRot = newVector3;
                hasPlayerOffsetRot = true;
            }
        }

        // Vehicle Character controller collider settings from xml
        this.SetEntityName(EntityClass.list[this.entityClass].entityClassName);
        string dbgMsg = (this.EntityName + " (" + this.GetType().ToString() + "):\nRead CharacterController settings from xml:\n");
        if (entityClass.Properties.Values.ContainsKey("ColliderCenter"))
        {
            Vector3 newVector3;
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["ColliderCenter"], out newVector3))
            {
                dbgMsg += ("\tcolliderCenter = " + newVector3.ToString("0.000") + "\n");
                colliderCenter = newVector3;
                hasColliderCenter = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ColliderRadius"))
        {
            float radius;
            if (float.TryParse(entityClass.Properties.Values["ColliderRadius"], out radius))
            {
                dbgMsg += ("\tcolliderRadius = " + radius.ToString("0.000") + "\n");
                colliderRadius = radius;
                hasColliderRadius = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ColliderHeight"))
        {
            float height;
            if (float.TryParse(entityClass.Properties.Values["ColliderHeight"], out height))
            {
                dbgMsg += ("\tcolliderHeight = " + height.ToString("0.000") + "\n");
                colliderHeight = height;
                hasColliderHeight = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ControllerSlopeLimit"))
        {
            float slopeLimit;
            if (float.TryParse(entityClass.Properties.Values["ControllerSlopeLimit"], out slopeLimit))
            {
                dbgMsg += ("\tcontrollerSlopeLimit = " + slopeLimit.ToString("0.000") + "\n");
                controllerSlopeLimit = slopeLimit;
                hasControllerSlopeLimit = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ControllerStepOffset"))
        {
            float stepOffset;
            if (float.TryParse(entityClass.Properties.Values["ControllerStepOffset"], out stepOffset))
            {
                dbgMsg += ("\tcontrollerStepOffset = " + stepOffset.ToString("0.000") + "\n");
                controllerStepOffset = stepOffset;
                hasControllerStepOffset = true;
            }
        }

        DebugMsg(dbgMsg);
        SetCharacterControllerFromXml();
    }

    protected void SetCharacterControllerFromXml()
    {
        string dbgMsg = (this.EntityName + " (" + this.GetType().ToString() + "):\nSet CharacterController settings from xml:\n");
        Collider[] colls = this.transform.root.GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
        {
            dbgMsg += ("Found " + coll.gameObject.name + " collider: " + coll.GetType().ToString() + "\n");
            if (coll.gameObject.name == "Physics")
            {
                CharacterController cc = coll as CharacterController;
                if (hasColliderCenter)
                {
                    // Inverse x and z sign, because the char controller seems to have it's root inversed relative to the vehicle.
                    colliderCenter.x = -colliderCenter.x;
                    colliderCenter.z = -colliderCenter.z;
                    cc.center = colliderCenter;
                    dbgMsg += ("\tnew colliderCenter = " + cc.center.ToString("0.000") + "\n");
                }
                if (hasColliderRadius)
                {
                    cc.radius = colliderRadius;
                    dbgMsg += ("\tnew radius = " + cc.radius.ToString("0.000") + "\n");
                }
                if (hasColliderHeight)
                {
                    cc.height = colliderHeight;
                    dbgMsg += ("\tnew height = " + cc.height.ToString("0.000") + "\n");
                }
                if (hasControllerSlopeLimit)
                {
                    cc.slopeLimit = controllerSlopeLimit;
                    dbgMsg += ("\tnew slopeLimit = " + cc.slopeLimit.ToString("0.000") + "\n");
                }
                if (hasControllerStepOffset)
                {
                    cc.stepOffset = controllerStepOffset;
                    dbgMsg += ("\tnew stepOffset = " + cc.stepOffset.ToString("0.000") + "\n");
                }
            }
        }
        DebugMsg(dbgMsg);
    }

    public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (!(this.AttachedEntities is global::EntityPlayer))
        {
            camAndPlayerOffsetsDone = false;
            return;
        }

        if (camAndPlayerOffsetsDone)
            return;

        EntityPlayerLocal entityPlayerLocal = this.AttachedEntities as EntityPlayerLocal;
        if (entityPlayerLocal != null)
        {
            if (hasCamOffset)
            {
                entityPlayerLocal.vp_FPCamera.Position3rdPersonOffset = cameraOffset;
            }

            entityPlayerLocal.emodel.SetVisible(thirdPersonModelVisible);

            if (hasPlayerOffsetPos)
            {
                entityPlayerLocal.ModelTransform.localPosition = playerOffsetPos;
            }
            if (hasPlayerOffsetRot)
            {
                entityPlayerLocal.ModelTransform.localEulerAngles = playerOffsetRot;
            }

            camAndPlayerOffsetsDone = true;
        }
    }
}


