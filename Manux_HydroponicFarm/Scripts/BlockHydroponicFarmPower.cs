using System;
using UnityEngine;

public class BlockHydroponicFarmPower : BlockPowered
{
    static bool showDebugLog = false;
    private string soundRepeat;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        this.shape.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);

        // create a workstation above the power block
        BlockValue workstationBlockValue = Block.GetBlockValue("hydroponicFarmWorkstation");
        Vector3i workstationBlockPos = _blockPos;
        workstationBlockPos.y = _blockPos.y + 1;
        workstationBlockValue.rotation = _blockValue.rotation;
        GameManager.Instance.World.SetBlockRPC(workstationBlockPos, workstationBlockValue);

        InitControlScript(_blockPos, _chunk.ClrIdx, _chunk.GetBlockEntity(_blockPos));
    }

    public void InitControlScript(Vector3i _blockPos, int _cIdx, BlockEntityData _ebcd)
    {
        try
        {
            if (_ebcd != null && _ebcd.bHasTransform)
            {
                GameObject gameObject = _ebcd.transform.gameObject;
                HydroponicFarmPowerControl HydroponicFarmControlScript = gameObject.GetComponent<HydroponicFarmPowerControl>();
                if (HydroponicFarmControlScript == null)
                {
                    HydroponicFarmControlScript = gameObject.AddComponent<HydroponicFarmPowerControl>();
                }
                HydroponicFarmControlScript.enabled = true;
                HydroponicFarmControlScript.cIdx = _cIdx;
                HydroponicFarmControlScript.blockPos = _blockPos;
            }
            else
                DebugMsg("ERROR: _ebcd null (BlockHydroponicFarmPower.InitControlScript)");
        }
        catch (Exception ex)
        {
            DebugMsg("Error Message: " + ex.ToString());
        }
    }

    public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
        DebugMsg("OnBlockEntityTransformBeforeActivated");
        Debug.Log("power block initial _blockPos = " + _blockPos.ToString());
        InitControlScript(_blockPos, _cIdx, _ebcd);
        /*
        // Offset wire so that it's at the base of the Healing Pod
        TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, _blockPos);
        if (tileEntityPowered != null)
        {
            tileEntityPowered.WireOffset = new Vector3(0, 0, 0);
        }*/
    }

    public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
        // put back original air...  
        DebugMsg("Removing Hydroponic Farm Workstation, putting back air");
        BlockValue airBlock = Block.GetBlockValue("air");
        Vector3i workstationBlockPos = _blockPos;
        workstationBlockPos.y = _blockPos.y + 1;              
        GameManager.Instance.World.SetBlockRPC(workstationBlockPos, airBlock);
    }


    public static bool IsBlockPoweredUp(Vector3i _blockPos, int _clrIdx)
    {
        WorldBase world = GameManager.Instance.World;
        if (world.IsRemote())
        {
            //Use HasActivePower power instead since directly powering blocks doesnt work on servers.
            return BlockHydroponicFarmPower.HasActivePower(world, _clrIdx, _blockPos);
        }
        TileEntityPowered tileEntityPowered = (TileEntityPowered)GameManager.Instance.World.GetTileEntity(_clrIdx, _blockPos);
        if (tileEntityPowered != null)
        {
            if (tileEntityPowered.IsPowered)
            {
                DebugMsg("Block Power Is On");
                return true;
            }
        }
        if (BlockHydroponicFarmPower.IsSpRemotePowerAllowed(_blockPos))
        {
            DebugMsg("No direct power found, checking for remote power");
            return BlockHydroponicFarmPower.HasActivePower(world, _clrIdx, _blockPos);
        }
        DebugMsg("Block Power Is Off");
        return false;
    }

    private static bool IsSpRemotePowerAllowed(Vector3i _blockPos)
    {
        BlockValue blockValue = GameManager.Instance.World.GetBlock(_blockPos);
        Block block = Block.list[blockValue.type];
        bool isSpRemotePowerAllowed = false;
        if (block.Properties.Values.ContainsKey("AllowRemotePower"))
        {
            bool.TryParse(block.Properties.Values["AllowRemotePower"], out isSpRemotePowerAllowed);
        }
        return isSpRemotePowerAllowed;
    }

    static Vector3i[] PowerInputLocations(Vector3i _blockPos)
    {
        int inputSpace = 2;
        Vector3i inputPosA = _blockPos;
        Vector3i inputPosB = _blockPos;
        Vector3i inputPosC = _blockPos;
        Vector3i inputPosD = _blockPos;
        Vector3i inputPosE = _blockPos;
        Vector3i inputPosF = _blockPos;

        inputPosA.y = _blockPos.y + inputSpace;
        inputPosB.y = _blockPos.y - inputSpace;
        inputPosC.x = _blockPos.x + inputSpace;
        inputPosD.x = _blockPos.x - inputSpace;
        inputPosE.z = _blockPos.z + inputSpace;
        inputPosF.z = _blockPos.z - inputSpace;

        Vector3i[] array = new Vector3i[6];
        array[0] = inputPosA;
        array[1] = inputPosB;
        array[2] = inputPosC;
        array[3] = inputPosD;
        array[4] = inputPosE;
        array[5] = inputPosF;
        return array;

    }

    //Used for severs, block will be NOT be powered directly. Also used in SP if AllowRemotePower is true in the xml.
    public static bool HasActivePower(WorldBase _world, int _cIdx, Vector3i _blockPos)
    {
        Vector3i[] locations = PowerInputLocations(_blockPos);
        foreach (Vector3i vector in locations)
        {
            BlockValue inputBlockValue = _world.GetBlock(vector);
            Type inputBlockType = Block.list[inputBlockValue.type].GetType();
            if (inputBlockType == typeof(BlockPowered))
            {
                TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, vector);
                if (tileEntityPowered != null)
                {
                    if (tileEntityPowered.IsPowered)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override TileEntityPowered CreateTileEntity(Chunk chunk)
    {
        PowerItem.PowerItemTypes powerItemType = PowerItem.PowerItemTypes.Consumer;
        powerItemType = PowerItem.PowerItemTypes.ConsumerToggle;
        TileEntityPowered tileEntityPowered = new TileEntityPoweredBlock(chunk)
        {
            PowerItemType = powerItemType,
        };
        // wire offset
        //tileEntityPowered.WireOffset = new Vector3(0, 0, 0);

        DebugMsg("BlockHydroponicFarmPower creating TileEntityPoweredBlock");
        return tileEntityPowered;
    }
}