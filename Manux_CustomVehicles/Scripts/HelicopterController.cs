using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public Entity entity;
    EntityCustomHelicopter entityHelico; 
    public HelicoControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 20f;
    public float ForwardForce = 10f;
    public float EngineForceRatio = 1f;
    public float AdjEngineForceRatio = 1f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 500f;
    public float lastHeight;

    public float MaxEngineForce = 60f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 10f;

    float dot;
    Vector3 mouseHitPos;
    Vector3 mousePos = new Vector3();

    public EntityPlayerLocal player;
    public Vector3 headlightTargetPos;

    float updatesDebugPrintDelay = 2f;
    float lastDebugPrintTime = -1f;

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
            if (entityHelico != null)
            {
                MainRotorController.RotarSpeed = value * 80;
                SubRotorController.RotarSpeed = value * 40;
                if (entityHelico.helicoRotorSound != null)
                {
                    if (value > 0)
                    {
                        entityHelico.helicoRotorSound.volume = 1;
                        entityHelico.helicoRotorSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
                    }
                    else
                    {
                        entityHelico.helicoRotorSound.volume = 0;
                    }
                }
            }

            _engineForce = Mathf.Clamp(value, 0f, MaxEngineForce);
        }
    }

    public Vector2 hMove = Vector2.zero;
    public Vector2 hTilt = Vector2.zero;
    public float hTurn = 0f;
    public float upForce = 0f;
    public bool IsOnGround = true;
    public bool IsMovingForward = false;
    public Vector3 straightForward = Vector3.zero;


    public void Start ()
	{
        ControlPanel.KeyPressed += OnKeyPressed;
        InitController();
        lastHeight = HelicopterModel.position.y;
        lastDebugPrintTime = Time.time;
    }

    public void InitController()
    {
        entityHelico = entity as EntityCustomHelicopter;
        player = entityHelico.player;
    }
  
    void FixedUpdate()
    {
        if (entityHelico == null)
        {
            InitController();
            return;
        }

        if (!entityHelico.hasDriver || !entityHelico.HasFuel())
        {
            EngineForce -= 0.5f;
            if (EngineForce < 0)
                EngineForce = 0;
            entityHelico.headlight_rot.localRotation = Quaternion.identity;
            return;
        }

        EngineForceRatio = CustomVehiclesUtils.GetRatio(Mathf.Clamp(EngineForce, 10f, MaxEngineForce), 10f, MaxEngineForce);
        AdjEngineForceRatio = (EngineForceRatio * 4f) + 1f;

        LiftProcess();
        MoveProcess();
        TiltProcess();
        HeadlightMovement();

        if(showDebugLog && Time.time - updatesDebugPrintDelay > lastDebugPrintTime)
        {
            DebugMsg("Engine force = " + EngineForce.ToString("0.000"));
            DebugMsg("Adjusted Engine force Ratio = " + AdjEngineForceRatio.ToString("0.000"));
            DebugMsg("Adjusted ForwardForce = " + (ForwardForce * AdjEngineForceRatio).ToString("0.000"));
            DebugMsg("IsOnGround = " + IsOnGround.ToString());
            DebugMsg("velocity = " + HelicopterModel.velocity.magnitude.ToString("0.000"));
            lastDebugPrintTime = Time.time;
        }
    }

    private void HeadlightMovement()
    {
        // Headlight movement from mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //headlightTargetPos = ray.GetPoint(100) + (Vector3.up * 20);
        headlightTargetPos = ray.GetPoint(200f);
        entityHelico.headlight_rot.LookAt(headlightTargetPos, entityHelico.headlight_rot.parent.transform.up);
        dot = Vector3.Dot(entityHelico.headlight_rot.forward, entityHelico.headlight_rot.parent.forward);
        //DebugMsg("dot = " + dot.ToString("0.0000"));
        entityHelico.headlight_rot.rotation = Quaternion.Slerp(entityHelico.headlight_rot.rotation, entityHelico.headlight_rot.parent.rotation, (Mathf.Abs(Mathf.Clamp(dot, -0.8f, -0.5f)) * 3.3333f) - 1.6666f);
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        //HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * (ForwardForce * AdjEngineForceRatio) * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        //HelicopterModel.AddRelativeForce(Vector3.up * upForce);
        if (IsMovingForward && EngineForce >= 10f)
        {
            HelicopterModel.AddRelativeForce(Vector3.up * upForce * (AdjEngineForceRatio * (EngineForceRatio + 1f)));
        }
        else
            HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnKeyPressed(PressedKeyCode[] obj)
    {
        if (!entityHelico.hasDriver || !entityHelico.HasFuel())
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

        IsMovingForward = false;
        foreach (var pressedKeyCode in obj)
        {
            switch (pressedKeyCode)
            {
                case PressedKeyCode.SpeedUpPressed:

                    EngineForce += 0.2f;
                    break;

                case PressedKeyCode.SpeedDownPressed:

                    EngineForce -= 0.2f;
                    if (EngineForce < 0) EngineForce = 0;
                    break;

                case PressedKeyCode.ForwardPressed:

                    if (IsOnGround) break;
                    tempY = Time.fixedDeltaTime;
                    IsMovingForward = true;
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
            }
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);
    }

    private void OnCollisionEnter()
    {
        if(EngineForce < 5f)
            IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
}