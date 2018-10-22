using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


class Vehicle_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static bool HasWheels(Vehicle vehicle)
    {
        if (vehicle.entity.GetType() == typeof(EntityCustomHelicopter))
        {
            return true;
        }
        return vehicle.GetPartItemValueByTag("wheel1").type != 0;
    }

    public static bool HasStorage(Vehicle vehicle)
    {
        if (vehicle.entity.GetType() == typeof(EntityCustomHelicopter))
        {
            return true;
        }
        return vehicle.GetPartItemValueByTag("storage").type != 0;
    }


    public static void SetPartInSlot(Vehicle vehicle, string _tag, ItemValue _itemValue)
    {
        List<VehiclePart> partsList = null;
        var listOfFieldNames = typeof(Vehicle).GetFields();
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            FieldInfo field = null;
            if (fieldInfo.FieldType == typeof(List<VehiclePart>))
            {
                field = fieldInfo;
            }
            if (field != null)
            {
                partsList = (List<VehiclePart>)field.GetValue(vehicle);
                DebugMsg("Found partsList field");
            }
        }

        bool isRightPart = false;
        ItemClass itemClass = null;
        if (_itemValue != null)
        {
            itemClass = ItemClass.GetForId(_itemValue.type);
        }

        for (int i = 0; i < partsList.Count; i++)
        {
            if (partsList[i].GetTag() == _tag)
            {
                isRightPart = true;
            }

            if (itemClass != null)
            {
                if (partsList[i].GetProperties().Values.ContainsKey("custom_slot_type"))
                {
                    DebugMsg("Vehicle.SetPartInSlot(): custom_slot_type = " + partsList[i].GetProperties().Values["custom_slot_type"] + " | item name = " + itemClass.GetItemName() + " | bIsVehicleCustomPart = " + itemClass.bIsVehicleCustomPart);
                    isRightPart = (partsList[i].GetProperties().Values["custom_slot_type"] == itemClass.GetItemName());
                    DebugMsg("isRightPart = " + isRightPart.ToString()); 
                }
                else if (isRightPart && itemClass.bIsVehicleCustomPart)
                {
                    isRightPart = false;
                }
            }
            else
            {
                DebugMsg("itemClass is NULL!");
            }

            DebugMsg("isRightPart = " + isRightPart.ToString());
            if (isRightPart)
            {
                if (itemClass != null)
                    DebugMsg("setting part = " + itemClass.GetItemName());
                else
                    DebugMsg("setting part = item class null");
                partsList[i].SetItemValue(_itemValue);
                partsList[i].SetActive(_itemValue.type != 0);
                for (int j = 0; j < partsList.Count; j++)
                {
                    if (partsList[j].GetSlotType() == string.Empty && partsList[j].GetParentPartTag() == _tag)
                    {
                        partsList[j].SetActive(_itemValue.type != 0);
                        return;
                    }
                }
                return;
            }
        }
    }
}


class EntityVehicle_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static void AddOrFixPartsInSlots(Vehicle vehicle, string partTag, string defaultPartName)
    {
        string partName = vehicle.GetPartProperty(partTag, "custom_slot_type");
        if(partName == string.Empty)
        {
            partName = defaultPartName;
        }
        ItemValue part = ItemClass.GetItem(partName, false);
        if (part != ItemValue.None)
        {
            part.Quality = UnityEngine.Random.Range(100, 600);
            vehicle.SetPartInSlot(partTag, part);
        }
    }

    public static void PopulatePartData_addition(EntityVehicle entityVehicle)
    {
        Vehicle vehicle = entityVehicle.GetVehicle();
        AddOrFixPartsInSlots(vehicle, "chassis", "minibikeChassis");
        AddOrFixPartsInSlots(vehicle, "engine", "smallEngine");
        AddOrFixPartsInSlots(vehicle, "handlebars", "minibikeHandlebars");
        AddOrFixPartsInSlots(vehicle, "driver", "minibikeSeat");
        AddOrFixPartsInSlots(vehicle, "wheel1", "minibikeWheels");
        AddOrFixPartsInSlots(vehicle, "battery", "carBattery");
        AddOrFixPartsInSlots(vehicle, "storage", "shoppingBasketItem");
        AddOrFixPartsInSlots(vehicle, "lock", "padlock");
    }
}


