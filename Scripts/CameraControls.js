var lookTarget : Transform;
var followTarget: Transform;
var kinectPointManRoot : Transform;
var followScaling : float;

var frustumTarget: Transform; 
var frustumAdjust: float;
var initHeightAtDist: float = 39.6;
private var dzEnabled: boolean;

var smoothDamping = 5.0;
var lookAtDamping = 6.0;
var smoothLook = true;
var smoothFollow = true;

static var camLocked: boolean;// indicates whether or not kinect camera has locked onto someone
var headTarget : Transform; // the head joint of KinectPointMan
static var headTargetVelocity : float ; // indicates the velocity of the kinectPointMan's head, anything greater than zero indicates the camera has locked on


function Start() {
	StartDZ();
	frustumAdjust = 7.0;
	TrackTargetVelocity();
	camLocked = false; //May need to 
}

function Update () {

print ("initHeightAtDist = " + initHeightAtDist);

	if (headTargetVelocity>0){ // if the head joint of the KinectPointMan is moving, we know the camera has locked on
		camLocked = true; // happens once and is never set to false except in Start(), so it won't revert back
	}

	if (camLocked){
		if (followTarget) {
			if (smoothFollow){
		    	transform.position = Vector3.Lerp (transform.position, followTarget.position * followScaling,Time.deltaTime * smoothDamping);
		    	
		    }
		}
	
		if (dzEnabled) {
			// Measure the new distance and readjust the FOV accordingly.
			var currDistance = Vector3.Distance(transform.position, frustumTarget.position);// calculates the distance between the camera and the frustum target
			GetComponent.<Camera>().fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);
		}
	
		// Simple control to allow the camera to be moved in and out using the up/down arrows.
		transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * 5);
		}
}



function LateUpdate () {
	if (lookTarget) {
		if (smoothLook)
		{
			// Look at and dampen the rotation
			var rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamping);
		}
		else
		{
			// Just lookat
		    transform.LookAt(lookTarget);
		}
	}
}

// Calculate the frustum height at a given distance from the camera.
function FrustumHeightAtDistance(distance: float) {
	return 2.0 * distance * Mathf.Tan(GetComponent.<Camera>().fieldOfView * 0.5 * Mathf.Deg2Rad);
}

// Calculate the FOV needed to get a given frustum height at a given distance.
function FOVForHeightAndDistance(height: float, distance: float) {
	return 2 * Mathf.Atan(height * 0.5 / distance) * Mathf.Rad2Deg;
}

// Start the dolly zoom effect.
function StartDZ() {
	var distance = Vector3.Distance(transform.position, frustumTarget.position);
	initHeightAtDist = FrustumHeightAtDistance(distance);
	//initHeightAtDist = 39.9;
	dzEnabled = true;
}

// Turn dolly zoom off.
function StopDZ() {
	dzEnabled = false;
}

function TrackTargetVelocity() {
	while (true) { // currently assuming there is always a target
			var oldPos = headTarget.position; // where the head was originally positioned
			yield WaitForSeconds(0.1); // wait for a moment 
			headTargetVelocity = (headTarget.position - oldPos).magnitude; //  set velocity by comparing old position to new position
	}
}


function OnGUI() {
	// GUI.Label (Rect (20, 20, 600, 20), "offset: " + kinectPointManRoot.position.x +" "+
		// kinectPointManRoot.position.y +" "+
		// kinectPointManRoot.position.z +" "+
		// "frustumSize: " + initHeightAtDist +" "+
		// "frame Y: " + GameObject.Find("frame").transform.position.y
		
	
	// );

}











