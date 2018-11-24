using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;



class CustomVehiclesUtils
{
    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public static Transform GetRootTransform(Transform fromTransform)
    {
        if (fromTransform.parent != null)
        {
            DebugMsg("GetRootTransform parent = " + fromTransform.gameObject.name + " | " + fromTransform.gameObject.GetInstanceID().ToString());
            return GetRootTransform(fromTransform.parent);
        }
        return fromTransform;
    }

    public static GameObject FindChildGameObject(GameObject fromGameObject, string name)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
            if (t.gameObject.name == name)
                return t.gameObject;
        return null;
    }

    public static void GetAllChildTransforms(Transform root, ref List<Transform> childrenList, ref List<int> childrenInstanceIds)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (!childrenInstanceIds.Contains(child.GetInstanceID()))
            {
                childrenList.Add(child);
                childrenInstanceIds.Add(child.GetInstanceID());
            }
            GetAllChildTransforms(child, ref childrenList, ref childrenInstanceIds);
        }
    }

    public static bool StringVectorToVector3(string stringVec, out Vector3 newVector3)
    {
        string[] stringVector;
        stringVector = stringVec.Split(',');
        if (stringVector.Length == 3)
        {
            float x;
            float.TryParse(stringVector[0], out x);
            float y;
            float.TryParse(stringVector[1], out y);
            float z;
            float.TryParse(stringVector[2], out z);
            newVector3 = new Vector3(x, y, z);
            return true;
        }
        else
        {
            Debug.LogError("Xml Vector is invalid");
        }
        newVector3 = Vector3.zero;
        return false;
    }

    public static bool StringVectorToVector2i(string stringVec, out Vector2i newVector2i)
    {
        string[] stringVector;
        stringVector = stringVec.Split(',');
        if (stringVector.Length == 2)
        {
            int x;
            int.TryParse(stringVector[0], out x);
            int y;
            int.TryParse(stringVector[1], out y);
            newVector2i = new Vector2i(x, y);
            return true;
        }
        else
        {
            Debug.LogError("Xml Vector is invalid");
        }
        
        newVector2i = Vector2i.zero;
        return false;
    }

    public static bool StringVectorToColor(string stringVec, out Color newColor)
    {
        string[] stringVector;
        stringVector = stringVec.Split(',');
        if (stringVector.Length == 3)
        {
            float r;
            float.TryParse(stringVector[0], NumberStyles.Float, CultureInfo.InvariantCulture, out r);
            float g;
            float.TryParse(stringVector[1], NumberStyles.Float, CultureInfo.InvariantCulture, out g);
            float b;
            float.TryParse(stringVector[2], NumberStyles.Float, CultureInfo.InvariantCulture, out b);
            newColor = new Color(r, g, b);
            return true;
        }
        else
        {
            Debug.Log("Xml Mesh Color is invalid");
        }
        newColor = new Color();
        return false;
    }

    public static void ChangeMeshesColor(Color color, Renderer[] renderers)
    {
        foreach (Renderer rend in renderers)
        {
            rend.material.EnableKeyword("_COLOR");
            rend.material.SetColor("_Color", color);
        }
    }

    public static void ChangeMeshesEmissiveColor(Color color, Renderer[] renderers)
    {
        foreach (Renderer rend in renderers)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", color);
        }
    }

    public static float GetRatio(float value, float min, float max)
    {
        float range = max - min;
        float mult = 1.0f / range;
        return Mathf.Abs(Mathf.Abs(value * mult) - Mathf.Abs(min * mult));
    }


    // Checks if there is something else than water in bounds
    public static bool IsLandInBounds(Bounds _aabb)
    {
        int num = Utils.Fastfloor(_aabb.min.x);
        int num2 = Utils.Fastfloor(_aabb.max.x + 1f);
        int num3 = Utils.Fastfloor(_aabb.min.y);
        int num4 = Utils.Fastfloor(_aabb.max.y + 1f);
        int num5 = Utils.Fastfloor(_aabb.min.z);
        int num6 = Utils.Fastfloor(_aabb.max.z + 1f);
        if (_aabb.min.x < 0f)
        {
            num--;
        }
        if (_aabb.min.y < 0f)
        {
            num3--;
        }
        if (_aabb.min.z < 0f)
        {
            num5--;
        }

        for (int i = num; i < num2; i++)
        {
            for (int j = num3; j < num4; j++)
            {
                for (int k = num5; k < num6; k++)
                {
                    int type = GameManager.Instance.World.GetBlock(i, j, k).type;
                    if (!(Block.list[type].blockMaterial.IsLiquid || Block.list[type].GetBlockName() == "air" || Block.list[type].GetBlockName() == "smallBoatDummyBlock" || Block.list[type].GetBlockName() == "waterSupportBlock"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool GetBlockPosInBounds(string blockName, Bounds _aabb, ref Vector3i resultPos)
    {
        Vector3i curPos;
        //Vector3 curPos;
        BlockValue curBlockVal;
        Block curBlock;

        int num = Utils.Fastfloor(_aabb.min.x);
        int num2 = Utils.Fastfloor(_aabb.max.x + 1f);
        int num3 = Utils.Fastfloor(_aabb.min.y);
        int num4 = Utils.Fastfloor(_aabb.max.y + 1f);
        int num5 = Utils.Fastfloor(_aabb.min.z);
        int num6 = Utils.Fastfloor(_aabb.max.z + 1f);
        if (_aabb.min.x < 0f)
        {
            num--;
        }
        if (_aabb.min.y < 0f)
        {
            num3--;
        }
        if (_aabb.min.z < 0f)
        {
            num5--;
        }

        for (int i = num; i < num2; i++)
        {
            for (int j = num3; j < num4; j++)
            {
                for (int k = num5; k < num6; k++)
                {
                    curPos = new Vector3i(i, j, k);
                    curBlockVal = GameManager.Instance.World.GetBlock(curPos);
                    curBlock = Block.list[curBlockVal.type];
                    if (curBlock.GetBlockName() == blockName)
                    {
                        DebugMsg("GetBlockPosInBounds: Found Block: " + curBlock.GetBlockName() + " | pos = " + curPos.ToString());
                        resultPos = curPos;
                        return true;
                    }
                }
            }
        }

        return false;
    }


    public static float BlockYRotToEularYRot(int _blockRot)
    {
        string msg = "CustomVehiclesUtils BlockYRotToEularYRot:\n";
        msg += "_blockRot = " + _blockRot.ToString() + "\n";
        float resultEularYRot = 0;

        if (_blockRot == 0)
        {
            resultEularYRot = 0;
        }
        else if (_blockRot == 24)
        {
            resultEularYRot = 45;
        }
        else if (_blockRot == 1)
        {
            resultEularYRot = 90;
        }
        else if (_blockRot == 25)
        {
            resultEularYRot = 135;
        }
        else if (_blockRot == 2)
        {
            resultEularYRot = 180;
        }
        else if (_blockRot == 26)
        {
            resultEularYRot = -135;
        }
        else if (_blockRot == 3)
        {
            resultEularYRot = -90;
        }
        else if(_blockRot == 27)
        {
            resultEularYRot = -45;
        }

        msg += "resultEularYRot = " + resultEularYRot.ToString("0.00") + "\n";
        DebugMsg(msg);
        return resultEularYRot;
    }


    public static int EularYRotToBlockYRot(Vector3 _entityForwardVec)
    {
        string msg = "CustomVehiclesUtils EularYRotToBlockYRot:\n";
        int resultBlockRot = 0;
        float angle = Vector3.Angle(Vector3.forward, _entityForwardVec);

        msg += "\t- eulatRot angle (pure) = " + angle.ToString("0.000") + "\n";
        if (angle > 0 && angle > 180)
            angle -= 360;
        else if (angle < 0 && angle < 180)
            angle += 360;
        msg += "\t- eulatRot angle (adjusted) = " + angle.ToString("0.000") + "\n";

        float dot = Vector3.Dot(Vector3.right, _entityForwardVec);
        bool negAngle = (dot < 0);
        msg += "\t- dot = " + dot.ToString("0.000") + " | negAnle = " + negAngle.ToString() + "\n";

        if ((negAngle && angle < 22.5f) || (!negAngle && angle <= 22.5f))
        {
            resultBlockRot = 0;
        }
        else if (!negAngle && angle > 22.5f && angle <= 67.5f)
        {
            resultBlockRot = 24;
        }
        else if (!negAngle && angle > 67.5f && angle <= 112.5f)
        {
            resultBlockRot = 1;
        }
        else if (!negAngle && angle > 112.5f && angle <= 157.5f)
        {
            resultBlockRot = 25;
        }
        else if ((!negAngle && angle > 157.5f) || (negAngle && angle >= 157.5f))
        {
            resultBlockRot = 2;
        }
        else if (negAngle && angle < 157.5f && angle >= 112.5f)
        {
            resultBlockRot = 26;
        }
        else if (negAngle && angle < 112.5f && angle >= 67.5f)
        {
            resultBlockRot = 3;
        }
        else
        {
            resultBlockRot = 27;
        }

        msg += "\t- resultBlockRot = " + resultBlockRot.ToString() + "\n";
        DebugMsg(msg);
        return resultBlockRot;
    }



    /*public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }*/
}

