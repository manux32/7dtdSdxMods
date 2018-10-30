using System;
using System.Collections.Generic;
using UnityEngine;


public class VehicleWeapons : MonoBehaviour
{
    public Entity entity = null;
    EntityCustomVehicle entityVehicle = null;

    public float gunShootDelay = 0.25f;
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


    void Update()
    {
        if (GameManager.Instance.IsPaused() || GameManager.Instance.m_GUIConsole.isInputActive || entityVehicle.uiforPlayer.windowManager.IsModalWindowOpen())
            return;

        if (entityVehicle == null || entityVehicle.player == null)
        {
            InitController();
            return;
        }

        if (!entityVehicle.hasDriver)
            return;

        if (!(entityVehicle.HasGun() || entityVehicle.HasExplosiveLauncher()))
            return;

        if (Input.GetMouseButton(0) && Time.time - gunShootDelay > lastGunShoot)
        {
            //DebugMsg("Left-click");
            ShootProjectile(entityVehicle.gunLauncher, "vehicleGun", "Weapons/Ranged/AK47/ak47_fire_start", true);
            lastGunShoot = Time.time;
        }
        if (Input.GetMouseButton(1) && Time.time - missileShootDelay > lastMissileShoot)
        {
            //DebugMsg("Right-click");
            ShootProjectile(entityVehicle.missileLauncher, "vehicleExplosiveLauncher", "Weapons/Ranged/M136/m136_fire", false);
            lastMissileShoot = Time.time;
        }
    }

    public void ShootProjectile(Transform projectileLauncher, string weaponSlotType, string soundPath, bool isGun)
    {
        if (isGun && (!entityVehicle.HasGun() || !entityVehicle.HasGunAmmo()))
            return;
        if (!isGun && (!entityVehicle.HasExplosiveLauncher() || !entityVehicle.HasExplosiveLauncherAmmo()))
            return;

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
        Utils.SetLayerRecursively(projectile.gameObject, (!(projectileLauncher != null)) ? 0 : projectileLauncher.gameObject.layer);
        BlockProjectileMoveScript blockProjectileMoveScript = projectile.gameObject.AddComponent<BlockProjectileMoveScript>();
        blockProjectileMoveScript.itemProjectile = ammoItem.ItemClass;
        blockProjectileMoveScript.itemValueProjectile = ammoItem;
        blockProjectileMoveScript.itemValueLauncher = ItemValue.None.Clone();
        blockProjectileMoveScript.itemActionProjectile = (ItemActionProjectile)((!(ammoItem.ItemClass.Actions[0] is ItemActionProjectile)) ? ammoItem.ItemClass.Actions[1] : ammoItem.ItemClass.Actions[0]);
        blockProjectileMoveScript.AttackerEntityId = 0;

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

        if (isGun)
        {
            ParticleEffect pe = new ParticleEffect("nozzleflash_ak", projectileLauncher.position, Quaternion.Euler(0f, 180f, 0f), 1f, Color.white, "Pistol_Fire", projectileLauncher);
            float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(projectileLauncher.position)) / 2f;
            ParticleEffect pe2 = new ParticleEffect("nozzlesmokeuzi", projectileLauncher.position, lightValue, new Color(1f, 1f, 1f, 0.3f), null, projectileLauncher, false);
            SpawnParticleEffect(pe, -1);
            SpawnParticleEffect(pe2, -1);
            entityVehicle.playerInventory.RemoveItem(itemStack);
            return;
        }

        entityVehicle.playerInventory.RemoveItem(itemStack);

        if (Steam.Network.IsServer)
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

