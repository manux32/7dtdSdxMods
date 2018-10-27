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

    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("==Custom Vehicles Patcher===");

        var gameClass = module.Types.First(d => d.Name == "EntityVehicle");
        var gameMethod = gameClass.Methods.First(d => d.Name == "isDriveable");
        SetMethodToPublic(gameMethod);
        SetMethodToVirtual(gameMethod);

        /*var field = gameClass.Fields.First(d => d.Name == "vehicle");
        SetFieldToPublic(field);

        field = gameClass.Fields.First(d => d.FieldType.Name == "CharacterController");
        SetFieldToPublic(field);*/

        gameMethod = gameClass.Methods.First(d => d.Name == "setupEntitySlotInfo");
        SetMethodToPublic(gameMethod);

        gameClass = module.Types.First(d => d.Name == "EntityPlayerLocal");
        gameMethod = gameClass.Methods.First(d => d.Name == "updateCameraPosition");
        SetMethodToPublic(gameMethod);

        gameClass = module.Types.First(d => d.Name == "Vehicle");
        //foreach (var f in gameClass.Fields)
            //Logging.LogInfo(string.Format("f name: {0} type: {1}", f.Name, f.FieldType.ToString()));
        var field = gameClass.Fields.First(d => d.FieldType.ToString() == "System.Collections.Generic.List`1<VehiclePart>");
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

        // Add a new field to XUiC_BasePartStack for storing the custom_slot_type
        FieldDefinition customSlotTypeField = new FieldDefinition("customSlotType", FieldAttributes.Public, module.Import(typeof(string)));
        SetFieldToPublic(customSlotTypeField);
        gameClass.Fields.Add(customSlotTypeField);
        customSlotTypeField.Constant = string.Empty;

        gameClass = module.Types.First(d => d.Name == "XUiC_VehiclePartStack");
        gameMethod = gameClass.Methods.First(d => d.Name == "SetEmptySpriteName");
        SetMethodToPublic(gameMethod);

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

        gameClass = module.Types.First(d => d.Name == "XUiController");
        field = gameClass.Fields.First(d => d.Name == "viewComponent");
        SetFieldToPublic(field);

        gameClass = module.Types.First(d => d.Name == "XUiC_ItemStackGrid");
        field = gameClass.Fields.First(d => d.Name == "items");
        SetFieldToPublic(field);
        field = gameClass.Fields.First(d => d.Name == "itemControllers");
        SetFieldToPublic(field);

        /*gameClass = module.Types.First(d => d.Name == "XUiC_VehicleWindowGroup");
        field = gameClass.Fields.First(d => d.Name == "ID");
        field.IsStatic = false;*/

        return true;
    }

    private void HookMethods(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        // Modify Vehicle.HasWheels()
        var gameClass = gameModule.Types.First(d => d.Name == "Vehicle");
        var gameMethod = gameClass.Methods.First(d => d.Name == "HasWheels");
 
        var myClass = modModule.Types.First(d => d.Name == "Vehicle_patchFunctions");
        var myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "HasWheels"));

        var instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Modify Vehicle.HasStorage()
        gameMethod = gameClass.Methods.First(d => d.Name == "HasStorage");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "HasStorage"));

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Modify Vehicle.SetPartInSlot()
        gameMethod = gameClass.Methods.First(d => d.Name == "SetPartInSlot");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetPartInSlot"));

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Modify XUiM_Vehicle.SetPart()
        gameClass = gameModule.Types.First(d => d.Name == "XUiM_Vehicle");
        gameMethod = gameClass.Methods.First(d => d.Name == "SetPart");
        myClass = modModule.Types.First(d => d.Name == "XUiM_Vehicle_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetPart"));

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();
        var pro = gameMethod.Body.GetILProcessor();

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

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Modify XUiC_VehiclePartStack.CanSwap()
        gameMethod = gameClass.Methods.First(d => d.Name == "CanSwap");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "CanSwap"));

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Modify XUiC_VehiclePartStackGrid.SetParts()
        gameClass = gameModule.Types.First(d => d.Name == "XUiC_VehiclePartStackGrid");
        gameMethod = gameClass.Methods.First(d => d.Name == "SetParts");
        myClass = modModule.Types.First(d => d.Name == "XUiC_VehiclePartStackGrid_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "SetParts"));

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

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

        instructions = gameMethod.Body.Instructions;
        instructions.Clear();

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Call, myMethod));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        // Add custom function call at the end of EntityVehicle.PopulatePartData()
        gameClass = gameModule.Types.First(d => d.Name == "EntityVehicle");
        gameMethod = gameClass.Methods.First(d => d.Name == "PopulatePartData");
        myClass = modModule.Types.First(d => d.Name == "EntityVehicle_patchFunctions");
        myMethod = gameModule.Import(myClass.Methods.First(d => d.Name == "PopulatePartData_addition"));

        instructions = gameMethod.Body.Instructions;
        var last = instructions.Last();
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

        instructions = gameMethod.Body.Instructions;
        last = instructions.Last();
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

