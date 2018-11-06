using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;


public class EntityPetCompanion : EntityAnimalHal
{
    Color meshColor;
    bool setMeshColor = false;
    Color meshEmissiveColor;
    bool setMeshEmissiveColor = false;
    float goBackToPlayerChecksDelay = 2;
    float goBackToPlayerChecksStart;

    static bool showDebugLog = true;
    
    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(Time.time.ToString() + ": " + msg);
        }
    }

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        if (entityClass.Properties.Values.ContainsKey("MeshColor"))
        {
            Color newColor;
            if (AnimalsUtils.StringVectorToColor(entityClass.Properties.Values["MeshColor"], out newColor))
            {
                setMeshColor = true;
                meshColor = newColor;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("MeshEmissiveColor"))
        {
            Color newColor;
            if (AnimalsUtils.StringVectorToColor(entityClass.Properties.Values["MeshEmissiveColor"], out newColor))
            {
                setMeshEmissiveColor = true;
                meshEmissiveColor = newColor;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //this.MaxLedgeHeight = 4;
        this.MaxLedgeHeight = 20;

        goBackToPlayerChecksStart = Time.time;
    }

    protected override void Start()
    {
        base.Start();
        if (setMeshColor)
        {
            AnimalsUtils.ChangeMeshesColor(meshColor, gameObject.GetComponentsInChildren<Renderer>());
        }
        if (setMeshEmissiveColor)
        {
            AnimalsUtils.ChangeMeshesEmissiveColor(meshEmissiveColor, gameObject.GetComponentsInChildren<Renderer>());
        }
    }

    public override bool WillForceToFollow(global::EntityAlive _other)
    {
        return true;
    }

    public override bool CanEntityJump()
    {
        return true;
    }

    protected override bool canDespawn()
    {
        return false;
    }

    new void Update()
    {
        base.Update();

        //if (Time.time > goBackToPlayerChecksStart + goBackToPlayerChecksDelay)
        {
            //goBackToPlayerChecksStart = Time.time;
            EntityAlive player = GameManager.Instance.World.GetLocalPlayer();
            //this.ChaseReturnLocation = player.gameObject.transform.position;

            //DebugMsg("Checking if we send the pet to player");
            if (Vector3.Distance(this.gameObject.transform.position, player.gameObject.transform.position) > 7)
            {
                if (this.GetRevengeTarget() == null || this.GetAttackTarget() == null || this.GetAttackTarget() != this.GetRevengeTarget())
                {
                    //this.targetPos = player.gameObject.transform.position;
                    EntityMoveHelper moveHelper = this.getMoveHelper();
                    moveHelper.setMoveTo(player.gameObject.transform.position, 1.6f);
                    //DebugMsg("Sending pet to player");
                }
            }
        }
    }

    public override float GetSeeDistance()
    {
        //return 20f;
        return 120f;
    }

}
