using System;
using System.Collections;
using SDX.Payload;
using UnityEngine;


class ItemActionSpawnCustomVehicle : ItemAction
{
    protected string entityToSpawn;
    protected int entityId = -1;
    protected Type entityClass = null;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
        if (_props.Values.ContainsKey("VehicleToSpawn"))
        {
            this.entityToSpawn = _props.Values["VehicleToSpawn"];
        }
        foreach (int v in EntityClass.list.Keys)
        {
            if (EntityClass.list[v].entityClassName == this.entityToSpawn)
            {
                this.entityId = v;
                this.entityClass = EntityClass.list[v].classname;
                //DebugMsg("ItemActionSpawnCustomVehicle.ReadFrom: entityToSpawn = " + this.entityToSpawn + " | ID = " + this.entityId.ToString() + " | class = " + this.entityClass);
                break;
            }
        }
    }


    /*/////////////////////////////////////////////////////////////////////////////////////////
    // All other functions below are for being able to spawn a boat chassis on Water.
    /////////////////////////////////////////////////////////////////////////////////////////*/

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle(_invData, _indexInEntityOfAction);
    }

    public override void StartHolding(ItemActionData _actionData)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)_actionData;
        if (itemActionDataSpawnMinibike.MinibikePreview == null && itemActionDataSpawnMinibike.invData.holdingEntity is EntityPlayerLocal)
        {
            GameObject gameObject = (GameObject)ResourceWrapper.Load1P(itemActionDataSpawnMinibike.invData.holdingEntity.inventory.holdingItem.MeshFile);
            itemActionDataSpawnMinibike.MinibikePreview = UnityEngine.Object.Instantiate<Transform>(gameObject.transform);
            this.BB(_actionData);
        }
    }

    private void BB(ItemActionData itemActionData)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)itemActionData;
        if (itemActionDataSpawnMinibike.PreviewRenderers == null || itemActionDataSpawnMinibike.PreviewRenderers.Length == 0 || itemActionDataSpawnMinibike.PreviewRenderers[0] == null)
        {
            itemActionDataSpawnMinibike.PreviewRenderers = itemActionDataSpawnMinibike.MinibikePreview.GetComponentsInChildren<Renderer>();
        }
        World world = itemActionData.invData.world;
        bool flag = this.UB(itemActionData, ref itemActionDataSpawnMinibike.Position) && world.CanPlaceBlockAt(itemActionDataSpawnMinibike.invData.hitInfo.hit.blockPos, world.GetGameManager().GetPersistentLocalPlayer(), true);

        if (!flag && IsCustomBoat(itemActionData))
        {
            flag = CheckWaterInRange(itemActionData, ref itemActionDataSpawnMinibike.Position);
        }

        if (itemActionDataSpawnMinibike.ValidPosition != flag)
        {
            itemActionDataSpawnMinibike.ValidPosition = flag;
            for (int i = 0; i < itemActionDataSpawnMinibike.PreviewRenderers.Length; i++)
            {
                itemActionDataSpawnMinibike.PreviewRenderers[i].material.color = ((!flag) ? new Color(2f, 0.25f, 0.25f) : new Color(0.25f, 2f, 0.25f));
            }
        }
        Quaternion localRotation = itemActionDataSpawnMinibike.MinibikePreview.localRotation;
        localRotation.eulerAngles = new Vector3(-90f, itemActionData.invData.holdingEntity.rotation.y + 90f, 0f);
        itemActionDataSpawnMinibike.MinibikePreview.localRotation = localRotation;
        itemActionDataSpawnMinibike.MinibikePreview.position = itemActionDataSpawnMinibike.Position;
    }


    public override void CancelAction(ItemActionData _actionData)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)_actionData;
        if (itemActionDataSpawnMinibike.MinibikePreview != null && itemActionDataSpawnMinibike.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnMinibike.MinibikePreview.gameObject);
        }
    }


    public override void StopHolding(ItemActionData _actionData)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)_actionData;
        if (itemActionDataSpawnMinibike.MinibikePreview != null && itemActionDataSpawnMinibike.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnMinibike.MinibikePreview.gameObject);
        }
    }


    public bool IsCustomBoat(ItemActionData _actionData)
    {
        if (this.entityId < 0)
        {
            foreach (int v in EntityClass.list.Keys)
            {
                if (EntityClass.list[v].entityClassName == this.entityToSpawn)
                {
                    //this.entityId = v;
                    this.entityClass = EntityClass.list[v].classname;
                    break;
                }
            }
        }
        //DebugMsg("ItemActionSpawnCustomVehicle.ReadFrom: entityToSpawn = " + this.entityToSpawn + " | ID = " + this.entityId.ToString() + " | class = " + this.entityClass);

        if (this.entityClass == typeof(EntityCustomBoat))
        {
            //DebugMsg("It's a Boat!");
            return true;
        }
        //DebugMsg("NOT a Boat!");
        return false;
    }


    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)_actionData;
        if (itemActionDataSpawnMinibike.MinibikePreview != null && itemActionDataSpawnMinibike.invData.holdingEntity is EntityPlayerLocal)
        {
            this.BB(_actionData);
        }
    }

    private bool CheckWaterInRange(ItemActionData itemActionData, ref Vector3 ptr)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)itemActionData;
        Ray lookRay = itemActionDataSpawnMinibike.invData.holdingEntity.GetLookRay();
        //if (Voxel.Raycast(GameManager.Instance.World, lookRay, 5f, 65536, 64, 0f))
        if (Voxel.Raycast(GameManager.Instance.World, lookRay, 10f, false, true))
        {
            ptr = Voxel.voxelRayHitInfo.hit.pos;
            Vector3 vector = new Vector3(ptr.x + 0.5f, ptr.y, ptr.z + 0.5f);

            BlockValue valor = GameManager.Instance.World.GetBlock(new Vector3i(vector));
            Block block = Block.list[valor.type];
            if (block.GetType().IsSubclassOf(typeof(BlockLiquid)) || block.GetType() == typeof(BlockLiquidv2))
            {
                //DebugMsg("Hitting Liquid Block");
                // needs to check if there's air on top of water
                valor = GameManager.Instance.World.GetBlock(new Vector3i(vector.x, vector.y + 1, vector.z));
                block = Block.list[valor.type];
                if (valor.type == 0)
                {
                    //DebugMsg("Liquid Block has Air on top");
                    ptr = vector;
                    return true;
                }
            }

        }
        return false;
    }


    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)_actionData;
        if (!(_actionData.invData.holdingEntity is EntityPlayerLocal))
        {
            return;
        }
        if (!_bReleased)
        {
            return;
        }
        if (Time.time - _actionData.lastUseTime < this.Delay)
        {
            return;
        }
        if (Time.time - _actionData.lastUseTime < Constants.cBuildIntervall)
        {
            return;
        }
        if (!itemActionDataSpawnMinibike.ValidPosition)
        {
            return;
        }
        ItemInventoryData invData = _actionData.invData;
        if (this.entityId < 0)
        {
            foreach (int v in EntityClass.list.Keys)
            {
                if (EntityClass.list[v].entityClassName == this.entityToSpawn)
                {
                    this.entityId = v;
                    break;
                }
            }
            if (this.entityId == 0)
            {
                return;
            }
        }
        if (!Steam.Network.IsServer)
        {
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(new NetPackageVehicleSpawn(this.entityId, itemActionDataSpawnMinibike.Position, new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f), invData.holdingEntity.inventory.holdingItemItemValue.Clone(), _actionData.invData.holdingEntity.entityId), true);
        }
        else
        {
            EntityVehicle entityVehicle = (EntityVehicle)EntityFactory.CreateEntity(this.entityId, itemActionDataSpawnMinibike.Position, new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f));
            entityVehicle.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
            Vehicle vehicle = entityVehicle.GetVehicle();
            string tag = "chassis";
            if (invData.holdingEntity.inventory.holdingItem != null)
            {
                tag = invData.holdingEntity.inventory.holdingItem.VehicleSlotType;
            }
            vehicle.SetPartInSlot(tag, invData.holdingEntity.inventory.holdingItemItemValue.Clone());
            vehicle.SaveVehiclePartsToInventory();
            entityVehicle.SetOwner(GamePrefs.GetString(EnumGamePrefs.PlayerId));
            GameManager.Instance.World.SpawnEntityInWorld(entityVehicle);
        }
        invData.holdingEntity.RightArmAnimationUse = true;
        GameManager.Instance.StartCoroutine(this.EB(invData, invData.holdingEntity.inventory.holdingItemIdx));
    }


    private IEnumerator EB(ItemInventoryData itemInventoryData, int idx)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        ItemStack itemStack = itemInventoryData.holdingEntity.inventory.GetItem(idx).Clone();
        if (itemStack.count > 0)
        {
            itemStack.count--;
        }

        itemInventoryData.holdingEntity.inventory.SetItem(idx, itemStack);
        itemInventoryData.holdingEntity.PlayOneShot((this.soundStart == null) ? "placeblock" : this.soundStart);
        yield break;
    }


    private bool UB(ItemActionData itemActionData, ref Vector3 ptr)
    {
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = (ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle)itemActionData;
        Ray lookRay = itemActionDataSpawnMinibike.invData.holdingEntity.GetLookRay();
        if (Voxel.Raycast(GameManager.Instance.World, lookRay, 5f, 65536, 64, 0f))
        {
            ptr = Voxel.voxelRayHitInfo.hit.pos;
            Vector3 vector = new Vector3(ptr.x + 0.5f, ptr.y, ptr.z + 0.5f);
            return GameManager.Instance.World.GetBlock(new Vector3i(vector)).type == 0 && GameManager.Instance.World.GetBlock(new Vector3i(vector + Vector3.down)).type != 0 && GameManager.Instance.World.GetBlock(new Vector3i(vector + Vector3.up)).type == 0;
        }
        return false;
    }


    public override void Cleanup(ItemActionData _data)
    {
        base.Cleanup(_data);
        ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle itemActionDataSpawnMinibike = _data as ItemActionSpawnCustomVehicle.ItemActionDataSpawnCustomVehicle;
        if (itemActionDataSpawnMinibike != null && itemActionDataSpawnMinibike.MinibikePreview != null && itemActionDataSpawnMinibike.invData != null && itemActionDataSpawnMinibike.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnMinibike.MinibikePreview.gameObject);
        }
    }


    protected class ItemActionDataSpawnCustomVehicle : ItemActionAttackData
    {
        public ItemActionDataSpawnCustomVehicle(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
        {
        }

        public Transform MinibikePreview;

        public Renderer[] PreviewRenderers;

        public bool ValidPosition;

        public Vector3 Position;
    }
}


