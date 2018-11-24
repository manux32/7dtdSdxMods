using System;
using UnityEngine;
using Random = System.Random;


public class BlockWaterSupport : Block
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

    /*public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, Random _rnd)
    {
        base.OnBlockPlaceBefore(_world, ref _bpResult, _ea, _rnd);
    }*/


    public void RemoveChilds(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
            return;
        }
        byte rotation = _blockValue.rotation;
        for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
        {
            Vector3i other = this.multiBlockPos.Get(i, _blockValue.type, (int)rotation);
            if ((other.x != 0 || other.y != 0 || other.z != 0) && chunkCluster.GetBlock(_blockPos + other).type == _blockValue.type)
            {
                //chunkCluster.SetBlock(_blockPos + other, true, Block.GetBlockValue("water"), true, MarchingCubes.DensityAir, false, false, false);
                chunkCluster.SetBlock(_blockPos + other, Block.GetBlockValue("water"), false, false);
            }
        }
    }

    public void RemoveParentBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
        if (chunkCluster == null)
        {
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
        DebugMsg("BlockWaterSupport OnBlockRemoved: Not deleting anything.");
        /*if (!_blockValue.ischild)
        {
            this.shape.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
            if (this.isMultiBlock)
            {
                this.RemoveChilds(_world, _chunk.ClrIdx, _blockPos, _blockValue);
            }
        }
        else if (this.isMultiBlock)
        {
            this.RemoveParentBlock(_world, _chunk.ClrIdx, _blockPos, _blockValue);
        }*/
    }

    /*public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Random _rnd)
    {
        //this.blockMaterial = MaterialBlock.water;

        DebugMsg("OnBlockPlaced rotation" + _blockValue.rotation.ToString());

        return base.OnBlockPlaced(_world, _clrIdx, _blockPos, _blockValue, _rnd);
    }*/


    public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        //DebugMsg("OnBlockPlaced rotation = " + _blockValue.rotation.ToString());

        base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
    }


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
}