using System;
using UnityEngine;
using System.Collections.Generic;

public class HydroponicFarmPowerControl : MonoBehaviour 
{
	public int cIdx;
	public Vector3i blockPos;
	bool isOn;
    bool curIsOn;
    bool isPowered;
    bool curIsPowered;
    Renderer[] powerBlockRenderers = null;
    Renderer[] workstationBlockRenderers = null;
    Light[] light = null;

    void Awake()
	{
        powerBlockRenderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in powerBlockRenderers)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
        }
        light = gameObject.GetComponentsInChildren<Light>();
        light[0].intensity = 0;
    }

    public void GetWorkstationBlockRenderers()
    {
        Vector3i workstationBlockPos = blockPos;
        workstationBlockPos.y += 1;
        Chunk workstationBlockChunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(workstationBlockPos);
        if (workstationBlockChunk != null)
        {
            BlockEntityData workstationBlockEntityData = workstationBlockChunk.GetBlockEntity(workstationBlockPos);
            if (workstationBlockEntityData != null)
            {
                workstationBlockRenderers = workstationBlockEntityData.transform.gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in workstationBlockRenderers)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
                }
            }
        }
    }

    void Update()
	{
        if (workstationBlockRenderers == null)
        {
            GetWorkstationBlockRenderers();
        }

        curIsPowered = BlockHydroponicFarmPower.IsBlockPoweredUp(blockPos, cIdx);
        if (curIsPowered != isPowered)
        {
            isPowered = curIsPowered;
            if (isPowered)
            {
                light[0].intensity = 1;
                foreach (Renderer rend in powerBlockRenderers)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor("_EmissionColor", new Color(1, 1, 1));
                }
                if (workstationBlockRenderers != null)
                {
                    foreach (Renderer rend in workstationBlockRenderers)
                    {
                        rend.material.EnableKeyword("_EMISSION");
                        rend.material.SetColor("_EmissionColor", new Color(1, 1, 1));
                    }
                }
            }
            else
            {
                light[0].intensity = 0;
                foreach (Renderer rend in powerBlockRenderers)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor ("_EmissionColor", new Color(0, 0, 0));
                }
                if (workstationBlockRenderers != null)
                {
                    foreach (Renderer rend in workstationBlockRenderers)
                    {
                        rend.material.EnableKeyword("_EMISSION");
                        rend.material.SetColor("_EmissionColor", new Color(0, 0, 0));
                    }
                }
            }
        }
	}
}