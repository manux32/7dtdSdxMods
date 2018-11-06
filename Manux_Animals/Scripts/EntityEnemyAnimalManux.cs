using System;
using UnityEngine;
using Object = UnityEngine.Object;


class EntityEnemyAnimalManu : EntityEnemyAnimal
{
    Color meshColor;
    bool setMeshColor = false;
    Color meshEmissiveColor;
    bool setMeshEmissiveColor = false;

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[_entityClass];
        if (entityClass.Properties.Values.ContainsKey("MeshScale"))
        {
            string meshScaleStr = entityClass.Properties.Values["MeshScale"];
            string[] parts = meshScaleStr.Split(new char[]
            {
                ','
            });
            float minScale = 1f;
            float maxScale = 1f;
            if (parts.Length == 1)
            {
                minScale = (maxScale = float.Parse(parts[0]));
            }
            else if (parts.Length == 2)
            {
                minScale = float.Parse(parts[0]);
                maxScale = float.Parse(parts[1]);
            }
            this.meshScale = UnityEngine.Random.Range(minScale, maxScale);
            base.gameObject.transform.localScale = new Vector3(this.meshScale, this.meshScale, this.meshScale);
        }

        if (entityClass.Properties.Values.ContainsKey("MeshColor"))
        {
            Color newColor;
            if (AnimalsUtils.StringVectorToColor(entityClass.Properties.Values["MeshColor"], out newColor))
            {
                setMeshColor = true;
                meshColor = newColor;
            }
        }
        if (entityClass.Properties.Values.ContainsKey("MeshEmissiveColor"))
        {
            Color newColor;
            if (AnimalsUtils.StringVectorToColor(entityClass.Properties.Values["MeshEmissiveColor"], out newColor))
            {
                setMeshEmissiveColor = true;
                meshEmissiveColor = newColor;
            }
        }

        string msg = "";
        this.SetTag(base.gameObject.transform, base.gameObject.transform, ref msg);
    }

    private void SetTag(Transform root, Transform t, ref string msg)
    {
        if (t.name != "GameObject" && t.name != "Graphics" && t.name != "Model" && t.name != "InactiveItems" && !t.name.StartsWith("#"))
        {
            if (t.name.ToLower().Contains("head"))
            {
                t.tag = "E_BP_Head";
            }
        }
        foreach (object obj in t)
        {
            Transform tran = (Transform)obj;
            this.SetTag(root, tran, ref msg);
        }
        if (root != t)
        {
            GameObject go = t.gameObject.gameObject;
            RootTransformRefParent c = go.GetComponent<RootTransformRefParent>();
            if (c == null)
            {
                c = go.AddComponent<RootTransformRefParent>();
                c.RootTransform = root;
            }
        }
    }

    protected override void Awake()
    {
        BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
        if (component)
        {
            component.center = new Vector3(0f, 0.85f, 0f);
            component.size = new Vector3(20f, 15.6f, 20f);
        }
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (setMeshColor)
        {
            AnimalsUtils.ChangeMeshesColor(meshColor, gameObject.GetComponentsInChildren<Renderer>());
        }
        if (setMeshEmissiveColor)
        {
            AnimalsUtils.ChangeMeshesEmissiveColor(meshEmissiveColor, gameObject.GetComponentsInChildren<Renderer>());
        }
    }


    public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
    {
        return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
    }

    public override Vector3 GetMapIconScale()
    {
        return new Vector3(0.45f, 0.45f, 1f);
    }

    public override void OnUpdateLive()
    {
        base.OnUpdateLive();
        if (DateTime.Now > this.NextUpdateCheck)
        {
            this.NextUpdateCheck = DateTime.Now.AddSeconds(20.0);
        }
    }


    private float meshScale = 1f;

    private DateTime NextUpdateCheck = DateTime.Now;
}

