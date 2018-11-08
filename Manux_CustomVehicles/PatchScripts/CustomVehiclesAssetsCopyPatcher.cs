using System;
using System.Collections.Generic;
using SDX.Core;
using Mono.Cecil;
using SDX.Compiler;
using System.Xml;
using System.IO;

public class CustomVehiclesAssetsCopyPatcher : IPatcherMod
{
    static string backPacksSwappingBaseFileName = "ui_edits_forBiggerBackPackMod.xml";
    static string regularBackPackDir = "regularBackPack";
    static string bigBackPackDir = "biggerBackPackMod";

    public bool Patch(ModuleDefinition module)
    {
        //PrintEnabledMods();
        CopyUiIconsFolders();
        VeridyIfBiggerBackPackModEnabled();

        return true;
    }

    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        return true;
    }

    private static void CopyUiIconsFolders()
    {
        Logging.LogInfo(" == Copying UI Icons == ");

        CopyFolderFiles("UI", "${GameDir}/Mods/SDX/UI");
        CopyFolderFiles("UI/Icons", "${GameDir}/Mods/SDX/UI/Icons");
    }


    private static void VeridyIfBiggerBackPackModEnabled()
    {
        SDX.Core.ModInfo customVehiclesModInfo = SDX.Core.ModManager.GetModByName("Manux - Custom Vehicles");
        SDX.Core.ModInfo biggerBackPackModInfo = SDX.Core.ModManager.GetModByName("Manux - Bigger BackPack(120) MiniBike(180) Containers(182) Crafting Slots(+1in +3out)");
        string backpacksBaseXmlFileName = customVehiclesModInfo.ModDirectory + "/Config/" + backPacksSwappingBaseFileName;
        string regularBackpackXmlFileName = customVehiclesModInfo.ModDirectory + "/Config/" + regularBackPackDir + "/" + backPacksSwappingBaseFileName;
        string bigBackpackXmlFileName = customVehiclesModInfo.ModDirectory + "/Config/" + bigBackPackDir + "/" + backPacksSwappingBaseFileName;
        //Logging.LogInfo("Backpacks Base Xml FileName = " + backpacksBaseXmlFileName);
        //Logging.LogInfo("Regular Backpack Xml FileName = " + regularBackpackXmlFileName);
        //Logging.LogInfo("Big Backpack Xml FileName = " + bigBackpackXmlFileName);

        if (biggerBackPackModInfo != null && biggerBackPackModInfo.Enabled)
        {
            Logging.LogInfo("Bigger Back Mod is Enabled, swapping BackPack XML files.");
            if (File.Exists(backpacksBaseXmlFileName))
            {
                File.Delete(backpacksBaseXmlFileName);
            }
            File.Copy(bigBackpackXmlFileName, backpacksBaseXmlFileName);
        }
        else
        {
            Logging.LogInfo("Bigger Back Mod is Disabled, swapping BackPack XML files.");
            if (File.Exists(backpacksBaseXmlFileName))
            {
                File.Delete(backpacksBaseXmlFileName);
            }
            File.Copy(regularBackpackXmlFileName, backpacksBaseXmlFileName);
        }
    }

    private static void PrintEnabledMods()
    {
        Logging.LogInfo("Enable Mods:");
        foreach (SDX.Core.ModInfo modInfo in SDX.Core.ModManager.EnabledMods)
        {
            Logging.LogInfo(modInfo.Name + " configs:");
            XmlElement xmlElement = modInfo.GetConfigModsNode();
            if (xmlElement != null)
            {
                foreach (XmlNode xmlNode in xmlElement.ChildNodes)
                {
                    foreach (XmlAttribute xmlAttr in xmlNode.Attributes)
                    {
                        Logging.LogInfo("\t- " + xmlAttr.Value);
                    }
                }
            }
        }
    }

    private static void CopyFolderFiles(string srcDir, string dstDir)
    {
        var uiIconsDestPath = GlobalVariables.Parse(dstDir);
        IOUtils.EnsureDirectory(uiIconsDestPath);

        var files = SDX.Core.ModManager.FindFilesInMods(srcDir, "*.*", false);
        foreach (var filePath in files)
        {
            IOUtils.CopyFileToDir(filePath, uiIconsDestPath);
        }
        Logging.LogInfo(string.Format(" == Finished Copying ({0}) files == ", files.Count));
    }
}