class XUiM_Vehicle_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static bool SetPart(XUiM_Vehicle xuim_vehicle, XUi _xui, string vehicleSlotName, ItemStack stack, out ItemStack resultStack)
    {
        if (_xui.vehicle == null)
        {
            resultStack = stack;
            return false;
        }
        VehiclePart part = xuim_vehicle.GetPart(_xui, vehicleSlotName);
        if (part == null)
        {
            resultStack = stack;
            return false;
        }

        ItemClass itemClass = ItemClass.GetForId(stack.itemValue.type);
        if (itemClass != null)
        {
            DebugMsg("XUiM_Vehicle.SetPart(): bIsVehicleCustomPart = " + itemClass.bIsVehicleCustomPart);
            if (part.GetProperties().Values.ContainsKey("custom_slot_type"))
            {
                DebugMsg("XUiM_Vehicle.SetPart(): customSlotType = " + part.GetProperties().Values["custom_slot_type"] + " | item name = " + itemClass.GetItemName());
                if (part.GetProperties().Values["custom_slot_type"] != itemClass.GetItemName())
                {
                    DebugMsg("= FALSE");
                    resultStack = stack;
                    return false;
                }
            }
            else 
            {
                if (itemClass.bIsVehicleCustomPart)
                {
                    DebugMsg("for default part, bIsVehicleCustomPart is TRUE, so return FALSE");
                    resultStack = stack;
                    return false;
                }
            }
        }

        ItemValue partItemValueByTag = _xui.vehicle.GetVehicle().GetPartItemValueByTag(part.GetTag());
        if (part.GetTag() == "engine")
        {
            ItemStack itemStack = new ItemStack(ItemClass.GetItem("gasCan", false), (int)_xui.vehicle.GetVehicle().GetFuelLevel());
            if (!_xui.PlayerInventory.AddItem(itemStack, true))
            {
                _xui.PlayerInventory.DropItem(itemStack);
            }
            _xui.vehicle.GetVehicle().SetFuelLevel(0f);
        }
        _xui.vehicle.GetVehicle().SetPartInSlot(part.GetTag(), stack.itemValue);

        XUiEvent_ItemChangedVehicle DW = null;
        var listOfFieldNames = typeof(XUiM_Vehicle).GetFields();
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            FieldInfo field = null;
            if (fieldInfo.FieldType == typeof(XUiEvent_ItemChangedVehicle))
            {
                field = fieldInfo;
            }
            if (field != null)
            {
                DW = (XUiEvent_ItemChangedVehicle)field.GetValue(xuim_vehicle);
            }
        }

        if (DW != null)
        {
            DW(stack, vehicleSlotName);
        }
        resultStack = new ItemStack(partItemValueByTag.Clone(), 1);
        xuim_vehicle.RefreshVehicle();
        return true;
    }
}

class XUiC_BasePartStack_patchFunctions
{
    public static string GetPartName(XUiC_BasePartStack xuic_basePartStack)
    {
        if(xuic_basePartStack.itemClass == null && xuic_basePartStack.customSlotType != string.Empty)
        {
            string localizedName = Localization.Get(xuic_basePartStack.customSlotType, string.Empty);
            if (localizedName != string.Empty)
            {
                return string.Format("[MISSING {0}]", localizedName).ToUpper();
            }
            else
            {
                return string.Format("[MISSING {0}]", xuic_basePartStack.customSlotType).ToUpper();
            }
        }
        return (xuic_basePartStack.itemClass == null) ? string.Format("[MISSING {0}]", xuic_basePartStack.SlotType).ToUpper() : xuic_basePartStack.itemClass.localizedName.ToUpper();
    }
}


