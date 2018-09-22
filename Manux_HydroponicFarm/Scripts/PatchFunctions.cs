using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


//class TileEntityWorkstationPatchFunctions : TileEntityWorkstation
class TileEntityWorkstationPatchFunctions
{
    static readonly bool showDebugLog = false;
    /*
    public TileEntityWorkstationPatchFunctions(Chunk _chunk) : base(_chunk)
    {
    }*/

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }
    

    public static void HaltUICrafting(XUiC_WorkstationWindowGroup workstationWindowGroup)
    {

    }

    public static void ResumeUICrafting(XUiC_WorkstationWindowGroup workstationWindowGroup)
    {

    }

    public static bool IsHydroponicFarmPowered(TileEntityWorkstation tew)
    {
        Vector3i powerBlockPos = tew.ToWorldPos();
        powerBlockPos.y -= 1;
        BlockValue powerBlockValue = GameManager.Instance.World.GetBlock(powerBlockPos);
        Block powerBlock = Block.list[powerBlockValue.type];

        bool isPowered = false;
        if (powerBlock.GetBlockName() == "hydroponicFarmPower")
        {
            // Is Power block powered
            Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(powerBlockPos);
            isPowered = BlockHydroponicFarmPower.IsBlockPoweredUp(powerBlockPos, chunk.ClrIdx);

            // Halt crafting UI
            global::LocalPlayerUI playerUI = (GameManager.Instance.World.GetLocalPlayer() as global::EntityPlayerLocal).PlayerUI;
            global::BlockValue block = GameManager.Instance.World.GetBlock(tew.ToWorldPos());
            string text = string.Format("workstation_{0}", global::Block.list[block.type].GetBlockName());
            if (playerUI.windowManager.Contains(text))
            {
                XUiC_WorkstationWindowGroup workstationWindowGroup = ((global::XUiC_WorkstationWindowGroup)((global::XUiWindowGroup)playerUI.windowManager.GetWindow(text)).Controller);
                if (workstationWindowGroup.WorkstationData != null && workstationWindowGroup.WorkstationData.TileEntity == tew)
                {
                    // The name of the XUiC_CraftingQueue field of XUiC_WorkstationWindowGroup is obfuscated so we find it by type to call it.
                    XUiC_CraftingQueue craftingQueue = null;
                    var listOfFieldNames = typeof(XUiC_WorkstationWindowGroup).GetFields();
                    foreach (FieldInfo fieldInfo in listOfFieldNames)
                    {
                        FieldInfo field = null;
                        if (fieldInfo.FieldType == typeof(XUiC_CraftingQueue))
                        {
                            field = fieldInfo;
                        }
                        if (field != null)
                        {
                            craftingQueue = (XUiC_CraftingQueue)field.GetValue(workstationWindowGroup);
                        }      
                    }

                    if (craftingQueue != null)
                    {
                        if (!isPowered)
                        {
                            //workstationWindowGroup.FNW.HaltCrafting();
                            craftingQueue.HaltCrafting();
                        }
                        else
                        {
                            //workstationWindowGroup.FNW.ResumeCrafting();
                            craftingQueue.ResumeCrafting();
                        }
                    }
                    
                }
            }

            TileEntityWorkstationPatchFunctions.ChangePlantIfNeeded(tew);
            return isPowered;
        }
        
        return true;
    }

    public static readonly Dictionary<string, string> hydroCropsDict = new Dictionary<string, string>()
        {
            {"potato",              "hydroponicPotato"},
            {"corn",                "hydroponicCorn"},
            {"aloePlant",           "hydroponicAloes"},
            {"blueberries",         "hydroponicBlueberries"},
            {"cottonPlant",         "hydroponicCotton"},
            {"goldenrodPlant",      "hydroponicGoldenRod"},
            {"chrysanthemumPlant",  "hydroponicChrysanthemum"},
            {"coffeeBeans",         "hydroponicCoffee"},
            {"hopsFlower",          "hydroponicHops"},
            {"foodYuccaFruit",      "hydroponicYucca"},
            {"mushrooms",           "hydroponicMushrooms"}
        };

    public static void ChangePlantIfNeeded(TileEntityWorkstation tew)
    {
        string newPlantAboveName = "";

        Vector3i blockAbovePos = tew.ToWorldPos();
        blockAbovePos.y += 1;
        BlockValue blockAbove = GameManager.Instance.World.GetBlock(blockAbovePos);
        Block block = Block.list[blockAbove.type];

        int curRecipeidx = tew.Queue.Length - 1;
        if (tew.Queue[curRecipeidx] != null && tew.Queue[curRecipeidx].Recipe != null)
        {
            string curRecipe = tew.Queue[curRecipeidx].Recipe.GetName();
            if ( hydroCropsDict.ContainsKey(curRecipe) && block.GetBlockName() != hydroCropsDict[curRecipe])
            {
                newPlantAboveName = hydroCropsDict[curRecipe];
            }
        }
        else
        {
            if (block.GetBlockName() != "air")
            {
                newPlantAboveName = "air";
            }
        }

        // if needed, change the plant above the workstation or fallback to air if nothing is cooking
        if (newPlantAboveName != "")
        {
            BlockValue newPlantBlock = Block.GetBlockValue(newPlantAboveName);
            GameManager.Instance.World.SetBlockRPC(blockAbovePos, newPlantBlock);
        }
    }
}

