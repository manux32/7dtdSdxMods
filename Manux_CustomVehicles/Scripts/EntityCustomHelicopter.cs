using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;


class EntityCustomHelicopter : EntityCustomVehicle
{
    public GameObject entityRoot = null;
    public GameObject heliSimDummy = null;
    public GameObject prefabRoot = null;
    public HelicopterController helicoCtrl = null;
    public Rigidbody rigidBody = null;
    public BoxCollider boxcoll = null;
    public CapsuleCollider capColl = null;
    public HelicoControlPanel ctrlPanel = null;

    public Transform rotor_joint = null;
    public Transform back_rotor_joint = null;
    public Transform headlight_rot = null;

    public Transform rotor_joint1 = null;
    public Transform back_rotor_joint1 = null;
    public Transform headlight_rot1 = null;

    public Transform rotor_joint2 = null;
    public Transform back_rotor_joint2 = null;
    public Transform headlight_rot2 = null;

    public AudioSource helicoRotorSound = null;
    public AudioSource helicoMusic = null;

    public bool helicoSettingsDone;

    static bool showDebugLog = false;

    public static new void DebugMsg(string msg)
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
        InitVehicle();
    }

    public override void InitVehicle()
    {
        base.InitVehicle();

        GetVehicleBones();

        // For printing all components in the Entity hierarchy
        /*entityRoot = GetRootTransform(this.m_characterController.gameObject.transform, "helicopter").gameObject;
        Component[] comps = entityRoot.GetComponentsInChildren<Component>();
        foreach(Component comp in comps)
        {
            DebugMsg("\tHelico coll = " + comp.gameObject.name + " | " + comp.GetType() + " | " + comp.gameObject.GetInstanceID().ToString());
        }
        DebugMsg("Helico nativeCollider = " + this.nativeCollider.gameObject.name + " | " + this.nativeCollider.gameObject.GetInstanceID().ToString() + " | isTrigger = " + this.nativeCollider.isTrigger.ToString());
        DebugMsg("Helico PhysicsTransform = " + this.PhysicsTransform.gameObject.name + " | " + this.PhysicsTransform.gameObject.GetInstanceID().ToString());*/

        if (allBonesSet1Found)
            CreateHelicoSimSystem();

        helicoSettingsDone = true;
    }

    public override void GetVehicleBones()
    {
        base.GetVehicleBones();

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
                    if (rotor_joint1 == null)
                        rotor_joint1 = child;
                    else
                        rotor_joint2 = child;
                    break;
                case "back_rotor_joint":
                    if (back_rotor_joint1 == null)
                        back_rotor_joint1 = child;
                    else
                        back_rotor_joint2 = child;
                    break;
                case "headlight":
                    if (headlight_rot1 == null)
                        headlight_rot1 = child;
                    else
                        headlight_rot2 = child;
                    break;
            }
        }

        if (rotor_joint1 == null || back_rotor_joint1 == null || headlight_rot1 == null)
        {
            if (allBonesSet1Found)
                allBonesSet1Found = false;
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Helicopter will not be fully functionnal.");
        }
        else
        {
            if (allBonesSet1Found)
            {
                allBonesSet1Found = true;
                DebugMsg(this.ToString() + " : All bones set 1 found.");
            }
        }

        if (rotor_joint2 == null || back_rotor_joint2 == null || headlight_rot2 == null)
        {
            if (allBonesSet2Found)
                allBonesSet2Found = false;
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            if (allBonesSet2Found)
            {
                allBonesSet2Found = true;
                DebugMsg(this.ToString() + " : All bones set 2 found.");
            }
        }

        if (allBonesSet2Found)
        {
            rotor_joint = rotor_joint2;
            back_rotor_joint = back_rotor_joint2;
            headlight_rot = headlight_rot2;
        }
        else if (allBonesSet1Found)
        {
            rotor_joint = rotor_joint1;
            back_rotor_joint = back_rotor_joint1;
            headlight_rot = headlight_rot1;
        }
        else
        {
            DebugMsg("No Bones sets found, cannot initiate Helicopter.");
        }
    }

    public void CreateHelicoSimSystem()
    {
        if (heliSimDummy != null)
            return;
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
        AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSrc in audioSources)
        {
            if(audioSrc.clip.name.Contains("RideOffTheValkyries"))
            {
                helicoMusic = audioSrc;
                prefabRoot = audioSrc.gameObject;
                DebugMsg("Found helico Music Audio Source");
            }
            if (audioSrc.clip.name.Contains("HelicopterRotor"))
            {
                helicoRotorSound = audioSrc;
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

        Vector3 newPos = this.transform.position;
        //newPos.y += 1;
        heliSimDummy.transform.position = newPos;
        heliSimDummy.transform.rotation = this.transform.rotation;
        //rigidBody.useGravity = true;

        helicoCtrl = heliSimDummy.AddComponent<HelicopterController>();
        helicoCtrl.HelicopterModel = rigidBody;
        if (helicoRotorSound != null)
        {
            Audio.Manager.AddPlayingAudioSource(helicoRotorSound);
        }
        ctrlPanel = heliSimDummy.AddComponent<HelicoControlPanel>();
        helicoCtrl.ControlPanel = ctrlPanel;
        if (helicoMusic != null)
        {
            Audio.Manager.AddPlayingAudioSource(helicoMusic);
        }
 
        HeliRotorController rotorCtrl = rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Z;
        helicoCtrl.MainRotorController = rotorCtrl;

        rotorCtrl = back_rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Z;
        helicoCtrl.SubRotorController = rotorCtrl;

        ctrlPanel.entity = this;
        ctrlPanel.Start();
        helicoCtrl.entity = this;
        helicoCtrl.Start();

        heliSimDummy.SetActive(false);
    }

    public override void OnDriverOn()
    {
        base.OnDriverOn();

        this.AttachedEntities.m_characterController.detectCollisions = false;
        this.AttachedEntities.m_characterController.enabled = false;

        this.m_characterController.center = new Vector3(colliderCenter.x, 10f, colliderCenter.z);
        this.m_characterController.radius = 0.1f;
        this.m_characterController.stepOffset = 0.01f;
        this.m_characterController.height = 0.1f;
        this.m_characterController.detectCollisions = false;
        this.m_characterController.enabled = false;
        this.nativeBoxCollider.center = new Vector3(vehicleActivationCenter.x, 10f, vehicleActivationCenter.z);
        this.nativeBoxCollider.size = Vector3.zero;
        this.nativeCollider.enabled = false;

        Vector3 yPos = this.transform.position;
        //yPos.y += 1;
        heliSimDummy.transform.position = yPos;
        heliSimDummy.transform.rotation = this.transform.rotation;
        rigidBody.useGravity = true;
        heliSimDummy.SetActive(true);

        hasDriver = true;
        //helicoCtrl.HelicopterSound.volume = 0;
        //helicoCtrl.HelicopterSound.Play();
        helicoRotorSound.volume = 0;
        helicoRotorSound.Play();
        //Audio.Manager.Play(this, "Vehicles/Minibike/helicopter_run_lp_");
        //Audio.Manager.AudioSourceData heliRotorAudioSourceData;
        //Audio.Manager.audioSourceDatas.TryGetValue("AudioSource_Vehicle", out heliRotorAudioSourceData);
        /*if (Audio.Manager.playingAudioSources != null)
        {
            for (int i = 0; i < Audio.Manager.playingAudioSources.Count; i++)
            {
                if(Audio.Manager.playingAudioSources[i].clip != null)
                    DebugMsg("AudioSource clip = " + Audio.Manager.playingAudioSources[i].clip.name);
                if (Audio.Manager.playingAudioSources[i].clip.name == "HelicopterRotor")
                {
                    Audio.Manager.playingAudioSources[i].volume = 0;
                    helicoCtrl.HelicopterSound = Audio.Manager.playingAudioSources[i];
                }
            }
        }*/
    }

    public override void OnDriverOff()
    {
        base.OnDriverOff();

        this.m_characterController.enabled = true;
        this.m_characterController.detectCollisions = true;
        this.m_characterController.center = colliderCenter;
        this.m_characterController.radius = colliderRadius;
        this.m_characterController.height = colliderHeight;
        this.m_characterController.stepOffset = controllerStepOffset;
        this.nativeCollider.enabled = true;
        this.nativeBoxCollider.center = vehicleActivationCenter;
        this.nativeBoxCollider.size = vehicleActivationSize;
        hasDriver = false;
        player.m_characterController.enabled = true;
        player.m_characterController.detectCollisions = true;
        heliSimDummy.SetActive(false);

        //helicoCtrl.HelicopterSound.Stop();
        helicoRotorSound.Stop();
        //Audio.Manager.Stop(this.entityId, "Vehicles/Minibike/helicopter_run_lp_");
    }


    public new void FixedUpdate()
    {
        // Try-catch for now because there is apparently an error in multi
        //try
        {
            if (!helicoSettingsDone || !allBonesSet1Found)
            {
                InitVehicle();
                return;
            }

            /*if (hasDriver)
            {
                this.IncomingRemoteSimulationInput = SimInput;
            }*/
            base.FixedUpdate();

            if (!(this.AttachedEntities is EntityPlayerLocal) && helicoCtrl.IsOnGround)
            {
                return;
            }

            Vector3 newPos = heliSimDummy.transform.position;
            this.transform.position = newPos;
            this.SetPosition(newPos);
            this.transform.rotation = heliSimDummy.transform.rotation;
            this.SetRotation(heliSimDummy.transform.rotation.eulerAngles);

            UpdateSimulation();
        }
        /*catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }*/
    }


    public VehicleSimulationInput SimInput;


    public bool IsMoving()
    {
        if (rigidBody.velocity.x > 0.2f || rigidBody.velocity.y > 0.2f || rigidBody.velocity.z > 0.2f || rigidBody.angularVelocity.x > 0.2f || rigidBody.angularVelocity.y > 0.2f || rigidBody.angularVelocity.z > 0.2f)
            return true;
        return false;
    }

    public void UpdateSimulation()
    {
        //SimInput = new VehicleSimulationInput();
        SimInput = default(VehicleSimulationInput);
        SimInput.bHasData = true;

        if (hasDriver)
        {
            //SimInput.bOnGround = helicoCtrl.IsOnGround;
            SimInput.bOnGround = true;
            DebugMsg("IsOnGround = " + SimInput.bOnGround.ToString());
            SimInput.velocity = rigidBody.velocity;
            DebugMsg("RigidBody velocity: pos = " + rigidBody.velocity.ToString("0.0000") + " | rot = " + rigidBody.angularVelocity.ToString("0.0000"));
            //SimInput.bAccelerate = IsMoving();
            SimInput.bAccelerate = !helicoCtrl.IsOnGround;
            DebugMsg("IsMoving = " + SimInput.bAccelerate.ToString());
            SimInput.steering = rigidBody.angularVelocity.y;
            DebugMsg("SimInput.steering = " + SimInput.steering.ToString("0.0000"));
            //this.vehicle.UpdateSimulation(SimInput);
        }
        else
        {
            SimInput.bOnGround = true;
            SimInput.velocity = Vector3.zero;
            SimInput.bAccelerate = false;
            this.vehicle.UpdateSimulation(SimInput);
        }
    }

    public new void ReadNetData(BinaryReader _br)
    {
        base.ReadNetData(_br);

        //this.IncomingRemoteSimulationInput.bHasData = true;
        this.IncomingRemoteSimulationInput.bAccelerate = SimInput.bAccelerate;
        this.IncomingRemoteSimulationInput.bOnGround = SimInput.bOnGround;
        this.IncomingRemoteSimulationInput.steering = SimInput.steering;
        this.IncomingRemoteSimulationInput.velocity = SimInput.velocity;
    }

    public override void OnEntityUnload()
    {
        Destroy(heliSimDummy);
        base.OnEntityUnload();
    }
}

