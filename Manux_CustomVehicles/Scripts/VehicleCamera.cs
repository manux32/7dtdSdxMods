using System;
using System.Collections.Generic;
using UnityEngine;


public class VehicleCamera : MonoBehaviour
{
    public Entity entity = null;
    EntityCustomVehicle entityVehicle = null;

    public bool is3rdPersonView = true;
    public Transform ThirdPcameraParent;
    public Vector3 newThirdPcameraOffset = new Vector3();
    public float thirdPcameraCloseupOffsetY;
    public float thirdPcamLerpMult;
    public Quaternion camTospine1Offset;
    public float lastCamToggle = -1;

    float screenWidth;
    float screenHeight;
    float midScreenX;
    float midScreenY;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }


    public void Start()
    {
        InitController();
    }

    public void InitController()
    {
        if (entity == null)
            return;
        entityVehicle = entity as EntityCustomVehicle;
        if (entityVehicle.cameraOffset == null)
            return;
        /*screenWidth = UnityEngine.Screen.width;
        screenHeight = UnityEngine.Screen.height;
        midScreenX = screenWidth / 2.0f;
        midScreenY = screenHeight / 2.0f;
        DebugMsg("Screen resolution = " + screenWidth.ToString() + " (" + midScreenX.ToString() + ") X " + screenHeight.ToString() + " (" + midScreenY.ToString() + ")");*/
        newThirdPcameraOffset = entityVehicle.cameraOffset;
    }


    void Update()
    {
        if(entityVehicle == null || entityVehicle.player == null || entityVehicle.cameraOffset == null)
        {
            InitController();
            return;
        }

        if (!entityVehicle.hasDriver)
            return;

        // toggle 1st-3rd person view
        if (Input.GetKey(KeyCode.Home) && Time.time - 1.0f > lastCamToggle)
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

        // 3rd person view Zoom in-out
        if (is3rdPersonView && entityVehicle.player != null)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                if (newThirdPcameraOffset.z >= entityVehicle.cameraOffset.z)
                    newThirdPcameraOffset.z += Mathf.Lerp(1.0f, 0.02f, thirdPcamLerpMult);
                else
                    newThirdPcameraOffset.z += Mathf.Lerp(1.0f, 5.0f, thirdPcamLerpMult);
                DebugMsg("thirdPcamLerpMult = " + thirdPcamLerpMult.ToString("0.0000"));
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                if (newThirdPcameraOffset.z >= entityVehicle.cameraOffset.z)
                    newThirdPcameraOffset.z -= Mathf.Lerp(1.0f, 0.2f, thirdPcamLerpMult);
                else
                    newThirdPcameraOffset.z -= Mathf.Lerp(1.0f, 5.0f, thirdPcamLerpMult);
                DebugMsg("thirdPcamLerpMult = " + thirdPcamLerpMult.ToString("0.0000"));
            }

            if (newThirdPcameraOffset.z > entityVehicle.cameraOffset.z / 3.0f)
            {
                newThirdPcameraOffset.z = entityVehicle.cameraOffset.z / 3.0f;
            }
            
            if(newThirdPcameraOffset.z >= entityVehicle.cameraOffset.z)
            {
                thirdPcamLerpMult = GetRatio(Mathf.Clamp(newThirdPcameraOffset.z, entityVehicle.cameraOffset.z, entityVehicle.cameraOffset.z / 3.0f), entityVehicle.cameraOffset.z, entityVehicle.cameraOffset.z / 3.0f);
                newThirdPcameraOffset.y = Mathf.Lerp(entityVehicle.cameraOffset.y, entityVehicle.cameraOffset.y + Mathf.Clamp(Mathf.Abs(entityVehicle.cameraOffset.y) * 2.0f, 0.5f, 2.0f), thirdPcamLerpMult);
            }
            else
            {
                thirdPcamLerpMult = 1f - GetRatio(Mathf.Clamp(newThirdPcameraOffset.z, -50.0f, entityVehicle.cameraOffset.z), -50.0f, entityVehicle.cameraOffset.z);
                newThirdPcameraOffset.y = Mathf.Lerp(entityVehicle.cameraOffset.y, entityVehicle.cameraOffset.y + 15.0f, thirdPcamLerpMult);
            }

            entityVehicle.player.vp_FPCamera.Position3rdPersonOffset = newThirdPcameraOffset;
        }
    }


    public float GetRatio(float value, float min, float max)
    {
        float range = max - min;
        float mult = 1.0f / range;
        return Mathf.Abs(Mathf.Abs(value * mult) - Mathf.Abs(min * mult));
    }

    public void LateUpdate()
    {
        if (entityVehicle == null || entityVehicle.player == null)
        {
            InitController();
            return;
        }

        if (!entityVehicle.hasDriver)
            return;

        if (!is3rdPersonView)
        {
            if (entityVehicle.playerSpine1Bone == null)
            {
                entityVehicle.GetPlayerSpine1Bone();
            }

            Vector3 newPos = entityVehicle.playerSpine1Bone.position + ((entityVehicle.playerSpine1Bone.forward) * 0.2f) + ((-entityVehicle.playerSpine1Bone.right) * 0.6f);
            entityVehicle.player.cameraTransform.position = newPos;
            //entityVehicle.player.cameraTransform.rotation = entityVehicle.transform.rotation;
            //entityVehicle.player.cameraTransform.rotation = (player.cameraTransform.rotation * Quaternion.Inverse(entityVehicle.playerSpine1Bone.rotation)) * (entityVehicle.playerSpine1Bone.rotation * camTospine1Offset);
        }
    }

    public void ToggleFirstAnd3rdPersonView(bool switchFirstPerson, bool bLerpPosition)
    {
        if (entityVehicle == null || entityVehicle.player == null)
        {
            InitController();
            return;
        }

        if (entityVehicle.playerSpine1Bone == null)
        {
            entityVehicle.GetPlayerSpine1Bone();
        }

        if (switchFirstPerson)
        {
            //entityVehicle.player.SwitchModelView(EnumEntityModelView.FirstPerson);
            //entityVehicle.player.SetPosition(helicoEntityPos + entityVehicle.playerOffsetPos);

            //entityVehicle.player.SetCameraAttachedToPlayer(false);
            //entityVehicle.player.vp_FPCamera.Position3rdPersonOffset = Vector3.zero;
            is3rdPersonView = false;

            ThirdPcameraParent = entityVehicle.player.cameraTransform.parent;
            //entityVehicle.player.cameraTransform.parent = entityVehicle.transform;
            entityVehicle.player.cameraTransform.parent = null;
            entityVehicle.player.cameraTransform.position = entityVehicle.playerSpine1Bone.position;
            //entityVehicle.player.cameraTransform.rotation = entityVehicle.transform.rotation;
            //camTospine1Offset = player.cameraTransform.rotation * Quaternion.Inverse(entityVehicle.playerSpine1Bone.rotation);
            //camTospine1Offset = entityVehicle.player.cameraTransform.rotation * Quaternion.Inverse(entityVehicle.playerSpine1Bone.rotation);
        }
        else
        {
            //entityVehicle.player.SwitchModelView(EnumEntityModelView.ThirdPerson);
            //entityVehicle.player.SetModelLayer(24, false);
            //newThirdPcameraOffset = entityVehicle.cameraOffset;
            //entityVehicle.player.SetCameraAttachedToPlayer(true);
            //entityVehicle.player.vp_FPCamera.Position3rdPersonOffset = newThirdPcameraOffset;
            entityVehicle.player.cameraTransform.parent = ThirdPcameraParent;
            entityVehicle.player.cameraTransform.localPosition = Vector3.zero;
            entityVehicle.player.cameraTransform.localEulerAngles = Vector3.zero;
            is3rdPersonView = true;
        }

        lastCamToggle = Time.time;
        entityVehicle.player.updateCameraPosition(bLerpPosition);
    }


    public void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint) || GameManager.Instance.IsPaused())
            return;

        if (entityVehicle == null || entityVehicle.player == null)
        {
            InitController();
            return;
        }

        if (entityVehicle.player.movementInput.bAltCameraMove || entityVehicle.player.IsDead() || !entityVehicle.hasDriver || !(entityVehicle.HasGun() || entityVehicle.HasExplosiveLauncher()))
            return;

        DrawCrosshair();
    }

    // from EntityPlayerLocal.guiDrawCrosshair()
    public void DrawCrosshair()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 crossHairPos = ray.GetPoint(1000);// + (Vector3.up * 20);

        Vector3 targetScreenPos = entityVehicle.player.playerCamera.WorldToScreenPoint(crossHairPos);  //helicoCtrl.targetPos
        //Vector2 crosshairPosition2D2 = player.GetCrosshairPosition2D();
        Vector2 crosshairPosition2D2 = new Vector2(targetScreenPos.x, targetScreenPos.y);
        crosshairPosition2D2.y = (float)Screen.height - crosshairPosition2D2.y;

        int crosshairOpenArea = entityVehicle.player.GetCrosshairOpenArea();
        int num = (int)crosshairPosition2D2.x;
        int num2 = (int)crosshairPosition2D2.y;
        Color black = Color.yellow;
        Color white = Color.yellow;
        //black.a = this.WSQ() * player.weaponCrossHairAlpha; // WSQ() = 0.5f
        //white.a = this.WSQ() * player.weaponCrossHairAlpha;
        black.a = 1.0f;
        white.a = 1.0f;
        // black
        GUIUtils.DrawLine(new Vector2((float)(num - crosshairOpenArea), (float)(num2 + 1)), new Vector2((float)(num - (crosshairOpenArea + 18)), (float)(num2 + 1)), black);
        GUIUtils.DrawLine(new Vector2((float)(num + crosshairOpenArea), (float)(num2 + 1)), new Vector2((float)(num + crosshairOpenArea + 18), (float)(num2 + 1)), black);
        GUIUtils.DrawLine(new Vector2((float)(num + 1), (float)(num2 - crosshairOpenArea)), new Vector2((float)(num + 1), (float)(num2 - (crosshairOpenArea + 18))), black);
        GUIUtils.DrawLine(new Vector2((float)(num + 1), (float)(num2 + crosshairOpenArea)), new Vector2((float)(num + 1), (float)(num2 + crosshairOpenArea + 18)), black);
        // white
        GUIUtils.DrawLine(new Vector2((float)(num + crosshairOpenArea), (float)num2), new Vector2((float)(num + crosshairOpenArea + 18), (float)num2), white);
        GUIUtils.DrawLine(new Vector2((float)num, (float)(num2 - crosshairOpenArea)), new Vector2((float)num, (float)(num2 - (crosshairOpenArea + 18))), white);
        GUIUtils.DrawLine(new Vector2((float)(num - crosshairOpenArea), (float)num2), new Vector2((float)(num - (crosshairOpenArea + 18)), (float)num2), white);
        GUIUtils.DrawLine(new Vector2((float)num, (float)(num2 + crosshairOpenArea)), new Vector2((float)num, (float)(num2 + crosshairOpenArea + 18)), white);
    }
}

