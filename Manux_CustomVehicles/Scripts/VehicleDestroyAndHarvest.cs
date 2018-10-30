using System;
using System.Collections.Generic;
using UnityEngine;


public class VehicleDestroyAndHarvest
{
    EntityCustomVehicle entityVehicle = null;

    public float vehicleDurabilityMax;
    public float vehicleDurabilityPercent;

    public int entityDamage = 0;
    public float entityHitMinSpeed = 5f;
    public float entityCriticalHitMinSpeed = 15f;
    public int blockDamage = 0;
    public float vehicleDamageFactor_blocks = 4.0f;
    public float vehicleDamageFactor_entities = 4.0f;

    public int destructionRadius = 0;
    public int destructionHeight = 2;
    public float destructionHarvestBonus = 1.0f;

    public List<string> destroyBlocks = new List<string>() {  };
    public List<string> harvestBlocks = new List<string>() {  };
    public bool harvestToVehicleInventory = false;

    public ItemValue item_fireAxeSteel = ItemClass.GetItem("fireaxeSteel", false);
    public ItemValue item_pickAxeSteel = ItemClass.GetItem("pickaxeSteel", false);
    public ItemValue item_shovelSteel = ItemClass.GetItem("shovelSteel", false);
    public ItemValue item_clawHammer = ItemClass.GetItem("clawHammer", false);
    public ItemValue item_wrench = ItemClass.GetItem("wrench", false);

    public bool isDestroyingBlocks;

    public float debugBlocksPrintDelay = 10.0f;
    public float lastDebugBlocksPrint = -1;

    public float destroyXPFactor = 1f;
    public float harvestXPFactor = 1f;

    public float entityHitAgainDelay = 2f;
    public float lastEntityHitTime = -1;

    public float entityHitSpeedRatio;
    public int entityHitDamage;
    public int lastHitEntityId;
    public float lastControllerVelocityMagnitude;

    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public VehicleDestroyAndHarvest(EntityCustomVehicle _entityVehicle)
    {
        this.entityVehicle = _entityVehicle;
    }

