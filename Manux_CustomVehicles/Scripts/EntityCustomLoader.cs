using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


class EntityCustomLoader : EntityCustomBike
{
    Transform Origin = null;
    Transform handlebar_joint = null;
    Transform chassis_joint = null;
    Transform loader_joint = null;
    Transform bucket_joint = null;

    Transform Origin2 = null;
    Transform handlebar_joint2 = null;
    Transform chassis_joint2 = null;
    Transform loader_joint2 = null;
    Transform bucket_joint2 = null;

    int currentBucketHeight = 1;

    //string loaderSound = "Weapons/Motorized/Chainsaw/chainsaw_fire_lp";
    bool isLoaderSoundStarted;
    //Vector3 loaderSoundPos;
    AudioSource audioSource;

    Vector3 lastLoaderRot;
    Vector3 lastBucketRot;

    Vector3i lastHitBlockPos;

    bool allBonesSet1Found;
    bool allBonesSet2Found;

    public float vehicleDurabilityMax;
    public float vehicleDurabilityPercent;

    int entityDamage = 0;
    int blockDamage = 0;

    int destructionRadius;
    bool hasDestructionRadius;
    int destructionHeight = 1;
    float destructionHarvestBonus = 1.0f;
    float destroySolidsDegradPerHit = 100.0f;

    List<string> destroyBlocks = new List<string>() { "grass", "plant", "cactus", "shrubOrBush", "tree", "rock", "bigBoulder", "rareOres", "tire" };
    List<string> harvestBlocks = new List<string>() { "grass", "plant", "cactus", "shrubOrBush", "tree", "rock", "bigBoulder", "rareOres", "tire" };
    bool harvestToVehicleInventory = false;

    public ItemValue item_fireAxeSteel = ItemClass.GetItem("fireaxeSteel", false);
    public ItemValue item_pickAxeSteel = ItemClass.GetItem("pickaxeSteel", false);
    public ItemValue item_shovelSteel = ItemClass.GetItem("shovelSteel", false);
    public ItemValue item_clawHammer = ItemClass.GetItem("clawHammer", false);
    public ItemValue item_wrench = ItemClass.GetItem("wrench", false);

    public bool isDestroyingBlocks;

    float debugBlocksPrintDelay = 10.0f;
    float lastDebugBlocksPrint = -1;

    global::EntityPlayerLocal player;
    global::LocalPlayerUI uiforPlayer;
    global::XUiM_PlayerInventory playerInventory;
    global::XUiC_VehicleContainer xuiC_VehicleContainer;
    public bool isPlayerOnVehicle;

    //CustomLoaderControl clc = null;

    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        /*if (entityClass.Properties.Values.ContainsKey("Sound_loader_moving_loop"))
            this.loaderSound = entityClass.Properties.Values["Sound_loader_moving_loop"];*/

