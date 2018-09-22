using System;
using UnityEngine;

public class HealingPodControl : MonoBehaviour 
{
	public int cIdx;
	public Vector3i blockPos;
	bool isPowered;
    bool curIsPowered;
    Material crossLitMat = null;
    ParticleSystem[] particleSystems = null;
    ParticleSystem particleRoot = null;


    void Awake()
	{
        GameObject healingPodMeshCross = FindChildGameObject(gameObject, "HealingPodMeshCross");
        Renderer mr = healingPodMeshCross.GetComponent<Renderer>();
        if (mr != null)
        {
            crossLitMat = mr.material;
            crossLitMat.EnableKeyword("_EMISSION");
            crossLitMat.SetColor("_EmissionColor", new Color(0, 0, 0));
        }

        particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particleSys in particleSystems)
        {
            if(particleSys.gameObject.name == "Particle_Recover_hold")
            {
                particleRoot = particleSys;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter, coll = " + other.gameObject.name);
        if(particleRoot != null && other.gameObject.name.Contains("Player_"))
        {
            particleRoot.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit, coll = " + other.gameObject.name);
        if (particleRoot != null && other.gameObject.name.Contains("Player_"))
        {
            particleRoot.Stop();
        }
    }


    static public GameObject FindChildGameObject(GameObject fromGameObject, string name)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == name) return t.gameObject;
        return null;
    }

    void Update()
	{
        curIsPowered = BlockHealingPod.IsBlockPoweredUp(blockPos, cIdx);
        if (crossLitMat != null && curIsPowered != isPowered)
        {
            isPowered = curIsPowered;
            if (isPowered)
            {
                {
                    crossLitMat.EnableKeyword("_EMISSION");
                    crossLitMat.SetColor("_EmissionColor", new Color(1,1,1));
                }
            }
            else
            {
                {
                    crossLitMat.EnableKeyword("_EMISSION");
                    crossLitMat.SetColor("_EmissionColor", new Color(0,0,0));
                }
            }
        }
	}
}