using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;


class EntityCustomHelicopter : EntityCustomBike
{
    global::EntityPlayerLocal player;
    Animator playerAnimator = null;
    Transform playerSpine1Bone = null;
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
    Transform headlight_rot = null;
    Transform missileLauncher = null;
    Transform gunLauncher = null;

    Transform rotor_joint2 = null;
    Transform back_rotor_joint2 = null;
    Transform headlight_rot2 = null;
    Transform missileLauncher2 = null;
    Transform gunLauncher2 = null;

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
                case "headlight":
                    if (headlight_rot == null)
                        headlight_rot = child;
                    else
                        headlight_rot2 = child;
                    break;
                case "missileLauncher":
                    if (missileLauncher == null)
                        missileLauncher = child;
                    else
                        missileLauncher2 = child;
                    break;
                case "gunLauncher":
                    if (gunLauncher == null)
                        gunLauncher = child;
                    else
                        gunLauncher2 = child;
                    break;
            }
        }

        if (rotor_joint == null || back_rotor_joint == null || headlight_rot == null || missileLauncher == null || gunLauncher == null)
        {
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            allBonesSet1Found = true;
            DebugMsg(this.ToString() + " : All bones set 1 found.");
        }

        if (rotor_joint2 == null || back_rotor_joint2 == null || headlight_rot2 == null || missileLauncher2 == null || gunLauncher2 == null)
        {
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            allBonesSet2Found = true;
            DebugMsg(this.ToString() + " : All bones set 2 found.");
        }

        entityRoot = GetRootTransform(this.m_characterController.gameObject.transform, "helicopter").gameObject;
        Component[] comps = entityRoot.GetComponentsInChildren<Component>();
        foreach(Component comp in comps)
        {
            DebugMsg("\tHelico coll = " + comp.gameObject.name + " | " + comp.GetType() + " | " + comp.gameObject.GetInstanceID().ToString());
        }

        DebugMsg("Helico nativeCollider = " + this.nativeCollider.gameObject.name + " | " + this.nativeCollider.gameObject.GetInstanceID().ToString() + " | isTrigger = " + this.nativeCollider.isTrigger.ToString());
        DebugMsg("Helico PhysicsTransform = " + this.PhysicsTransform.gameObject.name + " | " + this.PhysicsTransform.gameObject.GetInstanceID().ToString());

        if (allBonesSet2Found)
            CreateHelicoSimSystem(rotor_joint2, back_rotor_joint2, headlight_rot2, missileLauncher2, gunLauncher2);
        else if (allBonesSet1Found)
            CreateHelicoSimSystem(rotor_joint, back_rotor_joint, headlight_rot, missileLauncher, gunLauncher);
        else
            DebugMsg("No Bones sets found, cannot initiate Helicopter.");

        player = GameManager.Instance.World.GetLocalPlayer() as global::EntityPlayerLocal;
        PrintPlayerComps();

        helicoSettingsDone = true;
    }

    public void PrintPlayerComps()
    {
        GameObject playerRoot = GetRootTransform(player.transform, "player").gameObject;
        Component[] comps = playerRoot.GetComponentsInChildren<Component>();
        foreach (Component comp in comps)
        {
            //DebugMsg("\nPlayer comp = " + comp.gameObject.name + " | " + comp.GetType());
            /*if (comp.GetType() == typeof(Animator))
            {
                playerAnimator = comp as Animator;
                playerHeadBone = playerAnimator.GetBoneTransform(HumanBodyBones.Head);
            }*/
            if(comp.transform.name == "Spine1")
            {
                playerSpine1Bone = comp.transform;
                if(helicoCtrl != null)
                {
                    helicoCtrl.playerSpine1Bone = playerSpine1Bone;
                }
            }
        }
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

    public void CreateHelicoSimSystem(Transform rotor_joint, Transform back_rotor_joint, Transform headlight, Transform missileLauncher, Transform gunLauncher)
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
            if(audioSrc.clip.name.Contains("RideOffTheValkyries"))
            {
                helicoMusic = audioSrc;
                prefabRoot = audioSrc.gameObject;
                DebugMsg("Found helico Music Audio Source");
            }
            if (audioSrc.clip.name.Contains("HelicopterRotor"))
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
            Audio.Manager.AddPlayingAudioSource(helicoSoundSrc);
            helicoCtrl.HelicopterSound = helicoSoundSrc;
        }
        ctrlPanel = heliSimDummy.AddComponent<HelicoControlPanel>();
        helicoCtrl.ControlPanel = ctrlPanel;
        if (helicoMusic != null)
        {
            Audio.Manager.AddPlayingAudioSource(helicoMusic);
            ctrlPanel.MusicSound = helicoMusic;
        }
 
        HeliRotorController rotorCtrl = rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Z;
        helicoCtrl.MainRotorController = rotorCtrl;

        rotorCtrl = back_rotor_joint.gameObject.AddComponent<HeliRotorController>();
        rotorCtrl.RotateAxis = HeliRotorController.Axis.Z;
        helicoCtrl.SubRotorController = rotorCtrl;

        helicoCtrl.headlight_rot = headlight;
        helicoCtrl.missileLauncher = missileLauncher;
        helicoCtrl.gunLauncher = gunLauncher;
        helicoCtrl.ThirdPcameraOffset = cameraOffset;
        helicoCtrl.ThirdPlayerOffsetPos = playerOffsetPos;

        ctrlPanel.entityHelico = this;
        helicoCtrl.entityHelico = this;
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
                    helicoCtrl.player = player;

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
                    helicoCtrl.newThirdPcameraOffset = cameraOffset;

                    PrintPlayerComps();

                    if (!helicoCtrl.is3rdPersonView)
                    {
                        helicoCtrl.ToggleFirstAnd3rdPersonView(false, true);
                    }


                    helicoCtrl.HelicopterSound.volume = 0;
                    helicoCtrl.HelicopterSound.Play();
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

                    DebugMsg("Game Cam parent = " + player.vp_FPCamera.transform.parent.name + " (pos = " + player.vp_FPCamera.transform.position + " | vehicle pos = " + this.position + ")");
                }

                Vector3 yPos2 = heliSimDummy.transform.position;
                yPos2.y += 0.25f;
                this.transform.position = yPos2;
                this.SetPosition(yPos2);
                this.transform.rotation = heliSimDummy.transform.rotation;
                this.SetRotation(heliSimDummy.transform.rotation.eulerAngles);

                helicoCtrl.helicoEntityTransform = this.transform;
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

                    helicoCtrl.HelicopterSound.Stop();
                    //Audio.Manager.Stop(this.entityId, "Vehicles/Minibike/helicopter_run_lp_");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }
    }


    public void OnGUI()
    {
        DrawCrosshair();
    }

    // from EntityPlayerLocal.guiDrawCrosshair()
    //public void OnGUI()
    public void DrawCrosshair()
    {
        if (!(this.AttachedEntities is global::EntityPlayerLocal))
            return;
        //if (!Event.current.type.Equals(EventType.Repaint) || player.movementInput.bAltCameraMove)
        if (!Event.current.type.Equals(EventType.Repaint))
        {
            return;
        }
        if (player.IsDead())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 crossHairPos = ray.GetPoint(1000);// + (Vector3.up * 20);

        Vector3 targetScreenPos = player.playerCamera.WorldToScreenPoint(crossHairPos);  //helicoCtrl.targetPos
        //Vector2 crosshairPosition2D2 = player.GetCrosshairPosition2D();
        Vector2 crosshairPosition2D2 = new Vector2(targetScreenPos.x, targetScreenPos.y);
        crosshairPosition2D2.y = (float)Screen.height - crosshairPosition2D2.y;

        int crosshairOpenArea = player.GetCrosshairOpenArea();
        int num = (int)crosshairPosition2D2.x;
        int num2 = (int)crosshairPosition2D2.y;
        Color black = Color.yellow;
        Color white = Color.yellow;
        //black.a = this.WSQ() * player.weaponCrossHairAlpha; // WSQ() = 0.5f
        //white.a = this.WSQ() * player.weaponCrossHairAlpha;
        black.a = 1.0f;
        white.a = 1.0f;
        // black
        global::GUIUtils.DrawLine(new Vector2((float)(num - crosshairOpenArea), (float)(num2 + 1)), new Vector2((float)(num - (crosshairOpenArea + 18)), (float)(num2 + 1)), black);
        global::GUIUtils.DrawLine(new Vector2((float)(num + crosshairOpenArea), (float)(num2 + 1)), new Vector2((float)(num + crosshairOpenArea + 18), (float)(num2 + 1)), black);
        global::GUIUtils.DrawLine(new Vector2((float)(num + 1), (float)(num2 - crosshairOpenArea)), new Vector2((float)(num + 1), (float)(num2 - (crosshairOpenArea + 18))), black);
        global::GUIUtils.DrawLine(new Vector2((float)(num + 1), (float)(num2 + crosshairOpenArea)), new Vector2((float)(num + 1), (float)(num2 + crosshairOpenArea + 18)), black);
        // white
        global::GUIUtils.DrawLine(new Vector2((float)(num + crosshairOpenArea), (float)num2), new Vector2((float)(num + crosshairOpenArea + 18), (float)num2), white);
        global::GUIUtils.DrawLine(new Vector2((float)num, (float)(num2 - crosshairOpenArea)), new Vector2((float)num, (float)(num2 - (crosshairOpenArea + 18))), white);
        global::GUIUtils.DrawLine(new Vector2((float)(num - crosshairOpenArea), (float)num2), new Vector2((float)(num - (crosshairOpenArea + 18)), (float)num2), white);
        global::GUIUtils.DrawLine(new Vector2((float)num, (float)(num2 + crosshairOpenArea)), new Vector2((float)num, (float)(num2 + crosshairOpenArea + 18)), white);
    }

    public override void OnEntityUnload()
    {
        Destroy(heliSimDummy);
        base.OnEntityUnload();
    }
}

