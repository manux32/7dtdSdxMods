using System;
using UnityEngine;

public class BlockAdhesiveElectricSteelSpike : BlockPowered
{
	static bool showDebugLog = false;
    public float lastBuffTime = -1;
	
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
        bool.TryParse(this.Properties.Values[global::Block.PropCanPickup], out this.CanPickup);
        if (this.CanPickup && this.Properties.Params1.ContainsKey(global::Block.PropCanPickup))
        {
            this.PickedUpItemValue = this.Properties.Params1[global::Block.PropCanPickup];
        }
    }

    public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _targetEntity)
    {
        // Apply shockedBuff buff
        if (_targetEntity.IsAlive() && IsBlockPoweredUp(_blockPos, _clrIdx) && Time.time > lastBuffTime + 4.5f)
        {
            MultiBuffClassAction multiBuffClassAction = MultiBuffClassAction.NewAction("shockedBuff");
            multiBuffClassAction.Execute(-1, (EntityAlive)_targetEntity, false, EnumBodyPartHit.None, null);
            lastBuffTime = Time.time;
            //DebugMsg("shockedBuff BUFF");
        }
        else
        {
            return false;
        }
        //DebugMsg("BlockAdhesiveElectricSteelSpike AFTER BUFF");
        return true;
    }

    public static bool IsBlockPoweredUp(Vector3i _blockPos, int _clrIdx)
	{
		WorldBase world = GameManager.Instance.World;
		if(world.IsRemote())
		{
			//Use HasActivePower power instead since directly powering blocks doesnt work on servers.
			return BlockAdhesiveElectricSteelSpike.HasActivePower(world, _clrIdx, _blockPos);
		}
		TileEntityPowered tileEntityPowered = (TileEntityPowered)GameManager.Instance.World.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntityPowered != null)
		{
			if(tileEntityPowered.IsPowered)
			{
				//DebugMsg("Block Power Is On");
				return true;
			}
		}
		if(BlockAdhesiveElectricSteelSpike.IsSpRemotePowerAllowed(_blockPos))
		{
			//DebugMsg("No direct power found, checking for remote power");
			return BlockAdhesiveElectricSteelSpike.HasActivePower(world, _clrIdx, _blockPos);
		}
		//("Block Power Is Off");
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
		
		inputPosA.y = _blockPos.y+inputSpace;
		inputPosB.y = _blockPos.y-inputSpace;
		inputPosC.x = _blockPos.x+inputSpace;
		inputPosD.x = _blockPos.x-inputSpace;
		inputPosE.z = _blockPos.z+inputSpace;
		inputPosF.z = _blockPos.z-inputSpace;
		
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
			if(inputBlockType == typeof(BlockPowered))
			{
				TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, vector);
				if (tileEntityPowered != null)
				{
					if(tileEntityPowered.IsPowered)
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
            PowerItemType = powerItemType
        };
        return tileEntityPowered;
    }

    public override bool OnBlockActivated(int _indexInBlockActivationCommands, global::WorldBase _world, int _cIdx, global::Vector3i _blockPos, global::BlockValue _blockValue, global::EntityAlive _player)
    {
        global::BlockValue block = _world.GetBlock(_blockPos.x, _blockPos.y - 1, _blockPos.z);
        if (global::Block.list[block.type].HasTag(global::BlockTags.Door))
        {
            _blockPos = new global::Vector3i(_blockPos.x, _blockPos.y - 1, _blockPos.z);
            return this.OnBlockActivated(_indexInBlockActivationCommands, _world, _cIdx, _blockPos, _blockValue, _player);
        }
        if (_indexInBlockActivationCommands != 0)
        {
            return false;
        }

        this.TakeItemWithTimer2(_cIdx, _blockPos, _blockValue, _player);
        return true;
    }

    public override void OnBlockRemoved(global::WorldBase world, global::Chunk _chunk, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        if (_blockValue.damage > 0)
        {
            // Only remove TileEntity and wire when block is removed because it's destroyed, not when picked up by player, or upgraded to Fire version.
            // Removal of TileEntity and wire when player is picking it up is done in TakeBlock()
            base.OnBlockRemoved(world, (Chunk)world.GetChunkFromWorldPos(_blockPos), _blockPos, _blockValue);
        }
        else
        {
            this.shape.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
        }
    }

    public override string GetActivationText(global::WorldBase _world, global::BlockValue _blockValue, int _clrIdx, global::Vector3i _blockPos, global::EntityAlive _entityFocusing)
    {
        // If this.CanPickup is enabled, we don't want to be bothered by the pickup text when in the middle of a blood moon
        return string.Empty;
    }

    public override global::BlockActivationCommand[] GetBlockActivationCommands(global::WorldBase _world, global::BlockValue _blockValue, int _clrIdx, global::Vector3i _blockPos, global::EntityAlive _entityFocusing)
    {
        bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
        this.blockActivationCommand[0].enabled = (this.CanPickup && flag && this.TakeDelay > 0f);
        return this.blockActivationCommand;
    }

    private global::BlockActivationCommand[] blockActivationCommand = new global::BlockActivationCommand[]
    {
        new global::BlockActivationCommand("take", "hand", false)
    };

    public void TakeItemWithTimer2(int _cIdx, global::Vector3i _blockPos, global::BlockValue _blockValue, global::EntityAlive _player)
    {
        if (_blockValue.damage > 0)
        {
            global::GameManager.ShowTooltipWithAlert(_player as global::EntityPlayerLocal, global::Localization.Get("ttRepairBeforePickup", string.Empty), "ui_denied");
            return;
        }
        global::LocalPlayerUI playerUI = (_player as global::EntityPlayerLocal).PlayerUI;
        playerUI.windowManager.Open("timer", true, false, true);
        global::XUiC_Timer xuiC_Timer = (global::XUiC_Timer)playerUI.xui.GetChildByType<global::XUiC_Timer>();
        global::TimerEventData timerEventData = new global::TimerEventData();
        timerEventData.Data = new object[]
        {
        _cIdx,
        _blockValue,
        _blockPos,
        _player
        };
        timerEventData.Event += this.TakeBlock;
        xuiC_Timer.SetTimer(this.TakeDelay, timerEventData);
    }

    private void TakeBlock(object obj)
    {
        global::World world = global::GameManager.Instance.World;
        object[] array = (object[])obj;
        int clrIdx = (int)array[0];
        global::BlockValue blockValue = (global::BlockValue)array[1];
        global::Vector3i vector3i = (global::Vector3i)array[2];
        global::BlockValue block = world.GetBlock(vector3i);
        global::EntityPlayerLocal entityPlayerLocal = array[3] as global::EntityPlayerLocal;
        if (block.damage > 0)
        {
            global::GameManager.ShowTooltipWithAlert(entityPlayerLocal, global::Localization.Get("ttRepairBeforePickup", string.Empty), "ui_denied");
            return;
        }
        if (block.type != blockValue.type)
        {
            global::GameManager.ShowTooltipWithAlert(entityPlayerLocal, global::Localization.Get("ttBlockMissingPickup", string.Empty), "ui_denied");
            return;
        }
        global::TileEntityPowered tileEntityPowered = world.GetTileEntity(clrIdx, vector3i) as global::TileEntityPowered;
        if (tileEntityPowered != null && tileEntityPowered.IsUserAccessing())
        {
            global::GameManager.ShowTooltipWithAlert(entityPlayerLocal, global::Localization.Get("ttCantPickupInUse", string.Empty), "ui_denied");
            return;
        }

        // Remove TileEntity and wire
        base.OnBlockRemoved(world, (Chunk)world.GetChunkFromWorldPos(vector3i), vector3i, blockValue);

        global::LocalPlayerUI uiforPlayer = global::LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
        this.HandleTakeInternalItems(tileEntityPowered, uiforPlayer);
        global::ItemStack itemStack = new global::ItemStack(block.ToItemValue(), 1);
        if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack, true))
        {
            uiforPlayer.xui.PlayerInventory.DropItem(itemStack);
        }

        world.SetBlockRPC(clrIdx, vector3i, global::BlockValue.Air);
    }
}