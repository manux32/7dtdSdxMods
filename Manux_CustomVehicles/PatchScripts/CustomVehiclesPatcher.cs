using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SDX.Core;
using SDX.Compiler;
using SDX.Payload;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
//using System.Reflection;


class CustomVehiclesPatcher : IPatcherMod
{
    static bool bEnableCustomStorage = true;
    static bool bEnableNewUiButtons = true;
    static bool bEnableCustomVehicleIcons = true;
    static bool bEnableProperCompassIcons = true;
    static bool bEnableCustomVehicleParts = true;

    static string SP_DataFolder = GlobalVariables.Parse("${GameDir}/7DaysToDie_Data");
    static string DEDI_DataFolder = GlobalVariables.Parse("${GameDir}/7DaysToDieServer_Data");

    static bool IsDedi()
    {
        return Directory.Exists(DEDI_DataFolder);
    }

    public bool Patch(ModuleDefinition module)
    {
        FieldDefinition field;
        TypeDefinition gameClass;
        MethodDefinition gameMethod;
        TypeReference voidTypeRef = module.TypeSystem.Void;

        Console.WriteLine("==Custom Vehicles Patcher===");

        /*gameClass = module.Types.First(d => d.Name == "Entity");
        gameMethod = gameClass.Methods.First(d => d.Name == "IsInWater");
        SetMethodToPublic(gameMethod);
        SetMethodToVirtual(gameMethod);

        if (IsDedi())
        {
            gameMethod = gameClass.Methods.First(d => d.Name == "XQA");
            SetMethodToPublic(gameMethod);
            SetMethodToVirtual(gameMethod);
        }
        else
        {
            gameMethod = gameClass.Methods.First(d => d.Name == "HXQ");
            SetMethodToPublic(gameMethod);
            SetMethodToVirtual(gameMethod);
        }*/

        /*gameMethod = gameClass.Methods.First(d => d.Name == "isHeadUnderwater");
        SetMethodToPublic(gameMethod);
        SetMethodToVirtual(gameMethod);*/

        gameClass = module.Types.First(d => d.Name == "EntityVehicle");
        gameMethod = gameClass.Methods.First(d => d.Name == "isDriveable");
        SetMethodToPublic(gameMethod);
        SetMethodToVirtual(gameMethod);

        //var field = gameClass.Fields.First(d => d.Name == "vehicle");
        //SetFieldToPublic(field);

        //field = gameClass.Fields.First(d => d.FieldType.Name == "CharacterController");
        //SetFieldToPublic(field);

        // EntityVehicle.TS
        //field = gameClass.Fields.First(d => d.FieldType.Name == "Vector3i");
        //SetFieldToPublic(field);

        //gameMethod = gameClass.Methods.First(d => d.Name == "setupEntitySlotInfo");
        //SetMethodToPublic(gameMethod);

        //gameClass = module.Types.First(d => d.Name == "Entity");
        //gameMethod = gameClass.Methods.First(d => d.Name == "onUnderwaterStateChanged");
        //SetMethodToPublic(gameMethod);

        gameClass = module.Types.First(d => d.Name == "EntityPlayerLocal");
        gameMethod = gameClass.Methods.First(d => d.Name == "updateCameraPosition");
        SetMethodToPublic(gameMethod);

        //gameMethod = gameClass.Methods.First(d => d.Name == "onUnderwaterStateChanged");
        //SetMethodToPublic(gameMethod);

        if (bEnableCustomVehicleParts)
        {
            gameClass = module.Types.First(d => d.Name == "Vehicle");
            //foreach (var f in gameClass.Fields)
            //Logging.LogInfo(string.Format("f name: {0} type: {1}", f.Name, f.FieldType.ToString()));
            field = gameClass.Fields.First(d => d.FieldType.ToString() == "System.Collections.Generic.List`1<VehiclePart>");
            SetFieldToPublic(field);

            gameClass = module.Types.First(d => d.Name == "XUiM_Vehicle");
            field = gameClass.Fields.First(d => d.FieldType.Name == "XUiEvent_ItemChangedVehicle");
            SetFieldToPublic(field);

            gameClass = module.Types.First(d => d.Name == "XUiC_BasePartStack");
            field = gameClass.Fields.First(d => d.Name == "emptySpriteName");
            SetFieldToPublic(field);
            field = gameClass.Fields.First(d => d.Name == "slotType");
            SetFieldToPublic(field);

            gameClass = module.Types.First(d => d.Name == "XUiC_VehiclePartStackGrid");
            field = gameClass.Fields.First(d => d.Name == "itemControllers");
            SetFieldToPublic(field);
            gameMethod = gameClass.Methods.First(d => d.Name == "get_StackLocation");
            SetMethodToPublic(gameMethod);

            gameClass = module.Types.First(d => d.Name == "XUiC_BasePartStack");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetEmptySpriteName");
            SetMethodToPublic(gameMethod);
            field = gameClass.Fields.First(d => d.Name == "itemClass");
            SetFieldToPublic(field);

            gameClass = module.Types.First(d => d.Name == "XUiC_VehiclePartStack");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetEmptySpriteName");
            SetMethodToPublic(gameMethod);

            gameClass = module.Types.First(d => d.Name == "XUiController");
            field = gameClass.Fields.First(d => d.Name == "viewComponent");
            SetFieldToPublic(field);

            gameClass = module.Types.First(d => d.Name == "XUiC_ItemStackGrid");
            field = gameClass.Fields.First(d => d.Name == "items");
            SetFieldToPublic(field);
            field = gameClass.Fields.First(d => d.Name == "itemControllers");
            SetFieldToPublic(field);
        }

        // TileEntityLootContainer.MKZ
        //gameClass = module.Types.First(d => d.Name == "TileEntityLootContainer");
        //field = gameClass.Fields.First(d => d.FieldType.Name == "Vector2i");
        //SetFieldToPublic(field);

        // Add a new field to XUiC_BasePartStack for storing the custom_slot_type
        gameClass = module.Types.First(d => d.Name == "XUiC_BasePartStack");
        FieldDefinition customSlotTypeField = new FieldDefinition("customSlotType", FieldAttributes.Public, module.Import(typeof(string)));
        SetFieldToPublic(customSlotTypeField);
        gameClass.Fields.Add(customSlotTypeField);
        customSlotTypeField.Constant = string.Empty;

        // Add a new field to XUiC_VehiclePartStack for emptying/not emptying the basket when it is remove
        /*FieldDefinition bEmptyBasketOnRemoveField = new FieldDefinition("bEmptyBasketOnRemove", FieldAttributes.Public, module.Import(typeof(bool)));
        SetFieldToPublic(bEmptyBasketOnRemoveField);
        gameClass.Fields.Add(bEmptyBasketOnRemoveField);
        bEmptyBasketOnRemoveField.Constant = true;*/

        // Add a new field to ItemClass for storing IsVehicleCustomPart
        gameClass = module.Types.First(d => d.Name == "ItemClass");
        FieldDefinition isVehicleCustomPartField = new FieldDefinition("bIsVehicleCustomPart", FieldAttributes.Public, module.Import(typeof(bool)));
        SetFieldToPublic(isVehicleCustomPartField);
        gameClass.Fields.Add(isVehicleCustomPartField);
        isVehicleCustomPartField.Constant = false;

        // UIRect.mChanged --> Game does not load at all
        /*gameClass = module.Types.First(d => d.Name == "UIRect");
        field = gameClass.Fields.First(d => d.Name == "mChanged");
        SetFieldToPublic(field);*/

        if (bEnableNewUiButtons)
        {
            // Add new fields to XUiC_VehicleContainer for the new UI buttons
            gameClass = module.Types.First(d => d.Name == "XUiC_VehicleContainer");
            var xuiControllerTypeDef = module.Types.First(d => d.Name == "XUiController");
            FieldDefinition btnTakeAllField = new FieldDefinition("btnTakeAll", FieldAttributes.Public, xuiControllerTypeDef);
            gameClass.Fields.Add(btnTakeAllField);
            FieldDefinition btnDropAllField = new FieldDefinition("btnDropAll", FieldAttributes.Public, xuiControllerTypeDef);
            gameClass.Fields.Add(btnDropAllField);
            /*FieldDefinition takeAllLabelField = new FieldDefinition("takeAllLabel", FieldAttributes.Private, module.Import(typeof(XUiV_Label)));
            gameClass.Fields.Add(takeAllLabelField);*/

            MethodDefinition onButtonTakeAll = new MethodDefinition("OnButtonTakeAll", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            TypeReference onPressEventArgsTypeDef = module.Types.First(d => d.Name == "OnPressEventArgs");
            onButtonTakeAll.Parameters.Add(new ParameterDefinition(xuiControllerTypeDef) { Name = "xuiController" });
            onButtonTakeAll.Parameters.Add(new ParameterDefinition(onPressEventArgsTypeDef) { Name = "onPressEventArgs" });
            gameClass.Methods.Add(onButtonTakeAll);

            MethodDefinition onButtonDropAll = new MethodDefinition("OnButtonDropAll", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            onButtonDropAll.Parameters.Add(new ParameterDefinition(xuiControllerTypeDef) { Name = "xuiController" });
            onButtonDropAll.Parameters.Add(new ParameterDefinition(onPressEventArgsTypeDef) { Name = "onPressEventArgs" });
            gameClass.Methods.Add(onButtonDropAll);

            // Add new fields to XUiC_BackpackWindow for the new UI buttons
            gameClass = module.Types.First(d => d.Name == "XUiC_BackpackWindow");
            FieldDefinition btnStashAllField = new FieldDefinition("btnStashAll", FieldAttributes.Public, xuiControllerTypeDef);
            gameClass.Fields.Add(btnStashAllField);

            MethodDefinition onButtonStashAll = new MethodDefinition("OnButtonStashAll", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            onButtonStashAll.Parameters.Add(new ParameterDefinition(xuiControllerTypeDef) { Name = "xuiController" });
            onButtonStashAll.Parameters.Add(new ParameterDefinition(onPressEventArgsTypeDef) { Name = "onPressEventArgs" });
            gameClass.Methods.Add(onButtonStashAll);

            FieldDefinition btnStashAllButFirstField = new FieldDefinition("btnStashAllButFirst", FieldAttributes.Public, xuiControllerTypeDef);
            gameClass.Fields.Add(btnStashAllButFirstField);

            MethodDefinition onButtonStashAllButFirst = new MethodDefinition("OnButtonStashAllButFirst", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            onButtonStashAllButFirst.Parameters.Add(new ParameterDefinition(xuiControllerTypeDef) { Name = "xuiController" });
            onButtonStashAllButFirst.Parameters.Add(new ParameterDefinition(onPressEventArgsTypeDef) { Name = "onPressEventArgs" });
            gameClass.Methods.Add(onButtonStashAllButFirst);
        }

        /*// Add a new field to XUi for storing the new XUiC_VehicleCollectedItemList
        gameClass = module.Types.First(d => d.Name == "XUi");
        //FieldDefinition vehicleCollectedItemListField = new FieldDefinition("VehicleCollectedItemList", FieldAttributes.Public, module.Import(typeof(string)));
        //TypeReference xuic_VehicleCollectedItemList_typeRef = XUiC_VehicleCollectedItemList;
        TypeDefinition xuic_VehicleCollectedItemList_typeDef = new TypeDefinition("global", "XUiC_VehicleCollectedItemList", TypeAttributes.Class);
        //SDX.Core.GlobalVariables.
        //var xuic_VehicleCollectedItemList_typeDef = module.Types.First(d => d.Name == "XUiC_VehicleCollectedItemList");
        //var xuic_VehicleCollectedItemList_typeDef = module.Types.First(d => d.Name == "XUiC_CollectedItemList");
        FieldDefinition vehicleCollectedItemListField = new FieldDefinition("VehicleCollectedItemList", FieldAttributes.Public, xuic_VehicleCollectedItemList_typeDef);
        SetFieldToPublic(vehicleCollectedItemListField);
        gameClass.Fields.Add(vehicleCollectedItemListField);*/

        // Add a new EntityStats.set_WaterLevel method field
        /*gameClass = module.Types.First(d => d.Name == "EntityStats");
        TypeReference floatTypeRef = module.Import(typeof(float));
        //MethodDefinition set_WaterLevel = new MethodDefinition("set_WaterLevel", MethodAttributes.Public | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
        var gameProperty = gameClass.Properties.First(d => d.Name == "WaterLevel");
        //var newSetter = new PropertyDefinition("set_WaterLevel", PropertyAttributes.None, voidTypeRef);
        //newSetter.Parameters.Add(new ParameterDefinition(floatTypeRef) { Name = "value" });
        MethodDefinition set_WaterLevel = new MethodDefinition("set_WaterLevel", MethodAttributes.Public | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
        set_WaterLevel.Parameters.Add(new ParameterDefinition(floatTypeRef) { Name = "value" });
        //gameClass.Methods.Add(set_WaterLevel);
        gameProperty.SetMethod = set_WaterLevel;*/

        /*gameClass = module.Types.First(d => d.Name == "EntityStats");
        TypeReference floatTypeRef = module.Import(typeof(float));
        MethodDefinition setWaterLevelMethod = new MethodDefinition("SetWaterLevel", MethodAttributes.Public | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
        setWaterLevelMethod.Parameters.Add(new ParameterDefinition(floatTypeRef) { Name = "value" });
        gameClass.Methods.Add(setWaterLevelMethod);*/

        bool bHasSetWaterLevel = false;
        gameClass = module.Types.First(d => d.Name == "EntityStats");
        foreach (var method in gameClass.Methods)
        {
            if (method.Name == "SetWaterLevel")
            {
                bHasSetWaterLevel = true;
            }
        }

        if (!bHasSetWaterLevel)
        {
            TypeReference floatTypeRef = module.Import(typeof(float));
            MethodDefinition setWaterLevelMethod = new MethodDefinition("SetWaterLevel", MethodAttributes.Public | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            setWaterLevelMethod.Parameters.Add(new ParameterDefinition(floatTypeRef) { Name = "value" });
            gameClass.Methods.Add(setWaterLevelMethod);
        }

        return true;
    }

    private void HookMethods(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        TypeDefinition gameClass;
        MethodDefinition gameMethod;
        TypeDefinition myClass;
        MethodReference myMethod;

        Instruction first;
        Instruction last;
        ILProcessor pro;

        /*gameClass = gameModule.Types.First(d => d.Name == "XUi");
        var newClass = modModule.Types.First(d => d.Name == "XUiC_VehicleCollectedItemList");
        gameModule.Types.Add(newClass);
        FieldDefinition vehicleCollectedItemListField = new FieldDefinition("VehicleCollectedItemList", FieldAttributes.Public, newClass);
        SetFieldToPublic(vehicleCollectedItemListField);
        gameClass.Fields.Add(vehicleCollectedItemListField);*/

        /*// TEST FOR HAL
        // Add a new field to ItemClass for storing IsVehicleCustomPart
        gameClass = gameModule.Types.First(d => d.Name == "ItemClass");
        FieldDefinition isVehicleCustomPartField = new FieldDefinition("bIsVehicleCustomPart", FieldAttributes.Public, gameModule.Import(typeof(bool)));
        SetFieldToPublic(isVehicleCustomPartField);
        gameClass.Fields.Add(isVehicleCustomPartField);
        isVehicleCustomPartField.Constant = false;*/

        // Modify Vehicle.HasWheels()
        gameClass = gameModule.Types.First(d => d.Name == "Vehicle");
        gameMethod = gameClass.Methods.First(d => d.Name == "HasWheels");
 
        myClass = modModule.Types.First(d => d.Name == "Vehicle_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "HasWheels"));

        var gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));


        // Modify Vehicle.HasStorage()
        gameMethod = gameClass.Methods.First(d => d.Name == "HasStorage");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "HasStorage"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));

        if (bEnableCustomVehicleParts)
        {
            // Modify Vehicle.SetPartInSlot()
            gameMethod = gameClass.Methods.First(d => d.Name == "SetPartInSlot");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetPartInSlot"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            // Modify XUiM_Vehicle.SetPart()
            gameClass = gameModule.Types.First(d => d.Name == "XUiM_Vehicle");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetPart");
            myClass = modModule.Types.First(d => d.Name == "XUiM_Vehicle_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetPart"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();
            pro = gameMethod.Body.GetILProcessor();

            pro.Emit(OpCodes.Ldarg, 0);
            pro.Emit(OpCodes.Ldarg, 1);
            pro.Emit(OpCodes.Ldarg, 2);
            pro.Emit(OpCodes.Ldarg, 3);
            pro.Emit(OpCodes.Ldarg, 4);
            pro.Emit(OpCodes.Call, myMethod);
            pro.Emit(OpCodes.Ret);

            // Modify XUiC_VehiclePartStack.SetEmptySpriteName()
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehiclePartStack");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetEmptySpriteName");
            myClass = modModule.Types.First(d => d.Name == "XUiC_VehiclePartStack_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetEmptySpriteName"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            // Modify XUiC_VehiclePartStack.CanSwap()
            gameMethod = gameClass.Methods.First(d => d.Name == "CanSwap");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "CanSwap"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            // Modify XUiC_VehiclePartStackGrid.SetParts()
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehiclePartStackGrid");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetParts");
            myClass = modModule.Types.First(d => d.Name == "XUiC_VehiclePartStackGrid_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetParts"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            /*// Modify XUiC_VehiclePartStackGrid.RefreshParts()
            gameMethod = gameClass.Methods.First(d => d.Name == "RefreshParts");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "RefreshParts"));

            instructions = gameMethod.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            instructions.Add(Instruction.Create(OpCodes.Ret));

            // Add custom function call at the end of XUiC_VehiclePartStackGrid.OnOpen()
            gameMethod = gameClass.Methods.First(d => d.Name == "OnOpen");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnOpen_addition"));

            instructions = gameMethod.Body.Instructions;
            var last = instructions.Last();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(last, Instruction.Create(OpCodes.Call, myMethod));*/

            /*// Modify XUiC_VehiclePartStackGrid.HandleSlotChangedEvent()
            gameMethod = gameClass.Methods.First(d => d.Name == "HandleSlotChangedEvent");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "HandleSlotChangedEvent"));

            instructions = gameMethod.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            instructions.Add(Instruction.Create(OpCodes.Ret));*/

            // Modify XUiC_BasePartStack.GetPartName()
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_BasePartStack");
            gameMethod = gameClass.Methods.First(d => d.Name == "GetPartName");
            myClass = modModule.Types.First(d => d.Name == "XUiC_BasePartStack_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GetPartName"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            // Add custom function call at the end of EntityVehicle.PopulatePartData()
            gameClass = gameModule.Types.First(d => d.Name == "EntityVehicle");
            gameMethod = gameClass.Methods.First(d => d.Name == "PopulatePartData");
            myClass = modModule.Types.First(d => d.Name == "EntityVehicle_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "PopulatePartData_addition"));

            gameInstructions = gameMethod.Body.Instructions;
            last = gameInstructions.Last();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(last, Instruction.Create(OpCodes.Call, myMethod));

            // Modify EntityVehicle.Init()
            /*gameMethod = gameClass.Methods.First(d => d.Name == "Init");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "Init"));

            instructions = gameMethod.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            instructions.Add(Instruction.Create(OpCodes.Ret));*/

            // Add custom function call at the end of ItemClass.Init()
            gameClass = gameModule.Types.First(d => d.Name == "ItemClass");
            gameMethod = gameClass.Methods.First(d => d.Name == "Init");
            myClass = modModule.Types.First(d => d.Name == "ItemClass_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "Init_addition"));

            gameInstructions = gameMethod.Body.Instructions;
            last = gameInstructions.Last();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(last, Instruction.Create(OpCodes.Call, myMethod));

            // Modify XUiC_VehicleContainer.SetSlots()
            /*gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehicleContainer");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetSlots");
            myClass = modModule.Types.First(d => d.Name == "XUiC_VehicleContainer_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetSlots"));

            instructions = gameMethod.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            instructions.Add(Instruction.Create(OpCodes.Ret));*/
        }

        if (bEnableCustomStorage)
        {    
            // Add custom function call at the beginning of XUiC_VehicleWindowGroup.OnClose()
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehicleWindowGroup");
            gameMethod = gameClass.Methods.First(d => d.Name == "OnClose");
            myClass = modModule.Types.First(d => d.Name == "XUiC_VehicleWindowGroup_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnClose_addition"));

            gameInstructions = gameMethod.Body.Instructions;
            first = gameInstructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));

            // Insert custom function in XUiC_VehicleWindowGroup.OnOpen()
            gameMethod = gameClass.Methods.First(d => d.Name == "OnOpen");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnOpen_addition"));

            gameInstructions = gameMethod.Body.Instructions;
            first = gameInstructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(gameInstructions[7], Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(gameInstructions[8], Instruction.Create(OpCodes.Call, myMethod));

            // Modify TileEntityLootContainer.SetContainerSize()
            /*gameClass = gameModule.Types.First(d => d.Name == "TileEntityLootContainer");
            gameMethod = gameClass.Methods.First(d => d.Name == "SetContainerSize");
            myClass = modModule.Types.First(d => d.Name == "TileEntityLootContainer_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetContainerSize"));

            instructions = gameMethod.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            instructions.Add(Instruction.Create(OpCodes.Ret));*/
        }

        if (bEnableCustomVehicleIcons)
        {
            // Modifications for custom vehicle Icons
            // Add custom function call at the beginning of ModManager.GameStartDone()
            /*gameClass = gameModule.Types.First(d => d.Name == "ModManager");
            gameMethod = gameClass.Methods.First(d => d.Name == "GameStartDone");
            myClass = modModule.Types.First(d => d.Name == "ModManager_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GameStartDone_additions"));

            gameInstructions = gameMethod.Body.Instructions;
            first = gameInstructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));*/

            // Add custom function call at the end of EntityPlayerLocal.OnAddedToWorld()
            gameClass = gameModule.Types.First(d => d.Name == "EntityPlayerLocal");
            gameMethod = gameClass.Methods.First(d => d.Name == "OnAddedToWorld");
            myClass = modModule.Types.First(d => d.Name == "VehicleIcons_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "ModifyUIAtlases"));

            gameInstructions = gameMethod.Body.Instructions;
            last = gameInstructions.Last();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(last, Instruction.Create(OpCodes.Call, myMethod));

            // Add custom function call at the beginning of XUiC_CompassWindow.Update()
            /*gameClass = gameModule.Types.First(d => d.Name == "XUiC_CompassWindow");
            gameMethod = gameClass.Methods.First(d => d.Name == "Update");
            myClass = modModule.Types.First(d => d.Name == "XUiC_CompassWindow_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "Update_addition"));

            instructions = gameMethod.Body.Instructions;
            first = instructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));*/

            /*// Add custom function call at the beginning of UISprite.set_atlas
            gameClass = gameModule.Types.First(d => d.Name == "UISprite");
            gameMethod = gameClass.Methods.First(d => d.Name == "set_atlas");
            myClass = modModule.Types.First(d => d.Name == "UISprite_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GetProperAtlas"));

            instructions = gameMethod.Body.Instructions;
            first = instructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_1));
            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));
            //pro.InsertBefore(first, Instruction.Create(OpCodes.Starg_S, 1));*/

            // Add custom function call at the beginning of XUiC_CompassWindow.Update()
            /*gameClass = gameModule.Types.First(d => d.Name == "XUiC_CompassWindow");
            gameMethod = gameClass.Methods.First(d => d.Name == "Update");
            myClass = modModule.Types.First(d => d.Name == "XUiC_CompassWindow_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "Update_addition"));

            instructions = gameMethod.Body.Instructions;
            first = instructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));*/

            /*// Add custom function call at the beginning of UISprite.set_atlas
            gameClass = gameModule.Types.First(d => d.Name == "UISprite");
            //gameMethod = gameClass.Methods.First(d => d.Name == "OnFill");
            gameMethod = gameClass.Methods.First(d => d.Name == "OnUpdate");
            myClass = modModule.Types.First(d => d.Name == "UISprite_patchFunctions");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetProperAtlas"));

            instructions = gameMethod.Body.Instructions;
            first = instructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(first, Instruction.Create(OpCodes.Call, myMethod));*/
        }

        if (bEnableNewUiButtons)
        {    
            // Modifications for new UI Buttons
            // Modify XUiC_VehicleContainer
            // SubInit
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehicleContainer");
            myClass = modModule.Types.First(d => d.Name == "XUiC_VehicleContainer_patchFunctions");
            gameMethod = gameClass.Methods.First(d => d.Name == "Init");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SubInit"));

            gameInstructions = gameMethod.Body.Instructions;
            first = gameInstructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(gameInstructions[2], Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(gameInstructions[3], Instruction.Create(OpCodes.Callvirt, myMethod));

            // Take All button
            gameMethod = gameClass.Methods.First(d => d.Name == "OnButtonTakeAll");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "ButtonTakeAll"));

            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            // Drop All button
            gameMethod = gameClass.Methods.First(d => d.Name == "OnButtonDropAll");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "ButtonDropAll"));

            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            // Modify XUiC_BackpackWindow
            // SubInit
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_BackpackWindow");
            myClass = modModule.Types.First(d => d.Name == "XUiC_BackpackWindow_patchFunctions");
            gameMethod = gameClass.Methods.First(d => d.Name == "Init");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SubInit"));

            gameInstructions = gameMethod.Body.Instructions;
            first = gameInstructions.First();
            pro = gameMethod.Body.GetILProcessor();

            pro.InsertBefore(gameInstructions[2], Instruction.Create(OpCodes.Ldarg_0));
            pro.InsertBefore(gameInstructions[3], Instruction.Create(OpCodes.Callvirt, myMethod));

            // Stash All button
            gameMethod = gameClass.Methods.First(d => d.Name == "OnButtonStashAll");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "ButtonStashAll"));

            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            // Stash All But First button
            gameMethod = gameClass.Methods.First(d => d.Name == "OnButtonStashAllButFirst");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "ButtonStashAllButFirst"));

            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
            gameMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        if (bEnableProperCompassIcons)
        {
            // Modify MapObjectVehicle to use the proper compass icon
            gameClass = gameModule.Types.First(d => d.Name == "MapObjectVehicle");
            myClass = modModule.Types.First(d => d.Name == "MapObjectVehicle_patchFunctions");
            gameMethod = gameClass.Methods.First(d => d.Name == "GetCompassIcon");
            myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GetProperCompassIcon"));

            gameInstructions = gameMethod.Body.Instructions;
            gameInstructions.Clear();

            gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
            gameInstructions.Add(Instruction.Create(OpCodes.Ret));

            // Change XUiC_CompassWindow.SCQ() GetMapIcon() calls to GetCompassIcon() calls
            gameClass = gameModule.Types.First(d => d.Name == "XUiC_CompassWindow");
            var SCQgameMethod = gameClass.Methods.First(d => d.Parameters.Count == 3 && d.Parameters[0].ParameterType.FullName == "EntityPlayerLocal");

            /*Logging.LogInfo("XUiC_CompassWindow methods:");
            foreach (MethodDefinition methodDef in gameClass.Methods)
            {
                Logging.LogInfo(methodDef.Name);
                if(methodDef.Name == "SCQ")
                {
                    Logging.LogInfo("SCQ:");
                    foreach (ParameterDefinition paramDef in methodDef.Parameters)
                    {
                        Logging.LogInfo("name = " + paramDef.Name + " | type = " + paramDef.GetType() + " | ParameterType = " + paramDef.ParameterType);
                        //Logging.LogInfo(paramDef.GetType().ToString());
                        if(paramDef.ParameterType == entityPlayerLocalTypeRef)
                        {
                            Logging.LogInfo("Found the fucker");
                        }
                    }
                }
            }*/

            gameClass = gameModule.Types.First(d => d.Name == "MapObject");
            var getMapIconMethod = gameClass.Methods.First(d => d.Name == "GetMapIcon");
            var getCompassIconMethod = gameClass.Methods.First(d => d.Name == "GetCompassIcon");

            foreach (Instruction inst in SCQgameMethod.Body.Instructions)
            {
                if (inst.OpCode == OpCodes.Callvirt && inst.Operand == getMapIconMethod)
                {
                    inst.Operand = getCompassIconMethod;
                }
            }
        }

