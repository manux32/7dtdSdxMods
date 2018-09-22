using System;
using System.Collections.Generic;


class ConsoleCmdListSpawnableEntities : ConsoleCmdAbstract
{
    public override string[] GetCommands()
    {
        return new string[]
        {
        "listspawnableentities",
        "lse"
        };
    }

    public override void Execute(List<string> _params, global::CommandSenderInfo _senderInfo)
    {
        int num = 1;
        string output = "";

        foreach (int v in global::EntityClass.list.Keys)
        {
            if (global::EntityClass.list[v].bAllowUserInstantiate)
            {
                if (_params.Count == 1)
                {
                    if(global::EntityClass.list[v].entityClassName.Contains(_params[0]))
                    {
                        output += num + " - " + global::EntityClass.list[v].entityClassName + " | ";
                    }
                    num++;
                    continue;
                }

                if (_params.Count == 0)
                {
                    output += num + " - " + global::EntityClass.list[v].entityClassName + " | ";
                    num++;
                }
            }
        }
        global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output(output);
    }


    public override string GetDescription()
    {
        return "lists all spawnable entities. first param to return entites that contain that search string.";
    }
}

