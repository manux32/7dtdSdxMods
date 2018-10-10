using System;
using UnityEngine;


// This class is currently not used. I made it when struggling to detect and destroy trees.
public class CustomLoaderControl : MonoBehaviour 
{
    BoxCollider boxColl;
    float msgDelay = 5;
    float lastMsgTime = -1;
    public int currentClrIdx;

    void Awake()
	{
        boxColl = GetComponent<BoxCollider>();
        if(boxColl != null)
        {
            Debug.Log("CustomLoaderControl Awake: Found BoxCollider");
            Debug.Log("\tenabled = " + boxColl.enabled.ToString());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Loader OnTriggerEnter, coll = " + other.gameObject.name);
        Transform root = other.gameObject.transform.root;
        Debug.Log("\troot = " + root.name);
        Entity entity = root.GetComponent<Entity>();
        if(entity != null)
        {
            Debug.Log("\tentity = " + entity.GetType().ToString() + " | " + entity.name);
        }

        /*Vector3i blockPos = Vector3i.FromVector3Rounded(other.gameObject.transform.position);
        BlockValue blockValue = GameManager.Instance.World.GetBlock(blockPos);
        global::Block block = global::Block.list[blockValue.type];
        string blockName = block.GetBlockName();
        //if (blockName != "air" && blockName != lastHitBlock)
        if (blockName != "" && blockName != "air")
        {
            Debug.Log("OnTriggerEnter: " + block.GetType().ToString() + " | " + blockName);
            //lastHitBlock = blockName;
        }*/
        //FindSurroundingBlocks("OnTriggerEnter");
    }

    public void FindSurroundingBlocks(string callingFunction)
    {
        Vector3 vehicleFullPos = gameObject.transform.position;
        vehicleFullPos.y += 0.5f;
        Vector3i vehiclePos = Vector3i.FromVector3Rounded(vehicleFullPos);
        Vector3i blockPos = vehiclePos;
        for (int i = -5; i < 6; i++)
        {
            blockPos.x = vehiclePos.x + i;
            for (int j = -5; j < 6; j++)
            {
                blockPos.z = vehiclePos.z + j;
                BlockValue blockValue = GameManager.Instance.World.GetBlock(currentClrIdx, blockPos);
                global::Block block = global::Block.list[blockValue.type];
                string blockName = block.GetBlockName();
                //if (blockName != "air" && blockName != lastHitBlock)
                if (blockName != "" && blockName != "air")
                {
                    Debug.Log(callingFunction + ": " + block.GetType().ToString() + " | " + blockName);
                    //lastHitBlock = blockName;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Loader OnTriggerExit, coll = " + other.gameObject.name);
    }

    void Update()
	{
        /*
        if (boxColl != null && Time.time > lastMsgTime + msgDelay)
        {
            Debug.Log("\tLoader BoxCollider enabled = " + boxColl.enabled.ToString());
            lastMsgTime = Time.time;
        }*/
    }
}