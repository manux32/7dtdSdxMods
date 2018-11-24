using System.IO;
using System.Reflection;
using UnityEngine;


public class EntityCustomBoat : EntityCustomVehicle
{
    public EllipsoidParticleEmitter particleEmitter = null;
    public bool isTouchingGround = false;
    public float maxSpeedBackup;
    public float lastLandHitToolTipTime = -1;

    public VehicleSimulationInput vsi;

    public bool bReEnableCharCtrl = false;
    public float reEnableCharCtrlStartTime = -1;

    public Transform playerHeadTransform = null;

    public Chunk curChunk;
    public BlockEntityData boatDummyBlockEntityData = null;
    public bool bNeedBoatDummyBlockOffset;

    public float lastSyncTime = -1;
    public Vector3i boatDummyPos = Vector3i.zero;
    public bool bHasBoatDummy = false;
    public float boatDummyPos_YOffset = 0;
    public bool bNeedLoadAdjustments = false;
    public bool bSpawningBoatChassis;
    public int failedAdjustBoatPosCount = 0;

    // Obfuscated BlockShapeModelEntity Fields and Methods
    public static MethodInfo TW_MethodInfo = null;     // To get the Transform of the Dummy Boat Block

    // Was for reverse engineering the block rotations
    bool bRotateTest;
    float rotateTestStartTime;
    int rotateTestRot = 0;

    static bool showDebugLog = false;

