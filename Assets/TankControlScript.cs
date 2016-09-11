using UnityEngine;
using System.Collections;
using System;

public class TankControlScript : MonoBehaviour {

    private Vector3 respawnPosition;
    public GameObject GunStickUpDown;
    public GameObject GunStickLeftRight;
    public GameObject TankMoveForwardBackward;
    public GameObject TankMoveLeftRight;
    public GameObject TankFireButton;

    public GameObject TankBody;
    public GameObject TankTurret;

    public Rigidbody TankRigidBody;

    public GameObject BulletSpawner;
    public GameObject BulletPrefab;
    public GameObject SmokePrefab;

    public AudioSource rotateTurretSound;
    public AudioSource rollTankSound;
    public AudioSource dieSound;

    private Vector2 GunControlPosition = new Vector2(0.5f, 0.5f);
    private Vector2 TankControlPosition = new Vector2(0.5f, 0.5f);
    private float timeLeftToReload = 0f;

    public float reloadTime = 1f;
    public float turnSpeed = 0.1f;
    public float accelerationSpeed = 0.1f;
    public float maxMoveVelocity = 1f;
    public float turretRotationSpeed = 0.5f;
    public float bulletForce = 100f;
    public Vector4 MaxTankTurretRanges = new Vector4(-14f, 14f, 0f, 8f);
    public Vector2 MaxBodyRotationRange = new Vector2(-50f, 50f);

    public bool KeyboardEnabled = true;
    public bool TouchControllersEnabled = false;

	void Start () {
        respawnPosition = this.transform.position;
    }
	
	void Update () {
        if (KeyboardEnabled)
        {
            ProcessKeyboardInputs();
            DrawControlState();
        }
        if(TouchControllersEnabled)
        {
            ReadControlState();
        }
        ApplyStateToTank();
    }

    private void ApplyStateToTank()
    {

        rollTankSound.mute = !(TankControlPosition.y < 0.45f || TankControlPosition.y > 0.55f);
        Quaternion newRotation = Quaternion.Euler(new Vector3(
        Mathf.Lerp(MaxTankTurretRanges.z, MaxTankTurretRanges.w, GunControlPosition.y),
            Mathf.Lerp(MaxTankTurretRanges.x, MaxTankTurretRanges.y, GunControlPosition.x),
        0f));
        TankTurret.transform.localRotation = newRotation;

        newRotation = Quaternion.Euler(new Vector3(
        0f,
        Mathf.Lerp(MaxBodyRotationRange.x, MaxBodyRotationRange.y, TankControlPosition.x),
        0f));

        TankBody.transform.localRotation = newRotation;
        Vector3 rotationVector = new Vector3(0f, Mathf.Lerp(-50f, 50f, TankControlPosition.x), 0f);
        Vector3 force = (TankBody.transform.forward.normalized + newRotation.eulerAngles.normalized) * Mathf.Clamp(accelerationSpeed * Mathf.Lerp(1,-1,TankControlPosition.y),
            -maxMoveVelocity,
            maxMoveVelocity);
        TankRigidBody.AddForce(-TankRigidBody.velocity, ForceMode.VelocityChange);
        TankRigidBody.AddForce(force, ForceMode.VelocityChange);
        if (timeLeftToReload > 0f) timeLeftToReload -= Time.deltaTime;
    }

    private void DrawControlState()
    {
        Vector3 pos = GunStickUpDown.transform.localPosition;
        pos.z = Mathf.Lerp(0.45f, 0.15f, GunControlPosition.y);
        GunStickUpDown.transform.localPosition = pos;

        pos = GunStickLeftRight.transform.localPosition;
        pos.z = Mathf.Lerp(0.15f, 0.45f, GunControlPosition.x);
        GunStickLeftRight.transform.localPosition = pos;

        pos = TankMoveLeftRight.transform.localPosition;
        pos.z = Mathf.Lerp(0.15f, 0.45f, TankControlPosition.x);
        TankMoveLeftRight.transform.localPosition = pos;

        pos = TankMoveForwardBackward.transform.localPosition;
        pos.z = Mathf.Lerp(0.45f, 0.15f, TankControlPosition.y);
        TankMoveForwardBackward.transform.localPosition = pos;
    }

     private void ReadControlState()
    {
        float min = 0.15f;
        float max = 0.45f;
        Vector3 pos = GunStickUpDown.transform.localPosition;
        float value = pos.z - min;
        value = value / (max - min);
        GunControlPosition.y = value;

        pos = GunStickLeftRight.transform.localPosition;

        value = pos.z - min;
        value = value / (max - min);
        GunControlPosition.x = value;

        pos = TankMoveLeftRight.transform.localPosition;
        value = pos.z - min;
        value = value / (max - min);
        TankControlPosition.x = value;

        pos = TankMoveForwardBackward.transform.localPosition;
        value = pos.z - min;
        value = value / (max - min);
        TankControlPosition.y = value;
    }

    private void ProcessKeyboardInputs()
    {
        bool rotateTurretMute = true;
        if(Input.GetKey(KeyCode.Space) && timeLeftToReload <= 0f)
        {
            FireCannon();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            TankControlPosition.y += accelerationSpeed * Time.deltaTime;
            if (TankControlPosition.y >= 1f) TankControlPosition.y = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            TankControlPosition.y -= accelerationSpeed * Time.deltaTime;
            if (TankControlPosition.y <= 0f) TankControlPosition.y = 0f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TankControlPosition.x -= turnSpeed * Time.deltaTime;
            if (TankControlPosition.x <= 0f) TankControlPosition.x = 0f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TankControlPosition.x += turnSpeed * Time.deltaTime;
            if (TankControlPosition.x >= 1f) TankControlPosition.x = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rotateTurretMute = false;
            GunControlPosition.y -= turretRotationSpeed * Time.deltaTime;
            if (GunControlPosition.y <= 0f) GunControlPosition.y = 0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rotateTurretMute = false;
            GunControlPosition.y += turretRotationSpeed * Time.deltaTime;
            if (GunControlPosition.y >= 1f) GunControlPosition.y = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotateTurretMute = false;
            GunControlPosition.x -= turretRotationSpeed * Time.deltaTime;
            if (GunControlPosition.x <= 0f) GunControlPosition.x = 0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotateTurretMute = false;
            GunControlPosition.x += turretRotationSpeed * Time.deltaTime;
            if (GunControlPosition.x >= 1f) GunControlPosition.x = 1f;
        }
        rotateTurretSound.mute = rotateTurretMute;
    }

    private void FireCannon()
    {
        timeLeftToReload = reloadTime;
        Destroy((GameObject)Instantiate(SmokePrefab, BulletSpawner.transform.position, Quaternion.identity), 7f);
        GameObject bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawner.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(BulletSpawner.transform.forward * bulletForce);
    }

    public void BulletHit()
    {
        //Hit by a bullet.
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.transform.position = respawnPosition;
        this.GetComponent<Rigidbody>().isKinematic = false;
        dieSound.Play();
    }
}
