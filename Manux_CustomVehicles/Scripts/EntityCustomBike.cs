using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

class EntityCustomBike : EntityMinibike
{
    public Vector3 cameraOffset = new Vector3(0.5f, 0.1f, 0.75f);
    public bool hasCamOffset;
    public bool thirdPersonModelVisible = true;
    public Vector3 playerOffsetPos;
    public Vector3 playerOffsetRot;
    public bool hasPlayerOffsetPos;
    public bool hasPlayerOffsetRot;

    public Vector3 colliderCenter;
    public float colliderRadius;
    public float colliderHeight;
    public float colliderSkinWidth;
    public float controllerSlopeLimit;
    public float controllerStepOffset;
    public bool hasColliderCenter;
    public bool hasColliderRadius;
    public bool hasColliderHeight;
    public bool hasControllerSlopeLimit;
    public bool hasControllerStepOffset;
    public bool hasColliderSkinWidth;

    public Vector3 vehicleActivationCenter;
    public Vector3 vehicleActivationSize;
    public bool hasVehicleActivationCenter;
    public bool hasVehicleActivationSize;

    CharacterController charCtrl = null;

    bool camAndPlayerOffsetsDone;

    // Three8's WaterCraft
    bool isAirtight;
    bool isWaterCraft;
	
	public bool IsWaterCraft()
	{
		return isWaterCraft;
	}

    public bool IsAirtight()
    {
        return isAirtight;
    }

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
        EntityPlayerLocal entityPlayerLocal = GameManager.Instance.World.GetLocalPlayer() as global::EntityPlayerLocal;

