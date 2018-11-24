using System;
using UnityEngine;


public class BlockDummyBoat : Block
{

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    /*public override void Init()
    {
        base.Init();
        this.multiBlockPos = base.multiBlockPos;
    }*/


    public override void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
    {

    }

    public void RemoveChilds(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        DebugMsg("BlockDummyBoat RemoveChilds");
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
            DebugMsg("BlockDummyBoat RemoveChilds: Chunk is NULL");
            return;
        }
        byte rotation = _blockValue.rotation;
        for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
        {
            Vector3i other = this.multiBlockPos.Get(i, _blockValue.type, (int)rotation);
            if ((other.x != 0 || other.y != 0 || other.z != 0) && chunkCluster.GetBlock(_blockPos + other).type == _blockValue.type)
            {
                chunkCluster.SetBlock(_blockPos + other, Block.GetBlockValue("water"), false, false);
            }
        }
    }

    public void RemoveParentBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        DebugMsg("BlockDummyBoat RemoveParentBlock");
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
            DebugMsg("BlockDummyBoat RemoveParentBlock: Chunk is NULL");
            return;
        }
        Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
        BlockValue block = chunkCluster.GetBlock(parentPos);
        if (!block.ischild && block.type == _blockValue.type)
        {
            chunkCluster.SetBlock(parentPos, Block.GetBlockValue("water"), true, true);
        }
    }


    public override bool ShowModelOnFall()
    {
        return false;
    }

    public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        DebugMsg("BlockDummyBoat OnBlockRemoved: Deleting Childs.");
        if (!_blockValue.ischild)
        {
            this.shape.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
            if (this.isMultiBlock)
            {
                this.RemoveChilds(_world, _chunk.ClrIdx, _blockPos, _blockValue);
            }

            // Destroy Support Tower
            DestroySupportTower(_blockPos);
        }
        else if (this.isMultiBlock)
        {
            this.RemoveParentBlock(_world, _chunk.ClrIdx, _blockPos, _blockValue);
        }
    }

    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        if (_blockValue.ischild)
        {
            return;
        }
        this.shape.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
        if (this.isMultiBlock)
        {
            this.multiBlockPos.AddChilds(_world, _chunk.ClrIdx, _blockPos, _blockValue);
        }

        // Create Support Tower
        CreateSupportTower(_blockPos);

        // Try to get BlockEntityData
        BlockEntityData blockEntityData = _chunk.GetBlockEntity(_blockPos);

        // Try to find closest Boat, then set and offset it
        FindSetAndOffsetAssignedBoat(_chunk.ClrIdx, _blockPos, _blockValue, blockEntityData);
    }


    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        if (_blockValue.ischild)
        {
            return;
        }

        this.shape.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);

        // Try to get BlockEntityData
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
            DebugMsg("BlockDummyBoat OnBlockLoaded: ChunkCluster is NULL, aborting. Cannot find and set Boat.");
            return;
        }
        Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos);
        BlockEntityData blockEntityData = chunk.GetBlockEntity(_blockPos);

        // Try to find closest Boat, then set and offset it
        DebugMsg("BlockDummyBoat OnBlockLoaded: Entity count = " + GameManager.Instance.World.Entities.list.Count.ToString());
        if (!FindSetAndOffsetAssignedBoat(_clrIdx, _blockPos, _blockValue, blockEntityData) && GameManager.Instance.World.Entities.list.Count > 0)
        {
            // Destroying on Load output errors so I moved Destroy Orphans to OnBlockEntityTransformBeforeActivated
            DebugMsg("BlockDummyBoat OnBlockLoaded: No Boat close enough, skipping.");
        }
    }


    public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        if (_blockValue.ischild)
        {
            return;
        }

        // Try to rehide the dummy boat mesh in case it got visible again
        HideBlockMeshes(_world, _blockPos, _cIdx, _ebcd);

        DestroyOrphanBlock(_cIdx, _blockPos);
    }


    public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
    {
        if (_oldBlockValue.ischild)
        {
            return;
        }
        this.shape.OnBlockValueChanged(_world, _blockPos, _clrIdx, _oldBlockValue, _newBlockValue);
        if (this.isMultiBlock && _oldBlockValue.rotation != _newBlockValue.rotation)
        {
            if (_world.ChunkClusters[_clrIdx] == null)
            {
                return;
            }
            this.multiBlockPos.RemoveChilds(_world, _clrIdx, _blockPos, _oldBlockValue);
            this.multiBlockPos.AddChilds(_world, _clrIdx, _blockPos, _newBlockValue);
        }

        // Try to rehide the dummy boat mesh in case it got visible again
        HideBlockMeshes(_world, _blockPos, _clrIdx, null);
    }


    public bool FindSetAndOffsetAssignedBoat(int _cIdx, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        if (_ebcd == null)
        {
            DebugMsg("BlockDummyBoat FindSetAndOffsetAssignedBoat: BlockEntityData is NULL, aborting: _blockPos = " + _blockPos.ToString());
        }

        EntityCustomBoat assignedBoat = null;
        if(_FindSetAndOffsetAssignedBoat(_cIdx, _blockPos, _blockValue, ref assignedBoat))
        {
            DebugMsg("BlockDummyBoat FindSetAndOffsetAssignedBoat: Found Boat Dummy Block BlockEntityData: _blockPos = " + _blockPos.ToString());
            assignedBoat.boatDummyBlockEntityData = _ebcd;
            // Adjust Boat and dummy block transform
            assignedBoat.bNeedBoatDummyBlockOffset = true;
            assignedBoat.AdjustBoatAndDummyBoatBlockPositions();
            return true;
        }
        return false;
    }


    public bool _FindSetAndOffsetAssignedBoat(int _cIdx, Vector3i _blockPos, BlockValue _blockValue, ref EntityCustomBoat _assignedBoat)
    {
        // Try to find closest Boat
        _assignedBoat = GetClosestBoat(_blockPos);
        if (_assignedBoat != null)
        {
            if(!_assignedBoat.bHasBoatDummy)
            {
                DebugMsg("BlockDummyBoat _FindSetAndOffsetAssignedBoat: Found closest Boat but it is not flagged to have a Dummy Boat Block, skipping: _blockPos = " + _blockPos.ToString() + " | Boat pos = " + _assignedBoat.position.ToString("0.000"));
                return false;
            }
            if(_assignedBoat.bHasBoatDummy && _assignedBoat.boatDummyBlockEntityData != null && _assignedBoat.boatDummyPos != _blockPos)
            {
                DebugMsg("BlockDummyBoat _FindSetAndOffsetAssignedBoat: Found closest Boat but it already as an assigned Dummy Boat Block, skipping: _blockPos = " + _blockPos.ToString() + " | Boat pos = " + _assignedBoat.position.ToString("0.000"));
                return false;
            }

            float closestBoatDistance = Vector3.Distance(_blockPos.ToVector3(), _assignedBoat.position);
            if(closestBoatDistance > 3 && _assignedBoat.boatDummyPos != _blockPos)
            {
                DebugMsg("BlockDummyBoat _FindSetAndOffsetAssignedBoat: Found closest Boat but it's too far, skipping: _blockPos = " + _blockPos.ToString() + " | Boat pos = " + _assignedBoat.position.ToString("0.000"));
                return false;
            }

            DebugMsg("BlockDummyBoat _FindSetAndOffsetAssignedBoat: Found closest Boat: _blockPos = " + _blockPos.ToString() + " | Boat pos = " + _assignedBoat.position.ToString("0.000"));

            _assignedBoat.boatDummyPos = _blockPos;

            // Rotate boat to block
            Quaternion newQuat = Quaternion.AngleAxis(CustomVehiclesUtils.BlockYRotToEularYRot((int)_blockValue.rotation), Vector3.up);
            _assignedBoat.transform.rotation = newQuat;
            _assignedBoat.SetRotation(newQuat.eulerAngles);

            return true;
        }
        else
        {
            DebugMsg("BlockDummyBoat FindSetAndOffsetAssignedBoat: Could not find closest Boat: _blockPos = " + _blockPos.ToString());
        }

        return false;
    }

    public EntityCustomBoat GetClosestBoat(Vector3i _blockPos)
    {
        EntityCustomBoat closestBoat = null;
        float closestDistance = 1000;

        Entity curEntity;
        float curDistance;
        for (int i = GameManager.Instance.World.Entities.list.Count - 1; i >= 0; i--)
        {
            curEntity = GameManager.Instance.World.Entities.list[i];
            curDistance = Vector3.Distance(_blockPos.ToVector3(), curEntity.position);
            if (curEntity.GetType() == typeof(EntityCustomBoat) && curDistance < closestDistance)
            {
                closestDistance = curDistance;
                closestBoat = (EntityCustomBoat)curEntity;
            }
        }
        return closestBoat;
    }

    public void CreateSupportTower(Vector3i _blockPos)
    {
        DebugMsg("BlockDummyBoat CreateSupportTower: Creating water support tower blocks");
        BlockValue newBlockValue = Block.GetBlockValue("waterSupportBlock");
        Vector3i waterSupportPos = _blockPos;
        waterSupportPos.y -= 1;
        BlockValue curBlockValue = GameManager.Instance.World.GetBlock(waterSupportPos);
        Block curBlock = Block.list[curBlockValue.type];

        while (curBlock.blockMaterial.IsLiquid || curBlock.GetBlockName() == "waterSupportBlock")
        {
            GameManager.Instance.World.SetBlockRPC(waterSupportPos, newBlockValue);
            waterSupportPos.y -= 1;
            curBlockValue = GameManager.Instance.World.GetBlock(waterSupportPos);
            curBlock = Block.list[curBlockValue.type];
        }
    }

    public void DestroySupportTower(Vector3i _blockPos)
    {
        DebugMsg("BlockDummyBoat DestroySupportTower: Destroying water support tower blocks");
        BlockValue newBlockValue = Block.GetBlockValue("water");
        Vector3i waterSupportPos = _blockPos;
        waterSupportPos.y -= 1;
        BlockValue curBlockValue = GameManager.Instance.World.GetBlock(waterSupportPos);
        Block curBlock = Block.list[curBlockValue.type];

        while (curBlock.GetBlockName() == "waterSupportBlock")
        {
            //DebugMsg("Destroying tower block: " + curBlock.GetBlockName() + " | pos =" + waterSupportPos.ToString());
            GameManager.Instance.World.SetBlockRPC(waterSupportPos, newBlockValue);
            waterSupportPos.y -= 1;
            curBlockValue = GameManager.Instance.World.GetBlock(waterSupportPos);
            curBlock = Block.list[curBlockValue.type];
        }
    }


    public void DestroyOrphanBlock(int _cIdx, Vector3i _blockPos)
    {
        // Destroy if orphan block (no boat)
        EntityCustomBoat assignedBoat = GetClosestBoat(_blockPos);
        if (assignedBoat != null)
        {
            float closestBoatDistance = Vector3.Distance(_blockPos.ToVector3(), assignedBoat.position);
            if (closestBoatDistance > 3 && assignedBoat.boatDummyPos != _blockPos)
            {
                DebugMsg("BlockDummyBoat DestroyOrphanBlock: Found closest Boat but it's too far, discarding and destroying orphan Dummy Boat Block: _blockPos = " + _blockPos.ToString() + " | Boat pos = " + assignedBoat.position.ToString("0.000"));
                GameManager.Instance.World.SetBlockRPC(_cIdx, _blockPos, Block.GetBlockValue("water"));
            }
        }
    }


    public BlockEntityData GetBlockEntityData(WorldBase _world, Vector3i _blockPos, int _clrIdx)
    {
        // Try to get BlockEntityData
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
            DebugMsg("BlockDummyBoat GetBlockEntityData: ChunkCluster is NULL.");
            return null;
        }
        DebugMsg("BlockDummyBoat GetBlockEntityData: Getting BlockEntityData.");
        Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos);
        return chunk.GetBlockEntity(_blockPos);
    }


    public void HideBlockMeshes(WorldBase _world, Vector3i _blockPos, int _clrIdx, BlockEntityData _ebcd)
    {
        if (_ebcd == null)
        {
            DebugMsg("BlockDummyBoat HideBlockMeshes: _ebcd is NULL, trying to get it");
            _ebcd = GetBlockEntityData(_world, _blockPos, _clrIdx);
        }

        // Try to rehide the dummy boat mesh in case it got visible again
        if (_ebcd.bHasTransform || _ebcd.transform != null)
        {
            DebugMsg("BlockDummyBoat HideBlockMeshes: Hidding Meshes.");
            MeshRenderer[] meshRenderers = _ebcd.transform.gameObject.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.enabled = false;
            }
        }
    }


    public static new bool CanFallBelow(WorldBase _world, int _x, int _y, int _z)
    {
        return false;
    }

    public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, bool _bUseHarvestTool, int _recDepth)
    {
        return 0;
    }

    /*public override float GetHardness()
    {
        return 1f;
    }*/


    // Not needed, getting the BlockEntityData directly seems to work
    public BlockEntityData GetOrCreateBlockEntityData(Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        // Try to get an existing BlockEntityData
        BlockEntityData blockEntityData = _chunk.GetBlockEntity(_blockPos);

        if (blockEntityData == null)
        {
            // Create BlockEntityData and add stub
            DebugMsg("BlockDummyBoat GetOrCreateBlockEntityData: BlockEntityData is NULL, creating it.");
            blockEntityData = new BlockEntityData(_blockValue, _blockPos);
            _chunk.AddEntityBlockStub(blockEntityData);
        }
        else
        {
            DebugMsg("BlockDummyBoat GetOrCreateBlockEntityData: Found BlockEntityData.");
        }

        return blockEntityData;
    }


    /*public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Random _rnd)
    {
        //this.blockMaterial = MaterialBlock.water;

        DebugMsg("OnBlockPlaced rotation" + _blockValue.rotation.ToString());

        return base.OnBlockPlaced(_world, _clrIdx, _blockPos, _blockValue, _rnd);
    }*/


    /*public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, Random _rnd)
    {
        base.OnBlockPlaceBefore(_world, ref _bpResult, _ea, _rnd);
    }*/

    //public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck)
    //{
    /*bool result = base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck);
    if (!result)
    {
        int type = _world.GetBlock(_clrIdx, _blockPos).type;
        //DisplayChatAreaText(string.Format("Block name: {0}, Block Liquid_ {1}", Block.list[type].GetBlockName(), Block.list[type].blockMaterial.IsLiquid));
        return Block.list[type].blockMaterial.IsLiquid;
    }
    return result;*/

    //int type = _world.GetBlock(_clrIdx, _blockPos).type;
    //DisplayChatAreaText(string.Format("Block name: {0}, Block Liquid_ {1}", Block.list[type].GetBlockName(), Block.list[type].blockMaterial.IsLiquid));
    //return Block.list[type].blockMaterial.IsLiquid;
    //return false;
    //}


    // Was trying to get the dummy boat block BlockShapeModelEntity Transform directly, but that just gets the prefab in the center of the world
    /*public GameObject GetBlockShapeModelEntityGameObject(int _clrIdx, Vector3i _blockPos)
    {
        // Try to find the BlockShapeModelEntity Transform
        DebugMsg("BlockDummyBoat GetBlockShapeModelEntityTransform: this.shape = " + this.shape.GetType().ToString());
        if (this.shape.GetType() == typeof(BlockShapeModelEntity))
        {
            BlockShapeModelEntity bsme = (BlockShapeModelEntity)this.shape;
            DebugMsg("BlockDummyBoat GetBlockShapeModelEntityGameObject: this.shape.modelName = " + bsme.modelName);

            GameObject objectForType = GameObjectPool.Instance.GetObjectForType(bsme.modelName, false);
            if (objectForType != null)
            {
                DebugMsg("BlockDummyBoat GetBlockShapeModelEntityGameObject: Found bsme GameObject");
                return objectForType;
            }
        }
        return null;
    }*/
}