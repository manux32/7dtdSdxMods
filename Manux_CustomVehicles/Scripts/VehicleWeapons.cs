using System;
using System.Collections.Generic;
using UnityEngine;


public class VehicleWeapons : MonoBehaviour
{
    public bool gameWasPaused = false;
    public float gamePausedTime = -1;
    public Entity entity = null;
    EntityCustomVehicle entityVehicle = null;

    public float gunShootDelay = 0.25f;
    public float lastGunShoot = -1;
    public float missileShootDelay = 1.2f;
    public float lastMissileShoot = -1;

    public XUiV_Grid gunAmmoGrid = null;
    //public XUiV_Rect gunAmmoRect = null;
    public XUiV_Sprite gunAmmoSprite = null;
    public XUiV_Label gunAmmoLabel = null;
    public XUiV_Grid explosiveAmmoGrid = null;
    //public XUiV_Rect explosiveAmmoRect = null;
    public XUiV_Sprite explosiveAmmoSprite = null;
    public XUiV_Label explosiveAmmoLabel = null;

    public ItemValue gunAmmoItemValue = null;
    public ItemValue explosiveAmmoItemValue = null;

    public GameObject vehicleAmmoUIRoot = null;
    //public GameObject gunAmmoUIRoot = null;
    //public GameObject explosiveAmmoUIRoot = null;
    public UISprite gunAmmoUISprite = null;
    public UILabel gunAmmoUILabel = null;
    public UISprite explosiveAmmoUISprite = null;
    public UILabel explosiveAmmoUILabel = null;
    public UISprite miniBikeDefaultSprite = null;

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
    }

    public void OnDriverOn()
    {
        //InitController();

        if (entityVehicle != null && entityVehicle.player != null && entityVehicle.hudStatBar != null)
        {
            string msg = "VehicleWeapons.OnDriverOn: hudStatBarWinGroup children controllers:\n";
            XUiController hudStatBarWinGroup = entityVehicle.hudStatBar.WindowGroup.Controller;
            foreach (XUiController controller in hudStatBarWinGroup.Children)
            {
                msg += ( "- " + controller.ToString() + " | type = " + controller.GetType() + " | viewComponent ID = " + controller.viewComponent.ID + " | viewComponent type = " + controller.viewComponent.GetType().ToString() + "\n");
                if (controller.viewComponent.ID == "HUDRightStatBars")
                {
                    msg += "  Components:\n";
                    Transform[] transforms = controller.viewComponent.UiTransform.gameObject.GetComponentsInChildren<Transform>(true);
                    foreach (Transform transform in transforms)
                    {
                        msg += ("\t- " + transform.gameObject.name + " | " + transform.GetType() + "\n");

                        if (transform.name == "hudVehicleWeaponsAmmo")
                        {
                            vehicleAmmoUIRoot = transform.gameObject;
                            vehicleAmmoUIRoot.SetActive(true);
                        }

                        if (transform.name == "vehicleHealthIcon")
                        {
                            UISprite[] vehicleHealthSprites = transform.gameObject.GetComponentsInChildren<UISprite>(true);
                            if (vehicleHealthSprites.Length > 0)
                            {
                                vehicleHealthSprites[0].spriteName = entityVehicle.GetMapIcon();
                                msg += ("\t\t- vehicleHealthSprite = " + vehicleHealthSprites[0].spriteName + "\n");
                            }
                        }

                        if (transform.name == "vehicleHealth")
                        {
                            UISprite[] vehicleHealthSprites = transform.gameObject.GetComponentsInChildren<UISprite>(true);
                            foreach(UISprite sprite in vehicleHealthSprites)
                            {
                                if (sprite.gameObject.name == "Icon")
                                {
                                    miniBikeDefaultSprite = sprite;
                                    miniBikeDefaultSprite.spriteName = "";
                                }
                            }
                        }

                        if (transform.name == "hudVehicleGunAmmo")
                        {
                            //gunAmmoUIRoot = transform.gameObject;
                            //gunAmmoUIRoot.SetActive(entityVehicle.HasGun() && entityVehicle.HasGunAmmo());
                            bool bShowGun = entityVehicle.HasGun() && entityVehicle.HasGunAmmo();
                            Component[] comps = transform.gameObject.GetComponentsInChildren<Component>();
                            foreach(Component comp in comps)
                            {
                                if (comp.name == "BarContent" && comp.GetType() == typeof(UISprite))
                                {
                                    ((UISprite)comp).enabled = bShowGun;
                                }
                                if (comp.name == "Icon" && comp.GetType() == typeof(UISprite))
                                {
                                    msg += ("\t\t- " + comp.gameObject.name + " | " + comp.GetType() + "\n");
                                    gunAmmoUISprite = (UISprite)comp;

                                    gunAmmoItemValue = entityVehicle.GetWeaponAmmoType("vehicleGun");
                                    gunAmmoUISprite.spriteName = gunAmmoItemValue.ItemClass.GetIconName();
                                    gunAmmoUISprite.enabled = bShowGun;
                                }
                                if (comp.name == "TextContent" && comp.GetType() == typeof(UILabel))
                                {
                                    msg += ("\t\t- " + comp.gameObject.name + " | " + comp.GetType() + "\n");
                                    gunAmmoUILabel = (UILabel)comp;
                                    gunAmmoUILabel.text = entityVehicle.uiforPlayer.xui.PlayerInventory.GetItemCount(gunAmmoItemValue.ItemClass.Id).ToString();
                                    gunAmmoUILabel.enabled = bShowGun;
                                }
                            }
                        }
                        if (transform.name == "hudVehicleExplosiveLauncherAmmo")
                        {
                            //explosiveAmmoUIRoot = transform.gameObject;
                            //explosiveAmmoUIRoot.SetActive(entityVehicle.HasExplosiveLauncher() && entityVehicle.HasExplosiveLauncherAmmo());
                            bool bShowEL = entityVehicle.HasExplosiveLauncher() && entityVehicle.HasExplosiveLauncherAmmo();
                            Component[] comps = transform.gameObject.GetComponentsInChildren<Component>();
                            foreach (Component comp in comps)
                            {
                                if (comp.name == "BarContent" && comp.GetType() == typeof(UISprite))
                                {
                                    ((UISprite)comp).enabled = bShowEL;
                                }
                                if (comp.name == "Icon" && comp.GetType() == typeof(UISprite))
                                {
                                    msg += ("\t\t- " + comp.gameObject.name + " | " + comp.GetType() + "\n");
                                    explosiveAmmoUISprite = (UISprite)comp;

                                    explosiveAmmoItemValue = entityVehicle.GetWeaponAmmoType("vehicleExplosiveLauncher");
                                    explosiveAmmoUISprite.spriteName = explosiveAmmoItemValue.ItemClass.GetIconName();
                                    explosiveAmmoUISprite.enabled = bShowEL;
                                }
                                if (comp.name == "TextContent" && comp.GetType() == typeof(UILabel))
                                {
                                    msg += ("\t\t- " + comp.gameObject.name + " | " + comp.GetType() + "\n");
                                    explosiveAmmoUILabel = (UILabel)comp;
                                    explosiveAmmoUILabel.text = entityVehicle.uiforPlayer.xui.PlayerInventory.GetItemCount(explosiveAmmoItemValue.ItemClass.Id).ToString();
                                    explosiveAmmoUILabel.enabled = bShowEL;
                                }
                            }
                        }
                    }
                }
            }
            DebugMsg(msg);
        }
    }

    public void OnDriverOff()
    {
        if(vehicleAmmoUIRoot != null)
        {
            vehicleAmmoUIRoot.SetActive(false);
        }
        if (miniBikeDefaultSprite != null)
        {
            miniBikeDefaultSprite.spriteName = "ui_game_symbol_minibike";
        }
    }

    void Update()
    {
        if (GameManager.IsDedicatedServer)
            return;

        if (entityVehicle == null || entityVehicle.player == null)
        {
            InitController();
            return;
        }

        LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityVehicle.player);
        //if (GameManager.Instance.IsPaused() || GameManager.Instance.m_GUIConsole.isInputActive || entityVehicle.uiforPlayer.windowManager.IsModalWindowOpen())
        if (GameManager.Instance.IsPaused() || GameManager.Instance.m_GUIConsole.isInputActive || uiforPlayer.windowManager.IsModalWindowOpen())
        {
            if (GameManager.Instance.IsPaused())
            {
                DebugMsg("GAME WAS PAUSED!");
                gameWasPaused = true;
                gamePausedTime = Time.time;
            }
            return;
        }

        if (!entityVehicle.hasDriver)
            return;

        if (gameWasPaused && Time.time - 0.2f > gamePausedTime)
        {
            OnDriverOn();
            gameWasPaused = false;
        }

        //if (!(entityVehicle.HasGun() || entityVehicle.HasExplosiveLauncher()))
            //return;

        if (entityVehicle.HasGun() && Input.GetMouseButton(0) && Time.time - gunShootDelay > lastGunShoot)
        {
            //DebugMsg("Left-click");
            ShootProjectile(entityVehicle.gunLauncher, "vehicleGun", "Weapons/Ranged/AK47/ak47_fire_start", true);
            lastGunShoot = Time.time;
        }
        if (entityVehicle.HasExplosiveLauncher() && Input.GetMouseButton(1) && Time.time - missileShootDelay > lastMissileShoot)
        {
            //DebugMsg("Right-click");
            ShootProjectile(entityVehicle.missileLauncher, "vehicleExplosiveLauncher", "Weapons/Ranged/M136/m136_fire", false);
            lastMissileShoot = Time.time;
        }
    }

    public void ShootProjectile(Transform projectileLauncher, string weaponSlotType, string soundPath, bool isGun)
    {
        //if (isGun && (!entityVehicle.HasGun() || !entityVehicle.HasGunAmmo()))
        if (isGun && !entityVehicle.HasGunAmmo())
        {
            GameManager.ShowTooltip(entityVehicle.player, "No Vehicle Gun Ammo");
            return;
        }
        //if (!isGun && (!entityVehicle.HasExplosiveLauncher() || !entityVehicle.HasExplosiveLauncherAmmo()))
        if (!isGun && !entityVehicle.HasExplosiveLauncherAmmo())
        {
            GameManager.ShowTooltip(entityVehicle.player, "No Vehicle Explosive Ammo");
            return;
        }

        ItemValue ammoItem = entityVehicle.GetWeaponAmmoType(weaponSlotType);
        ItemStack itemStack = new ItemStack(ammoItem, 1);
        Transform projectile = ammoItem.ItemClass.CloneModel(GameManager.Instance.World, ammoItem, Vector3.zero, null, false, false);

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

        ItemValue launcherValue;
        if (isGun)
        {
            launcherValue = entityVehicle.GetGunItemValue();
            //DebugMsg("Gun: Quality = " + launcherValue.Quality.ToString() + " | UseTimes = " + launcherValue.UseTimes.ToString() + " | MaxUseTimes = " + launcherValue.MaxUseTimes.ToString() + " | GetHealthPercentage = " + entityVehicle.gunPart.GetHealthPercentage().ToString());
            // Change weapons UseTimes (degrade weapon)
            launcherValue.UseTimes += AttributeBase.GetVal<AttributeDegradationRate>(launcherValue, 1);
            entityVehicle.gunPart.SetItemValue(launcherValue);
            if (gunAmmoUILabel != null)
            {
                gunAmmoUILabel.text = (entityVehicle.uiforPlayer.xui.PlayerInventory.GetItemCount(gunAmmoItemValue.ItemClass.Id) - 1).ToString();
            }
        }
        else
        {
            launcherValue = entityVehicle.GetExplosiveLauncherItemValue();
            //DebugMsg("Explosive Launcher: Quality = " + launcherValue.Quality.ToString() + " | UseTimes = " + launcherValue.UseTimes.ToString() + " | MaxUseTimes = " + launcherValue.MaxUseTimes.ToString() + " | GetHealthPercentage = " + entityVehicle.explosiveLauncherPart.GetHealthPercentage().ToString());
            // Change weapons UseTimes (degrade weapon)
            launcherValue.UseTimes += AttributeBase.GetVal<AttributeDegradationRate>(launcherValue, 1);
            entityVehicle.explosiveLauncherPart.SetItemValue(launcherValue);
            if (explosiveAmmoUILabel != null)
            {
                explosiveAmmoUILabel.text = (entityVehicle.uiforPlayer.xui.PlayerInventory.GetItemCount(explosiveAmmoItemValue.ItemClass.Id) - 1).ToString();
            }
        }

        Utils.SetLayerRecursively(projectile.gameObject, (!(projectileLauncher != null)) ? 0 : projectileLauncher.gameObject.layer);
        BlockProjectileMoveScript blockProjectileMoveScript = projectile.gameObject.AddComponent<BlockProjectileMoveScript>();
        blockProjectileMoveScript.itemProjectile = ammoItem.ItemClass;
        blockProjectileMoveScript.itemValueProjectile = ammoItem;
        //blockProjectileMoveScript.itemValueLauncher = ItemValue.None.Clone();
        blockProjectileMoveScript.itemValueLauncher = launcherValue;
        blockProjectileMoveScript.itemActionProjectile = (ItemActionProjectile)((!(ammoItem.ItemClass.Actions[0] is ItemActionProjectile)) ? ammoItem.ItemClass.Actions[1] : ammoItem.ItemClass.Actions[0]);
        //blockProjectileMoveScript.AttackerEntityId = 0;
        blockProjectileMoveScript.AttackerEntityId = entityVehicle.player.entityId;

        //Vector3 target = headlightTargetPos - projectileLauncher.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayOffset = Vector3.Distance(entityVehicle.player.GetThirdPersonCameraTransform().position, projectileLauncher.position) + 2f;
        Vector3 rayStart = ray.GetPoint(rayOffset);
        RaycastHit hit;
        if (Physics.Raycast(rayStart, ray.direction, out hit))
        {
            //Vector3 crossHairPos = ray.GetPoint(1000);// + (Vector3.up * 20);
            //Vector3 targetScreenPos = player.playerCamera.WorldToScreenPoint(crossHairPos);     //headlightTargetPos
            blockProjectileMoveScript.Fire(projectileLauncher.position, hit.point - projectileLauncher.position, entityVehicle.player, 0);
            //blockProjectileMoveScript.Fire(projectileLauncher.position, ray.direction, player, 0);
        }
        else
        {
            Vector3 rayEnd = ray.GetPoint(200f);
            blockProjectileMoveScript.Fire(projectileLauncher.position, rayEnd - projectileLauncher.position, entityVehicle.player, 0);
        }


        //LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityVehicle.player);
        if (isGun)
        {
            ParticleEffect pe = new ParticleEffect("nozzleflash_ak", projectileLauncher.position, Quaternion.Euler(0f, 180f, 0f), 1f, Color.white, "Pistol_Fire", projectileLauncher);
            float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(projectileLauncher.position)) / 2f;
            ParticleEffect pe2 = new ParticleEffect("nozzlesmokeuzi", projectileLauncher.position, lightValue, new Color(1f, 1f, 1f, 0.3f), null, projectileLauncher, false);
            SpawnParticleEffect(pe, -1);
            SpawnParticleEffect(pe2, -1);
            //entityVehicle.playerInventory.RemoveItem(itemStack);
            //uiforPlayer.xui.PlayerInventory.RemoveItem(itemStack);
            entityVehicle.uiforPlayer.xui.PlayerInventory.RemoveItem(itemStack);
            return;
        }

        //entityVehicle.playerInventory.RemoveItem(itemStack);
        //uiforPlayer.xui.PlayerInventory.RemoveItem(itemStack);
        entityVehicle.uiforPlayer.xui.PlayerInventory.RemoveItem(itemStack);

        //if (Steam.Network.IsServer)
        {
            Audio.Manager.BroadcastPlay(projectileLauncher.position, soundPath);
        }
    }

    public void SpawnParticleEffect(ParticleEffect _pe, int _entityId)
    {
        if (Steam.Network.IsServer)
        {
            if (!GameManager.IsDedicatedServer)
            {
                GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId);
            }
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(new NetPackageParticleEffect(_pe, _entityId), false, -1, _entityId, -1, -1);
            return;
        }
        SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(new NetPackageParticleEffect(_pe, _entityId), false);
    }
}

