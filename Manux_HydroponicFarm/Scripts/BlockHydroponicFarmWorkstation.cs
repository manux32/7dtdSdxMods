using System;
using System.Collections;
using UnityEngine;

public class BlockHydroponicFarmWorkstation : BlockWorkstation
{
    static readonly bool showDebugLog = false;
    Chunk chunk = null;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
        string useString = Localization.Get("useWorkstation", string.Empty);
        string workstationName = Localization.Get("hydroponicFarmPower", string.Empty);

        Vector3i powerBlockPos = _blockPos;
        powerBlockPos.y -= 1;
        if (BlockHydroponicFarmPower.IsBlockPoweredUp(powerBlockPos, _clrIdx))
        {
            return (useString + " " + workstationName);
        }
        return (useString + " " + workstationName + " (**UNPOWERED**)");
    }

    public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
    {
        return base.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
        /*// To not be able to open the workstation when there is no power (not necessary)
        Vector3i powerBlockPos = _blockPos;
        powerBlockPos.y -= 1;
        if (BlockHydroponicFarmPower.IsBlockPoweredUp(powerBlockPos, _cIdx))
        {
            return base.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
        }
        return false;*/
    }
}