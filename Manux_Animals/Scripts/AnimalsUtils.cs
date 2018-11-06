using System;
using System.Globalization;
using UnityEngine;


class AnimalsUtils
{
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
}

