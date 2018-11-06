using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public static class EmuUtility
{
    // Token: 0x060000F6 RID: 246 RVA: 0x0000AFA4 File Offset: 0x00009FA4
    public static bool IsTerrainClear(Ray ray, float maxDistance, ref Vector3 hitPos)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, 65536))
        {
            hitPos = hitInfo.point;
            if (hitInfo.collider.CompareTag("T_Mesh") || hitInfo.collider.CompareTag("B_Mesh") || hitInfo.collider.CompareTag("T_Mesh_B"))
            {
                if (GameManager.Instance.World.GetBlock(new Vector3i(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z)).type == 0)
                {
                    if (GameManager.Instance.World.GetBlock(new Vector3i(hitInfo.point + Vector3.down)).type > 0)
                    {
                        return true;
                    }
                }
                else if (GameManager.Instance.World.GetBlock(new Vector3i(hitInfo.point + Vector3.up)).type == 0)
                {
                    return true;
                }
            }
        }
        else
        {
            hitPos = ray.GetPoint(5f);
        }
        return false;
    }
}
