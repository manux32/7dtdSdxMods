using System;
using UnityEngine;

public class MinerControl : MonoBehaviour 
{
	public int cIdx;
	public Vector3i blockPos;
    public Vector3i blockOnTopPos;
    public BlockValue blockOnTopBlockValue;
    Block blockOnTopBlock = null;
    bool isMining;
    bool curIsMining;
    Renderer[] minerBlockRenderers = null;
    Light[] light = null;
    GameObject blade = null;
    Vector3 bladeLocalEulerRot;
    Quaternion bladeLocalQuatRot;

    public string soundStart;
    public string soundRepeat;
    public string soundEnd;

    float soundRepeatOffsetStart = -1;

    void Awake()
	{
        blade = FindChildGameObject(gameObject, "Blade");
        minerBlockRenderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in minerBlockRenderers)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
        }
        light = gameObject.GetComponentsInChildren<Light>();
        light[0].intensity = 0;

        blockOnTopPos = blockPos;
        blockOnTopPos.y += 1;
    }

    static public GameObject FindChildGameObject(GameObject fromGameObject, string name)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == name) return t.gameObject;
        return null;
    }

    void Update()
	{
        blockOnTopBlockValue = GameManager.Instance.World.GetBlock(blockOnTopPos);
        blockOnTopBlock = Block.list[blockOnTopBlockValue.type];
        if (blockOnTopBlock.GetBlockName() == "minerRareOresGrowing" || blockOnTopBlock.GetBlockName() == "minerRegularOresGrowing")
        {
            curIsMining = true;
        }
        else
        {
            curIsMining = false;
        }

        if (curIsMining != isMining)
        {
            isMining = curIsMining;
            if (isMining)
            {
                light[0].intensity = 1;
                Audio.Manager.BroadcastPlay(blockPos.ToVector3(), soundStart);
                soundRepeatOffsetStart = Time.time;
                foreach (Renderer rend in minerBlockRenderers)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor("_EmissionColor", new Color(1, 1, 1));
                }
            }
            else
            {
                light[0].intensity = 0;
                Audio.Manager.BroadcastStop(blockPos.ToVector3(), soundRepeat);
                Audio.Manager.BroadcastPlay(blockPos.ToVector3(), soundEnd);
                foreach (Renderer rend in minerBlockRenderers)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
                }
            }
        }

        if(blade != null && isMining)
        {
            if(soundRepeatOffsetStart != -1 && Time.time - soundRepeatOffsetStart > 1.7f)
            {
                Audio.Manager.BroadcastPlay(blockPos.ToVector3(), soundRepeat);
                soundRepeatOffsetStart = -1;
            }
            bladeLocalEulerRot = blade.transform.localEulerAngles;
            bladeLocalEulerRot.z -= 10;
            bladeLocalQuatRot.eulerAngles = bladeLocalEulerRot;
            blade.transform.localRotation = bladeLocalQuatRot;
        }
    }
}