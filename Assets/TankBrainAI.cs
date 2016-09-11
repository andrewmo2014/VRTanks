using UnityEngine;
using System.Collections;

public class TankBrainAI : MonoBehaviour {
    private GameObject target;
    private bool canSeeTarget;
    private float distanceToTarget;
    public float minDistanceBeforeFiring = 10f;
    public float moveSpeed = 1f;
    public float reloadTime = 2f;
    private float timeLeftToFire = 2f;
    public float firePowerMultiplier = 4000f;
    public GameObject tank;
    public GameObject tankBody;
    public GameObject tankTurret;
    public GameObject BulletPrefab;
    public int counter = 0;
    private BrainState currentState;
    public enum BrainState
    {
        SCANNING,
        GETTING_CLOSER,
        FIRING
    }
	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        UpdateState();
        TickState();
	}

    private void UpdateState()
    {
        CheckTargetVisible();
        if (canSeeTarget == false)
        {
            currentState = BrainState.SCANNING;
            return;
        }
        if(distanceToTarget <= minDistanceBeforeFiring)
        {
            currentState = BrainState.FIRING;
        }
        else
        {
            currentState = BrainState.GETTING_CLOSER;
        }
    }

    private void TickState()
    {
        switch (currentState)
        {
            case BrainState.GETTING_CLOSER:
                tank.transform.LookAt(target.transform);
                this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
                break;
            case BrainState.FIRING:
                if(timeLeftToFire >= 0f)
                {
                    timeLeftToFire -= Time.deltaTime;
                    return;
                }
                else
                {
                    Fire();
                }
                break;
        }
    }

    private void Fire()
    {
        //Always look at the target before you fire.
        tank.transform.LookAt(target.transform);
        GameObject bullet = (GameObject)Instantiate(BulletPrefab, tankTurret.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(tank.transform.forward * firePowerMultiplier);
        timeLeftToFire = reloadTime;
    }

    public void BulletHit()
    {
        //Bang Bang...
        tankTurret.transform.parent = null;
        tankBody.transform.parent = null;
        tankBody.AddComponent<SphereCollider>();
        tankTurret.AddComponent<SphereCollider>();
        Rigidbody body = tankBody.AddComponent<Rigidbody>();
        body.AddExplosionForce(100f, this.transform.position - (this.transform.up * 2f), 3f);
        Rigidbody turret = tankTurret.AddComponent<Rigidbody>();
        turret.AddExplosionForce(50f, this.transform.position - (this.transform.up * 2f), 2f);
        Destroy(this.gameObject, 3f);
        Destroy(turret.gameObject, 7f);
        Destroy(body.gameObject, 7f);
    }

    private void CheckTargetVisible()
    {
        counter++;
        //This is a trick to make it only raycast once every 20 frames, so we can reduce the cost of raycasting.
        if (counter % 20 == 0)
        { 
            RaycastHit hit;
            canSeeTarget = false;
            Vector3 origin = this.transform.position + (this.transform.up * 5f);
           // Debug.DrawLine(origin, (target.transform.position - origin) * 100f, Color.blue, 4f);
            if (Physics.Raycast(origin, (target.transform.position - origin) * 100f, out hit))
            {
                if (hit.collider.tag == "Terrain")
                {
                    return;
                }
                if (hit.collider.tag == "Player")
                {
                    canSeeTarget = true;
                    distanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
                }
            }
        }
    }
}
