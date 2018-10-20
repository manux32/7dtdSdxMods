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


class CustomVehiclesPatcher : IPatcherMod
{

    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("==Custom Vehicles Patcher===");

        var gameClass = module.Types.First(d => d.Name == "EntityVehicle");
        var gameMethod = gameClass.Methods.First(d => d.Name == "isDriveable");
        SetMethodToPublic(gameMethod);
        SetMethodToVirtual(gameMethod);

        gameClass = module.Types.First(d => d.Name == "EntityPlayerLocal");
        gameMethod = gameClass.Methods.First(d => d.Name == "updateCameraPosition");
        SetMethodToPublic(gameMethod);

        return true;
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
        return true;
    }
}

