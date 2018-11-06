using System;
using System.Collections;
using SDX.Payload;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class ItemActionSpawnHorseHero : ItemAction
{
    // Token: 0x0600019D RID: 413 RVA: 0x00012240 File Offset: 0x00011240
    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)_actionData;
        if (_actionData.invData.holdingEntity is EntityPlayerLocal)
        {
            if (_bReleased)
            {
                if (Time.time - _actionData.lastUseTime >= this.Delay)
                {
                    if (Time.time - _actionData.lastUseTime >= Constants.cBuildIntervall)
                    {
                        if (itemAction.ValidPosition)
                        {
                            if (this.entityId < 0)
                            {
                                foreach (int v in EntityClass.list.Keys)
                                {
                                    if (!(EntityClass.list[v].entityClassName != this.entityToSpawn))
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
                            ItemInventoryData invData = _actionData.invData;
                            if (!Steam.Network.IsServer)
                            {
                                //SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(new NPSpawnVehicleReq(this.entityId, itemAction.Position, new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f), invData.holdingEntity.inventory.holdingItemItemValue.Clone(), _actionData.invData.holdingEntity.entityId), true);
                            }
                            else
                            {
                                EntityAlive horse = (EntityAlive)EntityFactory.CreateEntity(this.entityId, itemAction.Position, new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f));
                                horse.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
                                //horse.SetOwner(GamePrefs.GetString(EnumGamePrefs.PlayerId), _actionData.invData.holdingEntity.entityId);
                                GameManager.Instance.World.SpawnEntityInWorld(horse);
                            }
                            invData.holdingEntity.RightArmAnimationUse = true;
                            GameManager.Instance.StartCoroutine(this.RemoveFromInv(invData, invData.holdingEntity.inventory.holdingItemIdx));
                        }
                    }
                }
            }
        }
    }

    // Token: 0x0600019E RID: 414 RVA: 0x000124AC File Offset: 0x000114AC
    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero(_invData, _indexInEntityOfAction);
    }

    // Token: 0x0600019F RID: 415 RVA: 0x000124C8 File Offset: 0x000114C8
    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
        //this.entityToSpawn = "HorseHero";
        this.entityToSpawn = "HorseHero";
        
        foreach (int v in EntityClass.list.Keys)
        {
            if (!(EntityClass.list[v].entityClassName != this.entityToSpawn))
            {
                this.entityId = v;
                break;
            }
        }
    }

    // Token: 0x060001A0 RID: 416 RVA: 0x00012560 File Offset: 0x00011560
    public override void StartHolding(ItemActionData _actionData)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)_actionData;
        if (!(itemAction.HorseHeroPreview != null) && itemAction.invData.holdingEntity is EntityPlayerLocal)
        {
            GameObject gameObject = (GameObject)ResourceWrapper.Load1P(itemAction.invData.holdingEntity.inventory.holdingItem.MeshFile);
            if (gameObject == null)
            {
                Debug.Log("Failed to load the horse meshfile.");
            }
            else
            {
                itemAction.HorseHeroPreview = UnityEngine.Object.Instantiate<Transform>(gameObject.transform);
                UnityEngine.Object.Destroy(itemAction.HorseHeroPreview.GetComponent<Rigidbody>());
                ItemActionSpawnHorseHero.UpdatePreview(_actionData);
            }
        }
    }

    // Token: 0x060001A1 RID: 417 RVA: 0x0001260C File Offset: 0x0001160C
    public override void CancelAction(ItemActionData _actionData)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)_actionData;
        if (!(itemAction.HorseHeroPreview == null) && itemAction.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemAction.HorseHeroPreview.gameObject);
        }
    }

    // Token: 0x060001A2 RID: 418 RVA: 0x0001265C File Offset: 0x0001165C
    public override void StopHolding(ItemActionData actionData)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)actionData;
        if (!(itemAction.HorseHeroPreview == null) && itemAction.invData.holdingEntity is EntityPlayerLocal)
        {
            itemAction.ValidPosition = false;
            itemAction.PreviewRenderers = null;
            UnityEngine.Object.Destroy(itemAction.HorseHeroPreview.gameObject);
        }
    }

    // Token: 0x060001A3 RID: 419 RVA: 0x000126BC File Offset: 0x000116BC
    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)_actionData;
        if (!(itemAction.HorseHeroPreview == null) && itemAction.invData.holdingEntity is EntityPlayerLocal)
        {
            ItemActionSpawnHorseHero.UpdatePreview(_actionData);
        }
    }

    // Token: 0x060001A4 RID: 420 RVA: 0x00012704 File Offset: 0x00011704
    public override void Cleanup(ItemActionData _actionData)
    {
        base.Cleanup(_actionData);
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = _actionData as ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero;
        if (itemAction != null && itemAction.HorseHeroPreview != null && itemAction.invData != null && itemAction.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemAction.HorseHeroPreview.gameObject);
        }
    }

    // Token: 0x060001A5 RID: 421 RVA: 0x0001276C File Offset: 0x0001176C
    private static void UpdatePreview(ItemActionData _actionData)
    {
        ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero itemAction = (ItemActionSpawnHorseHero.ItemActionDataSpawnHorseHero)_actionData;
        if (itemAction.PreviewRenderers == null || itemAction.PreviewRenderers.Length == 0 || itemAction.PreviewRenderers[0] == null)
        {
            itemAction.PreviewRenderers = itemAction.HorseHeroPreview.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < itemAction.PreviewRenderers.Length; i++)
            {
                itemAction.PreviewRenderers[i].material.color = new Color(2f, 0.25f, 0.25f);
            }
        }
        World world = _actionData.invData.world;
        bool isValid = ItemActionSpawnHorseHero.ValidatePlacement(_actionData, ref itemAction.Position) && world.CanPlaceBlockAt(itemAction.invData.hitInfo.hit.blockPos, world.GetGameManager().GetPersistentLocalPlayer(), true);
        if (itemAction.ValidPosition != isValid)
        {
            itemAction.ValidPosition = isValid;
            foreach (Renderer renderer in itemAction.PreviewRenderers)
            {
                renderer.material.color = ((!isValid) ? new Color(2f, 0.25f, 0.25f) : new Color(0.25f, 2f, 0.25f));
            }
        }
        Quaternion localRotation = itemAction.HorseHeroPreview.localRotation;
        localRotation.eulerAngles = new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f);
        itemAction.HorseHeroPreview.localRotation = localRotation;
        itemAction.HorseHeroPreview.position = itemAction.Position;
    }

    // Token: 0x060001A6 RID: 422 RVA: 0x00012A5C File Offset: 0x00011A5C
    private IEnumerator RemoveFromInv(ItemInventoryData invData, int slotIdx)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        invData.holdingEntity.inventory.DecHoldingItem(invData.itemStack.count);
        invData.holdingEntity.PlayOneShot(this.soundStart ?? "placeblock");
        yield break;
    }

    // Token: 0x060001A7 RID: 423 RVA: 0x00012A8C File Offset: 0x00011A8C
    private static bool ValidatePlacement(ItemActionData actionData, ref Vector3 refPos)
    {
        return EmuUtility.IsTerrainClear(actionData.invData.holdingEntity.GetLookRay(), 5f, ref refPos);
    }

    // Token: 0x0400013F RID: 319
    protected string entityToSpawn;

    // Token: 0x04000140 RID: 320
    protected int entityId = -1;

    // Token: 0x0200002F RID: 47
    protected class ItemActionDataSpawnHorseHero : ItemActionAttackData
    {
        // Token: 0x060001A9 RID: 425 RVA: 0x00012AC9 File Offset: 0x00011AC9
        public ItemActionDataSpawnHorseHero(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
        {
        }

        // Token: 0x04000141 RID: 321
        public Transform HorseHeroPreview;

        // Token: 0x04000142 RID: 322
        public Renderer[] PreviewRenderers;

        // Token: 0x04000143 RID: 323
        public bool ValidPosition;

        // Token: 0x04000144 RID: 324
        public Vector3 Position;
    }
}
