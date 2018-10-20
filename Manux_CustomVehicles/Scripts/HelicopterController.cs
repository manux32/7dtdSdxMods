using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public Entity entityHelico;
    public AudioSource HelicopterSound = null;
    public HelicoControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 20f;
    public float ForwardForce = 20f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 500f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 10f;

    public Transform headlight_rot = null;
    float dot;
    Vector3 mouseHitPos;
    Vector3 mousePos = new Vector3();
    float screenWidth;
    float screenHeight;
    float midScreenX;
    float midScreenY;
    public bool hasDriver = false;

    public global::EntityPlayerLocal player;
    public Transform playerSpine1Bone = null;
    public Transform helicoEntityTransform;
    public bool is3rdPersonView = true;
    public Transform ThirdPcameraParent;
    public Vector3 ThirdPcameraOffset;
    public Vector3 newThirdPcameraOffset;
    public Quaternion camTospine1Offset;
    //public Vector3 FPcameraOffset;
    public Vector3 ThirdPlayerOffsetPos;
    public float lastCamToggle = -1;
    public Transform missileLauncher = null;
    public Transform gunLauncher = null;
    public Vector3 targetPos;
    public float gunShootDelay = 0.2f;
    public float lastGunShoot = -1;
    public float missileShootDelay = 1.2f;
    public float lastMissileShoot = -1;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            //if (HelicopterSound != null)
            {
                if (!GameManager.Instance.IsPaused() && value > 0)
                {
                    HelicopterSound.volume = 1;
                    HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
                }
                else
                {
                    HelicopterSound.volume = 0;
                }
            }
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;


    // Use this for initialization
    void Start ()
	{
        ControlPanel.KeyPressed += OnKeyPressed;

        screenWidth = UnityEngine.Screen.width;
        screenHeight = UnityEngine.Screen.height;
        midScreenX = screenWidth / 2.0f;
        midScreenY = screenHeight / 2.0f;
        DebugMsg("Screen resolution = " + screenWidth.ToString() + " (" + midScreenX.ToString() + ") X " + screenHeight.ToString() + " (" + midScreenY.ToString() + ")");
        newThirdPcameraOffset = ThirdPcameraOffset;
    }

	void Update ()
    {
        if (hasDriver)
        {
            if (Input.GetMouseButton(0) && Time.time - gunShootDelay > lastGunShoot)
            {
                DebugMsg("Left-click");
                ShootProjectile(gunLauncher, "helicopterBullet", "Weapons/Ranged/AK47/ak47_fire_start", true);
                lastGunShoot = Time.time;
            }
            if (Input.GetMouseButton(1) && Time.time - missileShootDelay > lastMissileShoot)
            {
                DebugMsg("Right-click");
                ShootProjectile(missileLauncher, "helicopterRocket", "Weapons/Ranged/M136/m136_fire", false);
                lastMissileShoot = Time.time;
            }
            // Zoom in-out
            if (is3rdPersonView)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                {
                    newThirdPcameraOffset.z += 2.0f;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
                {
                    newThirdPcameraOffset.z -= 2.0f;
                }
                if (newThirdPcameraOffset.z > -4f)
                {
                    //newThirdPcameraOffset.y = Mathf.Lerp(10f, 0, (Mathf.Abs(Mathf.Clamp(newThirdPcameraOffset.z, 0f, 2f)) * 0.5f));
                    newThirdPcameraOffset.z = -4f;
                }
                else
                {
                    newThirdPcameraOffset.y = Mathf.Lerp(ThirdPcameraOffset.y, 4f, 1f - (Mathf.Abs(Mathf.Clamp(newThirdPcameraOffset.z, ThirdPcameraOffset.z, -4f)) * (1f / (ThirdPcameraOffset.z - 4f))));
                }
                player.vp_FPCamera.Position3rdPersonOffset = newThirdPcameraOffset;
            }
        }
    }


    public void LateUpdate()
    {
        if (hasDriver && !is3rdPersonView)
        {
            //Vector3 newPos = helicoEntityTransform.position + ThirdPlayerOffsetPos;
            Vector3 newPos = playerSpine1Bone.position + ((playerSpine1Bone.forward) * 0.2f) + ((-playerSpine1Bone.right) * 0.6f);
            //newPos.y += 1;
            player.cameraTransform.position = newPos;
            //player.cameraTransform.position = playerHeadBone.position;
            //player.cameraTransform.rotation = helicoEntityTransform.rotation;
            //player.cameraTransform.rotation = (player.cameraTransform.rotation * Quaternion.Inverse(playerSpine1Bone.rotation)) * (playerSpine1Bone.rotation * camTospine1Offset);
        }
    }

  
    void FixedUpdate()
    {
        if (!hasDriver)
        {
            IsOnGround = true;
            EngineForce -= 1.2f;
            if (EngineForce < 0) EngineForce = 0;
            headlight_rot.localRotation = Quaternion.identity;
            return;
        }

        LiftProcess();
        MoveProcess();
        TiltProcess();
        HeadlightMovement();
    }

    private void HeadlightMovement()
    {
        // Headlight movement from mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //targetPos = ray.GetPoint(100) + (Vector3.up * 20);
        targetPos = ray.GetPoint(200f);
        headlight_rot.LookAt(targetPos, headlight_rot.parent.transform.up);
        dot = Vector3.Dot(headlight_rot.forward, headlight_rot.parent.forward);
        //DebugMsg("dot = " + dot.ToString("0.0000"));
        headlight_rot.rotation = Quaternion.Slerp(headlight_rot.rotation, headlight_rot.parent.rotation, (Mathf.Abs(Mathf.Clamp(dot, -0.8f, -0.5f)) * 3.3333f) - 1.6666f);
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    public void ToggleFirstAnd3rdPersonView(bool switchFirstPerson, bool bLerpPosition)
    {
        if (switchFirstPerson)
        {
            //player.SwitchModelView(global::EnumEntityModelView.FirstPerson);
            //player.SetPosition(helicoEntityPos + ThirdPlayerOffsetPos);

            //player.SetCameraAttachedToPlayer(false);
            //player.vp_FPCamera.Position3rdPersonOffset = Vector3.zero;
            is3rdPersonView = false;

            ThirdPcameraParent = player.cameraTransform.parent;
            //player.cameraTransform.parent = helicoEntityTransform;
            player.cameraTransform.parent = null;
            //Vector3 newPos = helicoEntityTransform.position + ThirdPlayerOffsetPos;
            //newPos.y += 1;
            player.cameraTransform.position = playerSpine1Bone.position;
            //player.cameraTransform.rotation = helicoEntityTransform.rotation;
            //camTospine1Offset = player.cameraTransform.rotation * Quaternion.Inverse(playerSpine1Bone.rotation);
            camTospine1Offset = player.cameraTransform.rotation * Quaternion.Inverse(playerSpine1Bone.rotation);
        }
        else
        {
            //player.SwitchModelView(global::EnumEntityModelView.ThirdPerson);
            //player.SetModelLayer(24, false);
            //newThirdPcameraOffset = ThirdPcameraOffset;
            //player.SetCameraAttachedToPlayer(true);
            //player.vp_FPCamera.Position3rdPersonOffset = newThirdPcameraOffset;
            player.cameraTransform.parent = ThirdPcameraParent;
            player.cameraTransform.localPosition = Vector3.zero;
            player.cameraTransform.localEulerAngles = Vector3.zero;
            is3rdPersonView = true;
        }
        lastCamToggle = Time.time;
        player.updateCameraPosition(bLerpPosition);
    }

    public void ShootProjectile(Transform projectileLauncher, string ammoName, string soundPath, bool isGun)
    {
        ItemClass ammoItem = ItemClass.GetItemClass(ammoName, false);
        ItemValue itemValue = ItemClass.GetItem(ammoItem.GetItemName(), false);
        Transform projectile = ammoItem.CloneModel(GameManager.Instance.World, itemValue, Vector3.zero, null, false, false);

        if (projectileLauncher != null)
        {
            projectile.parent = projectileLauncher;
            projectile.localPosition = Vector3.zero;
            projectile.localRotation = Quaternion.identity;
        }
        else
        {
            projectile.parent = null;
        }
        global::Utils.SetLayerRecursively(projectile.gameObject, (!(projectileLauncher != null)) ? 0 : projectileLauncher.gameObject.layer);
        global::BlockProjectileMoveScript blockProjectileMoveScript = projectile.gameObject.AddComponent<global::BlockProjectileMoveScript>();
        blockProjectileMoveScript.itemProjectile = ammoItem;
        blockProjectileMoveScript.itemValueProjectile = itemValue;
        blockProjectileMoveScript.itemValueLauncher = global::ItemValue.None.Clone();
        blockProjectileMoveScript.itemActionProjectile = (global::ItemActionProjectile)((!(ammoItem.Actions[0] is global::ItemActionProjectile)) ? ammoItem.Actions[1] : ammoItem.Actions[0]);
        blockProjectileMoveScript.AttackerEntityId = 0;

        //Vector3 target = targetPos - projectileLauncher.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayOffset = Vector3.Distance(player.GetThirdPersonCameraTransform().position, projectileLauncher.position) + 2f;
        Vector3 rayStart = ray.GetPoint(rayOffset);
        RaycastHit hit;
        if (Physics.Raycast(rayStart, ray.direction, out hit))
        {
            //Vector3 crossHairPos = ray.GetPoint(1000);// + (Vector3.up * 20);
            //Vector3 targetScreenPos = player.playerCamera.WorldToScreenPoint(crossHairPos);     //targetPos
            blockProjectileMoveScript.Fire(projectileLauncher.position, hit.point - projectileLauncher.position, player, 0);
            //blockProjectileMoveScript.Fire(projectileLauncher.position, ray.direction, player, 0);
        }
        else
        {
            Vector3 rayEnd = ray.GetPoint(200f);
            blockProjectileMoveScript.Fire(projectileLauncher.position, rayEnd - projectileLauncher.position, player, 0);
        }
        


        if (isGun)
        {
            global::ParticleEffect pe = new global::ParticleEffect("nozzleflash_ak", projectileLauncher.position, Quaternion.Euler(0f, 180f, 0f), 1f, Color.white, "Pistol_Fire", projectileLauncher);
            float lightValue = global::GameManager.Instance.World.GetLightBrightness(global::World.worldToBlockPos(projectileLauncher.position)) / 2f;
            global::ParticleEffect pe2 = new global::ParticleEffect("nozzlesmokeuzi", projectileLauncher.position, lightValue, new Color(1f, 1f, 1f, 0.3f), null, projectileLauncher, false);
            SpawnParticleEffect(pe, -1);
            SpawnParticleEffect(pe2, -1);
            return;
        }

        if (global::Steam.Network.IsServer)
        {
            Audio.Manager.BroadcastPlay(projectileLauncher.position, soundPath);
        }
    }

    public void SpawnParticleEffect(global::ParticleEffect _pe, int _entityId)
    {
        if (global::Steam.Network.IsServer)
        {
            if (!global::GameManager.IsDedicatedServer)
            {
                global::GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId);
            }
            global::SingletonMonoBehaviour<global::ConnectionManager>.Instance.SendPackage(new global::NetPackageParticleEffect(_pe, _entityId), false, -1, _entityId, -1, -1);
            return;
        }
        global::SingletonMonoBehaviour<global::ConnectionManager>.Instance.SendToServer(new global::NetPackageParticleEffect(_pe, _entityId), false);
    }

    private void OnKeyPressed(PressedKeyCode[] obj)
    {
        if (!hasDriver)
            return;

        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = - Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;

        foreach (var pressedKeyCode in obj)
        {
            switch (pressedKeyCode)
            {
                case PressedKeyCode.SpeedUpPressed:

                    //EngineForce += 0.1f;
                    EngineForce += 1f;
                    break;
                case PressedKeyCode.SpeedDownPressed:

                    //EngineForce -= 0.12f;
                    EngineForce -= 1.2f;
                    if (EngineForce < 0) EngineForce = 0;
                    break;

                    case PressedKeyCode.ForwardPressed:

                    if (IsOnGround) break;
                    tempY = Time.fixedDeltaTime;
                    break;
                    case PressedKeyCode.BackPressed:

                    if (IsOnGround) break;
                    tempY = -Time.fixedDeltaTime;
                    break;
                    case PressedKeyCode.LeftPressed:

                    if (IsOnGround) break;
                    tempX = -Time.fixedDeltaTime;
                    break;
                    case PressedKeyCode.RightPressed:

                    if (IsOnGround) break;
                    tempX = Time.fixedDeltaTime;
                    break;
                    case PressedKeyCode.TurnRightPressed:
                    {
                        if (IsOnGround) break;
                        var force = (turnForcePercent - Mathf.Abs(hMove.y))*HelicopterModel.mass;
                        HelicopterModel.AddRelativeTorque(0f, force, 0);
                    }
                    break;
                    case PressedKeyCode.TurnLeftPressed:
                    {
                        if (IsOnGround) break;
                        
                        var force = -(turnForcePercent - Mathf.Abs(hMove.y))*HelicopterModel.mass;
                        HelicopterModel.AddRelativeTorque(0f, force, 0);
                    }
                    break;
                    case PressedKeyCode.ToggleFirstThirdPersonPressed:
                    {
                        if (Time.time - 1.0f > lastCamToggle)
                        {
                            if (is3rdPersonView)
                            {
                                ToggleFirstAnd3rdPersonView(true, true);
                            }
                            else
                            {
                                ToggleFirstAnd3rdPersonView(false, true);
                            }
                        }
                    }
                    break;

            }
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
}