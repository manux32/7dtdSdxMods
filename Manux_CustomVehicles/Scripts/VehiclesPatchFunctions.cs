using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Xml;
using System.IO;
using SDX.Core;

class Vehicle_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static bool HasWheels(Vehicle vehicle)
    {
        if (vehicle.GetPartProperty("wheel1", "slot_type") == string.Empty)
            return true;
        return vehicle.GetPartItemValueByTag("wheel1").type != 0;
    }

    public static bool HasStorage(Vehicle vehicle)
    {
        string hasBuiltInStorageString = vehicle.GetPartProperty("storage", "is_built-in_storage");
        if (hasBuiltInStorageString != string.Empty)
        {
            bool hasBuiltInStorage;
            if (bool.TryParse(hasBuiltInStorageString, out hasBuiltInStorage))
            {
                if (hasBuiltInStorage)
                    return true;
            }
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
            UnityEngine.Debug.Log(msg);
        }
    }

    // Game doesn't like this, crash on load game - was for Testing different size storage
    /*public static void Init(EntityVehicle entityVehicle, int _entityClass)
    {
        entityVehicle.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[entityVehicle.entityClass];
        entityVehicle.vehicle = new Vehicle(entityClass.entityClassName, entityVehicle);
        (entityVehicle.inventory as EntityVehicle.MyDummyInventory).SetupSlots();
        entityVehicle.setupEntitySlotInfo();
        //Vector2i size = LootContainer.lootList[entityVehicle.GetLootList()].size;
        Vector2i size = new Vector2i(6, 7);
        entityVehicle.bag.SetupSlots(ItemStack.CreateArray(size.x * size.y));

        CharacterController MI;
        //var listOfFieldNames = typeof(Vehicle).GetFields();
        var listOfFieldNames = typeof(CharacterController).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            FieldInfo field = null;
            if (fieldInfo.FieldType == typeof(CharacterController))
            {
                field = fieldInfo;
                if (field != null)
                {
                    MI = (CharacterController)field.GetValue(entityVehicle);
                    DebugMsg("Found CharacterController MI");
                }
            }
        }

        MI = entityVehicle.PhysicsTransform.GetComponent<CharacterController>();
    }*/

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
        AddOrFixPartsInSlots(vehicle, "vehicleGun", "vehicle50calGun");
        AddOrFixPartsInSlots(vehicle, "vehicleExplosiveLauncher", "vehicleGrenadeLauncher");
    }
}