        /*// Modify XUiC_HUDStatBar.OnOpen() and XUiC_HUDStatBar.OnClose()
        gameClass = gameModule.Types.First(d => d.Name == "XUiC_HUDStatBar");
        gameMethod = gameClass.Methods.First(d => d.Name == "OnOpen");
        myClass = modModule.Types.First(d => d.Name == "XUiC_HUDStatBar_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnOpen"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));

        gameMethod = gameClass.Methods.First(d => d.Name == "OnClose");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnClose"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));*/

        // Add custom function call at the end of XUiC_HUDStatBar.OnOpen()
        gameClass = gameModule.Types.First(d => d.Name == "XUiC_HUDStatBar");
        gameMethod = gameClass.Methods.First(d => d.Name == "OnOpen");
        myClass = modModule.Types.First(d => d.Name == "XUiC_HUDStatBar_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnOpen_addition"));

        gameInstructions = gameMethod.Body.Instructions;
        last = gameInstructions.Last();
        pro = gameMethod.Body.GetILProcessor();

        pro.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
        pro.InsertBefore(last, Instruction.Create(OpCodes.Callvirt, myMethod));

        gameMethod = gameClass.Methods.First(d => d.Name == "OnClose");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "OnClose_addition"));

        gameInstructions = gameMethod.Body.Instructions;
        last = gameInstructions.Last();
        pro = gameMethod.Body.GetILProcessor();

        pro.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
        pro.InsertBefore(last, Instruction.Create(OpCodes.Callvirt, myMethod));

        /*// Replace the content of XUiC_VehicleStats.GetBindingValue()
        gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehicleStats");
        gameMethod = gameClass.Methods.First(d => d.Name == "GetBindingValue");
        myClass = modModule.Types.First(d => d.Name == "XUiC_VehicleStats_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GetBindingValue_patched"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
        gameInstructions.Add(Instruction.Create(OpCodes.Callvirt, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));*/

        // Add instructions to our new EntityStats.SetWaterLevel method
        gameClass = gameModule.Types.First(d => d.Name == "EntityStats");
        gameMethod = gameClass.Methods.First(d => d.Name == "SetWaterLevel");
        myClass = modModule.Types.First(d => d.Name == "EntityStats_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetWaterLevel"));

        gameInstructions = gameMethod.Body.Instructions;

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));

        // Change EntityStats_patchFunctions.get_WaterLevel
        gameMethod = gameClass.Methods.First(d => d.Name == "get_WaterLevel");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "GetWaterLevel"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));

        // Insert method EntityStats_patchFunctions.AdjustAirtightWaterLevel in EntityStats.Tick method
        gameMethod = gameClass.Methods.First(d => d.Name == "Tick");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "AdjustAirtightWaterLevel"));

        gameInstructions = gameMethod.Body.Instructions;
        pro = gameMethod.Body.GetILProcessor();

        pro.InsertBefore(gameInstructions[333], Instruction.Create(OpCodes.Ldarg_0));
        pro.InsertBefore(gameInstructions[334], Instruction.Create(OpCodes.Call, myMethod));

        // Insert method EntityMoveHelper_patchFunctions.onUpdateMoveHelper_addition in EntityMoveHelper.onUpdateMoveHelper
        /*gameClass = gameModule.Types.First(d => d.Name == "EntityMoveHelper");
        gameMethod = gameClass.Methods.First(d => d.Name == "onUpdateMoveHelper");
        myClass = modModule.Types.First(d => d.Name == "EntityMoveHelper_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "onUpdateMoveHelper_addition"));

        gameInstructions = gameMethod.Body.Instructions;
        pro = gameMethod.Body.GetILProcessor();

        VariableDefinition flag2Var = null;
        flag2Var = gameInstructions[238].Operand as VariableDefinition;

        pro.InsertBefore(gameInstructions[239], Instruction.Create(OpCodes.Ldarg_0));
        pro.InsertBefore(gameInstructions[240], Instruction.Create(OpCodes.Ldloc_S, flag2Var));
        pro.InsertBefore(gameInstructions[241], Instruction.Create(OpCodes.Call, myMethod));
        pro.InsertBefore(gameInstructions[242], Instruction.Create(OpCodes.Stloc_S, flag2Var));*/

        // Change StabilityCalculator.YZ obfuscated method to avoid a stupid null ref
        /*gameClass = gameModule.Types.First(d => d.Name == "StabilityCalculator");
        if (IsDedi())
            gameMethod = gameClass.Methods.First(d => d.Name == "MH");
        else
            gameMethod = gameClass.Methods.First(d => d.Name == "YZ");
        myClass = modModule.Types.First(d => d.Name == "StabilityCalculator_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "YZ_patch"));

        gameInstructions = gameMethod.Body.Instructions;
        gameInstructions.Clear();

        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        gameInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        gameInstructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        gameInstructions.Add(Instruction.Create(OpCodes.Ret));*/
    }

    private void SetAccessLevels(ModuleDefinition module)
    {
        var tew = module.Types.First(d => d.Name == "EntityVehicle");
        foreach (var field in tew.Fields)
            SetFieldToPublic(field);
        foreach (var meth in tew.Methods)
            SetMethodToPublic(meth);
    }

    // Helper functions to allow us to access and change variables that are otherwise unavailable.
    private void SetMethodToVirtual(MethodDefinition meth)
    {
        meth.IsVirtual = true;
    }

    private void SetFieldToPublic(FieldDefinition field)
    {
        field.IsFamily = false;
        field.IsPrivate = false;
        field.IsPublic = true;

    }
    private void SetMethodToPublic(MethodDefinition field)
    {
        field.IsFamily = false;
        field.IsPrivate = false;
        field.IsPublic = true;

    }

    // Called after the patching process and after scripts are compiled.
    // Used to link references between both assemblies
    // Return true if successful
    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        HookMethods(gameModule, modModule);
        return true;
    }
}

