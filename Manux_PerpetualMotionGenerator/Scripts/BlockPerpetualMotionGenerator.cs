using System;
using UnityEngine;

public class BlockPerpetualMotionGenerator : BlockBatteryBank
{
	static bool showDebugLog = false;
    public float lastBuffTime = -1;
    private string soundRepeat;

    public static void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
			Debug.Log(msg);
		}
	}

    public override void Init()
    {
        base.Init();
        if (this.Properties.Values.ContainsKey("Sound_repeat"))
            this.soundRepeat = this.Properties.Values["Sound_repeat"];
    }

    public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		DebugMsg("OnBlockEntityTransformBeforeActivated");
        try
        {
            if (_ebcd != null && _ebcd.bHasTransform)
            {
                GameObject gameObject = _ebcd.transform.gameObject;
                PerpetualMotionGeneratorControl PMGControlScript = gameObject.GetComponent<PerpetualMotionGeneratorControl>();
                if (PMGControlScript == null)
                {
                    PMGControlScript = gameObject.AddComponent<PerpetualMotionGeneratorControl>();
                }
                PMGControlScript.enabled = true;
                PMGControlScript.cIdx = _cIdx;
                PMGControlScript.blockPos = _blockPos;
                PMGControlScript.soundRepeat = soundRepeat;
            }
            else
                DebugMsg("ERROR: _ebcd null (OnBlockEntityTransformBeforeActivated)");
        }
        catch (Exception ex)
        {
            DebugMsg("Error Message: " + ex.ToString());
        }

        // Offset wire so that it's at the base of the Healing Pod
        TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)_world.GetTileEntity(_cIdx, _blockPos);
        if (tileEntityPowerSource != null)
        {
            tileEntityPowerSource.WireOffset = new Vector3(0.25f, 0.4f, 0);
        }
    }

    public override void OnBlockRemoved(global::WorldBase world, global::Chunk _chunk, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
        Audio.Manager.BroadcastStop(_blockPos.ToVector3(), soundRepeat);
    }

    public override global::TileEntityPowerSource CreateTileEntity(global::Chunk chunk)
    {
        if (this.slotItem == null)
        {
            this.slotItem = global::ItemClass.GetItemClass(this.SlotItemName, false);
        }

        global::TileEntityPowerSource tileEntityPowerSource = new global::TileEntityPowerSource(chunk)
        {
            PowerItemType = global::PowerItem.PowerItemTypes.BatteryBank,
            SlotItem = this.slotItem
        };

        tileEntityPowerSource.WireOffset = new Vector3(0.25f, 0.4f, 0);
        return tileEntityPowerSource;
    }
    /*
    protected override string GetPowerSourceIcon()
    {
        return "perpetualMotionGeneratorMagnet";
    }*/
}