    public void CopyPropertiesFromEntityClass(EntityClass entityClass)
    {
        if (entityClass.Properties.Values.ContainsKey("EntityDamage"))
        {
            int entityDmg;
            if (int.TryParse(entityClass.Properties.Values["EntityDamage"], out entityDmg))
            {
                DebugMsg("\tentityDamage = " + entityDmg.ToString("0.0000"));
                entityDamage = entityDmg;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("EntityHitMinSpeed"))
        {
            float minHitSpeed;
            if (float.TryParse(entityClass.Properties.Values["EntityHitMinSpeed"], out minHitSpeed))
            {
                DebugMsg("\tentityHitMinSpeed = " + minHitSpeed.ToString("0.0000"));
                entityHitMinSpeed = minHitSpeed;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("EntityCriticalHitMinSpeed"))
        {
            float minHitSpeed;
            if (float.TryParse(entityClass.Properties.Values["EntityCriticalHitMinSpeed"], out minHitSpeed))
            {
                DebugMsg("\tentityCriticalHitMinSpeed = " + minHitSpeed.ToString("0.0000"));
                entityCriticalHitMinSpeed = minHitSpeed;
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
        if (entityClass.Properties.Values.ContainsKey("VehicleDamageFactor_blocks"))
        {
            float vehDmgFactor;
            if (float.TryParse(entityClass.Properties.Values["VehicleDamageFactor_blocks"], out vehDmgFactor))
            {
                DebugMsg("\tvehicleDamageFactor_blocks = " + vehDmgFactor.ToString("0.0000"));
                vehicleDamageFactor_blocks = vehDmgFactor;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("VehicleDamageFactor_entities"))
        {
            float vehDmgFactor;
            if (float.TryParse(entityClass.Properties.Values["VehicleDamageFactor_entities"], out vehDmgFactor))
            {
                DebugMsg("\tvehicleDamageFactor_entities = " + vehDmgFactor.ToString("0.0000"));
                vehicleDamageFactor_entities = vehDmgFactor;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("DestructionRadius"))
        {
            int destRadius;
            if (int.TryParse(entityClass.Properties.Values["DestructionRadius"], out destRadius))
            {
                DebugMsg("\tdestructionRadius = " + destRadius.ToString("0.000"));
                destructionRadius = destRadius;
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
        if (entityClass.Properties.Values.ContainsKey("DestroyXPFactor"))
        {
            float destXPFactor;
            if (float.TryParse(entityClass.Properties.Values["DestroyXPFactor"], out destXPFactor))
            {
                DebugMsg("\tdestroyXPFactor = " + destXPFactor.ToString("0.000"));
                destroyXPFactor = destXPFactor;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("HarvestXPFactor"))
        {
            float hrvstXPFactor;
            if (float.TryParse(entityClass.Properties.Values["HarvestXPFactor"], out hrvstXPFactor))
            {
                DebugMsg("\tharvestXPFactor = " + hrvstXPFactor.ToString("0.000"));
                harvestXPFactor = hrvstXPFactor;
            }
        }
    }

    public void UpdateVehicleDestructionQuality()
    {
        vehicleDurabilityMax = entityVehicle.GetVehicle().GetVehicleMaxDurability();
        vehicleDurabilityPercent = entityVehicle.GetVehicle().GetVehicleDurabilityPercentage();

        // Convert the current Vehicle Health as the Quality of the dummy harvesting tools.
        int loaderAsMiningToolQuality = Mathf.RoundToInt(vehicleDurabilityPercent * 600);
        item_fireAxeSteel.Quality = loaderAsMiningToolQuality;
        item_pickAxeSteel.Quality = loaderAsMiningToolQuality;
        item_shovelSteel.Quality = loaderAsMiningToolQuality;
        item_clawHammer.Quality = loaderAsMiningToolQuality;
        item_wrench.Quality = loaderAsMiningToolQuality;
    }

    public void FindAndKillSurroundingEntities()
    {
            //if (entityDamage == 0 || entityVehicle.lastControllerVelocityMagnitude < 5f || Time.time - entityHitAgainDelay < lastEntityHitTime)
            if (entityDamage == 0 || lastControllerVelocityMagnitude < entityHitMinSpeed)
            {
                //DebugMsg("NOT damaging entity: entityDamage = " + entityDamage.ToString() + " | lastControllerVelocityMagnitude = " + entityVehicle.lastControllerVelocityMagnitude.ToString("0.000"));
                return;
            }
            else
            {
                //DebugMsg("lastControllerVelocityMagnitude = " + entityVehicle.lastControllerVelocityMagnitude.ToString("0.000"));
            }

        // Try-catch for now because of an error with Bandits and survivors
        try
        {
            Vector3 b = new Vector3(0f, entityVehicle.m_characterController.height / 2f, 0f);
            RaycastHit raycastHit;
            //if (Physics.CapsuleCast(entityVehicle.position - b, entityVehicle.position + b, destructionRadius, entityVehicle.motion.normalized, out raycastHit, entityVehicle.motion.magnitude, -1) && raycastHit.collider != null)
            if (Physics.CapsuleCast(entityVehicle.position - b, entityVehicle.position + b, destructionRadius, entityVehicle.motion.normalized, out raycastHit, destructionRadius + 1, -1) && raycastHit.collider != null)
            {
                RootTransformRefEntity component = raycastHit.collider.gameObject.GetComponent<RootTransformRefEntity>();
                if (component)
                {
                    EntityAlive entityAlive = component.RootTransform.GetComponent<Entity>() as EntityAlive;
                    //if (entityAlive != null && entityAlive != entityVehicle.AttachedEntities && entityAlive.Spawned && !entityAlive.IsDead())
                    if (entityAlive != null && entityAlive != entityVehicle.AttachedEntities && !entityAlive.IsDead() && !(entityAlive.entityId == lastHitEntityId && Time.time - entityHitAgainDelay < lastEntityHitTime))
                    {
                        int damage = entityDamage;
                        entityHitSpeedRatio = CustomVehiclesUtils.GetRatio(Mathf.Clamp(lastControllerVelocityMagnitude, entityHitMinSpeed, 18f), entityHitMinSpeed, 18f) + 1f;
                        entityHitDamage = Mathf.RoundToInt((float)damage * entityHitSpeedRatio);
                        bool isCritical = lastControllerVelocityMagnitude > entityCriticalHitMinSpeed;
                        // Doing this in order to properly kill entities. Otherwise they die instantly without animation, and animal corpses disappear.
                        if (entityHitDamage > entityAlive.Health)
                        {
                            entityHitDamage = entityAlive.Health;
                            isCritical = true;
                        }
                        DamageSourceEntity damageSourceEntity;
                        if (isCritical)
                        {
                            damageSourceEntity = new DamageSourceEntity(EnumDamageSourceType.Melee, entityVehicle.player.entityId, -raycastHit.normal);
                        }
                        else
                        {
                            damageSourceEntity = new DamageSourceEntity(EnumDamageSourceType.Melee, entityVehicle.player.entityId, -raycastHit.normal, raycastHit.transform.name, raycastHit.point, Vector2.zero);
                        }
                        //DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSourceType.Melee, entityVehicle.player.entityId, -raycastHit.normal, raycastHit.transform.name, raycastHit.point, Vector2.zero);
                        lastHitEntityId = entityAlive.entityId;
                        entityAlive.DamageEntity(damageSourceEntity, entityHitDamage, isCritical, isCritical? 3f : 2f);

                        DebugMsg("Damage Entity: " + entityAlive.entityId + " | lastControllerVelocityMagnitude = " + lastControllerVelocityMagnitude.ToString("0.000") + " (hit speed ratio = " + entityHitSpeedRatio.ToString("0.000") 
                            + ") | damage = " + entityHitDamage.ToString("0.000") + " (" + entityAlive.Health.ToString() + "/" + entityAlive.GetMaxHealth().ToString() + ") | hit transform name = " + raycastHit.transform.name + " | critical = " + isCritical.ToString());

                        float vehDmg = (((float)entityAlive.GetMaxHealth()) / 3000.0f) * vehicleDamageFactor_entities * entityHitSpeedRatio;
                        DamageVehicle(vehDmg, 2f);
                        lastEntityHitTime = Time.time;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }

    public List<string> terrainBlocks = new List<string> { "grass", "asphalt", "dirt", "gravel", "clay", "sandStone", "clayInSandstone", "sand", "concreteTerrain" };

    public void FindAndKillSurroundingBlocks(WorldBase _world, int _clrIdx, Vector3i _blockPos)
    {
        Vector3i vehiclePos = _blockPos;
        Vector3i blockPos = vehiclePos;
        for (int i = -destructionRadius; i <= destructionRadius; i++)
        {
            blockPos.x = vehiclePos.x + i;
            for (int j = -destructionRadius; j <= destructionRadius; j++)
            {
                blockPos.z = vehiclePos.z + j;
                for (int k = entityVehicle.currentDestroyHeight; k < entityVehicle.currentDestroyHeight + destructionHeight; k++)
                {
                    blockPos.y = vehiclePos.y + k;
                    BlockValue blockValue = GameManager.Instance.World.GetBlock(blockPos);
                    Block block = Block.list[blockValue.type];
                    string blockName = block.GetBlockName();

                    /*if (blockName == "air" || (destroyBlocks.Contains("terrain") ? false : terrainBlocks.Contains(blockName)) || block.GetType().IsSubclassOf(typeof(BlockLiquid)) || block.GetType() == typeof(BlockLiquidv2) ||
                        block.GetType() == typeof(BlockGore))*/
                    if (blockName == "air" || block.GetType().IsSubclassOf(typeof(BlockLiquid)) || block.GetType() == typeof(BlockLiquidv2) || block.GetType() == typeof(BlockGore))
                        continue;

                    // Destroy Blocks before harvesting them to not re-loop on the same blocks multiple times.
                    if (destroyBlocks.Contains("plant") && ((block.GetType().IsSubclassOf(typeof(BlockPlant)) && block.GetType() != typeof(BlockModelTree)) || blockName.Contains("mushroom") || blockName == "plantedCornDead" || 
                        block.GetType() == typeof(BlockDeadgrass)))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks * 0.1f);
                        if (harvestBlocks.Contains("plant"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("terrain") && (blockName.Contains("Ground") || terrainBlocks.Contains(blockName) || block.blockMaterial.id == "dirt" || block.blockMaterial.id == "snow"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_shovelSteel, vehicleDamageFactor_blocks * 0.6f);
                        if (harvestBlocks.Contains("terrain"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_shovelSteel);
                    }
                    else if (destroyBlocks.Contains("cactus") && block.GetType() == typeof(BlockCactus))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks * 0.7f);
                        if (harvestBlocks.Contains("cactus"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("tree") && block.GetType() == typeof(BlockModelTree))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("tree"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("curb") && (blockName.ToLower().Contains("curb")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("curb"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("tire") && blockName == "tire")
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_clawHammer, vehicleDamageFactor_blocks * 0.1f);
                        if (harvestBlocks.Contains("tire"))
                            HarvestOnDestroy(block, blockValue, 0.7f, item_clawHammer);
                    }
                    else if (destroyBlocks.Contains("car") && blockName.Contains("cntCar03"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, vehicleDamageFactor_blocks * 1.5f);
                        if (harvestBlocks.Contains("car"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("shrubOrBush") && (block.GetType() != typeof(BlockModelTree) && (blockName.Contains("tree") || blockName.StartsWith("driftwood"))))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks * 0.6f);
                        if (harvestBlocks.Contains("shrubOrBush"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("bigBoulder") && (blockName == "gravelPlusIron" || blockName == "gravelPlusLead" || blockName == "gravelPlusCoal" || blockName == "gravelPlusPotassium"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("bigBoulder"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("rock") && (blockName.Contains("rock") || blockName.Contains("graveStone") || blockName == "flagstoneHalf" || blockName == "destroyedStone"
                        || blockName == "stone"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("rock"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("trap") && ((block.GetType().IsSubclassOf(typeof(BlockDamage)) && block.GetType() != typeof(BlockCactus)) || block.GetType() == typeof(BlockMine) || blockName.Contains("barbed")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks * 1.5f);
                        if (harvestBlocks.Contains("trap"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("poleOrPillar") && (blockName.ToLower().Contains("pole") || blockName.Contains("iBeam") || blockName.Contains("Pillar") || blockName.Contains("Support") || blockName.Contains("Cross") ||
                        blockName.Contains("_cross") || blockName.ToLower().Contains("sign")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("poleOrPillar"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("softDebris") && (blockName.Contains("garbage_decor") || blockName == "hayBaleBlock" || blockName == "cobweb"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks * 0.1f);
                        if (harvestBlocks.Contains("softDebris"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("hardDebris") && (blockName == "woodDebris" || blockName == "hubcapNoMine" || blockName.Contains("emberPile") || blockName == "scrapMetalPile" || 
                        blockName.Contains("cinderBlocks")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
                        if (harvestBlocks.Contains("hardDebris"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("rareOres") && (blockName == "ironOre" || blockName == "leadOre" || blockName.Contains("stalactite") || blockName.Contains("stalagmite") || blockName == "potassiumNitrate" ||
                        blockName == "oilDeposit" || blockName == "silverOre" || blockName == "goldOre" || blockName == "diamondOre" || blockName == "coalOre"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks * 1.2f);
                        if (harvestBlocks.Contains("rareOres"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_pickAxeSteel);
                    }
                    else if (destroyBlocks.Contains("furniture") && block.GetType() != typeof(BlockLoot) && ((block.blockMaterial.id == "furniture") || block.GetType().IsSubclassOf(typeof(BlockSleepingBag)) || blockName.Contains("signShop") || blockName.ToLower().Contains("table") ||
                        blockName.ToLower().Contains("chair") || blockName == "barStool" || blockName.Contains("storeShelving") || blockName.StartsWith("tv") || blockName.Contains("faucet") || blockName.Contains("painting")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, vehicleDamageFactor_blocks * 0.5f);
                        if (harvestBlocks.Contains("furniture"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("devices") && (blockName == "airConditioner" || blockName.ToLower().Contains("controlpanel") || blockName == "shoppingCartEmpty" || blockName == "shoppingBasketEmpty" ||
                        blockName.ToLower().Contains("candle") || blockName.ToLower().Contains("torch") || blockName.ToLower().Contains("light") || blockName == "cashRegisterEmpty" || blockName == "fusebox" || blockName == "speaker"))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_wrench, vehicleDamageFactor_blocks * 0.5f);
                        if (harvestBlocks.Contains("devices"))
                            HarvestOnDestroy(block, blockValue, 1.0f, item_wrench);
                    }
                    else if (destroyBlocks.Contains("fenceOrDoor") && (blockName.ToLower().Contains("fence") || blockName.ToLower().Contains("door") || blockName.ToLower().Contains("window") || 
                        blockName.ToLower().Contains("blind") || blockName.Contains("curtain") || (blockName.ToLower().Contains("wood") && blockName.ToLower().Contains("plate")) || blockName.ToLower().Contains("chainlink") ||
                        blockName == "commercialBlindsBottom" || blockName.ToLower().Contains("corrugatedmetalsheet") || blockName == "corrugatedMetalPlate" || blockName.ToLower().Contains("ironbars") || blockName.Contains("Gate") ||
                        blockName.Contains("camoNet") || blockName.ToLower().Contains("railing") || blockName.ToLower().Contains("ladder") || blockName.ToLower().Contains("awning") || blockName.ToLower().Contains("glass")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks * 0.8f);
                        if (harvestBlocks.Contains("fenceOrDoor"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("lootCtn") && (block.GetType() == typeof(BlockLoot) || block.GetType().IsSubclassOf(typeof(BlockLoot)) || block.Properties.Contains("LootList") || blockName.Contains("Crate")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_fireAxeSteel, vehicleDamageFactor_blocks * 0.5f);
                        if (harvestBlocks.Contains("lootCtn"))
                            HarvestOnDestroy(block, blockValue, destructionHarvestBonus, item_fireAxeSteel);
                    }
                    else if (destroyBlocks.Contains("buildings") && ((blockName.ToLower().Contains("wood") && blockName.ToLower().Contains("block")) || (blockName.ToLower().Contains("concrete") && blockName.ToLower().Contains("block")) ||
                        (blockName.ToLower().Contains("brick") && blockName.ToLower().Contains("block")) || blockName.Contains("flagstone") || blockName.Contains("cobblestone") || blockName.Contains("burntWood") ||
                        blockName.Contains("rebar") || blockName.Contains("Ramp") || blockName.Contains("Wedge") || blockName.Contains("CNR") || blockName.ToLower().Contains("plate") || blockName.ToLower().Contains("plug") ||
                        blockName.ToLower().Contains("hatch") || blockName.ToLower().Contains("bridge") || blockName.Contains("sandbag") || blockName.ToLower().Contains("stairs") || blockName.Contains("pew_segment") || 
                        blockName.Contains("metalPipe") || blockName.ToLower().Contains("corrugatedmetal") || blockName.ToLower().Contains("catwalk") || blockName.ToLower().Contains("destroyed") || blockName.Contains("duct") || 
                        blockName.ToLower().Contains("scrapiron") || blockName.ToLower().Contains("steel") || blockName.Contains("conduit") || blockName.ToLower().Contains("arrowslit") || blockName.Contains("Half") || 
                        blockName.Contains("Filler") || blockName.Contains("Trussing")))
                    {
                        DamageBlock(_world, _clrIdx, blockPos, block, blockValue, item_pickAxeSteel, vehicleDamageFactor_blocks);
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

    public void DamageBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, Block _block, BlockValue _blockValue, ItemValue _miningTool, float _vehicleDamage)
    {
        int blockDmg = blockDamage;
        if (blockDamage > _block.MaxDamage)
        {
            blockDmg = _block.MaxDamage;
        }
        //DebugMsg("Damage Block: motion.magnitude = " + entityVehicle.motion.magnitude.ToString("0.000") + " | damage = " + Mathf.RoundToInt(blockDmg * entityVehicle.motion.magnitude).ToString("0.000"));
        //DebugMsg("Damage Block: motion.magnitude = " + entityVehicle.motion.magnitude.ToString("0.000") + " | damage = " + blockDmg.ToString());
        //DebugMsg("Damage Block: " + _block.GetBlockName() + " (" + _block.GetType().ToString() + ") | damage = " + blockDmg.ToString());
        _block.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, blockDmg, entityVehicle.AttachedEntities.entityId, true);
        //_block.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, Mathf.RoundToInt(blockDmg * entityVehicle.motion.magnitude), entityVehicle.AttachedEntities.entityId, true);

        //DebugMsg("Vehicle damage = " + _vehicleDamage);
        DamageVehicle(_vehicleDamage, 0f);

        ItemActionAttack iat = new ItemActionMelee();
        ItemValue dummy = null;
        float miningToolBlockDamage = iat.GetDamageBlock(_miningTool, entityVehicle.player, dummy);
        float blockMaxDamage = _block.MaxDamage;
        int skillExpMult = Mathf.RoundToInt(blockMaxDamage / miningToolBlockDamage);
        AddSkillExp(entityVehicle.player, _miningTool.type, (_blockValue.ToItemValue().ItemClass.ActionSkillExp * ((skillExpMult > 0) ? skillExpMult : 1)));
    }

    public void DamageVehicle(float _damage, float _minDamage)
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

            //entityVehicle.GetVehicle().TakeDamage(4f, false);
            //entityVehicle.GetVehicle().TakeDamage(0.1f, false);
            //entityVehicle.GetVehicle().TakeDamage(1f, false);
            //entityVehicle.GetVehicle().TakeDamage(3f, false);
            //entityVehicle.GetVehicle().TakeDamage(vehicleDamageFactor, false);
            //entityVehicle.GetVehicle().TakeDamage(_damage, false);
            //entityVehicle.GetVehicle().TakeDamage(Mathf.Round(_damage), false);
            float adjustedDamage = Mathf.Clamp(_damage, _minDamage, 100000f);
            //DebugMsg("Vehicle damage = " + _damage.ToString("0.000") + " | adjusted = " + adjustedDamage.ToString("0.000") + " | rounded = " + Mathf.Round(adjustedDamage).ToString("0.000"));
            entityVehicle.GetVehicle().TakeDamage(Mathf.Round(adjustedDamage), false);
        }
    }


    // Adapted from ItemAction.AddSkillExp()
    public void AddSkillExp(EntityPlayer _player, int _itemType, int _exp)
    {
        if (_player == null || destroyXPFactor == 0)
        {
            return;
        }
        if (entityVehicle.player.Skills != null)
        {
            Skill skill = _player.Skills.GetSkill(_itemType, false);
            ItemClass forId = ItemClass.GetForId(_itemType);
            if (skill != null && forId != null)
            {
                skill.AddExperience(Mathf.RoundToInt((float)_exp * destroyXPFactor), _player is EntityPlayerLocal, _player.entityId, true);
            }
        }
    }

    // Adaptation of GameUtils.WZ(ItemActionAttackData itemActionAttackData, ItemValue itemValue, int num, float num2, string text, bool flag = true).  
    // .WZ is obfuscated if will be named different on your machine.
    public void AddHarvestItemToInventory(ItemValue itemValue, int num, float num2, string text)
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
            ItemStack itemStack = new ItemStack(itemValue, num);
            bool addedToVehicleInv = false;
            if (harvestToVehicleInventory && entityVehicle.HasStorage())
            {
                addedToVehicleInv = entityVehicle.xuiC_VehicleContainer.AddItem(itemStack);
            }
            if (!addedToVehicleInv)
            {
                if (!entityVehicle.playerInventory.AddItem(itemStack, true))
                {
                    GameManager.Instance.ItemDropServer(new ItemStack(itemValue, num), entityVehicle.player.GetPosition(), Vector3.zero, entityVehicle.player.GetInstanceID(), 60f, false);
                }
            }
            // The harvesting XP is only added when destroyed blocks are also harvested and when HarvestXPFactor xml property > 0.
            if (harvestXPFactor != 0)
            {
                entityVehicle.player.AddExp(Mathf.RoundToInt(itemStack.itemValue.ItemClass.MadeOfMaterial.Experience * (float)num * harvestXPFactor));
            }
        }
    }

    // Adaptation of GameUtils.HarvestOnAttack()
    public void HarvestOnDestroy(Block _block, BlockValue _blockValue, float destructionHarvestBonus, ItemValue miningTool)
    {
        /*ItemActionMelee iat = new ItemActionMelee();
        iat.item = miningTool.ItemClass;
        iat.ReadFrom(miningTool.ItemClass.Properties);
        Dictionary<string, ItemActionAttack.Bonuses> ToolBonuses = iat.ToolBonuses;*/

        if (_block != null && _block.itemsToDrop != null)
        {
            if (!_block.itemsToDrop.ContainsKey(EnumDropEvent.Destroy))
            {
                if (_blockValue.type != 0)
                {
                    ItemValue itemValue = _blockValue.ToItemValue();
                    string itemName = ItemClass.list[itemValue.type].GetItemName();
                    AddHarvestItemToInventory(itemValue, 1, 1f, itemName);
                }
            }
            else
            {
                List<Block.SItemDropProb> list = _block.itemsToDrop[EnumDropEvent.Destroy];
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

                    ItemValue itemValue2 = (!list[i].name.Equals("*")) ? new ItemValue(ItemClass.GetItem(list[i].name, false).type, false) : _blockValue.ToItemValue();
                    if (itemValue2.type != 0 && ItemClass.list[itemValue2.type] != null && (list[i].prob > 0.999f || UnityEngine.Random.value <= list[i].prob))
                    {
                        //DebugMsg("Mining Tool = " + miningTool.ItemClass.Name + "\n\tDestroy ToolBonuses = " + num.ToString("0.000") + " (" + ItemClass.list[itemValue2.type].GetItemName() + ")");

                        int num2 = Mathf.RoundToInt((float)UnityEngine.Random.Range(list[i].minCount, list[i].maxCount + 1) * destructionHarvestBonus);
                        //int num2 = (int)((float)UnityEngine.Random.Range(list[i].minCount, list[i].maxCount + 1) * num);
                        if (num2 > 0)
                        {
                            AddHarvestItemToInventory(itemValue2, num2, 1f, ItemClass.list[itemValue2.type].GetItemName());
                        }
                    }
                }
            }
            if (_block.itemsToDrop.ContainsKey(EnumDropEvent.Harvest))
            {
                List<Block.SItemDropProb> list2 = _block.itemsToDrop[EnumDropEvent.Harvest];
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

                        ItemValue itemValue3 = (!list2[k].name.Equals("*")) ? new ItemValue(ItemClass.GetItem(list2[k].name, false).type, false) : _blockValue.ToItemValue();
                        if (itemValue3.type != 0 && ItemClass.list[itemValue3.type] != null)
                        {
                            //DebugMsg("\tHarvest ToolBonuses = " + num3.ToString("0.000") + " (" + ItemClass.list[itemValue3.type].GetItemName() + ")");

                            float num3 = destructionHarvestBonus;
                            entityVehicle.player.Skills.ModifyValue(Skill.Effects.HarvestCount, ref num3, miningTool.type, false);

                            int num4 = Mathf.RoundToInt((float)UnityEngine.Random.Range(list2[k].minCount, list2[k].maxCount + 1) * num3);
                            if (num4 > 0)
                            {
                                AddHarvestItemToInventory(itemValue3, num4, list2[k].prob, ItemClass.list[itemValue3.type].GetItemName());
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
}

