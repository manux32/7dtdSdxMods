using System;
using UnityEngine;

//public class BlockMiner : BlockPowered
public class BlockMiner : BlockWorkstation
{
	static bool showDebugLog = true;
    private string soundStart;
    private string soundRepeat;
    private string soundEnd;

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
        if (this.Properties.Values.ContainsKey("Sound_start"))
            this.soundStart = this.Properties.Values["Sound_start"];
        if (this.Properties.Values.ContainsKey("Sound_repeat"))
            this.soundRepeat = this.Properties.Values["Sound_repeat"];
        if (this.Properties.Values.ContainsKey("Sound_end"))
            this.soundEnd = this.Properties.Values["Sound_end"];
    }

    public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		DebugMsg("BlockMiner.OnBlockEntityTransformBeforeActivated");
        try
        {
            if (_ebcd != null && _ebcd.bHasTransform)
            {
                GameObject gameObject = _ebcd.transform.gameObject;
                MinerControl healingPodControlScript = gameObject.GetComponent<MinerControl>();
                if (healingPodControlScript == null)
                {
                    healingPodControlScript = gameObject.AddComponent<MinerControl>();
                }
                healingPodControlScript.enabled = true;
                healingPodControlScript.cIdx = _cIdx;
                healingPodControlScript.blockPos = _blockPos;
                healingPodControlScript.soundStart = soundStart;
                healingPodControlScript.soundRepeat = soundRepeat;
                healingPodControlScript.soundEnd = soundEnd;
            }
            else
                DebugMsg("ERROR: _ebcd null (BlockMiner.OnBlockEntityTransformBeforeActivated)");
        }
        catch (Exception ex)
        {
            DebugMsg("Error Message: " + ex.ToString());
        }
    }

    public override void OnBlockRemoved(global::WorldBase world, global::Chunk _chunk, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
        Audio.Manager.BroadcastStop(_blockPos.ToVector3(), soundRepeat);
    }

    public override bool OnBlockActivated(int _indexInBlockActivationCommands, global::WorldBase _world, int _cIdx, global::Vector3i _blockPos, global::BlockValue _blockValue, global::EntityAlive _player)
    {
        this.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
        return true;
    }

    public override string GetActivationText(global::WorldBase _world, global::BlockValue _blockValue, int _clrIdx, global::Vector3i _blockPos, global::EntityAlive _entityFocusing)
    {
        if (!_world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
        {
            return string.Empty;
        }

        global::Block block = global::Block.list[_blockValue.type];
        string blockName = block.GetBlockName();
        return string.Format(global::Localization.Get("pickupPrompt", string.Empty), global::Localization.Get(blockName, string.Empty));
    }
}