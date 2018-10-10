using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


class EntityCustomLoader : EntityCustomBike
{
    Transform Origin = null;
    Transform handlebar_joint = null;
    Transform chassis_joint = null;
    Transform loader_joint = null;
    Transform bucket_joint = null;

    Transform Origin2 = null;
    Transform handlebar_joint2 = null;
    Transform chassis_joint2 = null;
    Transform loader_joint2 = null;
    Transform bucket_joint2 = null;

    //string loaderSound = "Weapons/Motorized/Chainsaw/chainsaw_fire_lp";
    bool isLoaderSoundStarted;
    //Vector3 loaderSoundPos;
    AudioSource audioSource;

    Vector3 lastLoaderRot;
    Vector3 lastBucketRot;

    Vector3i lastHitBlockPos;

    bool allBonesSet1Found;
    bool allBonesSet2Found;

    int destructionRadius;
    bool hasDestructionRadius;

    //CustomLoaderControl clc = null;

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
        EntityClass entityClass = EntityClass.list[_entityClass];
        /*if (entityClass.Properties.Values.ContainsKey("Sound_loader_moving_loop"))
            this.loaderSound = entityClass.Properties.Values["Sound_loader_moving_loop"];*/

        if (entityClass.Properties.Values.ContainsKey("DestructionRadius"))
        {
            int destRadius;
            if (int.TryParse(entityClass.Properties.Values["DestructionRadius"], out destRadius))
            {
                DebugMsg("\tdestructionRadius = " + destRadius.ToString("0.000"));
                destructionRadius = destRadius;
                hasDestructionRadius = true;
            }
        }

        AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();
        if (audioSources.Length > 0)
        {
            audioSource = audioSources[0];
        }
    }

    protected override void Start()
    {
        base.Start();

        List<Transform> childrenList = new List<Transform>();
        List<int> childrenInstanceIds = new List<int>();
        childrenList.Add(this.transform);
        childrenInstanceIds.Add(this.transform.GetInstanceID());
        CustomVehiclesUtils.GetAllChildTransforms(this.transform, ref childrenList, ref childrenInstanceIds);

        foreach (Transform child in childrenList)
        {
            switch (child.name)
            {
                case "Origin":
                    if (Origin == null)
                        Origin = child;
                    else
                        Origin2 = child;
                    break;
                case "handlebar_joint":
                    if (handlebar_joint == null)
                        handlebar_joint = child;
                    else
                        handlebar_joint2 = child;
                    break;
                case "chassis_joint":
                    if (chassis_joint == null)
                        chassis_joint = child;
                    else
                        chassis_joint2 = child;
                    break;
                case "loader_joint":
                    if (loader_joint == null)
                        loader_joint = child;
                    else
                        loader_joint2 = child;
                    break;
                case "bucket_joint":
                    if (bucket_joint == null)
                        bucket_joint = child;
                    else
                        bucket_joint2 = child;
                    break;
            }
        }

        /*if (Origin != null)
        {
            BoxCollider bc = Origin.gameObject.GetComponent<BoxCollider>();
            if (bc != null)
            {
                bc.enabled = true;
                clc = Origin.gameObject.GetComponent<CustomLoaderControl>();
                if (clc == null)
                {
                    DebugMsg("Adding CustomLoaderControl script.");
                    clc = Origin.gameObject.AddComponent<CustomLoaderControl>();
                }
                clc.enabled = true;
            }
        }*/

        if (handlebar_joint == null || chassis_joint == null || loader_joint == null || bucket_joint == null)
        {
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            allBonesSet1Found = true;
            DebugMsg(this.ToString() + " : All bones set 1 found.");

            lastLoaderRot = loader_joint.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }

        if (handlebar_joint2 == null || chassis_joint2 == null || loader_joint2 == null || bucket_joint2 == null)
        {
            DebugMsg(this.ToString() + " : Some bones could not be found for set 2. (this is harmless)");
        }
        else
        {
            allBonesSet2Found = true;
            DebugMsg(this.ToString() + " : All bones set 2 found.");

            lastLoaderRot = loader_joint2.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }
    }

    public void AnimateExtraJoints(Transform handlebar_joint, Transform loader_joint, Transform bucket_joint)
    {
        bool isMovingBucket = false;
        string dbgMsg = "";
        Vector3 newRot = handlebar_joint.localRotation.eulerAngles;
        Quaternion newQuat = new Quaternion();
        if (newRot != Vector3.zero)
        {
            dbgMsg += ("handlebar Y: " + newRot.ToString("0.000") + "\n");
            if (newRot.y > 180)
            {
                newRot.y -= 360;
            }
            newRot.y = -(newRot.y / 2.0f);
            dbgMsg += ("-handlebar Y: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            chassis_joint.localRotation = newQuat;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = loader_joint.localRotation.eulerAngles;
            dbgMsg += ("UpArrow: " + newRot.ToString("0.000") + "\n");
            newRot.x -= 1.5f;
            if (newRot.x > 180)
            {
                newRot.x -= 360;
            }
            newRot.x = Mathf.Clamp(newRot.x, -90, 0);
            dbgMsg += ("UpArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            loader_joint.localRotation = newQuat;
            lastLoaderRot = newRot;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = loader_joint.localRotation.eulerAngles;
            dbgMsg += ("DownArrow: " + newRot.ToString("0.000") + "\n");
            newRot.x += 1.5f;
            if (newRot.x > 180)
            {
                newRot.x -= 360;
            }
            newRot.x = Mathf.Clamp(newRot.x, -90, 0);
            dbgMsg += ("DownArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            loader_joint.localRotation = newQuat;
            lastLoaderRot = newRot;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = bucket_joint.localRotation.eulerAngles;
            //newRot = new Vector3(newRot.z, 0, 0);
            dbgMsg += ("LeftArrow: " + newRot.ToString("0.000") + "\n");
            newRot.z += 3;
            if (newRot.z > 180)
            {
                newRot.z -= 360;
            }
            newRot.z = Mathf.Clamp(newRot.z, -40, 150);
            dbgMsg += ("LeftArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMovingBucket = true;
            if (!isLoaderSoundStarted)
            {
                //Audio.Manager.BroadcastPlay(this.transform.position, loaderSound);
                audioSource.Play();
                isLoaderSoundStarted = true;
                //loaderSoundPos = this.transform.position;
            }
            newRot = bucket_joint.localRotation.eulerAngles;
            //newRot = new Vector3(newRot.z, 0, 0);
            dbgMsg += ("RightArrow: " + newRot.ToString("0.000") + "\n");
            newRot.z -= 3;
            if (newRot.z > 180)
            {
                newRot.z -= 360;
            }
            newRot.z = Mathf.Clamp(newRot.z, -40, 150);
            dbgMsg += ("RightArrow clamped: " + newRot.ToString("0.000") + "\n");
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }

        if (isMovingBucket == false && isLoaderSoundStarted)
        {
            //Audio.Manager.StopAllSequencesOnEntity(this);
            audioSource.Pause();
            //Audio.Manager.BroadcastStop(loaderSoundPos, loaderSound);
            isLoaderSoundStarted = false;
        }

        // bring back bucket when loader arm is close to the ground
        if (lastLoaderRot.x > -20.0f && lastBucketRot.z > 0)
        {
            newRot = lastBucketRot;
            newRot.z = (Mathf.Abs(lastLoaderRot.x * 5.0f) / 100.0f) * newRot.z;
            newRot.z = Mathf.Clamp(newRot.z, -40.0f, 150.0f);
            newQuat.eulerAngles = newRot;
            bucket_joint.localRotation = newQuat;
            lastBucketRot = newRot;
        }
        // This creates weird binding errors when used
        //DebugMsg(dbgMsg);
    }

    public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (!allBonesSet1Found || !(this.AttachedEntities is global::EntityPlayer))
            return;

        // There is sometimes 2 versions of the car prefab in the game, we need to use the second one when it exists.
        if (allBonesSet2Found)
        {
            AnimateExtraJoints(handlebar_joint2, loader_joint2, bucket_joint2);
        }
        else
        {
            AnimateExtraJoints(handlebar_joint, loader_joint, bucket_joint);
        }

        FindAndKillSurroundingEntities();
    }

    public override void OnCollidedWithBlock(global::WorldBase _world, int _clrIdx, global::Vector3i _blockPos, global::BlockValue _blockValue)
    {
        base.OnCollidedWithBlock(_world, _clrIdx, _blockPos, _blockValue);
        if (!(this.AttachedEntities is global::EntityPlayer))
            return;

        if (_blockPos != lastHitBlockPos)
        {
            if (hasDestructionRadius)
            {
                FindAndKillSurroundingBlocks(_world, _clrIdx, _blockPos);
            }
            lastHitBlockPos = _blockPos;
        }
    }

    public void FindAndKillSurroundingBlocks(global::WorldBase _world, int _clrIdx, global::Vector3i _blockPos)
    {
        //DebugMsg("initial _blockPos = " + _blockPos.ToString());
        Vector3i vehiclePos = _blockPos;
        Vector3i blockPos = vehiclePos;
        //for (int i = -5; i < 6; i++)
        for (int i = -destructionRadius; i < (destructionRadius + 1); i++)
        {
            blockPos.x = vehiclePos.x + i;
            //for (int j = -5; j < 6; j++)
            for (int j = -destructionRadius; j < (destructionRadius + 1); j++)
            {
                blockPos.z = vehiclePos.z + j;
                BlockValue blockValue = GameManager.Instance.World.GetBlock(blockPos);
                global::Block block = global::Block.list[blockValue.type];
                string blockName = block.GetBlockName();
                /*if (blockName != "" && blockName != "air")
                {
                    // This slows down the scene enormously
                    DebugMsg("OnCollidedWithBlock: " + _clrIdx.ToString() + " | " + blockPos.ToString() + " | " + block.GetType().ToString() + " | " + blockName + " | " + block.BlockTag.ToString() + " | " + block.shape.ToString() + " | " + block.MaxDamage.ToString());
                }*/
                if (block.GetType().IsSubclassOf(typeof(BlockPlant)) || block.GetType() == typeof(BlockCactus) || blockName.Contains("rock") || blockName.Contains("tree"))
                {
                    block.DamageBlock(_world, _clrIdx, blockPos, blockValue, 10000, this.AttachedEntities.entityId, true);
                }
            }
        }
    }


    // Not working either even though it's the same things that's done in EntityVehicle.FixedUpdate()
    public void FindAndKillSurroundingEntities()
    {
        Vector3 b = new Vector3(0f, this.m_characterController.height / 2f, 0f);
        RaycastHit raycastHit;
        if (Physics.CapsuleCast(this.position - b, this.position + b, this.destructionRadius, this.motion.normalized, out raycastHit, this.motion.magnitude, -1) && raycastHit.collider != null)
        {
            global::RootTransformRefEntity component = raycastHit.collider.gameObject.GetComponent<global::RootTransformRefEntity>();
            if (component)
            {
                global::EntityAlive entityAlive = component.RootTransform.GetComponent<global::Entity>() as global::EntityAlive;
                if (entityAlive != null && entityAlive != this.AttachedEntities)
                {
                    DebugMsg("FindAndKillSurroundingEntities: " + " | " + entityAlive.EntityName + " | " + entityAlive.GetType().ToString());
                    entityAlive.DamageEntity(new global::DamageSource(global::EnumDamageSourceType.Bullet), 99999, false, 1f);
                }
            }
        }
    }
}


