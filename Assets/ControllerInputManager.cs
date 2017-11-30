using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour {

    private SteamVR_TrackedObject trackObj;
    private SteamVR_Controller.Device device;

    // Check Hand
    private bool isLeft;

    // Teleporter
    public int rayLength;
    public GameObject player;
    public GameObject aimerObj;
    public float aimerYAdjust = 1f; // specific to adjust teleportAimer y position
    public LayerMask laserMask;
    private LineRenderer laser;
    private Vector3 teleportLocation;

    // Ball
    public BallReset ball;

    // Throw
    public float throwForce = 1.5f;

    // Object Menu
    public GameObject objMenu;
    public ObjectMenuManager objMenuManager;

    // Use this for initialization
    void Start () {
        trackObj = GetComponent<SteamVR_TrackedObject>();
        
        if (trackObj.name == "Controller (left)")
        {
            isLeft = true;
        }
        else
        {
            isLeft = false;
        }

        laser = GetComponentInChildren<LineRenderer>();

    }
	
	// Update is called once per frame
	void Update () {

        device = SteamVR_Controller.Input((int)trackObj.index);

        // Teleportation w/ Left Touchpad
        if (isLeft)
        {
            if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                laser.gameObject.SetActive(true);
                laser.SetPosition(0, gameObject.transform.position);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, laserMask))
                {
                    teleportLocation = hit.point;
                    laser.SetPosition(1, teleportLocation);
                    aimerObj.SetActive(true);
                    aimerObj.transform.position = teleportLocation + new Vector3(0, aimerYAdjust, 0);
                }
                else
                {
                    teleportLocation = transform.forward * rayLength + transform.position;
                    laser.SetPosition(1, teleportLocation);
                    RaycastHit groundRay;
                    if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                    {
                        teleportLocation = new Vector3(
                            transform.forward.x * 5 + transform.position.x,
                            groundRay.point.y,
                            transform.forward.z * rayLength + transform.position.z);
                        aimerObj.SetActive(true);
                        aimerObj.transform.position = teleportLocation + new Vector3(0, aimerYAdjust, 0);
                    }
                    else
                    {
                        teleportLocation = player.transform.position;
                    }
                }
            }

            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {

                laser.gameObject.SetActive(false);
                aimerObj.SetActive(false);

                player.transform.position = teleportLocation;

            }
        }

        
        if (!isLeft)
        {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                objMenu.SetActive(true);
            }
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                objMenu.SetActive(false);
            }
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                float pressCurr = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                if (pressCurr > 0)
                {
                    objMenuManager.MenuRight();
                } else
                {
                    objMenuManager.MenuLeft();
                }
            }
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad) && device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                objMenuManager.SpawnCurrentObject();
            }
        }
	}

    // Grab and Throw
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable") || col.gameObject.CompareTag("Structure"))
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(col);
            }
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(col);
            }
        }
    }

    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
    }

    void ThrowObject(Collider coli)
    {
        coli.transform.SetParent(null);
        if (coli.gameObject.CompareTag("Throwable"))
        {
            ball.checkCheat(coli.gameObject.transform.position);
            Rigidbody rigidbody = coli.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.velocity = device.velocity * throwForce;
            rigidbody.angularVelocity = device.angularVelocity;
        }
        
    }

}