        if (entityClass.Properties.Values.ContainsKey("EntityDamage"))
        {
            int entityDmg;
            if (int.TryParse(entityClass.Properties.Values["EntityDamage"], out entityDmg))
            {
                DebugMsg("\tentityDamage = " + entityDmg.ToString("0.0000"));
                entityDamage = entityDmg;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("BlockDamage"))
        {
            int blockDmg;
            if (int.TryParse(entityClass.Properties.Values["BlockDamage"], out blockDmg))
            {
                DebugMsg("\tblockDamage = " + blockDmg.ToString("0.0000"));
                blockDamage = blockDmg;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestructionRadius"))
        {
            int destRadius;
            if (int.TryParse(entityClass.Properties.Values["DestructionRadius"], out destRadius))
            {
                DebugMsg("\tdestructionRadius = " + destRadius.ToString("0.000"));
                destructionRadius = destRadius;
                hasDestructionRadius = true;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestructionHeight"))
        {
            int destHeight;
            if (int.TryParse(entityClass.Properties.Values["DestructionHeight"], out destHeight))
            {
                DebugMsg("\tdestructionHeight = " + destHeight.ToString());
                destructionHeight = destHeight;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestructionHarvestBonus"))
        {
            float destBonus;
            if (float.TryParse(entityClass.Properties.Values["DestructionHarvestBonus"], out destBonus))
            {
                DebugMsg("\tdestructionHarvestBonus = " + destBonus.ToString("0.000"));
                destructionHarvestBonus = destBonus;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestroySolidsDegradationPerHit"))
        {
            float degradPerHit;
            if (float.TryParse(entityClass.Properties.Values["DestroySolidsDegradationPerHit"], out degradPerHit))
            {
                DebugMsg("\tdestroySolidsDegradPerHit = " + degradPerHit.ToString("0.0000"));
                destroySolidsDegradPerHit = degradPerHit;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestroyBlocks"))
        {
            destroyBlocks = new List<string>(entityClass.Properties.Values["DestroyBlocks"].Split(','));
            DebugMsg("\tdestroyBlocks = " + string.Join(",", destroyBlocks.ToArray()));
        }
        else
        {
            destroyBlocks.Clear();
        }
        if (entityClass.Properties.Values.ContainsKey("HarvestBlocks"))
        {
            harvestBlocks = new List<string>(entityClass.Properties.Values["HarvestBlocks"].Split(','));
            DebugMsg("\tharvestBlocks = " + string.Join(",", harvestBlocks.ToArray()));
        }
        else
        {
            harvestBlocks.Clear();
        }
        if (entityClass.Properties.Values.ContainsKey("HarvestToVehicleInventory"))
        {
            bool harvestToVehicleInv;
            if (bool.TryParse(entityClass.Properties.Values["HarvestToVehicleInventory"], out harvestToVehicleInv))
            {
                DebugMsg("\tharvestToVehicleInventory = " + harvestToVehicleInv.ToString());
                harvestToVehicleInventory = harvestToVehicleInv;
            }
        }

        AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();
        if (audioSources.Length > 0)
        {
            audioSource = audioSources[0];
        }
    }

    protected override void Start()
    {
        base.Start();

        if (this.AttachedEntities is global::EntityPlayerLocal)
        {
            InitRefs();
        }

        List<Transform> childrenList = new List<Transform>();
        List<int> childrenInstanceIds = new List<int>();
        childrenList.Add(this.transform);
        childrenInstanceIds.Add(this.transform.GetInstanceID());
        CustomVehiclesUtils.GetAllChildTransforms(this.transform, ref childrenList, ref childrenInstanceIds);

        foreach (Transform child in childrenList)
        {
            switch (child.name)
            {
                case "Origin":
                    if (Origin == null)
                        Origin = child;
                    else
                        Origin2 = child;
                    break;
                case "handlebar_joint":
                    if (handlebar_joint == null)
                        handlebar_joint = child;
                    else
                        handlebar_joint2 = child;
                    break;
                case "chassis_joint":
                    if (chassis_joint == null)
                        chassis_joint = child;
                    else
                        chassis_joint2 = child;
                    break;
                case "loader_joint":
                    if (loader_joint == null)
                        loader_joint = child;
                    else
                        loader_joint2 = child;
                    break;
                case "bucket_joint":
                    if (bucket_joint == null)
                        bucket_joint = child;
                    else
                        bucket_joint2 = child;
                    break;
            }
        }

        // Was for testing additional colliders with a custom script. May reuse later...
        /*if (Origin != null)
        {
            BoxCollider bc = Origin.gameObject.GetComponent<BoxCollider>();
            if (bc != null)
            {
                bc.enabled = true;
                clc = Origin.gameObject.GetComponent<CustomLoaderControl>();
                if (clc == null)
                {
                    DebugMsg("Adding CustomLoaderControl script.");
                    clc = Origin.gameObject.AddComponent<CustomLoaderControl>();
                }
                clc.enabled = true;
            }
        }*/

        if (handlebar_joint == null || chassis_joint == null || loader_joint == null || bucket_joint == null)
        {
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            allBonesSet1Found = true;
            DebugMsg(this.ToString() + " : All bones set 1 found.");

            lastLoaderRot = loader_joint.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }

        if (handlebar_joint2 == null || chassis_joint2 == null || loader_joint2 == null || bucket_joint2 == null)
        {
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            allBonesSet2Found = true;
            DebugMsg(this.ToString() + " : All bones set 2 found.");

            lastLoaderRot = loader_joint2.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }
    }

    public void AnimateExtraJoints(Transform Origin, Transform handlebar_joint, Transform loader_joint, Transform bucket_joint)
    {
        bool isMovingBucket = false;
        string dbgMsg = "";
        Vector3 newRot = handlebar_joint.localRotation.eulerAngles;
        Quaternion newQuat = new Quaternion();
        if (newRot != Vector3.zero)
        {
            dbgMsg += ("handlebar Y: " + newRot.ToString("0.000") + "\n");
            if (newRot.y > 180)
            {
                newRot.y -= 360;
            }
            newRot.y = -(newRot.y / 2.0f);
            dbgMsg += ("-handlebar Y: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            chassis_joint.localRotation = newQuat;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = loader_joint.localRotation.eulerAngles;
            dbgMsg += ("UpArrow: " + newRot.ToString("0.000") + "\n");
            newRot.x -= 3f;
            if (newRot.x > 180)
            {
                newRot.x -= 360;
            }
            newRot.x = Mathf.Clamp(newRot.x, -90, 0);
            dbgMsg += ("UpArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            loader_joint.localRotation = newQuat;
            lastLoaderRot = newRot;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = loader_joint.localRotation.eulerAngles;
            dbgMsg += ("DownArrow: " + newRot.ToString("0.000") + "\n");
            newRot.x += 3f;
            if (newRot.x > 180)
            {
                newRot.x -= 360;
            }
            newRot.x = Mathf.Clamp(newRot.x, -90, 0);
            dbgMsg += ("DownArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            loader_joint.localRotation = newQuat;
            lastLoaderRot = newRot;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = bucket_joint.localRotation.eulerAngles;
            //newRot = new Vector3(newRot.z, 0, 0);
            dbgMsg += ("LeftArrow: " + newRot.ToString("0.000") + "\n");
            newRot.z += 4;
            if (newRot.z > 180)
            {
                newRot.z -= 360;
            }
            newRot.z = Mathf.Clamp(newRot.z, -40, 150);
            dbgMsg += ("LeftArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = bucket_joint.localRotation.eulerAngles;
            //newRot = new Vector3(newRot.z, 0, 0);
            dbgMsg += ("RightArrow: " + newRot.ToString("0.000") + "\n");
            newRot.z -= 4;
            if (newRot.z > 180)
            {
                newRot.z -= 360;
            }
            newRot.z = Mathf.Clamp(newRot.z, -40, 150);
            dbgMsg += ("RightArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }

        if (isMovingBucket == false && isLoaderSoundStarted)
        {
            //Audio.Manager.StopAllSequencesOnEntity(this);
            audioSource.Pause();
            //Audio.Manager.BroadcastStop(loaderSoundPos, loaderSound);
            isLoaderSoundStarted = false;
        }

        // bring back bucket when loader arm is close to the ground
        if (lastLoaderRot.x > -20.0f && lastBucketRot.z > 0)
        {
            newRot = lastBucketRot;
            newRot.z = (Mathf.Abs(lastLoaderRot.x * 5.0f) / 100.0f) * newRot.z;
            newRot.z = Mathf.Clamp(newRot.z, -40.0f, 150.0f);
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }

        int originFloor = (int)Mathf.Round(Origin.transform.position.y);
        currentBucketHeight = ((int)Mathf.Floor(bucket_joint.transform.position.y - Origin.transform.position.y)) + 1;
        //if (currentBucketHeight < originFloor)
            //currentBucketHeight = originFloor;
        //int originFloor = (int)Mathf.Round(Origin.transform.position.y);
        //int bucketMinusOrigin = ((int)Mathf.Floor(bucket_joint.transform.position.y)) - originFloor;
        //currentBucketHeight = originFloor + Mathf.Clamp(bucketMinusOrigin, 0, bucketMinusOrigin);
        //int originRounded = (int)Mathf.Round(Origin.transform.position.y);
        //int bucketFloor = (int)Mathf.Floor(bucket_joint.transform.position.y);
        //int currentBucketHeight = originRounded + Mathf.Max(originRounded, bucketFloor - originRounded);



            // This creates weird binding errors when used
            //DebugMsg(dbgMsg);
    }

    public void InitRefs()
    {
        player = this.AttachedEntities as global::EntityPlayerLocal;
        uiforPlayer = global::LocalPlayerUI.GetUIForPlayer(global::GameManager.Instance.World.GetLocalPlayer() as global::EntityPlayerLocal);
        playerInventory = uiforPlayer.xui.PlayerInventory;
        global::GUIWindowManager windowManager = uiforPlayer.windowManager;
        ((global::XUiC_VehicleWindowGroup)((global::XUiWindowGroup)windowManager.GetWindow("vehicle")).Controller).CurrentVehicleEntity = this;
        xuiC_VehicleContainer = (global::XUiC_VehicleContainer)uiforPlayer.xui.FindWindowGroupByName(global::XUiC_VehicleWindowGroup.ID).GetChildByType<global::XUiC_VehicleContainer>();

        vehicleDurabilityMax = this.vehicle.GetVehicleMaxDurability();
        vehicleDurabilityPercent = this.vehicle.GetVehicleDurabilityPercentage();

        // Convert the current Vehicle Health as the Quality of the dummy harvesting tools.
        int loaderAsMiningToolQuality = (int)(vehicleDurabilityPercent * 600);
        item_fireAxeSteel.Quality = loaderAsMiningToolQuality;
        item_pickAxeSteel.Quality = loaderAsMiningToolQuality;
        item_shovelSteel.Quality = loaderAsMiningToolQuality;
        item_clawHammer.Quality = loaderAsMiningToolQuality;
        item_wrench.Quality = loaderAsMiningToolQuality;

        isPlayerOnVehicle = true;
    }


    public new void FixedUpdate()
    {
        try
        {
            base.FixedUpdate();

            if (!allBonesSet1Found || !(this.AttachedEntities is global::EntityPlayerLocal))
            {
                isPlayerOnVehicle = false;
                return;
            }

            if (!isPlayerOnVehicle)
            {
                InitRefs();
            }

            // There is sometimes 2 versions of the car prefab in the game, we need to use the second one when it exists.
            if (allBonesSet2Found)
            {
                AnimateExtraJoints(Origin2, handlebar_joint2, loader_joint2, bucket_joint2);
            }
            else
            {
                AnimateExtraJoints(Origin, handlebar_joint, loader_joint, bucket_joint);
            }

            FindAndKillSurroundingEntities();
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }

    public void FindAndKillSurroundingEntities()
    {
        if (entityDamage == 0)
            return;

        Vector3 b = new Vector3(0f, this.m_characterController.height / 2f, 0f);
        RaycastHit raycastHit;
        if (Physics.CapsuleCast(this.position - b, this.position + b, this.destructionRadius, this.motion.normalized, out raycastHit, this.motion.magnitude, -1) && raycastHit.collider != null)
        {
            global::RootTransformRefEntity component = raycastHit.collider.gameObject.GetComponent<global::RootTransformRefEntity>();
            if (component)
            {
                global::EntityAlive entityAlive = component.RootTransform.GetComponent<global::Entity>() as global::EntityAlive;
                if (entityAlive != null && entityAlive != this.AttachedEntities && entityAlive.Spawned && !entityAlive.IsDead())
                {
                    int damage = entityDamage;
                    // Doing this in order to properly kill entities. Otherwise they die instantly without animation, and animal corpses disappear.
                    if (entityDamage > entityAlive.Health)
                    {
                        damage = entityAlive.Health;
                    }
                    entityAlive.DamageEntity(new global::DamageSource(global::EnumDamageSourceType.Bullet), damage, false, 3f);
                    float vegDmg = ((((float)entityAlive.GetMaxHealth()) / 3000.0f) * 0.5f) * destroySolidsDegradPerHit;
                    //DamageVehicle(destroySolidsDegradPerHit * 0.7f);
                    DamageVehicle(vegDmg);
                    DebugMsg("FindAndKillSurroundingEntities: " + " | " + entityAlive.EntityName + " | " + entityAlive.GetType().ToString() + " | Vechicle damage = " + vegDmg.ToString("0.0000"));
                }
            }
        }
    }

    public override void OnCollidedWithBlock(global::WorldBase _world, int _clrIdx, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        base.OnCollidedWithBlock(_world, _clrIdx, _blockPos, _blockValue);
        if (!(this.AttachedEntities is global::EntityPlayerLocal))
            return;

        if (!isPlayerOnVehicle)
        {
            InitRefs();
        }

        //if (hasDestructionRadius && _blockPos != lastHitBlockPos && blockDamage != 0 && !this.world.IsLiquidInBounds(new Bounds(this.position + new Vector3(0f, 1.5f, 0f), Vector3.one)))
        if (!isDestroyingBlocks && hasDestructionRadius && _blockPos != lastHitBlockPos && blockDamage != 0)
        {
            player = this.AttachedEntities as global::EntityPlayerLocal;
            isDestroyingBlocks = true;
            FindAndKillSurroundingBlocks(_world, _clrIdx, _blockPos);
            lastHitBlockPos = _blockPos;
        }
    }


    public List<string> terrainBlocks = new List<string> {"grass", "asphalt", "dirt", "gravel", "clay", "sandStone", "clayInSandstone", "sand", "concreteTerrain" };

    public void FindAndKillSurroundingBlocks(global::WorldBase _world, int _clrIdx, global::Vector3i _blockPos)
    {
        Vector3i vehiclePos = _blockPos;
        Vector3i blockPos = vehiclePos;
        for (int i = -destructionRadius; i <= destructionRadius; i++)
        {
            blockPos.x = vehiclePos.x + i;
            for (int j = -destructionRadius; j <= destructionRadius; j++)
            {
                blockPos.z = vehiclePos.z + j;
                //for (int k = 1; k <= destructionHeight; k++)
                //for (int k = destructionHeight; k >= 1; k
                for (int k = currentBucketHeight; k < currentBucketHeight + destructionHeight; k++)
                {
                    blockPos.y = vehiclePos.y + k;
                    BlockValue blockValue = GameManager.Instance.World.GetBlock(blockPos);
                    global::Block block = global::Block.list[blockValue.type];
                    string blockName = block.GetBlockName();

                    // Will need to look into this later, for some reason we can't destroy blocks when in the water, it generates tons of exceptions.
                    //if (blockName.Contains("water") || this.world.IsLiquidInBounds(new Bounds(this.position + new Vector3(0f, 1.5f, 0f), Vector3.one)))
                        //return;

                    if (blockName == "air" || (destroyBlocks.Contains("terrain") ? false : terrainBlocks.Contains(blockName)) || block.GetType().IsSubclassOf(typeof(BlockLiquid)) || block.GetType() == typeof(BlockLiquidv2) || 
                        block.GetType() == typeof(BlockGore))
                        continue;

                    // Destroy Blocks before harvesting them to not re-loop on the same blocks multiple times.
                    if (destroyBlocks.Contains("terrain") && (blockName.Contains("Ground") || terrainBlocks.Contains(blockName) || block.blockMaterial.id == "dirt"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_shovelSteel, destroySolidsDegradPerHit * 0.3f);
                        if (harvestBlocks.Contains("terrain"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_shovelSteel);
                    }
                    if (destroyBlocks.Contains("snow") && (block.blockMaterial.id == "snow"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_shovelSteel, destroySolidsDegradPerHit * 0.1f);
                        if (harvestBlocks.Contains("snow"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_shovelSteel);
                    }
                    else if (destroyBlocks.Contains("tree") && block.GetType() == typeof(BlockModelTree))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("tree"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("cactus") && block.GetType() == typeof(BlockCactus))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, destroySolidsDegradPerHit * 0.2f);
                        if (harvestBlocks.Contains("cactus"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("curb") && (blockName.ToLower().Contains("curb")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("curb"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("tire") && blockName == "tire")
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_clawHammer, destroySolidsDegradPerHit * 0.1f);
                        if (harvestBlocks.Contains("tire"))
                            HarvestOnDestroy(block, blockValue, 0.7f, item_clawHammer);
                    }
                    else if (destroyBlocks.Contains("car") && blockName.Contains("cntCar03"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("car"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("shrubOrBush") && (block.GetType() != typeof(BlockModelTree) && (blockName.Contains("tree") || blockName == "driftwood")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, destroySolidsDegradPerHit * 0.2f);
                        if (harvestBlocks.Contains("shrubOrBush"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("bigBoulder") && (blockName == "gravelPlusIron" || blockName == "gravelPlusLead" || blockName == "gravelPlusCoal" || blockName == "gravelPlusPotassium"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("bigBoulder"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("rock") && (blockName.Contains("rock") || blockName.Contains("cinderBlocks") || blockName.Contains("graveStone") || blockName == "flagstoneHalf" || blockName == "destroyedStone"
                        || blockName == "stone"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("rock"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("plant") && ((block.GetType().IsSubclassOf(typeof(BlockPlant)) && block.GetType() != typeof(BlockModelTree)) || blockName.Contains("mushroom") || blockName == "plantedCornDead"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, 0);
                        if (harvestBlocks.Contains("plant"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel); 
                    }
                    else if (destroyBlocks.Contains("trap") && ((block.GetType().IsSubclassOf(typeof(BlockDamage)) && block.GetType() != typeof(BlockCactus)) || block.GetType() == typeof(BlockMine) || blockName.Contains("barbed")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit * 1.5f);
                        if (harvestBlocks.Contains("trap"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("poleOrPillar") && (blockName.ToLower().Contains("pole") || blockName.Contains("iBeam") || blockName.Contains("Pillar") || blockName.Contains("Support") || blockName.Contains("Cross") || 
                        blockName.Contains("_cross") || blockName.ToLower().Contains("sign")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("poleOrPillar"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("debris") && (blockName == "scrapMetalPile" || blockName == "woodDebris" || blockName.Contains("garbage_decor") || blockName == "hubcapNoMine" || blockName == "hayBaleBlock" || 
                        blockName.Contains("emberPile") || blockName == "cobweb"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("debris"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("rareOres") && (blockName == "ironOre" || blockName == "leadOre" || blockName.Contains("stalactite") || blockName.Contains("stalagmite") || blockName == "potassiumNitrate" ||
                        blockName == "oilDeposit" || blockName == "silverOre" || blockName == "goldOre" || blockName == "diamondOre" || blockName == "coalOre"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("rareOres"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("furniture") && block.GetType() != typeof(BlockLoot) && ((block.blockMaterial.id == "furniture") || block.GetType().IsSubclassOf(typeof(BlockSleepingBag)) || blockName.Contains("signShop") || blockName.ToLower().Contains("table") ||
                        blockName.ToLower().Contains("chair") || blockName == "barStool" || blockName.Contains("storeShelving") || blockName.StartsWith("tv") || blockName.Contains("faucet") || blockName.Contains("painting")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, destroySolidsDegradPerHit * 0.3f);
                        if (harvestBlocks.Contains("furniture"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("devices") && (blockName == "airConditioner" || blockName.ToLower().Contains("controlpanel") || blockName == "shoppingCartEmpty" || blockName == "shoppingBasketEmpty" ||
                        blockName.ToLower().Contains("candle") || blockName.ToLower().Contains("torch") || blockName.ToLower().Contains("light") || blockName == "cashRegisterEmpty" || blockName == "fusebox" || blockName == "speaker"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, destroySolidsDegradPerHit * 0.5f);
                        if (harvestBlocks.Contains("devices"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("fence") && (blockName.ToLower().Contains("fence") || (blockName.ToLower().Contains("wood") && blockName.ToLower().Contains("plate")) || blockName.ToLower().Contains("chainlink") ||
                        blockName == "commercialBlindsBottom" || blockName.ToLower().Contains("corrugatedmetalsheet") || blockName == "corrugatedMetalPlate" || blockName.ToLower().Contains("ironbars") || blockName.Contains("Gate") ||
                        blockName.Contains("camoNet")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, destroySolidsDegradPerHit * 0.5f);
                        if (harvestBlocks.Contains("fence"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("lootCtn") && (block.GetType() == typeof(BlockLoot) || block.GetType().IsSubclassOf(typeof(BlockLoot)) || block.Properties.Contains("LootList") || blockName.Contains("Crate")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, destroySolidsDegradPerHit * 0.3f);
                        if (harvestBlocks.Contains("lootCtn"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("buildings") && ((blockName.ToLower().Contains("wood") && blockName.ToLower().Contains("block")) || (blockName.ToLower().Contains("concrete") && blockName.ToLower().Contains("block")) ||
                        (blockName.ToLower().Contains("brick") && blockName.ToLower().Contains("block")) || blockName.Contains("flagstone") || blockName.Contains("cobblestone") || blockName.Contains("burntWood") ||
                        blockName.Contains("rebar") || blockName.Contains("Ramp") || blockName.Contains("Wedge") || blockName.Contains("CNR") || blockName.ToLower().Contains("plate") || blockName.ToLower().Contains("plug") ||
                        blockName.ToLower().Contains("door") || blockName.ToLower().Contains("glass") || blockName.ToLower().Contains("hatch") || blockName.ToLower().Contains("bridge") || blockName.Contains("sandbag") ||
                        blockName.ToLower().Contains("stairs") || blockName.ToLower().Contains("ladder") || blockName.Contains("pew_segment") || blockName.Contains("metalPipe") || blockName.ToLower().Contains("corrugatedmetal") ||
                        blockName.ToLower().Contains("catwalk") || blockName.ToLower().Contains("railing") || blockName.ToLower().Contains("destroyed") || blockName.Contains("duct") || blockName.ToLower().Contains("scrapiron") ||
                        blockName.ToLower().Contains("steel") || blockName.Contains("conduit") || blockName.ToLower().Contains("arrowslit") || blockName.ToLower().Contains("window") || blockName.ToLower().Contains("blind") ||
                        blockName.Contains("Half") || blockName.ToLower().Contains("awning") || blockName.Contains("curtain") || blockName.Contains("Filler") || blockName.Contains("Trussing")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, destroySolidsDegradPerHit);
                        if (harvestBlocks.Contains("buildings"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    // This is for debug printing remaining blocks that are not yet supported for destruction
                    else
                    {
                        if (Time.time - debugBlocksPrintDelay > lastDebugBlocksPrint)
                        {
                            DebugMsg("block = " + block.GetType().ToString() + " | " + blockName);
                            lastDebugBlocksPrint = Time.time;
                        }
                    }
                }
            }
        }
        isDestroyingBlocks = false;
    }

    public void DamageBlock(global::WorldBase _world, int _clrIdx, global::Vector3i _blockPos, global::Block _block, global::BlockValue _blockValue, ItemValue _miningTool, float _degradPercPerHit)
    {
        int blockDmg = blockDamage;
        if(blockDamage > _block.MaxDamage)
        {
            blockDmg = _block.MaxDamage;
        }
        _block.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, blockDmg, this.AttachedEntities.entityId, true);

        DamageVehicle(_degradPercPerHit);

        ItemActionAttack iat = new ItemActionMelee();
        ItemValue dummy = null;
        float miningToolBlockDamage = iat.GetDamageBlock(_miningTool, player, dummy);
        float blockMaxDamage = _block.MaxDamage;
        int skillExpMult = (int)(blockMaxDamage / miningToolBlockDamage);
        AddSkillExp(player, _miningTool.type, _blockValue.ToItemValue().ItemClass.ActionSkillExp * ((skillExpMult > 0) ? skillExpMult : 1));
    }

    public void DamageVehicle(float _damage)
    {
        if (_damage != 0)
        {
            //float divided = vehicleDurabilityPercent; // ((float)this.Health / this.Stats.Health.Max);
            /*float oneMinus = 1.0f - vehicleDurabilityPercent;
            float oneMinusAdj = oneMinus;
            if (oneMinusAdj < 10f)
            {
                oneMinusAdj = 10f;
            }
            float vehDmg = oneMinusAdj * _damage;*/
            //this.vehicle.TakeDamage(damage >= 1 ? damage : 1, true);
            //float vehDmgAdj = vehDmg;
            /*if (vehDmgAdj < 1.0f)
            {
                vehDmgAdj = 1.0f;
            }*/
            /*if (Time.time - debugBlocksPrintDelay > lastDebugBlocksPrint)
            {
                DebugMsg("Vehicle damage = " + vehDmg.ToString("0.00000") + " (from " + _damage.ToString("0.00000") + "): divided = " + vehicleDurabilityPercent.ToString("0.00000") + " | oneMinus = " + oneMinus.ToString("0.00000") +
                    " | oneMinusAdj = " + oneMinusAdj.ToString("0.00000"));
            }*/

            this.vehicle.TakeDamage(4f, false);
        }
    }


    // Adapted from ItemAction.AddSkillExp()
    public void AddSkillExp(global::EntityPlayer _player, int _itemType, int _exp)
    {
        if (_player == null)
        {
            return;
        }
        if (player.Skills != null)
        {
            global::Skill skill = _player.Skills.GetSkill(_itemType, false);
            global::ItemClass forId = global::ItemClass.GetForId(_itemType);
            if (skill != null && forId != null)
            {
                skill.AddExperience(_exp, _player is global::EntityPlayerLocal, _player.entityId, true);
            }
        }
    }

    // Adaptation of GameUtils.WZ(global::ItemActionAttackData itemActionAttackData, global::ItemValue itemValue, int num, float num2, string text, bool flag = true).  
    // .WZ is obfuscated if will be named different on your machine.
    public void AddHarvestItemToInventory(global::ItemValue itemValue, int num, float num2, string text)
    {
        //string msg = "Harvest: " + itemValue.ItemClass.GetItemName();
        if (itemValue == null || itemValue.ItemClass == null || (itemValue.ItemClass.GetItemName() == "yuccaFibers" && !harvestBlocks.Contains("grass")))
        {
            //DebugMsg(msg += " (NOT HARVESTED)");
            return;
        }
        //DebugMsg(msg);

        if (UnityEngine.Random.value <= num2 && num > 0)
        {
            global::ItemStack itemStack = new global::ItemStack(itemValue, num);
            bool addedToVehicleInv = false;
            if (harvestToVehicleInventory && this.hasStorage())
            {
                addedToVehicleInv = xuiC_VehicleContainer.AddItem(itemStack);
            }
            if (!addedToVehicleInv)
            {
                if (!playerInventory.AddItem(itemStack, true))
                {
                    global::GameManager.Instance.ItemDropServer(new global::ItemStack(itemValue, num), player.GetPosition(), Vector3.zero, player.GetInstanceID(), 60f, false);
                }
            }
            // The harvesting XP is only added when destroyed blocks are also harvested.
            player.AddExp((int)(itemStack.itemValue.ItemClass.MadeOfMaterial.Experience * (float)num));
        }
    }

    // Adaptation of GameUtils.HarvestOnAttack()
    public void HarvestOnDestroy(Block _block, BlockValue _blockValue, float destructionHarvestBonus, ItemValue miningTool)
    {
        /*ItemActionMelee iat = new ItemActionMelee();
        iat.item = miningTool.ItemClass;
        iat.ReadFrom(miningTool.ItemClass.Properties);
        Dictionary<string, global::ItemActionAttack.Bonuses> ToolBonuses = iat.ToolBonuses;*/

        if (_block != null && _block.itemsToDrop != null)
        {
            if (!_block.itemsToDrop.ContainsKey(global::EnumDropEvent.Destroy))
            {
                if (_blockValue.type != 0)
                {
                    global::ItemValue itemValue = _blockValue.ToItemValue();
                    string itemName = global::ItemClass.list[itemValue.type].GetItemName();
                    AddHarvestItemToInventory(itemValue, 1, 1f, itemName);
                }
            }
            else
            {
                List<global::Block.SItemDropProb> list = _block.itemsToDrop[global::EnumDropEvent.Destroy];
                for (int i = 0; i < list.Count; i++)
                {
                    /*float num = 1f;
                    if (list[i].toolCategory != null)
                    {
                        num = 0f;
                        if (ToolBonuses != null && ToolBonuses.ContainsKey(list[i].toolCategory))
                        {
                            num = ToolBonuses[list[i].toolCategory].Tool;
                        }
                    }*/
                    
                    global::ItemValue itemValue2 = (!list[i].name.Equals("*")) ? new global::ItemValue(global::ItemClass.GetItem(list[i].name, false).type, false) : _blockValue.ToItemValue();
                    if (itemValue2.type != 0 && global::ItemClass.list[itemValue2.type] != null && (list[i].prob > 0.999f || UnityEngine.Random.value <= list[i].prob))
                    {
                        //DebugMsg("Mining Tool = " + miningTool.ItemClass.Name + "\n\tDestroy ToolBonuses = " + num.ToString("0.000") + " (" + global::ItemClass.list[itemValue2.type].GetItemName() + ")");

                        int num2 = (int)((float)UnityEngine.Random.Range(list[i].minCount, list[i].maxCount + 1) * destructionHarvestBonus);
                        //int num2 = (int)((float)UnityEngine.Random.Range(list[i].minCount, list[i].maxCount + 1) * num);
                        if (num2 > 0)
                        {
                            AddHarvestItemToInventory(itemValue2, num2, 1f, global::ItemClass.list[itemValue2.type].GetItemName());
                        }
                    }
                }
            }
            if (_block.itemsToDrop.ContainsKey(global::EnumDropEvent.Harvest))
            {
                List<global::Block.SItemDropProb> list2 = _block.itemsToDrop[global::EnumDropEvent.Harvest];
                //string msg = "Destroying " + _block.GetBlockName() + ":\n";
                for (int k = 0; k < list2.Count; k++)
                {
                    /*float num3 = 1f;
                    if (list2[k].toolCategory != null)
                    {
                        num3 = 0f;
                        if (ToolBonuses != null && ToolBonuses.ContainsKey(list2[k].toolCategory))
                        {
                            num3 = ToolBonuses[list2[k].toolCategory].Tool;
                        }
                    }*/
                    // Simulate the multiple harvesting hits of regular harvesting tools
                    /*float blockDamage = _block.MaxDamage;
                    ItemActionAttack iat = new ItemActionMelee();
                    ItemValue dummy = null;
                    float miningToolBlockDamage = iat.GetDamageBlock(miningTool, player, dummy);*/
                    //msg += ("\tTool Block Damage = " + miningToolBlockDamage.ToString() + ":\n");
                    //for (int l = 0; blockDamage > 0 && l < 20; l++)
                    {
                        //msg += ("\tBlock Damage = " + blockDamage.ToString() + ":\n");
                        //blockDamage -= miningToolBlockDamage;

                        global::ItemValue itemValue3 = (!list2[k].name.Equals("*")) ? new global::ItemValue(global::ItemClass.GetItem(list2[k].name, false).type, false) : _blockValue.ToItemValue();
                        if (itemValue3.type != 0 && global::ItemClass.list[itemValue3.type] != null)
                        {
                            //DebugMsg("\tHarvest ToolBonuses = " + num3.ToString("0.000") + " (" + global::ItemClass.list[itemValue3.type].GetItemName() + ")");

                            float num3 = destructionHarvestBonus;
                            player.Skills.ModifyValue(global::Skill.Effects.HarvestCount, ref num3, miningTool.type, false);

                            int num4 = (int)((float)UnityEngine.Random.Range(list2[k].minCount, list2[k].maxCount + 1) * num3);
                            if (num4 > 0)
                            {
                                AddHarvestItemToInventory(itemValue3, num4, list2[k].prob, global::ItemClass.list[itemValue3.type].GetItemName());
                            }
                        }
                    }
                    // High hit on performance
                    //DebugMsg(msg);
                }
            }
            return;
        }
    }

    /*public override bool OnEntityActivated(int _indexInBlockActivationCommands, global::Vector3i _tePos, global::EntityAlive _entityFocusing)
    {
        base.OnEntityActivated(_indexInBlockActivationCommands, _tePos, _entityFocusing);
        return true;
    }*/
}


