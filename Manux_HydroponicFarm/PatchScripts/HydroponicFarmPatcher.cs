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
using System.Reflection;


class HydroponicFarmPatcher : IPatcherMod
{

    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("==Hydroponic Farm Patcher===");
        SetAccessLevels(module);
        return true;
    }

    private void HookUpdateTickMethods(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        var gameClass = gameModule.Types.First(d => d.Name == "TileEntityWorkstation");
        var gameMethod = gameClass.Methods.First(d => d.Name == "UpdateTick");
        var modClass = modModule.Types.First(d => d.Name == "TileEntityWorkstationPatchFunctions");
        var modMethod = gameModule.Import(modClass.Methods.First(d => d.Name == "IsHydroponicFarmPowered"));

        var gamePro = gameMethod.Body.GetILProcessor();
        var firstIns = gamePro.Body.Instructions.First();

        var instructionsCopy = new Instruction[gamePro.Body.Instructions.Count];
        gamePro.Body.Instructions.CopyTo(instructionsCopy, 0);

        /* The IL instructions below will insert this code a the beginning of TileEntityWorkstation.UpdateTick()
        if (!TileEntityWorkstationPatchFunctions.IsHydroponicFarmPowered(this))
        {
            this.QKZ = global::GameTimer.Instance.ticks;    // this.QKZ is obfuscated and changes on different machines, so we insert this line by copying the original 4 IL instructions (62-65 below).
            return;
        }*/

        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Ldarg_0));
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Call, modMethod));
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Ldc_I4_0));
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Ceq));
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Brfalse, firstIns));
        gamePro.InsertBefore(firstIns, instructionsCopy[62]);
        gamePro.InsertBefore(firstIns, instructionsCopy[63]);
        gamePro.InsertBefore(firstIns, instructionsCopy[64]);
        gamePro.InsertBefore(firstIns, instructionsCopy[65]);
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Ret));
        gamePro.InsertBefore(firstIns, Instruction.Create(OpCodes.Br_S, firstIns));
    }

    private void SetAccessLevels(ModuleDefinition module)
    {
        var tew = module.Types.First(d => d.Name == "TileEntityWorkstation");
        foreach (var field in tew.Fields)
            SetFieldToPublic(field);
        foreach (var meth in tew.Methods)
            SetMethodToPublic(meth);

        var xuicWwg = module.Types.First(d => d.Name == "XUiC_WorkstationWindowGroup");
        foreach (var field in xuicWwg.Fields)
            SetFieldToPublic(field);
        foreach (var meth in xuicWwg.Methods)
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
        HookUpdateTickMethods(gameModule, modModule);
        return true;
    }
}

