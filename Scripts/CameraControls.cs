using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CameraControls : MonoBehaviour
{
    public Transform lookTarget;
    public Transform followTarget;
    public Transform kinectPointManRoot;
    public float followScaling;
    public Transform frustumTarget;
    public float frustumAdjust;
    public float initHeightAtDist;
    private bool dzEnabled;
    public float smoothDamping;
    public float lookAtDamping;
    public bool smoothLook;
    public bool smoothFollow;
    public static bool camLocked;// indicates whether or not kinect camera has locked onto someone
    public Transform headTarget; // the head joint of KinectPointMan
    public static float headTargetVelocity; // indicates the velocity of the kinectPointMan's head, anything greater than zero indicates the camera has locked on
    public virtual void Start()
    {
        this.StartDZ();
        this.frustumAdjust = 7f;
        this.StartCoroutine(this.TrackTargetVelocity());
        CameraControls.camLocked = false; //May need to 
    }

    public virtual void Update()
    {
        MonoBehaviour.print("initHeightAtDist = " + this.initHeightAtDist);
        if (CameraControls.headTargetVelocity > 0) // if the head joint of the KinectPointMan is moving, we know the camera has locked on
        {
            CameraControls.camLocked = true; // happens once and is never set to false except in Start(), so it won't revert back
        }
        if (CameraControls.camLocked)
        {
            if (this.followTarget)
            {
                if (this.smoothFollow)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, this.followTarget.position * this.followScaling, Time.deltaTime * this.smoothDamping);
                }
            }
            if (this.dzEnabled)
            {
                // Measure the new distance and readjust the FOV accordingly.
                float currDistance = Vector3.Distance(this.transform.position, this.frustumTarget.position);// calculates the distance between the camera and the frustum target
                this.GetComponent<Camera>().fieldOfView = this.FOVForHeightAndDistance(this.initHeightAtDist, currDistance);
            }
            // Simple control to allow the camera to be moved in and out using the up/down arrows.
            this.transform.Translate(((Input.GetAxis("Vertical") * Vector3.forward) * Time.deltaTime) * 5);
        }
    }

    public virtual void LateUpdate()
    {
        if (this.lookTarget)
        {
            if (this.smoothLook)
            {
                 // Look at and dampen the rotation
                Quaternion rotation = Quaternion.LookRotation(this.lookTarget.position - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * this.lookAtDamping);
            }
            else
            {
                 // Just lookat
                this.transform.LookAt(this.lookTarget);
            }
        }
    }

    // Calculate the frustum height at a given distance from the camera.
    public virtual float FrustumHeightAtDistance(float distance)
    {
        return (2f * distance) * Mathf.Tan((this.GetComponent<Camera>().fieldOfView * 0.5f) * Mathf.Deg2Rad);
    }

    // Calculate the FOV needed to get a given frustum height at a given distance.
    public virtual float FOVForHeightAndDistance(float height, float distance)
    {
        return (2 * Mathf.Atan((height * 0.5f) / distance)) * Mathf.Rad2Deg;
    }

    // Start the dolly zoom effect.
    public virtual void StartDZ()
    {
        float distance = Vector3.Distance(this.transform.position, this.frustumTarget.position);
        this.initHeightAtDist = this.FrustumHeightAtDistance(distance);
        //initHeightAtDist = 39.9;
        this.dzEnabled = true;
    }

    // Turn dolly zoom off.
    public virtual void StopDZ()
    {
        this.dzEnabled = false;
    }

    public virtual IEnumerator TrackTargetVelocity()
    {
        while (true) // currently assuming there is always a target
        {
            Vector3 oldPos = this.headTarget.position; // where the head was originally positioned
            yield return new WaitForSeconds(0.1f);
            CameraControls.headTargetVelocity = (this.headTarget.position - oldPos).magnitude; //  set velocity by comparing old position to new position
        }
    }

    public virtual void OnGUI()// );
    {
    }

    public CameraControls()
    {
        this.initHeightAtDist = 39.6f;
        this.smoothDamping = 5f;
        this.lookAtDamping = 6f;
        this.smoothLook = true;
        this.smoothFollow = true;
    }

}