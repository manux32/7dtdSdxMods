using System;
using System.Collections.Generic;
using System.Reflection;
using Audio;
using UnityEngine;


public class XUiC_VehicleCollectedItemList : XUiC_CollectedItemList
{
    // Obfuscated Entity Fields and Methods
    public FieldInfo DHZ_float_field = null;
    //public MethodInfo QXQ_MethodInfo = null;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void Init()
    {
        base.Init();
        FindObfuscatedFieldsAndMethods();
        global::XUiController childById = base.GetChildById("item");
        this.PrefabItems = childById.ViewComponent.UiTransform;
        if (DHZ_float_field != null)
        {
            //this.DHZ = (float)(childById.ViewComponent.Size.y + 2);
            this.DHZ_float_field.SetValue(this, (float)(childById.ViewComponent.Size.y + 2));
        }
        //childById.xui.CollectedItemList = this;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        this.PrefabItems.gameObject.SetActive(false);
        DebugMsg("XUiC_VehicleCollectedItemList.OnOpen: Hi, I'm a stupid fuck!");
    }

    public void FindObfuscatedFieldsAndMethods()
    {
        FieldInfo[] listOfFieldNames;
        MethodInfo[] listOfMethodNames;

        string DHZ_name = "DHZ";
        //string QXQ_name = "QXQ";

        if (GameManager.IsDedicatedServer)
        {
            DHZ_name = "CUH";
            //QXQ_name = "KQA";
        }

        // Get hooks on Entity Obfuscated fields and methods
        listOfFieldNames = typeof(Entity).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in listOfFieldNames)
        {
            if (fieldInfo.Name == DHZ_name)
            {
                DHZ_float_field = fieldInfo;
                if (DHZ_float_field != null)
                {
                    DebugMsg("Found field DHZ");
                }
            }
        }

        /*listOfMethodNames = typeof(Entity).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (MethodInfo methodInfo in listOfMethodNames)
        {
            if (methodInfo.Name == QXQ_name)
            {
                QXQ_MethodInfo = methodInfo;
                if (QXQ_MethodInfo != null)
                {
                    DebugMsg("Found method QXQ");
                }
            }
        }*/
    }
}

