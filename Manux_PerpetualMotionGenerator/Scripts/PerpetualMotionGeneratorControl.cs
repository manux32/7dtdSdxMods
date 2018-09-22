using System;
using UnityEngine;
using System.Collections.Generic;

public class PerpetualMotionGeneratorControl : MonoBehaviour 
{
	public int cIdx;
	public Vector3i blockPos;
	bool isOn;
    bool curIsOn;
    Renderer[] renderers = null;
    public string soundRepeat;
    float soundRepeatOffsetStart = -1;

    void Awake()
	{
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend.material.name.Contains("Mod4Lightcone"))
            {
                rend.material.SetColor("_Color", new Color(0, 0, 0));
            }
            else
            {
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
            }
        }
    }

    void Update()
	{
        TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)GameManager.Instance.World.GetTileEntity(cIdx, blockPos);
        if (tileEntityPowerSource != null && renderers != null)
        {
            curIsOn = tileEntityPowerSource.IsOn;
            if (curIsOn != isOn)
            {
                isOn = curIsOn;
                if (isOn)
                {
                    //Audio.Manager.BroadcastPlay(blockPos.ToVector3(), soundRepeat);
                    soundRepeatOffsetStart = Time.time;
                    foreach (Renderer rend in renderers)
                    {
                        if (rend.material.name.Contains("Mod4Lightcone"))
                        {
                            rend.material.SetColor("_Color", new Color(0, 0.67f, 1));
                        }
                        else
                        {
                            rend.material.EnableKeyword("_EMISSION");
                            rend.material.SetColor("_EmissionColor", new Color(1, 1, 1));
                        }
                    }
                }
                else
                {
                    Audio.Manager.BroadcastStop(blockPos.ToVector3(), soundRepeat);
                    foreach (Renderer rend in renderers)
                    {
                        if (rend.material.name.Contains("Mod4Lightcone"))
                        {
                            rend.material.SetColor("_Color", new Color(0, 0, 0));
                        }
                        else
                        {
                            rend.material.EnableKeyword("_EMISSION");
                            rend.material.SetColor ("_EmissionColor", new Color(0, 0, 0));
                        }
                    }
                }
            }

            if (isOn)
            {
                if (soundRepeatOffsetStart != -1 && Time.time - soundRepeatOffsetStart > 0.85f)
                {
                    Audio.Manager.BroadcastPlay(blockPos.ToVector3(), soundRepeat);
                    soundRepeatOffsetStart = -1;
                }
            }

        }
	}
}