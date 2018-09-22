using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


class BlockVendingMachine_pickable : BlockVendingMachine
{
    private float TakeDelay = 2f;
    private global::ItemStack tmpPickupItemStack;
    private global::LocalPlayerUI tmpPickupUiforPlayer;
    private int tmpPickupClrIdx;
    private Vector3i tmpPickupBlockPos;

    public override void Init()
    {
        base.Init();
        bool.TryParse(this.Properties.Values[global::Block.PropCanPickup], out this.CanPickup);
        if (this.CanPickup && this.Properties.Params1.ContainsKey(global::Block.PropCanPickup))
        {
            this.PickedUpItemValue = this.Properties.Params1[global::Block.PropCanPickup];
        }

        if (this.Properties.Values.ContainsKey("TakeDelay"))
        {
            this.TakeDelay = global::Utils.ParseFloat(this.Properties.Values["TakeDelay"]);
        }
        else
        {
            this.TakeDelay = 2f;
        }
    }

    public override global::BlockActivationCommand[] GetBlockActivationCommands(global::WorldBase _world, global::BlockValue _blockValue, 
        int _clrIdx, global::Vector3i _blockPos, global::EntityAlive _entityFocusing)
    {
        BlockActivationCommand[] returnVal = base.GetBlockActivationCommands(_world, _blockValue, _clrIdx, _blockPos, _entityFocusing);

        global::TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_clrIdx, _blockPos) as global::TileEntityVendingMachine;
        string @string = global::GamePrefs.GetString(global::EnumGamePrefs.PlayerId);
        returnVal[1].enabled = (this.CanPickup && _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false));

        return returnVal;
    }

    public override bool OnBlockActivated(int _indexInBlockActivationCommands, global::WorldBase _world, int _cIdx, global::Vector3i _blockPos, global::BlockValue _blockValue, global::EntityAlive _player)
    {
        global::BlockValue block = _world.GetBlock(_blockPos.x, _blockPos.y - 1, _blockPos.z);
        if (global::Block.list[block.type].HasTag(global::BlockTags.Door))
        {
            _blockPos = new global::Vector3i(_blockPos.x, _blockPos.y - 1, _blockPos.z);
            return this.OnBlockActivated(_indexInBlockActivationCommands, _world, _cIdx, _blockPos, _blockValue, _player);
        }
        global::TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_cIdx, _blockPos) as global::TileEntityVendingMachine;
        if (tileEntityVendingMachine == null)
        {
            return false;
        }
        global::LocalPlayerUI uiforPlayer = global::LocalPlayerUI.GetUIForPlayer(_player as global::EntityPlayerLocal);
        if (null != uiforPlayer)
        {
            switch (_indexInBlockActivationCommands)
            {
                case 0:
                    return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
                case 1:
                    {
                        this.tmpPickupUiforPlayer = uiforPlayer;
                        this.tmpPickupClrIdx = _cIdx;
                        this.tmpPickupBlockPos = _blockPos;
                        this.tmpPickupItemStack = new global::ItemStack(_blockValue.ToItemValue(), 1);
                        this.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
                        return true;
                    }
                case 2:
                    uiforPlayer.windowManager.Open(global::GUIWindowKeypad.ID, true, false, true);
                    global::NGuiKeypad.Instance.LockedItem = tileEntityVendingMachine;
                    return true;
            }
        }
        return false;
    }

    public void TakeItemWithTimer(int _cIdx, global::Vector3i _blockPos, global::BlockValue _blockValue, global::EntityAlive _player)
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
        if (this.tmpPickupUiforPlayer.xui.PlayerInventory.AddItem(this.tmpPickupItemStack, true))
        {
            GameManager.Instance.World.SetBlockRPC(this.tmpPickupClrIdx, this.tmpPickupBlockPos, global::BlockValue.Air);
        }
    }
}