class XUiM_Vehicle_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
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
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static string GetPartName(XUiC_BasePartStack xuic_basePartStack)
    {
        DebugMsg("xuic_basePartStack.slotType = " + xuic_basePartStack.slotType);
        if (xuic_basePartStack.slotType == "empty")
            return string.Empty;

        if (xuic_basePartStack.itemClass == null && xuic_basePartStack.customSlotType != string.Empty)
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

        //return (xuic_basePartStack.itemClass == null) ? string.Format("[MISSING {0}]", xuic_basePartStack.SlotType).ToUpper() : xuic_basePartStack.itemClass.localizedName.ToUpper();
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
            UnityEngine.Debug.Log(msg);
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
            //DebugMsg("XUiC_VehiclePartStack.SetEmptySpriteName() setting slotType from customSlotType");
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
            /*case "vehicleGun":
                xuic_vehPartStack.emptySpriteName = "gunAK47";
                break;
            case "vehicleExplosiveLauncher":
                xuic_vehPartStack.emptySpriteName = "gunRocketLauncher";
                break;*/
            case "empty":
                xuic_vehPartStack.emptySpriteName = string.Empty;
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
            UnityEngine.Debug.Log(msg);
        }
    }

    /*public static void HandleSlotChangedEvent(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid, int slotNumber, ItemStack stack)
    {
        XUiC_VehiclePartStack xuiC_VehiclePartStack = (XUiC_VehiclePartStack)xuic_vehPartStackGrid.itemControllers[slotNumber];

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
    }*/

    /*public static void OnOpen_addition(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid)
    {
        xuic_vehPartStackGrid.RefreshParts();
    }

    public static void RefreshParts(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid)
    {
        //for (int i = 0; i < xuic_vehPartStackGrid.itemControllers.Length; i++)
        for (int i = 0; i < 10; i++)
        {
            ((XUiC_VehiclePartStack)xuic_vehPartStackGrid.itemControllers[i]).RefreshBindings(true);
        }
    }*/

    public static void SetParts(XUiC_VehiclePartStackGrid xuic_vehPartStackGrid, VehiclePart[] stackList)
    {
        if (stackList == null)
        {
            return;
        }
        int num = 0;
        XUiC_ItemInfoWindow infoWindow = (XUiC_ItemInfoWindow)xuic_vehPartStackGrid.xui.GetChildByType<XUiC_ItemInfoWindow>();
        int num2 = 0;
        while (num2 < stackList.Length && xuic_vehPartStackGrid.itemControllers.Length > num && stackList.Length > num2)
        {
            if (!(stackList[num2].GetSlotType() == string.Empty))
            {
                XUiC_VehiclePartStack xuiC_VehiclePartStack = (XUiC_VehiclePartStack)xuic_vehPartStackGrid.itemControllers[num];

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

                /*if(stackList[num2].GetProperties().Values.ContainsKey("empty_basket_on_remove"))
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
                }*/

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
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static void Init_addition(ItemClass itemClass)
    {
        itemClass.bIsVehicleCustomPart = false;
        if (itemClass.Properties.Values.ContainsKey("IsVehicleCustomPart"))
        {
            bool.TryParse(itemClass.Properties.Values["IsVehicleCustomPart"], out itemClass.bIsVehicleCustomPart);
        }
    }
}


/*class XUiC_VehicleContainer_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }


    public static void SetSlots_addition(XUiC_VehicleContainer xuic_vehicleContainer, ItemStack[] stackList)
    {
        xuiV_Grid.Columns = 6;
        xuiV_Grid.Rows = 7;
    }*/

/*public static void SetSlots(XUiC_VehicleContainer xuic_vehicleContainer, ItemStack[] stackList)
{
    if (stackList == null)
    {
        return;
    }
    if (xuic_vehicleContainer.xui.vehicle.GetVehicle() == null)
    {
        return;
    }
    xuic_vehicleContainer.xui.vehicle.bag.OnBackpackItemsChangedInternal += xuic_vehicleContainer.OnBagItemChangedInternal;
    xuic_vehicleContainer.items = stackList;
    XUiC_ItemInfoWindow infoWindow = (XUiC_ItemInfoWindow)xuic_vehicleContainer.xui.GetChildByType<XUiC_ItemInfoWindow>();
    XUiV_Grid xuiV_Grid = (XUiV_Grid)xuic_vehicleContainer.viewComponent;
    xuiV_Grid.Columns = xuic_vehicleContainer.xui.vehicle.lootContainer.GetContainerSize().x;
    xuiV_Grid.Rows = xuic_vehicleContainer.xui.vehicle.lootContainer.GetContainerSize().y;
    //xuiV_Grid.Columns = 6;
    //xuiV_Grid.Rows = 7;
    int num = stackList.Length;
    for (int i = 0; i < xuic_vehicleContainer.itemControllers.Length; i++)
    //for (int i = 0; i < 42; i++)
    {
        XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)xuic_vehicleContainer.itemControllers[i];
        xuiC_ItemStack.InfoWindow = infoWindow;
        xuiC_ItemStack.SlotNumber = i;
        xuiC_ItemStack.SlotChangedEvent -= xuic_vehicleContainer.HandleLootSlotChangedEvent;
        xuiC_ItemStack.InfoWindow = infoWindow;
        xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.LootContainer;
        xuiC_ItemStack.UnlockStack();
        if (i < num)
        {
            xuiC_ItemStack.ForceSetItemStack(xuic_vehicleContainer.items[i]);
            xuic_vehicleContainer.itemControllers[i].ViewComponent.IsVisible = true;
            xuiC_ItemStack.SlotChangedEvent += xuic_vehicleContainer.HandleLootSlotChangedEvent;
        }
        else
        {
            xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
            xuic_vehicleContainer.itemControllers[i].ViewComponent.IsVisible = false;
        }
    }
}
}*/


class XUiC_VehicleWindowGroup_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    // for different size storage to fallback to the default XUiC_VehicleWindowGroup.ID value ("vehicle"), when the window is closed.
    public static void OnClose_addition()
    {
        XUiC_VehicleWindowGroup.ID = "vehicle";
    }
}


class TileEntityLootContainer_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static void SetContainerSize(TileEntityLootContainer tileEntityLootContainer, Vector2i _containerSize, bool clearItems)
    {
        DebugMsg("TileEntityLootContainer.SetContainerSize:");
        //Vector2i MKZ;
        FieldInfo MKZ_field = null;
        var listOfFieldNames = typeof(TileEntityLootContainer).GetFields();
        //var listOfFieldNames = typeof(TileEntityLootContainer).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            if (fieldInfo.FieldType == typeof(Vector2i))
            {
                MKZ_field = fieldInfo;
                if (MKZ_field != null)
                {
                    //MKZ = (Vector2i)MKZ_field.GetValue(tileEntityLootContainer);
                    DebugMsg("Found Vector2i MKZ");
                    break;
                }
            }
        }

        Entity entity = GameManager.Instance.World.GetEntity(tileEntityLootContainer.entityId);
        if(entity != null && (entity.GetType() == typeof(EntityCustomVehicle) || entity.GetType().IsSubclassOf(typeof(EntityCustomVehicle))))
        {
            EntityCustomVehicle vehicle = (EntityCustomVehicle)entity;
            DebugMsg("TileEntityLootContainer.SetContainerSize: is EntityCustomVehicle");
            _containerSize = vehicle.storageSize;
        }

        //MKZ = _containerSize;
        if (MKZ_field != null)
        {
            DebugMsg("Setting Vector2i MKZ_field to: " + _containerSize.ToString());
            MKZ_field.SetValue(tileEntityLootContainer, _containerSize);
        }
        if (clearItems)
        {
            tileEntityLootContainer.items = ItemStack.CreateArray(_containerSize.x * _containerSize.y);
        }
    }
}


