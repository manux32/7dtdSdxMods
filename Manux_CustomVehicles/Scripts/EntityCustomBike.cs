using UnityEngine;


class EntityCustomBike : EntityCustomVehicle
{
    public bool canDoWheelies = true;
    float wheelieRotateAngle;
    float lastWheelieRotateAngle;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
    }

    protected override void Start()
    {
        base.Start();
    }

    // Below are many failed attempts at having Custom Bikes do stupid wheelies, grrrrr!!!!!
    /*public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (!hasDriver || !canDoWheelies)
            return;

        //Vector3 newRot = this.rotation.eulerAngles;
        //Quaternion newQuat = new Quaternion();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            DebugMsg("Doing Wheelie");
            //this.transform.Rotate(Vector3.right * 10);
            this.transform.Rotate(Vector3.right * 10);
            //this.m_characterController.rota
        }
    }*/

    /*public new void Update()
    {
        base.Update();

        if (!hasDriver || !canDoWheelies)
            return;

        //Vector3 newRot = this.rotation.eulerAngles;
        //Quaternion newQuat = new Quaternion();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //this.transform.Rotate(Vector3.right * 10);
            //this.transform.Rotate(Vector3.right * 10);

            wheelieRotateAngle = lastWheelieRotateAngle + (20f * Time.deltaTime);
            //this.transform.RotateAround(this.transform.position - this.transform.forward, this.transform.right, wheelieRotateAngle);
            //this.SetRotation(this.transform.rotation.eulerAngles);
            //this.RotateTo(this.position.x, this.position.y, this.position.z, 0, wheelieRotateAngle);
            //this.rotation = this.rotation + (this.transform.right * (20f * Time.deltaTime));
            this.rotation.x = EntityAlive.UpdateRotation(this.rotation.x, this.rotation.x + 10, 20f * Time.deltaTime);
            DebugMsg("Doing Wheelie: rotateAngle = " + wheelieRotateAngle.ToString("0.000") + " | deltaTime = " + Time.deltaTime.ToString("0.000"));
            lastWheelieRotateAngle = wheelieRotateAngle;
        }
        else
        {
            lastWheelieRotateAngle = 0f;
        }
    }*/

    /*protected override void updateTransform()
    {
        base.updateTransform();

        if (!hasDriver || !canDoWheelies)
            return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //this.transform.Rotate(Vector3.right * 10);
            //this.transform.Rotate(Vector3.right * 10);

            wheelieRotateAngle = lastWheelieRotateAngle + (20f * Time.deltaTime);
            this.transform.RotateAround(this.transform.position - this.transform.forward, this.transform.right, -wheelieRotateAngle);
            this.SetRotation(this.transform.rotation.eulerAngles);
            this.rotation = this.transform.rotation.eulerAngles;
            this.prevRotation = this.rotation;
            //this.SetRotation(this.transform.rotation.eulerAngles);
            //this.RotateTo(this.position.x, this.position.y, this.position.z, 0, wheelieRotateAngle);
            //this.rotation = this.rotation + (this.transform.right * (20f * Time.deltaTime));
            //this.rotation.x = EntityAlive.UpdateRotation(this.rotation.x, this.rotation.x + 10, 20f * Time.deltaTime);
            DebugMsg("Doing Wheelie: rotateAngle = " + wheelieRotateAngle.ToString("0.000") + " | deltaTime = " + Time.deltaTime.ToString("0.000"));
            lastWheelieRotateAngle = wheelieRotateAngle;
        }
        else
        {
            lastWheelieRotateAngle = 0f;
        }
    }*/

    /*public void LateUpdate()
    {
        base.Update();

        if (!hasDriver || !canDoWheelies)
            return;

        //Vector3 newRot = this.rotation.eulerAngles;
        //Quaternion newQuat = new Quaternion();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //this.transform.Rotate(Vector3.right * 10);
            //this.transform.Rotate(Vector3.right * 10);

            wheelieRotateAngle = lastWheelieRotateAngle + (20f * Time.deltaTime);
            this.transform.RotateAround(this.transform.position - this.transform.forward, this.transform.right, wheelieRotateAngle);
            this.SetRotation(this.transform.rotation.eulerAngles);
            this.rotation = this.transform.rotation.eulerAngles;
            this.prevRotation = this.rotation;
            //this.SetRotation(this.transform.rotation.eulerAngles);
            //this.RotateTo(this.position.x, this.position.y, this.position.z, 0, wheelieRotateAngle);
            //this.rotation = this.rotation + (this.transform.right * (20f * Time.deltaTime));
            //this.rotation.x = EntityAlive.UpdateRotation(this.rotation.x, this.rotation.x + 10, 20f * Time.deltaTime);
            DebugMsg("Doing Wheelie: rotateAngle = " + wheelieRotateAngle.ToString("0.000") + " | deltaTime = " + Time.deltaTime.ToString("0.000"));
            lastWheelieRotateAngle = wheelieRotateAngle;
        }
        else
        {
            lastWheelieRotateAngle = 0f;
        }
    }*/

    /*public override void OnUpdatePosition(float _partialTicks)
    {
        base.OnUpdatePosition(_partialTicks);

        if (!hasDriver || !canDoWheelies)
            return;

        //Vector3 newRot = this.rotation.eulerAngles;
        //Quaternion newQuat = new Quaternion();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //this.transform.Rotate(Vector3.right * 10);
            //this.transform.Rotate(Vector3.right * 10);

            wheelieRotateAngle = lastWheelieRotateAngle + (20f * Time.deltaTime);
            this.transform.RotateAround(this.transform.position - this.transform.forward, this.transform.right, wheelieRotateAngle);
            this.SetRotation(this.transform.rotation.eulerAngles);
            DebugMsg("Doing Wheelie: rotateAngle = " + wheelieRotateAngle.ToString("0.000") + " | deltaTime = " + Time.deltaTime.ToString("0.000"));
            //this.RotateTo(this.position.x, this.position.y, this.position.z, 0, wheelieRotateAngle);
            lastWheelieRotateAngle = wheelieRotateAngle;
        }
        else
        {
            lastWheelieRotateAngle = 0f;
        } 
    }*/
}


