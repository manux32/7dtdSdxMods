using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;



class CustomVehiclesUtils
{
    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
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