class UISprite_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    //value = UISprite_patchFunctions.GetProperAtlas(this, value);

    public static UIAtlas GetProperAtlas(UISprite sprite, UIAtlas atlas)
    {
        EntityPlayerLocal player = GameManager.Instance.World.GetLocalPlayer() as EntityPlayerLocal;
        LocalPlayerUI uiforPlayer = null;
        if (player != null)
        {
            uiforPlayer = LocalPlayerUI.GetUIForPlayer(player);
        }

        if (uiforPlayer == null)
        {
            return atlas;
        }

        if (sprite.spriteName.ToLower().StartsWith("manux_"))
        {
            DebugMsg("GetProperAtlas for icon: " + sprite.spriteName);
            return uiforPlayer.xui.GetAtlasByName("itemIconAtlas");
        }
        return uiforPlayer.xui.GetAtlasByName("UIAtlas");
    }

    public static void SetProperAtlas(UISprite sprite)
    {
        if (GameManager.Instance == null || !GameManager.Instance.gameStateManager.IsGameStarted())
            return;

        EntityPlayerLocal player = GameManager.Instance.World.GetLocalPlayer() as EntityPlayerLocal;
        LocalPlayerUI uiforPlayer = null;
        if (player != null)
        {
            uiforPlayer = LocalPlayerUI.GetUIForPlayer(player);
        }

        if (uiforPlayer == null)
        {
            return;
        }

        if (sprite.spriteName.ToLower().StartsWith("manux_"))
        {
            DebugMsg("GetProperAtlas for icon: " + sprite.spriteName);
            UIAtlas itemIconAtlas = uiforPlayer.xui.GetAtlasByName("itemIconAtlas");
            if (sprite.atlas != itemIconAtlas)
            {
                sprite.atlas = itemIconAtlas;
                //sprite.mChanged = true;
            }
        }
        //sprite.atlas = uiforPlayer.xui.GetAtlasByName("UIAtlas");
    }
}