class XUiC_VehiclePartStack_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static string GetCustomSlotType(VehiclePart part)
    {
        if (part.GetProperties().Values.ContainsKey("custom_slot_type"))
        {
            return part.GetProperties().Values["custom_slot_type"];
        }
        return "";
    }

    public static bool CanSwap(XUiC_VehiclePartStack xuic_vehPartStack, ItemStack stack)
    {
        ItemClass forId = ItemClass.GetForId(stack.itemValue.type);

        DebugMsg("XUiC_VehiclePartStack.CanSwap(): customSlotType = " + xuic_vehPartStack.customSlotType + " | item name = " + forId.GetItemName() + " | bIsVehicleCustomPart = " + forId.bIsVehicleCustomPart);
        if (xuic_vehPartStack.customSlotType != string.Empty)
        {
            if (xuic_vehPartStack.customSlotType == forId.GetItemName())
                return true;
            else
                return false;
        }
        return (forId.VehicleSlotType == xuic_vehPartStack.slotType && !forId.bIsVehicleCustomPart);
    }

        public static void SetEmptySpriteName(XUiC_VehiclePartStack xuic_vehPartStack)
    {
        string slotType = xuic_vehPartStack.slotType;
        if (xuic_vehPartStack.customSlotType != string.Empty)
        {
            DebugMsg("XUiC_VehiclePartStack.SetEmptySpriteName() setting slotType from customSlotType");
            slotType = xuic_vehPartStack.customSlotType;
        }
        DebugMsg("XUiC_VehiclePartStack.SetEmptySpriteName() slotType = " + slotType);
        switch (slotType)
        {
            case "basket":
                xuic_vehPartStack.emptySpriteName = "shoppingBasketEmpty";
                break;
            case "battery":
                xuic_vehPartStack.emptySpriteName = "carBattery";
                break;
            case "chassis":
                xuic_vehPartStack.emptySpriteName = "minibikeChassis";
                break;
            case "engine":
                xuic_vehPartStack.emptySpriteName = "smallEngine";
                break;
            case "handlebars":
                xuic_vehPartStack.emptySpriteName = "minibikeHandlebars";
                break;
            case "headlight":
                xuic_vehPartStack.emptySpriteName = "headlight";
                break;
            case "lock":
                xuic_vehPartStack.emptySpriteName = "padlock";
                break;
            case "seat":
                xuic_vehPartStack.emptySpriteName = "minibikeSeat";
                break;
            case "wheel":
                xuic_vehPartStack.emptySpriteName = "minibikeWheels";
                break;
            default:
                {
                    xuic_vehPartStack.emptySpriteName = slotType;
                    break;
                }
        }
    }
}


