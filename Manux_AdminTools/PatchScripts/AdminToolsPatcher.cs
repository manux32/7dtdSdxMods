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


class AdminToolsPatcher : IPatcherMod
{

    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("==Admin Tools Patcher===");

        bool bHasSetWaterLevel = false;
        var gameClass = module.Types.First(d => d.Name == "EntityStats");
        foreach (var method in gameClass.Methods)
        {
            if (method.Name == "SetWaterLevel")
            {
                bHasSetWaterLevel = true;
            }
        }

        if (!bHasSetWaterLevel)
        {
            TypeReference voidTypeRef = module.TypeSystem.Void;
            TypeReference floatTypeRef = module.Import(typeof(float));
            MethodDefinition setWaterLevelMethod = new MethodDefinition("SetWaterLevel", MethodAttributes.Public | MethodAttributes.HideBySig, voidTypeRef) { HasThis = true };
            setWaterLevelMethod.Parameters.Add(new ParameterDefinition(floatTypeRef) { Name = "value" });
            gameClass.Methods.Add(setWaterLevelMethod);
        }

        return true;
    }

    // Helper functions to allow us to access and change variables that are otherwise unavailable.
    private void SetAccessLevels(ModuleDefinition module, string className)
    {
        var classTypeDef = module.Types.First(d => d.Name == className);
        foreach (var field in classTypeDef.Fields)
            SetFieldToPublic(field);
        foreach (var meth in classTypeDef.Methods)
            SetMethodToPublic(meth);
    }
    
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