class XUiC_CompassWindow_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static void Update_addition(XUiC_CompassWindow xuic_CompassWindow)
    {
        EntityPlayerLocal player = GameManager.Instance.World.GetLocalPlayer() as EntityPlayerLocal;
        LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(player);

        foreach (UISprite sprite in xuic_CompassWindow.waypointSpriteList)
        {
            if(sprite.spriteName.ToLower().StartsWith("manux_"))
            {
                //DebugMsg("Setting itemIconAtlas for icon: " + sprite.spriteName);
                sprite.atlas = uiforPlayer.xui.GetAtlasByName("itemIconAtlas");
            }
            else
            {
                sprite.atlas = uiforPlayer.xui.GetAtlasByName("UIAtlas");
            }
        }
    }
}

class MapObjectVehicle_patchFunctions : MapObjectVehicle
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public MapObjectVehicle_patchFunctions(Entity _entity) : base(_entity)
    {
    }

    public string GetProperCompassIcon()
    {
        if (this.type == EnumMapObjectType.Entity && this.entity != null)
        {
            //DebugMsg("Map Icon = " + this.entity.GetMapIcon());
            //DebugMsg("Compass Icon = " + this.entity.GetCompassIcon());
            return this.entity.GetCompassIcon();
        }
        return null;
    }
}


