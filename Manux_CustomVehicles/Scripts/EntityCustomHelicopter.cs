using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;


class EntityCustomHelicopter : EntityCustomBike
{
    global::EntityPlayerLocal player;
    GameObject entityRoot = null;
    GameObject heliSimDummy = null;
    GameObject prefabRoot = null;
    HelicopterController helicoCtrl = null;
    Rigidbody rigidBody = null;
    BoxCollider boxcoll = null;
    CapsuleCollider capColl = null;
    HelicoControlPanel ctrlPanel = null;

    Transform rotor_joint = null;
    Transform back_rotor_joint = null;

    Transform rotor_joint2 = null;
    Transform back_rotor_joint2 = null;

    bool helicoSettingsDone;

    bool allBonesSet1Found;
    bool allBonesSet2Found;

    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    public void Init()
    {
        List<Transform> childrenList = new List<Transform>();
        List<int> childrenInstanceIds = new List<int>();
        childrenList.Add(this.transform);
        childrenInstanceIds.Add(this.transform.GetInstanceID());
        CustomVehiclesUtils.GetAllChildTransforms(this.transform, ref childrenList, ref childrenInstanceIds);

        foreach (Transform child in childrenList)
        {
            switch (child.name)
            {
                case "rotor_joint":
                    if (rotor_joint == null)
                        rotor_joint = child;
                    else
                        rotor_joint2 = child;
                    break;
                case "back_rotor_joint":
                    if (back_rotor_joint == null)
                        back_rotor_joint = child;
                    else
                        back_rotor_joint2 = child;
                    break;
            }
        }

        if (rotor_joint == null || back_rotor_joint == null)
        {
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            allBonesSet1Found = true;
            DebugMsg(this.ToString() + " : All bones set 1 found.");
        }

        if (rotor_joint2 == null || back_rotor_joint2 == null)
        {
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            allBonesSet2Found = true;
            DebugMsg(this.ToString() + " : All bones set 2 found.");
        }

        entityRoot = GetRootTransform(this.m_characterController.gameObject.transform, "helicopter").gameObject;
        Collider[] colls = entityRoot.GetComponentsInChildren<Collider>();
        foreach(Collider coll in colls)
        {
            DebugMsg("\tHelico coll = " + coll.gameObject.name + " | " + coll.GetType() + " | " + coll.gameObject.GetInstanceID().ToString());
        }

        DebugMsg("Helico nativeCollider = " + this.nativeCollider.gameObject.name + " | " + this.nativeCollider.gameObject.GetInstanceID().ToString() + " | isTrigger = " + this.nativeCollider.isTrigger.ToString());
        DebugMsg("Helico PhysicsTransform = " + this.PhysicsTransform.gameObject.name + " | " + this.PhysicsTransform.gameObject.GetInstanceID().ToString());

        if (allBonesSet2Found)
            CreateHelicoSimSystem(rotor_joint2, back_rotor_joint2);
        else if (allBonesSet1Found)
            CreateHelicoSimSystem(rotor_joint, back_rotor_joint);
        else
            DebugMsg("No Bones sets found, cannot initiate Helicopter.");

        player = GameManager.Instance.World.GetLocalPlayer() as global::EntityPlayerLocal;
        GameObject playerRoot = GetRootTransform(player.transform, "player").gameObject;
        colls = playerRoot.GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
        {
            DebugMsg("\nPlayer coll = " + coll.gameObject.name + " | " + coll.GetType());
        }

        helicoSettingsDone = true;
    }

    public static Transform GetRootTransform(Transform fromTransform, string stopAtString)
    {
        if (fromTransform.parent != null)
        {
            DebugMsg("GetRootTransform parent = " + fromTransform.gameObject.name + " | " + fromTransform.gameObject.GetInstanceID().ToString());
            if (stopAtString != null && fromTransform.gameObject.name.ToLower().Contains(stopAtString))
                return fromTransform;

            return GetRootTransform(fromTransform.parent, stopAtString);
        }
        return fromTransform;
    }