    public new static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }


    #region Main Override Methods

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);

        Transform engine_splash = null;
        Transform[] transforms = this.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in transforms)
        {
            if (transform.name == "engine_splash")
            {
                engine_splash = transform;
            }
        }

        if (engine_splash != null)
        {
            DebugMsg("Found engine_splash Transform");
            particleEmitter = engine_splash.GetComponent<EllipsoidParticleEmitter>();
            if (particleEmitter != null)
            {
                DebugMsg("Found EllipsoidParticleEmitter");
                particleEmitter.emit = false;
            }
        }

        if (LZ_float_field != null)
        {
            maxSpeedBackup = (float)LZ_float_field.GetValue(this.vehicle);
        }

        if (GS_VehicleSimulationInput_field != null)
        {
            vsi = (VehicleSimulationInput)GS_VehicleSimulationInput_field.GetValue(this);
        }
    }

    public override void FindObfuscatedFieldsAndMethods()
    {
        base.FindObfuscatedFieldsAndMethods();

        if (TW_MethodInfo != null)
            return;

        MethodInfo[] listOfMethodNames;
        string TW_name = "TW";
        if (GameManager.IsDedicatedServer)
        {
            TW_name = "BV";
        }

        listOfMethodNames = typeof(BlockShapeModelEntity).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (MethodInfo methodInfo in listOfMethodNames)
        {
            if (methodInfo.Name == TW_name)
            {
                TW_MethodInfo = methodInfo;
                if (TW_MethodInfo != null)
                {
                    DebugMsg("Found BlockShapeModelEntity method TW");
                }
            }
        }
    }

    public override void CopyPropertiesFromEntityClass()
    {
        base.CopyPropertiesFromEntityClass();
        EntityClass entityClass = EntityClass.list[this.entityClass];
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        this.m_characterController.enabled = false;

        DebugMsg("Start: bHasBoatDummy = " + bHasBoatDummy.ToString());
        if (bHasBoatDummy)
        {
            // When loading and boat has a boat dummy block
            GetDummyBoatBlockEntityData(boatDummyPos);

            // Only modify the size the UI Activation if the engine is present
            if (this.hasEngine())
            {
                this.nativeBoxCollider.center = new Vector3(0, 0.5f, -3.37f);
                this.nativeBoxCollider.size = new Vector3(2, 2, 1);
            }
        }
        else
        {
            // When spawning new chassis on water
            if (hasWaterBelow())
            {
                MoveBoatDownToClosestWaterBlock();
                bSpawningBoatChassis = true;
                CreateDummyBoat();
            }
        }
    }

    public override void OnDriverOn()
    {
        DebugMsg("OnDriverOn");

        if (bHasBoatDummy)
        {
            DestroyDummyBoat();
        }

        this.m_characterController.enabled = true;

        this.nativeBoxCollider.center = vehicleActivationCenter;
        this.nativeBoxCollider.size = vehicleActivationSize;

        base.OnDriverOn();

        if (player == null)
            return;

        playerHeadTransform = player.emodel.GetHeadTransform();

        // Doesn't seem to do anything
        //player.vp_FPCamera.PositionOffset = playerOffsetPos;
    }


    public string[] randomDriverOffToolTips = new string[] {    "Sweet, I could fish from here!",
                                                                "That's the life!",
                                                                "Love this boat!",
                                                                "I wish my mate was still alive to see this",
                                                                "Now, where's my fishing gear?",
                                                                "Oh, I think I saw a fish jump up there!",
                                                                "Zeds will never find me up here!",
                                                                "This is the best vehicle ever!"
                                                                                                                   };

    public int lastRandomDriverOffToolTipIndex = 0;

    public override void OnDriverOff()
    {
        DebugMsg("OnDriverOff");
        base.OnDriverOff();
        this.m_characterController.enabled = false;

        if (!isTouchingGround && hasWaterBelow())
        {
            MoveBoatDownToClosestWaterBlock();
            CreateDummyBoat();

            if (!CustomVehiclesUtils.IsLandInBounds(new Bounds(this.position, new Vector3(5f, 1f, 5f))))
            {
                // random on water tooltips
                //GameManager.ShowTooltip(this.player, randomDriverOffToolTips[Random.Range(0, randomDriverOffToolTips.Length-1)]);
                GameManager.ShowTooltip(this.player, randomDriverOffToolTips[lastRandomDriverOffToolTipIndex]);
                lastRandomDriverOffToolTipIndex += 1;
                if(lastRandomDriverOffToolTipIndex > randomDriverOffToolTips.Length - 1)
                {
                    lastRandomDriverOffToolTipIndex = 0;
                }
            }

            this.nativeBoxCollider.center = new Vector3(0, 0.5f, -3.37f);
            this.nativeBoxCollider.size = new Vector3(2, 2, 1);
        }
        else
        {
            this.nativeBoxCollider.center = vehicleActivationCenter;
            this.nativeBoxCollider.size = vehicleActivationSize;
        }
    }

    public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (bNeedBoatDummyBlockOffset)
        {
            AdjustBoatAndDummyBoatBlockPositions();
        }

        //BlockRotsReverseEngineerTest();

        if (!hasDriver)
            return;

        if (particleEmitter != null)
        {
            //DebugMsg("FixedUpdate: m_characterController.velocity.magnitude = " + m_characterController.velocity.magnitude.ToString());
            particleEmitter.emit = (!isTouchingGround && m_characterController.velocity.magnitude > 1.8f);
        }

        // Disabling vehicle and kicking player out if he gets too far on land with it
        if(isTouchingGround)
        {
            if (!hasWaterArround())
            {
                DisableVehicle();
                this.nativeBoxCollider.center = vehicleActivationCenter;
                this.nativeBoxCollider.size = vehicleActivationSize;
                bHasBoatDummy = false;
                boatDummyBlockEntityData = null;
            }
        }

        // Was trying to remove the annoying repeating noise that happens when disabling the Character Controller
        /*if(bReEnableCharCtrl && Time.time - 1f > reEnableCharCtrlStartTime)
        {
            //this.m_characterController.enabled = true;
            bReEnableCharCtrl = false;
            reEnableCharCtrlStartTime = -1;
            DestroyBoatDeck();
            CreateDummyBoat();
        }*/
    }


    public new void LateUpdate()
    {
        base.LateUpdate();
    }


    public void DisableVehicle()
    {
        GameManager.ShowTooltip(this.player, "This Vehicle has been disabled, it's too far from water.");

        //this.FI = true;
        if (FI_bool_field != null)
            FI_bool_field.SetValue(this, true);
        this.vehicle.EmitEvent(Vehicle.Event.Disabled);
        //this.set_RSQ(false);

        if (set_RSQ_MethodInfo != null)
        {
            object[] rsq_params = new object[1] { false };
            set_RSQ_MethodInfo.Invoke(this, rsq_params);
        }
        if (this.AttachedEntities != null)
        {
            ((EntityAlive)this.AttachedEntities).SetHandIKTargets(null, null);
            this.vehicle.EmitEvent(VPIgnition.Action.TurnOff);
            //this.SI = false;
            //this.CI = null;
            if (SI_bool_field != null)
                SI_bool_field.SetValue(this, false);
            if (CI_MovementInput_field != null)
                CI_MovementInput_field.SetValue(this, null);
            this.isInteractionLocked = false;
            this.AttachedEntities.SetAttachedTo(0, null);
            base.AttachEntity(0, null);
        }
    }

    public override void OnCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnCollidedWithBlock(_world, _clrIdx, _blockPos, _blockValue);

        //if (!this.hasDriver || player == null || lastLandHitToolTipTime != -1 && Time.time - 2f < lastLandHitToolTipTime)
        if (!this.hasDriver || lastLandHitToolTipTime != -1 && Time.time - 2f < lastLandHitToolTipTime)
            return;

        // Slowing down the boat when it touches the ground
        Block block = Block.list[_blockValue.type];
        if (!hasWaterAbove() && !(block.GetBlockName() == "air") && !block.GetType().IsSubclassOf(typeof(BlockLiquid)) && !(block.GetType() == typeof(BlockLiquidv2)) && !(block.GetBlockName() == "smallBoatDummyBlock") && !(block.GetBlockName() == "waterSupportBlock"))
        {
            //DebugMsg("Boat is touching Ground!");
            isTouchingGround = true;
            if (LZ_float_field != null)
            {
                LZ_float_field.SetValue(this.vehicle, 1f);

                if (this.player != null && Time.time - 5f > lastLandHitToolTipTime)
                {
                    GameManager.ShowTooltip(this.player, "This Vehicle can only move on Water.");
                    lastLandHitToolTipTime = Time.time;
                }
            }
        }
        else
        {
            // Back to normal speed when in water
            isTouchingGround = false;
            if (LZ_float_field != null)
            {
                LZ_float_field.SetValue(this.vehicle, maxSpeedBackup);
                lastLandHitToolTipTime = -1;
            }
        }
    }

    #endregion


    #region Dummy Boat Block methods

    public virtual void CreateDummyBoat()
    {
        // Create Dummy boat Block
        BlockValue newBlockValue = Block.GetBlockValue("smallBoatDummyBlock");
        int rot = CustomVehiclesUtils.EularYRotToBlockYRot(this.transform.forward);
        newBlockValue.rotation = (byte)rot;
        DebugMsg("EntityCustomBoat Create Dummy Boat: rot = " + rot.ToString());

        bHasBoatDummy = true;

        if (Steam.Network.IsServer)
            GameManager.Instance.World.SetBlockRPC(boatDummyPos, newBlockValue);
    }

    public virtual void DestroyDummyBoat()
    {
        BlockValue newBlockValue = Block.GetBlockValue("water");

        // Destroy Dummy boat
        DebugMsg("EntityCustomBoat DestroyDummyBoat: Destroy Dummy Boat Block: pos = " + boatDummyPos.ToString());
        //if (Steam.Network.IsServer)
        GameManager.Instance.World.SetBlockRPC(boatDummyPos, newBlockValue);

        bHasBoatDummy = false;
        boatDummyBlockEntityData = null;
    }

    public virtual Vector3i GetClosestBlockPosition()
    {
        int posX = Mathf.RoundToInt(this.transform.position.x);
        int posY = Mathf.RoundToInt(this.transform.position.y);
        int posZ = Mathf.RoundToInt(this.transform.position.z);
        return new Vector3i(posX, posY, posZ);
    }

    public virtual Vector3i GetClosestWaterBlockPos()
    {
        Chunk chunk;
        int curBlockDensity;
        Vector3i blockPos;
        BlockValue blockVal;
        Block block;
        Vector3i closestBlockPos = GetClosestBlockPosition();
        bool firstBlockFound = false;
        Vector3i firstFoundBlockPos = closestBlockPos;
        int firstFoundBlockDensity = 0;

        Vector3i resultPos = closestBlockPos;
        for (int i = closestBlockPos.y; i > closestBlockPos.y - 5; i--)
        {
            blockPos = new Vector3i(closestBlockPos.x, i, closestBlockPos.z);
            blockVal = this.world.GetBlock(blockPos);
            block = Block.list[blockVal.type];
            if (block.blockMaterial.IsLiquid)
            {
                chunk = (Chunk)this.world.GetChunkFromWorldPos(blockPos);
                curBlockDensity = (int)this.world.GetDensity(chunk.ClrIdx, blockPos);
                DebugMsg("GetClosestWaterBlockPos: Found closest water block: Pos = " + blockPos.ToString() +  " | Density = " + ((int)curBlockDensity).ToString("0.000"));

                if(!firstBlockFound)
                {
                    firstFoundBlockPos = blockPos;
                    firstBlockFound = true;
                    firstFoundBlockDensity = curBlockDensity;
                }
            }
        }
        if(firstBlockFound)
        {
            // If water block density is not full we return the one under it
            if (firstFoundBlockDensity < 127)
            {
                
                resultPos = new Vector3i(firstFoundBlockPos.x, firstFoundBlockPos.y - 1, firstFoundBlockPos.z);
                DebugMsg("GetClosestWaterBlockPos: returning first water block found -1: pos = " + resultPos.ToString());
            }
            else
            {
                resultPos = firstFoundBlockPos;
                DebugMsg("GetClosestWaterBlockPos: returning first water block found: pos = " + resultPos.ToString());
            }
        }

        return resultPos;
    }


    public virtual void MoveBoatDownToClosestWaterBlock()
    {
        boatDummyPos = GetClosestWaterBlockPos();
        DebugMsg("MoveDownToClosestWaterBlock: pos = " + boatDummyPos.ToString());
        Vector3 newPos = new Vector3(boatDummyPos.x, (float)boatDummyPos.y + 0.3f, boatDummyPos.z);
        this.transform.position = newPos;
        this.SetPosition(newPos);
    }

    public virtual void GetDummyBoatBlockEntityData(Vector3i _boatDummyPos)
    {
        if (boatDummyBlockEntityData != null)
        {
            DebugMsg("GetDummyBoatBlockEntityData: DummyBoatBlockEntityData already exists, calling delayed mesh offset.");
            bNeedBoatDummyBlockOffset = true;
            return;
        }

        curChunk = (Chunk)this.world.GetChunkFromWorldPos(_boatDummyPos);
        if (curChunk != null)
        {
            DebugMsg("GetDummyBoatBlockEntityData: Found Boat Chunk");
            boatDummyBlockEntityData = curChunk.GetBlockEntity(_boatDummyPos);

            if (boatDummyBlockEntityData != null)
            {
                string blockName = Block.list[boatDummyBlockEntityData.blockValue.type].GetBlockName();
                if (blockName == "smallBoatDummyBlock")
                {
                    DebugMsg("GetDummyBoatBlockEntityData: Found valid boatDummyBlockEntityData, calling delayed mesh offset");

                    bNeedBoatDummyBlockOffset = true;
                }
                else
                {
                    DebugMsg("GetDummyBoatBlockEntityData: boatDummyBlockEntityData is Invalid, setting to null. Found this block instead: " + blockName);
                    boatDummyBlockEntityData = null;
                }
            }
            else
            {
                DebugMsg("GetDummyBoatBlockEntityData: DummyBoatBlockEntityData is NULL");
            }

            /*if (bOnGround)
            {
                bRotateTest = true;
                rotateTestStartTime = Time.time;
            }*/
        }
        else
        {
            DebugMsg("GetDummyBoatBlockEntityData: Chunk is NULL");
        }
    }


    public virtual void AdjustBoatAndDummyBoatBlockPositions()
    {
        if ((boatDummyBlockEntityData != null && boatDummyBlockEntityData.bHasTransform))
        {
            DebugMsg("AdjustBoatAndDummyBoatBlockPositions: Found Boat BlockEntityData: positioning Dummy Boat Mesh");

            // Offset the dummy boat block mesh to the Y position of the boat vehicle
            Vector3 boatDummyPosVec3 = boatDummyPos.ToVector3();
            DebugMsg("AdjustBoatAndDummyBoatBlockPositions: before Offset: boatDummyPos = " + boatDummyPosVec3.ToString("0.000") + " | boat dummy transform pos = " + boatDummyBlockEntityData.transform.position.ToString("0.000") + " | boat vehicle pos = " + this.transform.position.ToString("0.000"));
            boatDummyPosVec3 = boatDummyBlockEntityData.transform.position;

            if (bNeedLoadAdjustments)
            {
                boatDummyPosVec3.y = boatDummyPos_YOffset;
            }
            else
            {
                //boatDummyPosVec3.y = this.transform.position.y;
                boatDummyPosVec3.y += 0.3f;  // In MP the current boat position is flaky, so I'm harcoding it for now.
            }

            boatDummyPos = boatDummyBlockEntityData.pos;    // for persistence
            boatDummyBlockEntityData.transform.position = boatDummyPosVec3;
            boatDummyPos_YOffset = boatDummyPosVec3.y;  // for persistence

            // Snap the boat vehicle to the boat dummy block XZ position
            Transform tmpOffsetTransform = new GameObject("tmp").transform;
            tmpOffsetTransform.SetParent(boatDummyBlockEntityData.transform);
            tmpOffsetTransform.localRotation = Quaternion.identity;
            tmpOffsetTransform.localPosition = new Vector3(-0.5f, 0, -0.5f);

            this.transform.position = tmpOffsetTransform.position;
            this.SetPosition(tmpOffsetTransform.position);

            if (player != null && !bNeedLoadAdjustments && !bSpawningBoatChassis && hasPlayerExitPos)
            {
                Vector3 exitPos = tmpOffsetTransform.position + playerExitPos;
                player.transform.position = exitPos;
                player.SetPosition(exitPos);
            }
            bSpawningBoatChassis = false;

            if (tmpOffsetTransform != null)
            {
                Destroy(tmpOffsetTransform.gameObject);
            }

            // Hide the dummy boat block mesh
            MeshRenderer[] meshRenderers = boatDummyBlockEntityData.transform.gameObject.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.enabled = false;
            }

            bNeedBoatDummyBlockOffset = false;
            failedAdjustBoatPosCount = 0;
            if (bNeedLoadAdjustments)
            {
                DebugMsg("AdjustBoatAndDummyBoatBlockPositions: Load Adjustments Done.");
                bNeedLoadAdjustments = false;
            }
        }
        else
        {
            if (boatDummyBlockEntityData != null)
            {
                DebugMsg("AdjustBoatAndDummyBoatBlockPositions: Boat BlockEntityData does not have a transform");
            }
            else if(boatDummyBlockEntityData == null)
            {
                DebugMsg("AdjustBoatAndDummyBoatBlockPositions: Boat BlockEntityData is NULL.");
            }
            else
            {
                DebugMsg("AdjustBoatAndDummyBoatBlockPositions: boatDummyBlockTransform is NULL."); 
            }
        }
    }

    // Was trying to get the dummy boat block BlockShapeModelEntity Transform directly, but that just gets the prefab in the center of the world
    /*public static GameObject GetBlockShapeModelEntityGameObject(BlockShapeModelEntity _bsme)
    {
        Transform resultTransform = null;
        if (TW_MethodInfo != null)
        {
            object[] rsq_params = new object[0];
            resultTransform = (Transform)TW_MethodInfo.Invoke(_bsme, rsq_params);
            string msg = "EntityCustomBoat GetBlockShapeModelEntityGameObject: Found bsme GameObject:\n";
            if (resultTransform != null && resultTransform.gameObject != null)
            {
                msg += "root transform pos = " + resultTransform.position.ToString("0.000") + "\nChildren:\n";
                Transform[] transforms = resultTransform.gameObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in transforms)
                {
                    msg += "\t- " + t.name + " | pos = " + t.position.ToString("0.000");
                }
            }
            DebugMsg(msg);
        }
        if (resultTransform != null && resultTransform.gameObject != null)
            return resultTransform.gameObject;
        else
            DebugMsg("EntityCustomBoat GetBlockShapeModelEntityGameObject: GameObject is NULL:\n");
        return null;
    }*/

    // This was just to revese-ingineer the different blocks orientations
    public void BlockRotsReverseEngineerTest()
    {
        if (bRotateTest && Time.time - 3f > rotateTestStartTime)
        {
            DebugMsg("Rotating boat to: " + rotateTestRot.ToString());
            BlockValue newBlockValue = Block.GetBlockValue("smallBoatDummyBlock");
            newBlockValue.rotation = (byte)rotateTestRot;
            GameManager.Instance.World.SetBlockRPC(boatDummyPos, newBlockValue);
            rotateTestRot += 1;
            rotateTestStartTime = Time.time;

            if (rotateTestRot >= 1000)
            {
                bRotateTest = false;
                rotateTestRot = 0;
            }
        }
    }

    #endregion


    #region Misc

    // Was trying to be able to push the boat, but doesn't seem to do anything. Didn't really try to dig further...
    public override bool CanBePushed()
    {
        return true;
    }

    public override void OnEntityUnload()
    {
        base.OnEntityUnload();
    }

    #endregion


    #region Serialization

    public bool bInitialLoadDone = false;
    public Vector3i boatDummyPos_Load = new Vector3i();
    public bool bHasBoatDummy_Load = false;
    public float boatDummyPos_YOffset_Load = 0;

    public override void Read(byte _version, BinaryReader _br)
    {
        base.Read(_version, _br);
        if (_br.BaseStream == null || _version <= 11)
            return;

        //if (GameManager.IsDedicatedServer || _br.BaseStream.Position == _br.BaseStream.Length)
        if (_br.BaseStream.Position == _br.BaseStream.Length)
            return;

        string msg = "";
        try
        {
            // Quick and dirty hack: Offset the read by 1. Vehicles have a more complex way of serializing, and it won't read properly unless I do this.
            msg += "EntityCustomBoat Read:\n";
            msg += "\t- Entity InstanceID = " + this.GetInstanceID() + "\n";
            msg += "\t- Read Start Pos BEFORE +1 hack = " + _br.BaseStream.Position.ToString("0.000") + "\n";
            msg += "\t- Read Length BEFORE +1 hack = " + _br.BaseStream.Length.ToString("0.000") + "\n";
            if(_br.BaseStream.Position == 1008L)
                _br.ReadByte();
            msg += "\t- Read Start Pos AFTER +1 hack = " + _br.BaseStream.Position.ToString("0.000") + "\n";
            msg += "\t- Read Length AFTER +1 hack = " + _br.BaseStream.Length.ToString("0.000") + "\n";
            if (_br.BaseStream.Position == _br.BaseStream.Length)
            {
                DebugMsg(msg);
                return;
            }

            bHasBoatDummy_Load = _br.ReadBoolean();
            msg += "\t- boatDummyPos_Load = " + bHasBoatDummy_Load.ToString() + "\n";

            boatDummyPos_Load = new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());

            boatDummyPos_YOffset_Load = _br.ReadSingle();
                
            msg += "\t- boatDummyPos_Load = " + boatDummyPos_Load.ToString() + "\n";
            msg += "\t- boatDummyPos_YOffset_Load = " + boatDummyPos_YOffset_Load.ToString("0.000") + "\n";
            msg += "\t- Read End Pos = " + _br.BaseStream.Position.ToString("0.000") + "\n";
            msg += "\t- Read Length = " + _br.BaseStream.Length.ToString("0.000") + "\n";
            DebugMsg(msg);

            //if (GameManager.IsDedicatedServer && bHasBoatDummy_Load)
            if (bHasBoatDummy_Load)
            {
                bHasBoatDummy = bHasBoatDummy_Load;
                boatDummyPos = boatDummyPos_Load;
                boatDummyPos_YOffset = boatDummyPos_YOffset_Load;
                if (!bInitialLoadDone)
                {
                    DebugMsg("EntityCustomBoat Read: Initial Load, triggering Boat Load adjuments.");
                    bNeedLoadAdjustments = true;
                    bInitialLoadDone = true;
                }
            }
        }
        catch (System.Exception e)
        {
            DebugMsg(msg);
            Debug.LogError("EntityCustomBoat: Failed to read from stream: " + e);
        }
    }

    public override void Write(BinaryWriter _bw)
    {
        base.Write(_bw);

        //if (player == null)
            //return;

        try
        {
            string msg = "EntityCustomBoat Write:\n";
            msg += "\t- Entity InstanceID = " + this.GetInstanceID() + "\n";
            msg += "\t- Write Start Pos = " + _bw.BaseStream.Position.ToString("0.000") + "\n";
            msg += "\t- Write Length = " + _bw.BaseStream.Length.ToString("0.000") + "\n";

            _bw.Write(bHasBoatDummy);
            msg += "\t- bHasBoatDummy = " + bHasBoatDummy.ToString() + "\n";

            _bw.Write(boatDummyPos.x);
            _bw.Write(boatDummyPos.y);
            _bw.Write(boatDummyPos.z);

            if(boatDummyBlockEntityData != null && boatDummyBlockEntityData.bHasTransform)
            {
                boatDummyPos_YOffset = boatDummyBlockEntityData.transform.position.y;
            }
            _bw.Write(boatDummyPos_YOffset);

            msg += "\t- boatDummyPos = " + boatDummyPos.ToString() + "\n";
            msg += "\t- boatDummyPos_YOffset = " + boatDummyPos_YOffset.ToString("0.000") + "\n";
            msg += "\t- Write End Pos = " + _bw.BaseStream.Position.ToString("0.000") + "\n";
            msg += "\t- Write Length = " + _bw.BaseStream.Length.ToString("0.000") + "\n";
            DebugMsg(msg);

        }
        catch (System.Exception e)
        {
            Debug.LogError("EntityCustomBoat: Failed to write to stream: " + e);
        }
    }

    #endregion


    #region Unused first dummy boat block implementation

    /*
    public void CreateDeckRamps(bool remove)
    {
        int posX = Mathf.RoundToInt(this.transform.position.x);
        int posY = Mathf.RoundToInt(this.transform.position.y - 1f);
        int posZ = Mathf.RoundToInt(this.transform.position.z);

        MakeDeckRampRow(posX - 4, posX + 4, posY, posZ + 3, true, remove);
        MakeDeckRampRow(posX - 4, posX + 4, posY, posZ - 4, true, remove);
        MakeDeckRampRow(posZ - 3, posZ + 3, posY, posX + 3, false, remove);
        MakeDeckRampRow(posZ - 3, posZ + 3, posY, posX - 4, false, remove);
    }

    public void MakeDeckRampRow(int start, int end, int posY, int perpAxisPos, bool isZ, bool remove)
    {
        Vector3i newPos;
        BlockValue curBlockValue;
        Block curBlock;
        BlockValue newBlockValue;

        for (int i = start; i < end; i++)
        {
            if (isZ)
                newPos = new Vector3i(i, posY, perpAxisPos);
            else
                newPos = new Vector3i(perpAxisPos, posY, i);

            curBlockValue = GameManager.Instance.World.GetBlock(newPos);
            curBlock = Block.list[curBlockValue.type];

            if (remove)
            {
                if (curBlock.GetBlockName() == "boatDeckBlock")
                {
                    newBlockValue = Block.GetBlockValue("water");
                    GameManager.Instance.World.SetBlockRPC(newPos, newBlockValue);
                }
            }
            else
            {
                if (curBlock.GetBlockName() == "water")
                {
                    newBlockValue = Block.GetBlockValue("boatDeckBlock");
                    GameManager.Instance.World.SetBlockRPC(newPos, newBlockValue);
                }
            }
        }
    }


    public void SwitchBoatCoveringBlocks(string curBlockName, string newBlockName, string underBlockName, int Yoffset)
    {
        int posX = Mathf.RoundToInt(this.transform.position.x);
        int posY = Mathf.RoundToInt(this.transform.position.y + (float)Yoffset);
        int posZ = Mathf.RoundToInt(this.transform.position.z);

        DebugMsg("SwitchBoatCoveringBlocks:");

        for (int i = posX - 3; i < posX + 3; i++)
        {
            for (int k = posZ - 3; k < posZ + 3; k++)
            {
                Vector3i curPos = new Vector3i(i, posY, k);
                BlockValue curBlockValue = GameManager.Instance.World.GetBlock(curPos);
                Block curBlock = Block.list[curBlockValue.type];
                DebugMsg("curBlock = " + curBlock.GetBlockName() + " | block pos = " + curPos.ToString());

                if (curBlock.GetBlockName() == curBlockName)
                {
                    BlockValue newBlockValue = Block.GetBlockValue(newBlockName);
                    Block newBlock = Block.list[newBlockValue.type];

                    DebugMsg("swtiching curBlock to: " + newBlockName + " | from block name = " + newBlock.GetBlockName());
                    if (newBlock.blockMaterial != null)
                    {
                        DebugMsg("blockMaterial = " + newBlock.blockMaterial.id);
                    }
                    if (newBlock.blockMaterial != null && newBlock.blockMaterial.stepSound != null)
                    {
                        DebugMsg("stepSound = " + newBlock.blockMaterial.stepSound.name);
                    }

                    if (underBlockName != "none")
                    {
                        Vector3i underPos = curPos;
                        underPos.y -= 1;
                        BlockValue underBlockValue = GameManager.Instance.World.GetBlock(underPos);
                        Block underBlock = Block.list[underBlockValue.type];
                        if (underBlock.GetBlockName() == underBlockName)
                            GameManager.Instance.World.SetBlockRPC(curPos, newBlockValue);
                    }
                    else
                    {
                        GameManager.Instance.World.SetBlockRPC(curPos, newBlockValue);
                    }
                }
            }
        }
    }

    public void SwitchBoatCoveringBlocks_Old(string curBlockName, string newBlockName)
    {
        Bounds _aabb = new Bounds(new Vector3(this.boundingBox.center.x, this.boundingBox.center.y - 2f, this.boundingBox.center.z), new Vector3(this.boundingBox.size.x * 2f, 0f, this.boundingBox.size.z * 2f));

        int num = Utils.Fastfloor(_aabb.min.x);
        int num2 = Utils.Fastfloor(_aabb.max.x + 1f);
        int num3 = Utils.Fastfloor(_aabb.min.y);
        int num4 = Utils.Fastfloor(_aabb.max.y + 1f);
        int num5 = Utils.Fastfloor(_aabb.min.z);
        int num6 = Utils.Fastfloor(_aabb.max.z + 1f);
        if (_aabb.min.x < 0f)
        {
            num--;
        }
        if (_aabb.min.y < 0f)
        {
            num3--;
        }
        if (_aabb.min.z < 0f)
        {
            num5--;
        }

        DebugMsg("SwitchBoatCoveringBlocks:");

        for (int i = num; i < num2; i++)
        {
            for (int j = num3; j < num4; j++)
            {
                for (int k = num5; k < num6; k++)
                {
                    Vector3i curPos = new Vector3i(i, j, k);
                    BlockValue curBlockValue = GameManager.Instance.World.GetBlock(curPos);
                    Block curBlock = Block.list[curBlockValue.type];
                    DebugMsg("curBlock = " + curBlock.GetBlockName() + " | block pos = " + curPos.ToString());

                    if (curBlock.GetBlockName() == curBlockName)
                    {
                        BlockValue newBlockValue = Block.GetBlockValue(newBlockName);
                        Block newBlock = Block.list[newBlockValue.type];

                        DebugMsg("swtiching curBlock to: " + newBlockName + " | from block name = " + newBlock.GetBlockName());
                        GameManager.Instance.World.SetBlockRPC(curPos, newBlockValue);
                    }
                }
            }
        }
    }

    // Not used, using serialization instead.
    public virtual void TryToFindExistingDummyBoatBlock()
    {
        Vector3i blockPos;
        BlockValue blockVal;
        Block block;
        Vector3i closestBlockPos = GetClosestBlockPosition();
        for (int i = closestBlockPos.x - 5; i < closestBlockPos.x + 5; i++)
        {
            for (int j = closestBlockPos.z - 5; j < closestBlockPos.z + 5; j++)
            {
                for (int k = closestBlockPos.y; k > closestBlockPos.y - 5; k--)
                {
                    //blockPos = new Vector3i(i, closestBlockPos.y, j);
                    blockPos = new Vector3i(i, k, j);
                    blockVal = this.world.GetBlock(blockPos);
                    block = Block.list[blockVal.type];
                    if (block.GetBlockName() == "smallBoatDummyBlock")
                    {
                        DebugMsg("TryToFindExistingDummyBoatBlock: Found Dummy Boat Block");
                        boatDummyPos = blockPos;
                        break;
                    }
                }
            }
        }
    }
    */

    #endregion
}