class ModManager_patchFunctions
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }


    public static void PrintMapObjects(EnumMapObjectType mapObjectType)
    {
        DebugMsg("StartGame_additions: Map Objects (" + mapObjectType.ToString() + "):");
        List<MapObject> mapObjects = GameManager.Instance.World.GetObjectOnMapList(mapObjectType);
        foreach (MapObject mapObject in mapObjects)
        {
            DebugMsg("\t - " + mapObject.GetName() + " | icon = " + mapObject.GetMapIcon() + " | mapObject.key = " + mapObject.key.ToString());
        }
    }


    public static UISpriteData SetUISpriteData(UISpriteData srcUiSpriteData, string name, int x, int y)
    {
        UISpriteData newUiSpriteData = new UISpriteData();
        newUiSpriteData.CopyFrom(srcUiSpriteData);

        newUiSpriteData.name = name;
        newUiSpriteData.x = x;
        newUiSpriteData.y = y;
        return newUiSpriteData;
    }

    public static void AddNewGameSymbolsDataToUiAtlas(UIAtlas uiAtlas)
    {
        List<UISpriteData> ZZ = null;
        FieldInfo field = null;
        var listOfFieldNames = typeof(UIAtlas).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            if (fieldInfo.FieldType == typeof(List<UISpriteData>))
            {
                field = fieldInfo;
            }
        }

        if(field == null)
        {
            DebugMsg("AddNewGameSymbolsDataToUiAtlas: Cannot find ZZ Field. Aborting!");
            return;
        }

        DebugMsg("AddNewGameSymbolsDataToUiAtlas: ZZ Field found.");

        ZZ = (List<UISpriteData>)field.GetValue(uiAtlas);

        UISpriteData minibikeUiSpriteData = null;
        foreach(UISpriteData uiSpriteData in ZZ)
        {
            if(uiSpriteData.name == "ui_game_symbol_minibike")
            {
                minibikeUiSpriteData = uiSpriteData;
                break;
            }
        }

        if (minibikeUiSpriteData == null)
        {
            DebugMsg("AddNewGameSymbolsDataToUiAtlas: minibike UiSpriteData cannot be found. Aborting!");
            return;
        }

        DebugMsg("AddNewGameSymbolsDataToUiAtlas: minibike UiSpriteData found.");
        //DebugMsg("AddNewGameSymbolsDataToUiAtlas: minibikeUiSpriteData: " + minibikeUiSpriteData.name + "(" + minibikeUiSpriteData.x.ToString() + ", " + minibikeUiSpriteData.y.ToString() + ")" + "[" + minibikeUiSpriteData.width.ToString() + ", " + minibikeUiSpriteData.height.ToString() + "]");

        try
        {
            DebugMsg("AddNewGameSymbolsDataToUiAtlas: Reading new game symbols definitions from xml file.");
            string newUiGameSymbolsDefXmlFile = Utils.GetGameDir("Mods/SDX/UI");
            newUiGameSymbolsDefXmlFile += "/new_ui_game_symbols_definitions.xml";
            DebugMsg("newUiGameSymbolsDefXmlFile = " + newUiGameSymbolsDefXmlFile);

            XmlDocument document = new XmlDocument();
            document.Load(newUiGameSymbolsDefXmlFile);
            XmlElement root = document.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                AddNewGameSymbolDefinition(ref ZZ, minibikeUiSpriteData, node);
            }

            field.SetValue(uiAtlas, ZZ);
            DebugMsg("AddNewGameSymbolsDataToUiAtlas: Finished adding new game symbols.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }


    public static void AddNewGameSymbolDefinition(ref List<UISpriteData> iconsDefData, UISpriteData srcUiSpriteData, XmlNode xmlNode)
    {
        if(xmlNode.Attributes == null || xmlNode.Attributes["name"] == null)
        {
            Debug.LogWarning("Warning!: AddNewGameSymbolDefinition: Xml Node or 'name' attribute is null, skipping.");
            return;
        }

        Vector2i iconPos;
        if (CustomVehiclesUtils.StringVectorToVector2i(xmlNode.Attributes["atlas_pos"].Value, out iconPos))
        {
            DebugMsg("Adding new ui_game_symbol: " + xmlNode.Attributes["name"].Value + " (" + iconPos.x.ToString() + ", " + iconPos.y.ToString() + ")");
            iconsDefData.Add(SetUISpriteData(srcUiSpriteData, xmlNode.Attributes["name"].Value, iconPos.x, iconPos.y));
        }
    }


    public static void PrintBlipMapObjects()
    {
        DebugMsg("StartGame_additions: Blip Map Objects: (" + GameManager.Instance.World.Blips.ActiveBlips.Count.ToString() + ")");
        List<BlipMapObject> blipMapObjects = GameManager.Instance.World.Blips.ActiveBlips;
        foreach (BlipMapObject blipMapObject in blipMapObjects)
        {
            DebugMsg("\t - " + blipMapObject.spriteName);
            /*if (blipMapObject.spriteName == "manux_ui_game_symbol_vehicle")
            {
                UISprite uiSprite = blipMapObject.sprite;
                uiSprite.atlas = itemIconAtlas;
            }*/

        }
    }

    public static void PrintSceneContent()
    {
        GameObject nguiRoot2D = GameObject.Find("/NGUI Root (2D)");
        if (nguiRoot2D == null)
        {
            DebugMsg("StartGame_additions: Cannot find 'NGUI Root (2D)'");
            return;
        }

        GameObject root = CustomVehiclesUtils.GetRootTransform(nguiRoot2D.transform).gameObject;

        Component[] transforms = root.GetComponentsInChildren<Transform>();
        DebugMsg("StartGame_additions: Root UIAtlas Children:");
        foreach (Transform transform in transforms)
        {
            DebugMsg("- " + transform.name);
            Component[] comps = transform.gameObject.GetComponents<Component>();
            DebugMsg("Components:");
            foreach (Component comp in comps)
            {
                DebugMsg("\t- " + comp.name + " | type = " + comp.GetType().ToString());
                if (comp.GetType() == typeof(UIAtlas) || comp.GetType().IsSubclassOf(typeof(UIAtlas)))
                    DebugMsg("\t- UIAtlas: " + transform.name + " | comp = " + comp.name + " | type = " + comp.GetType().ToString());
            }
        }
    }

    public static void PrintMapWaypoints()
    {
        // Print Map waypoints
        PrintMapObjects(EnumMapObjectType.MapMarker);   // Prints vehicles
        PrintMapObjects(EnumMapObjectType.Entity);
        PrintMapObjects(EnumMapObjectType.SleepingBag);
        PrintMapObjects(EnumMapObjectType.StartPoint);
        PrintMapObjects(EnumMapObjectType.Backpack);
        PrintMapObjects(EnumMapObjectType.Prefab);
        PrintMapObjects(EnumMapObjectType.EntitySpawner);
        PrintMapObjects(EnumMapObjectType.MapMarker);
        PrintMapObjects(EnumMapObjectType.MapQuickMarker);
        PrintMapObjects(EnumMapObjectType.TreasureChest);
        PrintMapObjects(EnumMapObjectType.Quest);
        PrintMapObjects(EnumMapObjectType.SupplyDrop);
        PrintMapObjects(EnumMapObjectType.VendingMachine);
    }

    

    public static void GameStartDone_additions()
    {
        EntityPlayerLocal player = GameManager.Instance.World.GetLocalPlayer() as EntityPlayerLocal;
        LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(player);
        UIAtlas uiAtlas = uiforPlayer.xui.GetAtlasByName("UIAtlas");
        UIAtlas itemIconAtlas = uiforPlayer.xui.GetAtlasByName("itemIconAtlas");

        // Add new ui_game_symbols icons positions to the UIAtlas
        AddNewGameSymbolsDataToUiAtlas(uiAtlas);

        //PrintSceneContent();
        //PrintBlipMapObjects();
        //PrintMapWaypoints();
        //ImageManipUtils.PrintProjectTextures();
        //ImageManipUtils.PrintItemIconAtlasesSprites();

        // Swap the UIAtlas image with our modified version
        var uiIconsDestPath = Utils.GetGameDir("Mods/SDX/UI");
        DebugMsg("Mods/SDX/UI path = " + uiIconsDestPath);
        DirectoryInfo d = new DirectoryInfo(uiIconsDestPath);
        FileInfo[] Files = d.GetFiles("*.*");

        FieldInfo ZZfield = null;
        FieldInfo KZfield = null;
        var listOfFieldNames = typeof(UIAtlas).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            if (fieldInfo.FieldType == typeof(List<UISpriteData>))
            {
                ZZfield = fieldInfo;
            }

            if (fieldInfo.FieldType == typeof(Material))
            {
                KZfield = fieldInfo;
            }
        }

        /*DebugMsg("UISpriteData BEFORE:");
        List<UISpriteData> uiSpriteData = (List<UISpriteData>)ZZfield.GetValue(uiAtlas);
        foreach(UISpriteData sd in uiSpriteData)
        {
            //DebugMsg("\t - " + sd.name + " (" + sd.x + ", " + sd.y + ")");
        }*/

        DebugMsg("Mods UI files:");
        foreach (FileInfo file in Files)
        {
            //DebugMsg("\t- " + file.FullName);
            //DebugMsg("\t- " + file.Name);
            if (Path.GetFileNameWithoutExtension(file.Name) == "UIAtlas")
            {
                UnityEngine.Debug.LogWarning("Found new UIAtlas in Custom Vehicles mod, switching texture image to new UIAtlas.");
                byte[] fileData;
                fileData = File.ReadAllBytes(file.FullName);

                Texture2D text2Dsrc = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
                Material mat = (Material)KZfield.GetValue(uiAtlas);
                DebugMsg("original texture setting (BEFORE): size = " + mat.mainTexture.width.ToString() + " x " + mat.mainTexture.height.ToString() + " | mipMapBias = " + mat.mainTexture.mipMapBias.ToString("0.0000") + " | filterMode = " + mat.mainTexture.filterMode.ToString() + " | anisoLevel = " + mat.mainTexture.anisoLevel.ToString() + " | wrapMode = " + mat.mainTexture.wrapMode.ToString());
                DebugMsg("new texture settings (BEFORE): size = " + text2Dsrc.width.ToString() + " x " + text2Dsrc.height.ToString() + " | format = " + text2Dsrc.format.ToString() + " | mipMapBias = " + text2Dsrc.mipMapBias.ToString("0.0000") + " | filterMode = " + text2Dsrc.filterMode.ToString() + " | anisoLevel = " + text2Dsrc.anisoLevel.ToString() + " | wrapMode = " + text2Dsrc.wrapMode.ToString());
                text2Dsrc.mipMapBias = mat.mainTexture.mipMapBias;
                text2Dsrc.filterMode = mat.mainTexture.filterMode;
                text2Dsrc.anisoLevel = mat.mainTexture.anisoLevel;
                text2Dsrc.wrapMode = mat.mainTexture.wrapMode;
                DebugMsg("original texture setting (AFTER): size = " + mat.mainTexture.width.ToString() + " x " + mat.mainTexture.height.ToString() + " | mipMapBias = " + mat.mainTexture.mipMapBias.ToString("0.0000") + " | filterMode = " + mat.mainTexture.filterMode.ToString() + " | anisoLevel = " + mat.mainTexture.anisoLevel.ToString() + " | wrapMode = " + mat.mainTexture.wrapMode.ToString());
                DebugMsg("new texture settings (AFTER): size = " + text2Dsrc.width.ToString() + " x " + text2Dsrc.height.ToString() + " | format = " + text2Dsrc.format.ToString() + " | mipMapBias = " + text2Dsrc.mipMapBias.ToString("0.0000") + " | filterMode = " + text2Dsrc.filterMode.ToString() + " | anisoLevel = " + text2Dsrc.anisoLevel.ToString() + " | wrapMode = " + text2Dsrc.wrapMode.ToString());

                text2Dsrc.LoadImage(fileData);

                text2Dsrc.Compress(true);
                text2Dsrc.Apply(true, true);
                DebugMsg("new texture settings (AFTER COMPRESS and APPLY): size = " + text2Dsrc.width.ToString() + " x " + text2Dsrc.height.ToString() + " | format = " + text2Dsrc.format.ToString() + " | mipMapBias = " + text2Dsrc.mipMapBias.ToString("0.0000") + " | filterMode = " + text2Dsrc.filterMode.ToString() + " | anisoLevel = " + text2Dsrc.anisoLevel.ToString() + " | wrapMode = " + text2Dsrc.wrapMode.ToString());
                mat.mainTexture = text2Dsrc;
            }
        }
    }
}


