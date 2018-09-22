using System;
using UnityEngine;

public class BlockAdhesiveElectricSteelFireSpike : BlockAdhesiveElectricSteelSpike
{
	static bool showDebugLog = false;

    public override void Init()
    {
        base.Init();
        bool.TryParse(this.Properties.Values[global::Block.PropCanPickup], out this.CanPickup);
        if (this.CanPickup && this.Properties.Params1.ContainsKey(global::Block.PropCanPickup))
        {
            this.PickedUpItemValue = this.Properties.Params1[global::Block.PropCanPickup];
        }
    }

    public override void OnBlockRemoved(global::WorldBase world, global::Chunk _chunk, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        // Don't remove TileEntity and wire when block is removed because it's destroyed, we keep it for the downgrade version of the block so wires stay connected
        this.shape.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
    }
}