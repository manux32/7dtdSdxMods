using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


class ConsoleCmdGetEntityPrefabInfos : ConsoleCmdAbstract
{
    public override string[] GetCommands()
    {
        return new string[]
        {
        "getentityprefabinfos",
        "gepi"
        };
    }

    public override void Execute(List<string> _params, global::CommandSenderInfo _senderInfo)
    {
        int id;
        if (int.TryParse(_params[0], out id))
        {
            global::Entity entity = null;
            for (int i = global::GameManager.Instance.World.Entities.list.Count - 1; i >= 0; i--)
            {
                global::Entity curEntity = global::GameManager.Instance.World.Entities.list[i];
                if (curEntity.entityId == id)
                {
                    entity = curEntity;
                    break;
                }
            }
            if (entity == null)
            {
                global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output("Not a valid entity");
                return;
            }

            string output = "------------------------------------------------------------\n";
            output += ("------------------------------------------------------------\n");
            output += "ENTITY PREFAB INFOS:\n";
            output += (entity.ToString() + "\n");
            output += ("------------------------------------------------------------\n");
            output += ("------------------------------------------------------------\n");
            Transform root = GetRootTransform(entity.transform, entity.name.ToLower());
            List<Transform> childrenList = new List<Transform>();
            List<int> childrenInstanceIds = new List<int>();
            if (root != null && root.gameObject != null)
            {
                output += ("Root = " + root.gameObject.name + " | " + root.gameObject.GetInstanceID().ToString());
            }
            else
            {
                root = entity.transform;
            }
            childrenList.Add(root);
            childrenInstanceIds.Add(root.GetInstanceID());
            GetAllChildTransforms(root, ref childrenList, ref childrenInstanceIds);

            output += ("CHILDREN:\n");
            output += ("------------------------------------------------------------\n");
            output += ("------------------------------------------------------------\n");
            foreach (Transform child in childrenList)
            {
                output += (child.name + " | parent: " + child.parent.name + ":\n");                
            }
            output += ("------------------------------------------------------------\n");
            output += ("------------------------------------------------------------\n");
            output += ("DETAILED INFOS:\n");
            output += ("------------------------------------------------------------\n");
            foreach (Transform child in childrenList)
            {
                output += ("------------------------------------------------------------\n");
                output += ("name: " + child.name + "\n");
                output += ("\tparent: " + child.parent.name + "\n");
                output += ("\ttag: " + child.tag.ToString() + "\n");
                output += ("\tinstance id: " + child.GetInstanceID().ToString() + "\n");
                output += ("components:\n");
                Component[] components = child.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    output += ("\t" + comp.ToString() + "\n");
                }
            }
            output += ("------------------------------------------------------------\n");

            global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output(output);
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "7D2D");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, "GetEntityPrefabInfos.txt");
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            File.WriteAllText(path, output);
        }
        else
        {
            global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output("Entity '" + _params[0] + "' not found");
        }
    }

    public void GetAllChildTransforms(Transform root, ref List<Transform> childrenList, ref List<int> childrenInstanceIds)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if(!childrenInstanceIds.Contains(child.GetInstanceID()))
            {
                childrenList.Add(child);
                childrenInstanceIds.Add(child.GetInstanceID());
            }
            GetAllChildTransforms(child, ref childrenList, ref childrenInstanceIds);
        }
    }

    public static Transform GetRootTransform(Transform fromTransform, string stopAtString)
    {
        if (fromTransform.parent != null)
        {
            if (stopAtString != null && fromTransform.gameObject.name.ToLower().Contains(stopAtString))
                return fromTransform;

            return GetRootTransform(fromTransform.parent, stopAtString);
        }
        return fromTransform;
    }

    public override string GetDescription()
    {
        return "lists infos on en entity prefab. first param is the id of the entity.";
    }
}