class XUiC_VehicleContainer_patchFunctions : XUiC_VehicleContainer
{
    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public void SubInit()
    {
        //this.takeAllLabel = (XUiV_Label)base.GetChildById("takeAllLabel").ViewComponent;
        //this.takeAllLabel.TruetypeFont = null;
        this.btnTakeAll = base.Parent.GetChildById("btnTakeAll");
        if (this.btnTakeAll != null)
        {
            this.btnTakeAll.OnPress += this.OnButtonTakeAll;
        }
        this.btnDropAll = base.Parent.GetChildById("btnDropAll");
        if (this.btnDropAll != null)
        {
            this.btnDropAll.OnPress += this.OnButtonDropAll;
        }
    }

    public void ButtonTakeAll()
    {
        if (base.xui.vehicle.GetVehicle() == null)
        {
            return;
        }
        bool result = TakeAll();
        if (result && GameManager.Instance.World != null)
        {
            Audio.Manager.BroadcastPlayByLocalPlayer(base.xui.vehicle.transform.position + Vector3.one * 0.5f, "UseActions/takeall1");
        }
    }

    public void ButtonDropAll()
    {
        if (base.xui.vehicle.GetVehicle() == null)
        {
            return;
        }

        ItemStack[] slots = base.xui.vehicle.bag.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty())
            {
                GameManager.Instance.ItemDropServer(slots[i].Clone(), base.xui.vehicle.transform.position, Vector3.zero, -1, 60f, false);
                base.xui.vehicle.bag.SetSlot(i, ItemStack.Empty.Clone(), true);
            }
        }

        if (GameManager.Instance.World != null)
        {
            Audio.Manager.BroadcastPlayByLocalPlayer(base.xui.vehicle.transform.position + Vector3.one * 0.5f, "UseActions/takeall1");
        }
    }
}


