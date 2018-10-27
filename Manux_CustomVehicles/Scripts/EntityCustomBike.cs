using UnityEngine;


class EntityCustomBike : EntityCustomVehicle
{
    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
    }

    protected override void Start()
    {
        base.Start();
    }

    public new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}


