using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;


class ImageManipUtils
{
    static readonly bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            UnityEngine.Debug.Log(msg);
        }
    }

    public static void WriteItemIconAtlasImageToDisc()
    {
        EntityPlayerLocal player = GameManager.Instance.World.GetLocalPlayer() as EntityPlayerLocal;
        LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(player);
        UIAtlas uiAtlas = uiforPlayer.xui.GetAtlasByName("UIAtlas");
        UIAtlas itemIconAtlas = uiforPlayer.xui.GetAtlasByName("itemIconAtlas");

        DynamicUIAtlas dynItemIconAtlas = itemIconAtlas.gameObject.GetComponent<DynamicUIAtlas>();
        if (dynItemIconAtlas != null)
        {
            DebugMsg("itemIconAtlas PrebakedAtlas: " + dynItemIconAtlas.PrebakedAtlas);
        }
        else
        {
            DebugMsg("Cannot find dynItemIconAtlas");
        }

        Texture2D texture2Dsrc;
        //if (!DynamicUIAtlasTools.ReadPrebakedAtlasTexture(dynItemIconAtlas.PrebakedAtlas, out texture2DSrc))
        if (!DynamicUIAtlasTools.ReadPrebakedAtlasTexture("GUI/Atlases/UIAtlas", out texture2Dsrc))
        {
            DebugMsg("Could not read dynamic atlas texture");
            return;
        }

        DebugMsg("itemIconAtlas Texture: " + texture2Dsrc.name + " (" + texture2Dsrc.width.ToString() + "x" + texture2Dsrc.height.ToString() + ")");
        Texture2D texture2D = new Texture2D(texture2Dsrc.width, texture2Dsrc.height, TextureFormat.ARGB32, false);
        texture2D.SetPixels(texture2Dsrc.GetPixels());
        byte[] bytes = texture2D.EncodeToPNG();
        //File.WriteAllBytes("C:/SDXModding/Gimp/itemIconAtlas_extract.png", bytes);
        File.WriteAllBytes("C:/SDXModding/Gimp/ui_atlas_extract.png", bytes);
        UnityEngine.Object.Destroy(texture2D);
        Resources.UnloadAsset(texture2Dsrc);

        //Texture2D texture2Dsrc;
        if (!DynamicUIAtlasTools.ReadPrebakedAtlasTexture(dynItemIconAtlas.PrebakedAtlas, out texture2D))
        {
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Could not read dynamic atlas texture");
            return;
        }
        //DebugMsg("uiAtlas Texture: " + uiAtlas.texture.name + " (" + uiAtlas.texture.width.ToString() + "x" + uiAtlas.texture.height.ToString() + ")");
        texture2D = new Texture2D(uiAtlas.texture.width, uiAtlas.texture.height, TextureFormat.ARGB32, false);
        texture2D.SetPixels(((Texture2D)uiAtlas.texture).GetPixels());
        bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes("C:/SDXModding/Gimp/ui_atlas_extract.png", bytes);

        //DebugMsg("itemIconAtlas Texture: " + itemIconAtlas.texture.name + " (" + itemIconAtlas.texture.width.ToString() + "x" + itemIconAtlas.texture.height.ToString() + ")");
        texture2D = new Texture2D(itemIconAtlas.texture.width, itemIconAtlas.texture.height, TextureFormat.ARGB32, false);
        texture2D.SetPixels(((Texture2D)itemIconAtlas.texture).GetPixels());
        bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes("C:/SDXModding/Gimp/itemIconAtlas_extract.png", bytes);
        UnityEngine.Object.Destroy(texture2D);
        Resources.UnloadAsset(texture2Dsrc);
    }

    // LoadAdditionalIconsInAtlas(uiAtlas, "C:/SDXModding/Game/Working/Mods/SDX/UI/Icons")
    public static void LoadAdditionalIconsInAtlas(UIAtlas atlas, string iconsPath)
    {
        DynamicUIAtlas dynUiAtlas = atlas as DynamicUIAtlas;
        //DynamicUIAtlas dynUiAtlas = uiAtlas.gameObject.GetComponent<DynamicUIAtlas>();

        if (dynUiAtlas == null)
        {
            DebugMsg("No DynamicUIAtlas component on uiAtlas");
            return;
        }

        //DynamicUIAtlas dynUiAtlas = new DynamicUIAtlas();

        dynUiAtlas.ResetAtlas();
        Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
        DebugMsg("uiAtlas DynamicUIAtlas PrebakedAtlas = " + dynUiAtlas.PrebakedAtlas);

        //string path = "C:/SDXModding/Game/Working/Mods/SDX/UI/Icons";
        if (Directory.Exists(iconsPath))
        {
            string[] array = Directory.GetFiles(iconsPath);
            foreach (string image in array)
            {
                if (image.ToLower().EndsWith(".png"))
                {
                    DebugMsg("Adding icon to UIAtlas: " + image);
                    Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    if (texture2D.LoadImage(File.ReadAllBytes(image)))
                    {
                        dictionary.Add(Path.GetFileNameWithoutExtension(image), texture2D);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(texture2D);
                    }
                }
            }

            dynUiAtlas.LoadAdditionalSprites(dictionary);

            foreach (Texture2D obj in dictionary.Values)
            {
                UnityEngine.Object.Destroy(obj);
            }

            dynUiAtlas.Compress();
        }
        else
        {
            DebugMsg("Cannot find path: " + iconsPath);
        }
    }

    public static void PrintProjectTextures()
    {
        Texture2D texture2Dsrc = null;
        DebugMsg("Project Textures:");
        Texture[] projectTextures = (Texture[])Resources.FindObjectsOfTypeAll(typeof(Texture));
        foreach (Texture texture in projectTextures)
        {
            DebugMsg("\t- " + texture.name);
            if (texture.name == "UIAtlas")
            {
                //DebugMsg("\t- " + texture.name, texture.path);
                //texture2Dsrc = (Texture2D)texture;
                //Texture2D text2D = (Texture2D)texture;
                //texture2Dsrc = new Texture2D(text2D.width, text2D.height, text2D.format, text2D.mipmapCount > 1);
                //texture2Dsrc.LoadRawTextureData(text2D.GetRawTextureData());
                //texture2Dsrc.Apply();
            }
        }
    }

    public static void PrintItemIconAtlasesSprites()
    {
        //UIAtlas itemIconAtlas = uiforPlayer.xui.GetAtlasByName("itemIconAtlas");

        GameObject itemIconAtlasGO = GameObject.Find("/NGUI Root (2D)/ItemIconAtlas");
        if (itemIconAtlasGO == null)
        {
            DebugMsg("StartGame_additions: Cannot find 'ItemIconAtlas'");
        }

        DebugMsg("ItemIconAtlas sprites:");
        UIAtlas itemIconAtlas = itemIconAtlasGO.GetComponent<UIAtlas>();
        foreach (string sprite in itemIconAtlas.GetListOfSprites())
        {
            DebugMsg("\t - " + sprite);
        }

        GameObject itemIconAtlasGreyscaleGO = GameObject.Find("/NGUI Root (2D)/ItemIconAtlasGreyscale");
        if (itemIconAtlasGreyscaleGO == null)
        {
            DebugMsg("StartGame_additions: Cannot find 'ItemIconAtlasGreyscale'");
        }

        DebugMsg("ItemIconAtlasGreyscale sprites:");
        UIAtlas itemIconAtlasGreyscale = itemIconAtlasGreyscaleGO.GetComponent<UIAtlas>();
        foreach (string sprite in itemIconAtlasGreyscale.GetListOfSprites())
        {
            DebugMsg("\t - " + sprite);
        }
    }
}