class XUiC_BackpackWindow_patchFunctions : XUiC_BackpackWindow
{
    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public void SubInit()
    {
        this.btnStashAll = base.Parent.GetChildById("btnStashAll");
        if (this.btnStashAll != null)
        {
            this.btnStashAll.OnPress += this.OnButtonStashAll;
        }
        this.btnStashAllButFirst = base.Parent.GetChildById("btnStashAllButFirst");
        if (this.btnStashAllButFirst != null)
        {
            this.btnStashAllButFirst.OnPress += this.OnButtonStashAllButFirst;
        }
    }


    public void ButtonStashAll()
    {
        XUiC_ItemStackGrid xuiC_ItemStackGrid = (XUiC_ItemStackGrid)base.GetChildByType<XUiC_ItemStackGrid>();
        XUiController[] slots = xuiC_ItemStackGrid.itemControllers;
        for (int i = 0; i < slots.Length; i++)
        {
            {
                XUiC_ItemStack xuic_ItemStack = (XUiC_ItemStack)slots[i];
                xuic_ItemStack.HandleMoveToPreferredLocation();
            }
        }
    }

    public void ButtonStashAllButFirst()
    {
        XUiC_ItemStackGrid xuiC_ItemStackGrid = (XUiC_ItemStackGrid)base.GetChildByType<XUiC_ItemStackGrid>();
        XUiController[] slots = xuiC_ItemStackGrid.itemControllers;
        XUiV_Grid xuiv_Grid = (XUiV_Grid)xuiC_ItemStackGrid.viewComponent;

        for (int i = xuiv_Grid.Columns; i < slots.Length; i++)
        {
            {
                XUiC_ItemStack xuic_ItemStack = (XUiC_ItemStack)slots[i];
                xuic_ItemStack.HandleMoveToPreferredLocation();
            }
        }
    }
}


