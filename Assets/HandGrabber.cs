using UnityEngine;
using System.Collections;

public class HandGrabber : MonoBehaviour {
    public GameObject grabTarget = null;
    public GameObject selectionObject;
    public bool isGrabbing = false;
    public GameObject selection;
    public bool isLeftHand;
    public bool isRightHand;
    private float startPosition = 0f;
    private float targetStartPosition = 0f;
    void Start () {
	
	}
	
	void Update () {
        SteamVR_Controller.DeviceRelation idx = SteamVR_Controller.DeviceRelation.FarthestRight ;
        if (isLeftHand) idx = SteamVR_Controller.DeviceRelation.Leftmost;
        if (isRightHand) idx = SteamVR_Controller.DeviceRelation.FarthestRight;

        int hand = SteamVR_Controller.GetDeviceIndex(idx);
        
        if(!isGrabbing && (SteamVR_Controller.Input(hand).GetHairTriggerDown()))
        {
            isGrabbing = true;
        }
        if(isGrabbing && (SteamVR_Controller.Input(hand).GetHairTriggerUp()))
        {
            isGrabbing = false;
            selection = null;
        }
        Debug.Log("IsGrabbing For Index:" + idx.ToString() + ","  + isGrabbing.ToString());
        if (isGrabbing && grabTarget != null && selection == null)
        {
            selection = grabTarget;
            targetStartPosition = selection.transform.localPosition.z;
            startPosition = this.transform.position.z;
        }


        selectionObject.SetActive(selection != null);
        if (selection != null)
        {
            Vector3 lastPos = selection.transform.localPosition;
            selection.transform.position = this.transform.position;
            float zPos = Mathf.Clamp(selection.transform.localPosition.z, 0.15f,0.45f);
            selection.transform.localPosition = new Vector3(lastPos.x, lastPos.y, zPos);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(c.gameObject == grabTarget)
        {
            Debug.Log("Collision Left GrabTarget");
            grabTarget = null;
            isGrabbing = false;
            selection = null;
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if(grabTarget == null && c.tag == "Control")
        {
            Debug.Log("Collision Entered Controllable Surface");
            grabTarget = c.gameObject;
        }
    }
}
