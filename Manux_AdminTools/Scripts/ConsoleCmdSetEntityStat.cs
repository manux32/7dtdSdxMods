using System;
using System.Collections.Generic;


class ConsoleCmdSetEntityStat : ConsoleCmdAbstract
{
    public override string[] GetCommands()
    {
        return new string[]
        {
        "setentitystat",
        "ses"
        };
    }

    public List<string> supportedStats = new List<string> { "health",
                                                            "stamina",
                                                            "sickness",
                                                            "gassiness",
                                                            "speedmodifier",
                                                            "wellness",
                                                            "coretemp",
                                                            "food",
                                                            "water",
                                                            "waterlevel"
                                                                    };

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        if(_params.Count < 3)
        {
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("You need to use 3 parameters: [Entity ID] [Stat Name] [Stat Value]");
            return;
        }

        if(!supportedStats.Contains(_params[1].ToLower()))
        {
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output(_params[1] + "is not a valid Entity Stat");
            return;
        }

        int id;
        if (int.TryParse(_params[0], out id))
        {
            Entity entity = null;
            for (int i = GameManager.Instance.World.Entities.list.Count - 1; i >= 0; i--)
            {
                Entity curEntity = GameManager.Instance.World.Entities.list[i];
                if (curEntity.entityId == id)
                {
                    entity = curEntity;
                    break;
                }
            }
            if (entity == null || !entity.GetType().IsSubclassOf(typeof(EntityAlive)))
            {
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Not a valid entity");
                return;
            }

            EntityAlive entityAlive = (EntityAlive)entity;

            float value;
            if (float.TryParse(_params[2], out value))
            {
                // special case to modify the wetness level of an entity
                if (_params[1].ToLower() == "waterlevel")
                {
                    entityAlive.Stats.SetWaterLevel(value);
                }
                // all other stats
                else
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output(entityAlive.ToString() + ": Setting Stat " + _params[1] + " to " + _params[2]);
                    entityAlive.Stats.SetValue(_params[1].ToLower(), value);
                }
            }
        }
        else
        {
            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Entity '" + _params[0] + "' not found");
        }
    }


    public override string GetDescription()
    {
        return "Sets an Entity Stat: setentitystat <entity id> <entity stat name> <new entity stat value>";
    }

    public override string GetHelp()
    {
        string returnString = "Usage:\n";
        returnString += "    setentitystat <entity id> <entity stat name> <new entity stat value>\n";
        returnString += "    ses <entity id> <entity stat name> <new entity stat value>\n";
        returnString += "Sets an Entity Stat value.\n";
        returnString += "Supported Entity Stats:\n";

        foreach(string stat in supportedStats)
        {
            returnString += ("    - " + stat + "\n");
        }

        return returnString;
    }
}

