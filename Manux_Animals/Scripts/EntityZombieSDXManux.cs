using System;
using UnityEngine;


class EntityZombieSDXManux : EntityZombieSDX
{
    private float meshScale = 1;
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
            string[] parts = meshScaleStr.Split(',');

            float minScale = 1;
            float maxScale = 1;

            if (parts.Length == 1)
            {
                maxScale = minScale = float.Parse(parts[0]);
            }
            else if (parts.Length == 2)
            {
                minScale = float.Parse(parts[0]);
                maxScale = float.Parse(parts[1]);
            }

            meshScale = UnityEngine.Random.Range(minScale, maxScale);
            this.gameObject.transform.localScale = new Vector3(meshScale, meshScale, meshScale);
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
}
