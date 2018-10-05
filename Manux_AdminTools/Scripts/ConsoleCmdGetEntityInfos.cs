using System;
using System.Collections.Generic;
using UnityEngine;


class ConsoleCmdGetEntityInfos : ConsoleCmdAbstract
{
    public override string[] GetCommands()
    {
        return new string[]
        {
        "getentityinfos",
        "gei"
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
            global::EntityAlive entityAlive = null;
            if (entity is global::EntityAlive)
            {
                entityAlive = (global::EntityAlive)entity;
            }

            string output = "Entity infos:\n";
            output += (entity.ToString() + "\n");
            output += ("pos = " + entity.GetPosition().ToString() + "\n");
            output += ("rot = " + entity.rotation.ToString() + "\n");
            output += ("lifetime = " + ((entity.lifetime != float.MaxValue) ? entity.lifetime.ToCultureInvariantString("0.0").ToString() : "float.Max".ToString()));
            output += ("remote = " + entity.isEntityRemote.ToString() + "\n");
            output += ("dead = " + entity.IsDead().ToString() + "\n");

            if (entityAlive != null)
            {
                output += ("health = " + entityAlive.Health.ToString() + " / " + entityAlive.classMaxHealth.ToString() + "\n");
                output += ("stamina = " + entityAlive.Stamina.ToString() + " / " + entityAlive.classMaxStamina.ToString() + "\n");
                output += ("food = " + entityAlive.Stats.Food.ToString() + " / " + entityAlive.classMaxFood.ToString() + "\n");
                output += ("water = " + entityAlive.Stats.Water.ToString() + " / " + entityAlive.classMaxWater.ToString() + "\n");
                output += ("sickness = " + entityAlive.Stats.Sickness.ToString() + " / " + entityAlive.classMaxSickness.ToString() + "\n");

                Animation[] animation = entityAlive.gameObject.GetComponentsInChildren<Animation>();
                if (animation != null && animation.Length > 0)
                {
                    output += ("Animations:\n");
                    foreach (AnimationState state in animation[0])
                    {
                        output += ("state: " + state.name + " / clip: " + state.clip.name + " / enabled = " + state.enabled.ToString() + " / length: " + state.clip.length.ToString() + "\n");
                    }
                }

                Animator[] animator = entityAlive.gameObject.GetComponentsInChildren<Animator>();
                if (animator != null && animator.Length > 0)
                {
                    AnimatorClipInfo[] animatorClipInfo = animator[0].GetCurrentAnimatorClipInfo(0);
                    if(animatorClipInfo != null && animatorClipInfo.Length > 0)
                    {
                        output += ("AnimatorClipInfo: " + animatorClipInfo[0].clip.name + " / length = " + animatorClipInfo[0].clip.length.ToString() + "\n");
                    }

                    AnimatorStateInfo animatorStateInfo = animator[0].GetCurrentAnimatorStateInfo(0);
                    output += ("AnimatorStateInfo: length = " + animatorStateInfo.length.ToString() + " / speed = " + animatorStateInfo.speed.ToString() + "\n");
                }

                Renderer[] renderers = entityAlive.GetComponentsInChildren<Renderer>();
                output += "Renderers:\n";
                foreach (Renderer rend in renderers)
                {
                    output += ("object " + rend.gameObject.name +  " material = " + rend.material.shader.name + "\n");
                }
            }

            global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output(output);
        }
        else
        {
            global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output("Entity '" + _params[0] + "' not found");
        }
    }


    public override string GetDescription()
    {
        return "lists infos on en entity. first param is the id of the entity.";
    }
}