/*public static class ModManager_additions
{
    private static DictionaryList<string, Mod> ZZ = new DictionaryList<string, Mod>();

    public static IEnumerator LoadUIIcons()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        GameObject gameObject = GameObject.Find("/NGUI Root (2D)/ItemIconAtlas");
        if (gameObject == null)
        {
            Log.Warning("[MODS] Could not load custom icons: Atlas object not found");
            yield break;
        }
        DynamicUIAtlas component = gameObject.GetComponent<DynamicUIAtlas>();
        if (component == null)
        {
            Log.Warning("[MODS] Could not load custom icons: Atlas component not found");
            yield break;
        }
        //yield return null;
        //return 1;
        component.ResetAtlas();
        //yield return null;
        //return 1;
        Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
        MicroStopwatch microStopwatch = new MicroStopwatch(true);
        int num = 0;
        goto IL_131;
        microStopwatch.ResetAndRestart();
    IL_102:
        int num2;
        num2++;
    IL_110:
        string[] array;
        if (num2 < array.Length)
        {
            goto IL_1CA;
        }
    IL_123:
        num++;
    IL_131:
        if (num >= ZZ.Count)
        {
            goto IL_2A0;
        }
        Mod mod = ZZ.list[num];
        string path = mod.Path + "/ItemIcons";
        if (!Directory.Exists(path))
        {
            goto IL_123;
        }
        array = null;
        try
        {
            array = Directory.GetFiles(path);
            goto IL_290;
        }
        catch (Exception ex)
        {
            Exception e = ex;
            Log.Exception(e);
            goto IL_290;
        }
    IL_1BE:
        num2 = 0;
        goto IL_110;
    IL_1CA:
        string text = array[num2];
        try
        {
            if (text.ToLower().EndsWith(".png"))
            {
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                if (texture2D.LoadImage(File.ReadAllBytes(text)))
                {
                    dictionary.Add(Path.GetFileNameWithoutExtension(text), texture2D);
                }
                else
                {
                    UnityEngine.Object.Destroy(texture2D);
                }
            }
        }
        catch (Exception ex2)
        {
            Exception e2 = ex2;
            Log.Error("Adding file " + text + " failed:");
            Log.Exception(e2);
        }
        if (microStopwatch.ElapsedMilliseconds <= 90L)
        {
            goto IL_102;
        }
        yield return null;
        //return 1;
    IL_290:
        if (array == null)
        {
            goto IL_123;
        }
        goto IL_1BE;
    IL_2A0:
        yield return null;
        //return 1;
        if (dictionary.Count <= 0)
        {
            goto IL_3D1;
        }
        try
        {
            component.LoadAdditionalSprites(dictionary);
        }
        catch (Exception ex3)
        {
            Exception e3 = ex3;
            Log.Exception(e3);
        }
        yield return null;
        //return 1;
        try
        {
            foreach (Texture2D obj in dictionary.Values)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
        catch (Exception ex4)
        {
            Exception e4 = ex4;
            Log.Exception(e4);
        }
        stopwatch.Stop();
        Log.Out("Adding {0} sprites to atlas took {1} ms", new object[]
        {
        dictionary.Count,
        stopwatch.ElapsedMilliseconds
        });
        yield return null;
        //return 1;
    IL_3D1:
        component.Compress();
        yield break;
    }
}*/