    public void CreateHelicoSimSystem(Transform rotor_joint, Transform back_rotor_joint)
    {
        heliSimDummy = new GameObject("helicoCtrl");
        // For debugging with a 3d cube that is visible in the game
        /*heliSimDummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        BoxCollider bcoll = heliSimDummy.GetComponent<BoxCollider>();
        if(bcoll != null)
        {
            DebugMsg("Destroying cube BoxCollider");
            Destroy(bcoll);
        }*/
        DebugMsg("Helico heliSimDummy = " + heliSimDummy.name + " | " + heliSimDummy.GetInstanceID().ToString());

        AudioSource helicoSoundSrc = null;
        AudioSource helicoMusic = null;
        AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSrc in audioSources)
        {
            if(audioSrc.clip.name.Contains("ride of the valkyries"))
            {
                helicoMusic = audioSrc;
                prefabRoot = audioSrc.gameObject;
                DebugMsg("Found helico Music Audio Source");
            }
            if (audioSrc.clip.name.Contains("Helicopter"))
            {
                helicoSoundSrc = audioSrc;
                DebugMsg("Found helico rotor Audio Source");
            }
        }

        rigidBody = heliSimDummy.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.mass = 100;
        rigidBody.drag = 1;
        rigidBody.angularDrag = 4;
        rigidBody.interpolation = RigidbodyInterpolation.None;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        capColl = heliSimDummy.AddComponent<CapsuleCollider>();
        capColl.center = new Vector3(0, 0.125f, 0.3f);
        capColl.radius = 0.125f;
        capColl.height = 7f;
        capColl.direction = 2;

        boxcoll = heliSimDummy.AddComponent<BoxCollider>();
        boxcoll.center = new Vector3(0, 0.125f, 0.3f);
        boxcoll.size = new Vector3(2.5f, 0.25f, 7f);

        Vector3 newPos = this.PhysicsTransform.position;
        newPos.y += 1;
        heliSimDummy.transform.position = newPos;
        heliSimDummy.transform.rotation = this.PhysicsTransform.rotation;
        this.m_characterController.center = new Vector3(colliderCenter.x, 1.5f, colliderCenter.z);
        rigidBody.useGravity = true;

        helicoCtrl = heliSimDummy.AddComponent<HelicopterController>();
        helicoCtrl.HelicopterModel = rigidBody;
        if (helicoSoundSrc != null)
        {
            helicoCtrl.HelicopterSound = helicoSoundSrc;
        }
        ctrlPanel = heliSimDummy.AddComponent<HelicoControlPanel>();
        helicoCtrl.ControlPanel = ctrlPanel;
        if (helicoMusic != null)
        {
            helicoCtrl.ControlPanel.MusicSound = helicoMusic;
        }
 
        HeliRotorController rotorCtrl = rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Y;
        helicoCtrl.MainRotorController = rotorCtrl;

        rotorCtrl = back_rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Z;
        helicoCtrl.SubRotorController = rotorCtrl;
    }

    public new void Update()
    {
        try
        {
            base.Update();
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }

    public new void FixedUpdate()
    {
        try
        {
            if (!helicoSettingsDone)
            {
                Init();
                return;
            }

            base.FixedUpdate();

            if ((this.AttachedEntities is global::EntityPlayerLocal))
            {
                if (!helicoCtrl.hasDriver)
                {
                    player = this.AttachedEntities as global::EntityPlayerLocal;

                    this.AttachedEntities.m_characterController.detectCollisions = false;
                    this.AttachedEntities.m_characterController.enabled = false;

                    this.m_characterController.center = new Vector3(colliderCenter.x, 2f, colliderCenter.z);
                    this.m_characterController.detectCollisions = false;
                    this.m_characterController.enabled = false;
                    this.nativeCollider.enabled = false;
 
                    Vector3 yPos = this.PhysicsTransform.position;
                    yPos.y += 1;
                    heliSimDummy.transform.position = yPos;
                    heliSimDummy.transform.rotation = this.PhysicsTransform.rotation;

                    helicoCtrl.hasDriver = true;
                    ctrlPanel.hasDriver = true;
                }

                Vector3 yPos2 = heliSimDummy.transform.position;
                yPos2.y += 0.25f;
                this.transform.position = yPos2;
                this.SetPosition(yPos2);
                this.transform.rotation = heliSimDummy.transform.rotation;
                this.SetRotation(heliSimDummy.transform.rotation.eulerAngles);
            }
            else
            {
                if (helicoCtrl.hasDriver)
                {   
                    this.m_characterController.enabled = true;
                    this.m_characterController.detectCollisions = true;
                    this.m_characterController.center = new Vector3(colliderCenter.x, 1.5f, colliderCenter.z);
                    this.nativeCollider.enabled = true;
                    helicoCtrl.hasDriver = false;
                    ctrlPanel.hasDriver = false;
                    player.m_characterController.enabled = true;
                    player.m_characterController.detectCollisions = true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }

    public override void OnEntityUnload()
    {
        Destroy(heliSimDummy);
        base.OnEntityUnload();
    }
}

