using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


class EntityCustomLoader : EntityCustomVehicle
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

        AudioSource[] audioSources = this.GetComponentsInChildren<AudioSource>();
        if (audioSources.Length > 0)
        {
            audioSource = audioSources[0];
        }
    }

    public void CopyPropertiesFromEntityClass(int _entityClass)
    {
        base.CopyPropertiesFromEntityClass();

        //EntityClass entityClass = EntityClass.list[this.entityClass];
        /*if (entityClass.Properties.Values.ContainsKey("Sound_loader_moving_loop"))
            this.loaderSound = entityClass.Properties.Values["Sound_loader_moving_loop"];*/
    }

    protected override void Start()
    {
        base.Start();
        GetVehicleBones();
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

        // Was for testing additional colliders with a custom script. May reuse later...
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
            if (allBonesSet1Found)
                allBonesSet1Found = false;
            Debug.LogError(this.ToString() + " : Some bones could not be found for set 1. Custom Car will not be fully functionnal.");
        }
        else
        {
            if (allBonesSet1Found)
            {
                allBonesSet1Found = true;
                DebugMsg(this.ToString() + " : All bones set 1 found.");
            }

            lastLoaderRot = loader_joint.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }

        if (handlebar_joint2 == null || chassis_joint2 == null || loader_joint2 == null || bucket_joint2 == null)
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

            lastLoaderRot = loader_joint2.localRotation.eulerAngles;
            if (lastLoaderRot.x > 180)
            {
                lastLoaderRot.x -= 360;
            }
        }
    }

    public void AnimateExtraBones(Transform Origin, Transform handlebar_joint, Transform loader_joint, Transform bucket_joint)
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
            newRot.x -= 3f;
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
            newRot.x += 3f;
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
            newRot.z += 4;
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
            newRot.z -= 4;
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

        int originFloor = (int)Mathf.Round(Origin.transform.position.y);
        destructionStartHeight = ((int)Mathf.Floor(bucket_joint.transform.position.y - Origin.transform.position.y)) + 1;
        //if (currentBucketHeight < originFloor)
            //currentBucketHeight = originFloor;
        //int originFloor = (int)Mathf.Round(Origin.transform.position.y);
        //int bucketMinusOrigin = ((int)Mathf.Floor(bucket_joint.transform.position.y)) - originFloor;
        //currentBucketHeight = originFloor + Mathf.Clamp(bucketMinusOrigin, 0, bucketMinusOrigin);
        //int originRounded = (int)Mathf.Round(Origin.transform.position.y);
        //int bucketFloor = (int)Mathf.Floor(bucket_joint.transform.position.y);
        //int currentBucketHeight = originRounded + Mathf.Max(originRounded, bucketFloor - originRounded);

        // This creates weird binding errors when used
        //DebugMsg(dbgMsg);
    }


    /*public override void OnDriverOn()
    {
        base.OnDriverOn();
    }*/

    public new void FixedUpdate()
    {
        //try
        {
            base.FixedUpdate();

            /*if (!allBonesSet1Found || !(this.AttachedEntities is EntityPlayerLocal))
            {
                hasDriver = false;
                return;
            }*/

            if (!hasDriver)
                return;

            // There is sometimes 2 versions of the car prefab in the game, we need to use the second one when it exists.
            if (allBonesSet2Found)
            {
                AnimateExtraBones(Origin2, handlebar_joint2, loader_joint2, bucket_joint2);
            }
            else
            {
                AnimateExtraBones(Origin, handlebar_joint, loader_joint, bucket_joint);
            }
        }
        /*catch (System.Exception e)
        {
            Debug.LogError("An error occurred: " + e);
        }*/
    }
}


