


class ItemActionSpawnCustomVehicle : ItemActionSpawnMinibike
{
    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
        if (_props.Values.ContainsKey("VehicleToSpawn"))
        {
            this.entityToSpawn = _props.Values["VehicleToSpawn"];
        }
        foreach (int v in EntityClass.list.Keys)
        {
            if (!(EntityClass.list[v].entityClassName != this.entityToSpawn))
            {
                this.entityId = v;
                break;
            }
        }
    }
}