        // Cam offset from xml
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        if (entityClass.Properties.Values.ContainsKey("CameraOffset"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["CameraOffset"], out cameraOffset))
            {
                hasCamOffset = true;
            }
        }

        // Player settings from xml
        if (entityClass.Properties.Values.ContainsKey("3rdPersonModelVisible"))
        {
            bool.TryParse(entityClass.Properties.Values["3rdPersonModelVisible"], out thirdPersonModelVisible);
        }
        if (entityClass.Properties.Values.ContainsKey("PlayerPositionOffset"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["PlayerPositionOffset"], out playerOffsetPos))
            {
                hasPlayerOffsetPos = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("PlayerRotationOffset"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["PlayerRotationOffset"], out playerOffsetRot))
            {
                hasPlayerOffsetRot = true;
            }
        }

        // Vehicle Character controller collider settings from xml
        this.SetEntityName(EntityClass.list[this.entityClass].entityClassName);

        if (entityClass.Properties.Values.ContainsKey("ColliderCenter"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["ColliderCenter"], out colliderCenter))
            {
                hasColliderCenter = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ColliderRadius"))
        {
            if (float.TryParse(entityClass.Properties.Values["ColliderRadius"], out colliderRadius))
            {
                hasColliderRadius = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ColliderHeight"))
        {
            if (float.TryParse(entityClass.Properties.Values["ColliderHeight"], out colliderHeight))
            {
                hasColliderHeight = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ColliderSkinWidth"))
        {
            if (float.TryParse(entityClass.Properties.Values["ColliderSkinWidth"], out colliderSkinWidth))
            {
                hasColliderSkinWidth = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ControllerSlopeLimit"))
        {
            if (float.TryParse(entityClass.Properties.Values["ControllerSlopeLimit"], out controllerSlopeLimit))
            {
                hasControllerSlopeLimit = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("ControllerStepOffset"))
        {
            if (float.TryParse(entityClass.Properties.Values["ControllerStepOffset"], out controllerStepOffset))
            {
                hasControllerStepOffset = true;
            }
        }

        // Vehicle activation area from xml
        if (entityClass.Properties.Values.ContainsKey("VehicleActivationCenter"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["VehicleActivationCenter"], out vehicleActivationCenter))
            {
                hasVehicleActivationCenter = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("VehicleActivationSize"))
        {
            if (CustomVehiclesUtils.StringVectorToVector3(entityClass.Properties.Values["VehicleActivationSize"], out vehicleActivationSize))
            {
                hasVehicleActivationSize = true;
            }
        }

        // Three8's WaterCraft settings from xml
        // Add this to xml to turn on underwater use <property name="WaterCraft" value="true" />
        if (entityClass.Properties.Values.ContainsKey("WaterCraft"))
		{
			bool.TryParse(entityClass.Properties.Values["WaterCraft"], out isWaterCraft);
		}
        // Add this to xml so you dont drown when underwater <property name="Airtight" value="true" />
        if (entityClass.Properties.Values.ContainsKey("Airtight"))
		{
			bool.TryParse(entityClass.Properties.Values["Airtight"], out isAirtight);
		}

        SetCharCtrlAndBocCollFromXml();
    }

    protected void SetCharCtrlAndBocCollFromXml()
    {
        string dbgMsg = (this.EntityName + " (" + this.GetType().ToString() + "):\nSet CharacterController settings from xml:\n");
        Collider[] colls = this.transform.root.GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
        {
            dbgMsg += ("Found " + coll.gameObject.name + " collider: " + coll.GetType().ToString() + "\n");
            if (coll.gameObject.name == "Physics")
            {
                charCtrl = coll as CharacterController;
                dbgMsg += ("\tFound CharacterController = " + charCtrl.ToString() + "|" + charCtrl.GetInstanceID().ToString() + "\n");
                if (this.m_characterController != null)
                {
                    dbgMsg += ("\tthis.m_characterController = " + this.m_characterController.ToString() + "|" + this.m_characterController.GetInstanceID().ToString() + "\n");
                }
                if (this.nativeCollider != null)
                {
                    dbgMsg += ("\tthis.nativeCollider = " + this.nativeCollider.ToString() + "|" + this.nativeCollider.GetType().ToString() +"|" + this.nativeCollider.GetInstanceID().ToString() + "\n");
                }

                if (hasColliderCenter)
                {
                    // Inverse x and z sign, because the char controller seems to have it's root inversed relative to the vehicle.
                    colliderCenter.x = -colliderCenter.x;
                    colliderCenter.z = -colliderCenter.z;
                    dbgMsg += ("\tnew colliderCenter = " + colliderCenter.ToString("0.000") + " (was: " + charCtrl.center.ToString("0.000") + ")\n");
                    charCtrl.center = colliderCenter;
                    
                }
                if (hasColliderRadius)
                {
                    dbgMsg += ("\tnew radius = " + colliderRadius.ToString("0.000") + " (was: " + charCtrl.radius.ToString("0.000") + ")\n");
                    charCtrl.radius = colliderRadius;
                }
                if (hasColliderHeight)
                {
                    dbgMsg += ("\tnew height = " + colliderHeight.ToString("0.000") + " (was: " + charCtrl.height.ToString("0.000") + ")\n");
                    charCtrl.height = colliderHeight;
                }
                if (hasColliderSkinWidth)
                {
                    dbgMsg += ("\tnew skinWidth = " + colliderSkinWidth.ToString("0.0000") + " (was: " + charCtrl.skinWidth.ToString("0.000") + ")\n");
                    charCtrl.skinWidth = colliderSkinWidth;
                }
                if (hasControllerSlopeLimit)
                {
                    dbgMsg += ("\tnew slopeLimit = " + controllerSlopeLimit.ToString("0.000") + " (was: " + charCtrl.slopeLimit.ToString("0.000") + ")\n");
                    charCtrl.slopeLimit = controllerSlopeLimit;
                }
                if (hasControllerStepOffset)
                {
                    dbgMsg += ("\tnew stepOffset = " + controllerStepOffset.ToString("0.000") + " (was: " + charCtrl.stepOffset.ToString("0.000") + ")\n");
                    charCtrl.stepOffset = controllerStepOffset;
                }
            }
        }

        // The Box Collider present on vehicles seems to only be for what area of the vehicle will show the Vehicle Activate menu (which is still useful)
        BoxCollider bcoll = this.nativeCollider as BoxCollider;
        if (bcoll == null)
        {
            dbgMsg += ("Box Collider for Vehicle Activation Area could not be found.\n");
        }
        else
        {
            if (hasVehicleActivationCenter)
            {
                dbgMsg += ("\tnew vehicleActivationCenter = " + vehicleActivationCenter.ToString("0.000") + " (was: " + bcoll.center.ToString("0.000") + ")\n");
                bcoll.center = vehicleActivationCenter;
            }
            if (hasVehicleActivationSize)
            {
                dbgMsg += ("\tnew vehicleActivationSize = " + vehicleActivationSize.ToString("0.000") + " (was: " + bcoll.size.ToString("0.000") + ")\n");
                bcoll.size = vehicleActivationSize;
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
            string dbgMsg = (this.EntityName + " (" + this.GetType().ToString() + "):\nSet Vehicle Player settings from xml:\n");
            if (hasCamOffset)
            {
                dbgMsg += ("\tPosition3rdPersonOffset = " + cameraOffset.ToString("0.000") + " (was: " + entityPlayerLocal.vp_FPCamera.Position3rdPersonOffset.ToString("0.000") + ")\n");
                entityPlayerLocal.vp_FPCamera.Position3rdPersonOffset = cameraOffset;
            }

            entityPlayerLocal.emodel.SetVisible(thirdPersonModelVisible);

            if (hasPlayerOffsetPos)
            {
                dbgMsg += ("\tPlayerPositionOffset = " + playerOffsetPos.ToString("0.000") + " (was: " + entityPlayerLocal.ModelTransform.localPosition.ToString("0.000") + ")\n");
                entityPlayerLocal.ModelTransform.localPosition = playerOffsetPos;
            }
            if (hasPlayerOffsetRot)
            {
                dbgMsg += ("\tPlayerRotationOffset = " + playerOffsetRot.ToString("0.000") + " (was: " + entityPlayerLocal.ModelTransform.localEulerAngles.ToString("0.000") + ")\n");
                entityPlayerLocal.ModelTransform.localEulerAngles = playerOffsetRot;
            }

            DebugMsg(dbgMsg);
            camAndPlayerOffsetsDone = true;
        }

        // Vehicle sometimes gets stuck
        //if (this.isDriveable() && this.vehicle.GetFuelLevel() > 0 && this.vehicle.SimulationInput.bWheelSpinForward && this.vehicle.SimulationInput.velocity == Vector3.zero)
        /*if (this.isDriveable() && this.vehicle.GetFuelLevel() > 0 && this.IncomingRemoteSimulationInput.bAccelerate && this.vehicle.SimulationInput.velocity == Vector3.zero)
        {
            charCtrl.SimpleMove(new Vector3(0,2,0));
        }*/
    }


    // Three8's WaterCraft: calls only while attached - must do this or it wont work under water.
    public new void Update() 
    {
		if(!IsWaterCraft())
		{
			base.Update();
            return;
		}

		if(IsAirtight())
		{
			if (GameManager.Instance.World.IsLiquidInBounds(new Bounds(this.position + new Vector3(0f, 1.5f, 0f), Vector3.one)))
			{
				EntityPlayerLocal entity = this.AttachedEntities as EntityPlayerLocal;
				if (entity != null)
				{
					entity.Stats.Debuff("cannotBreath");
					entity.Stats.Debuff("drowning");
				}
			}
		}

        if (!this.vehicle.HasAnyParts())
        {
            this.Kill();
        }
        this.vehicle.Update(Time.deltaTime);
	}

    public override bool isDriveable()
    {
        if(IsWaterCraft())
            return this.vehicle.IsDriveable();

        return this.vehicle.IsDriveable() && !this.world.IsLiquidInBounds(new Bounds(this.position + new Vector3(0f, 1.5f, 0f), Vector3.one));
    }
}