class XUiC_VehiclePartStackGrid_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static void HandleSlotChangedEvent(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid, int slotNumber, global::ItemStack stack)
    {
        global::XUiC_VehiclePartStack xuiC_VehiclePartStack = (global::XUiC_VehiclePartStack)xuic_vehPartStackGrid.itemControllers[slotNumber];

        bool emptyBasket = true;
        // redundant crap because the same thing in XUiC_VehiclePartStackGrid.SetParts() doesn't work.  But here neither.
        if (xuiC_VehiclePartStack.VehiclePart.GetProperties().Values.ContainsKey("empty_basket_on_remove"))
        {
            DebugMsg("XUiC_VehiclePartStackGrid.HandleSlotChangedEvent(): empty_basket_on_remove prop exists.");
            
            if (bool.TryParse(xuiC_VehiclePartStack.VehiclePart.GetProperties().Values["empty_basket_on_remove"], out emptyBasket))
            {
                DebugMsg("setting basket bEmptyBasketOnRemove prop from xml");
                xuiC_VehiclePartStack.bEmptyBasketOnRemove = emptyBasket;
            }
            else
            {
                DebugMsg("setting basket bEmptyBasketOnRemove prop to default");
                xuiC_VehiclePartStack.bEmptyBasketOnRemove = true;
            }
            DebugMsg("emptyBasket = " + emptyBasket.ToString() + " | bEmptyBasketOnRemove = " + xuiC_VehiclePartStack.bEmptyBasketOnRemove.ToString());
        }
        else
        {
            emptyBasket = true;
            DebugMsg("empty_basket_on_remove does not exist, setting emptyBasket to default (true) | emptyBasket = " + emptyBasket.ToString());
        }

        DebugMsg("XUiC_VehiclePartStackGrid.HandleSlotChangedEvent: SlotType = " + xuiC_VehiclePartStack.SlotType + " | bEmptyBasketOnRemove = " + xuiC_VehiclePartStack.bEmptyBasketOnRemove.ToString());
        if (xuiC_VehiclePartStack.SlotType == "basket" && emptyBasket)
        {
            DebugMsg("Taking all items on basket remove.");
            xuic_vehPartStackGrid.StorageContainer.TakeAll();
        }
        xuic_vehPartStackGrid.CurrentVehicle.SetPartInSlot(xuiC_VehiclePartStack.VehiclePart.GetTag(), stack.itemValue);
        if (xuiC_VehiclePartStack.SlotType == "basket")
        {
            xuic_vehPartStackGrid.StorageContainer.ShowStorage(xuic_vehPartStackGrid.CurrentVehicle.HasStorage());
        }
        if (xuiC_VehiclePartStack.SlotType == "engine")
        {
            xuic_vehPartStackGrid.CurrentVehicle.SetFuelLevel(0f);
            if (xuic_vehPartStackGrid.CurrentVehicle.GetFuelLevel() > xuic_vehPartStackGrid.CurrentVehicle.GetMaxFuelLevel())
            {
                xuic_vehPartStackGrid.CurrentVehicle.SetFuelLevel(xuic_vehPartStackGrid.CurrentVehicle.GetMaxFuelLevel());
            }
        }

        //((XUiC_VehicleWindowGroup)xuic_vehPartStackGrid.WindowGroup.Controller).QEG();
        XUiC_VehicleFrameWindow AUW = null;
        XUiC_VehicleStats THZ = null;
        //var listOfFieldNames = typeof(Vehicle).GetFields();
        var listOfFieldNames = typeof(XUiC_VehicleWindowGroup).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            FieldInfo field = null;
            if (fieldInfo.FieldType == typeof(XUiC_VehicleFrameWindow))
            {
                field = fieldInfo;
                if (field != null)
                {
                    AUW = (XUiC_VehicleFrameWindow)field.GetValue(((XUiC_VehicleWindowGroup)xuic_vehPartStackGrid.WindowGroup.Controller));
                    DebugMsg("Found XUiC_VehicleFrameWindow");
                }
            }
            else if (fieldInfo.FieldType == typeof(XUiC_VehicleStats))
            {
                field = fieldInfo;
                if (field != null)
                {
                    THZ = (XUiC_VehicleStats)field.GetValue(((XUiC_VehicleWindowGroup)xuic_vehPartStackGrid.WindowGroup.Controller));
                    DebugMsg("Found XUiC_VehicleStats");
                }
            }
        }

        if (AUW != null)
        {
            DebugMsg("AUW.RefreshBindings()");
            AUW.RefreshBindings(false);
        }
        if (THZ != null)
        {
            DebugMsg("THZ.RefreshBindings()");
            THZ.RefreshBindings(false);
        }

        if (xuic_vehPartStackGrid.previewWindow != null)
        {
            xuic_vehPartStackGrid.previewWindow.IsPreviewDirty = true;
        }
    }


    public static void SetParts(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid, global::VehiclePart[] stackList)
    {
        if (stackList == null)
        {
            return;
        }
        int num = 0;
        global::XUiC_ItemInfoWindow infoWindow = (global::XUiC_ItemInfoWindow)xuic_vehPartStackGrid.xui.GetChildByType<global::XUiC_ItemInfoWindow>();
        int num2 = 0;
        while (num2 < stackList.Length && xuic_vehPartStackGrid.itemControllers.Length > num && stackList.Length > num2)
        {
            if (!(stackList[num2].GetSlotType() == string.Empty))
            {
                global::XUiC_VehiclePartStack xuiC_VehiclePartStack = (global::XUiC_VehiclePartStack)xuic_vehPartStackGrid.itemControllers[num];

                if (stackList[num2].GetProperties().Values.ContainsKey("custom_slot_type"))
                {
                    DebugMsg("XUiC_VehiclePartStackGrid.SetParts(): custom_slot_type = " + stackList[num2].GetProperties().Values["custom_slot_type"]);
                    xuiC_VehiclePartStack.customSlotType = stackList[num2].GetProperties().Values["custom_slot_type"];
                }
                else
                {
                    xuiC_VehiclePartStack.customSlotType = string.Empty;
                    DebugMsg("XUiC_VehiclePartStackGrid.SetParts(): NO custom_slot_type found");
                }

                if(stackList[num2].GetProperties().Values.ContainsKey("empty_basket_on_remove"))
                {
                    DebugMsg("XUiC_VehiclePartStackGrid.SetParts(): empty_basket_on_remove prop exists.");
                    bool emptyBasket = true;
                    if(bool.TryParse(stackList[num2].GetProperties().Values["empty_basket_on_remove"], out emptyBasket))
                    {
                        DebugMsg("setting basket bEmptyBasketOnRemove prop from xml");
                        xuiC_VehiclePartStack.bEmptyBasketOnRemove = emptyBasket;
                    }
                    else
                    {
                        DebugMsg("setting basket bEmptyBasketOnRemove prop to default");
                        xuiC_VehiclePartStack.bEmptyBasketOnRemove = true;
                    }
                    DebugMsg("emptyBasket = " + emptyBasket.ToString() + " | bEmptyBasketOnRemove = " + xuiC_VehiclePartStack.bEmptyBasketOnRemove.ToString());
                }
                else
                {
                    DebugMsg("XUiC_VehiclePartStackGrid.SetParts(): empty_basket_on_remove does NOT exist.");
                }

                xuiC_VehiclePartStack.SlotType = stackList[num2].GetSlotType();
                xuiC_VehiclePartStack.SlotChangedEvent -= xuic_vehPartStackGrid.HandleSlotChangedEvent;
                xuiC_VehiclePartStack.SlotChangingEvent -= xuic_vehPartStackGrid.HandleSlotChangingEvent;
                xuiC_VehiclePartStack.VehiclePart = stackList[num2];
                xuiC_VehiclePartStack.SlotChangedEvent += xuic_vehPartStackGrid.HandleSlotChangedEvent;
                xuiC_VehiclePartStack.SlotChangingEvent += xuic_vehPartStackGrid.HandleSlotChangingEvent;
                xuiC_VehiclePartStack.SlotNumber = num;
                xuiC_VehiclePartStack.InfoWindow = infoWindow;
                xuiC_VehiclePartStack.StackLocation = xuic_vehPartStackGrid.StackLocation;
                num++;
            }
            num2++;
        }
        xuic_vehPartStackGrid.StorageContainer.ShowStorage(xuic_vehPartStackGrid.CurrentVehicle.HasStorage());
    }
}


class ItemClass_patchFunctions
{
    public static void Init_addition(ItemClass itemClass)
    {
        itemClass.bIsVehicleCustomPart = false;
        if (itemClass.Properties.Values.ContainsKey("IsVehicleCustomPart"))
        {
            bool.TryParse(itemClass.Properties.Values["IsVehicleCustomPart"], out itemClass.bIsVehicleCustomPart);
        }
    }
}