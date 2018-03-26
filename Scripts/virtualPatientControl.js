import System;
import System.Text;

var webBuild: boolean = false;

var showEmotionBlendValues : boolean = true; //for testing
var cycleEmotionTest : boolean = true;  // for testing
var useAudioFiles : boolean = false;	// voice recordings are played
var useTTS_asBackup : boolean = true;	// text to speech is used (and as backup if a voice recording is not found)

var speakingSpeed : float = 0.47; // lipsync anim tweak
var blendSpeed : float = 0.2; // lipsync anim tweak

var aaa : AnimationClip; //phonemes
var eee : AnimationClip;
var iye : AnimationClip;
var oh : AnimationClip;
var ooh : AnimationClip;
var fuh : AnimationClip;
var mmm : AnimationClip;
var laa : AnimationClip;
var ess : AnimationClip;
var rest : AnimationClip;

var faceHappy : AnimationClip; //facial expressions
var faceContempt : AnimationClip;
var faceDisgust : AnimationClip;
var faceFear : AnimationClip;
var faceAnger : AnimationClip;
var facePain : AnimationClip;
var faceSurprise : AnimationClip;
var faceSad: AnimationClip; 

var faceTransitionSpeed : float = 2.0;  // length of blend btwn face expressions
private var faceStateWeight : float = 1.0;

var nodYes : AnimationClip; // for yes/no responses
var nodNo : AnimationClip;

var bodyNeutral : AnimationClip; //body expressions
var bodyHappy : AnimationClip;
var bodyContempt : AnimationClip;
var bodyDisgust : AnimationClip;
var bodyFear : AnimationClip;
var bodyAnger : AnimationClip;
var bodyPain : AnimationClip;
var bodySurprise : AnimationClip;
var bodySad : AnimationClip;

var bodyTransitionSpeed : float = 0.5;  // length of blend btwn body expressions
private var bodyStateWeight : float = 1.0;

//var switchRate : Vector2 = Vector2(7.0,15);
var transitionCurve : AnimationCurve;  // NOT YET USED

var audioFiles : AudioClip[];   // ADD MORE AUDIO FILES HERE IN INSPECTOR

var joints : Transform[];  // for body corrections (currently used to tilt head up a little more)
var jointTweak : Vector3[];

// BEGIN EYE BLINK AND GAZE VARS
var leftEyeLid : Transform;
var rightEyeLid : Transform;
var leftEyeJoint : Transform;
var rightEyeJoint : Transform;

var lookCorrection : Vector3 = Vector3(-182,0,-90);
var eyeCorrection : Vector3 = Vector3(-270,7.4,-103.8);
var eyeMovementRange : float = 2.0;
var eyePacing : float = 0.5;
var lookAtCamDistanceLimit : float = 1.0;
var LockEyesOnCam : boolean = false; 

private var eyesLookAtTarget : Transform; //using with Kinect
private var headAimPoint : Transform; //using with Kinect
private var neckJoint : Transform; //using with Kinect

private var blinkTimer : float = 1.0;
private var initialLeftPos : Vector3;
private var initialRightPos : Vector3;
private var eyeTimer : float;

//KINECT
private var kinectHeadAimRotationRef : Transform;
var kinectHeadAimOffset : Vector3;

// END EYE BLINK AND GAZE VARS

// facial expressions are set up as states for additive layers
private var faceHappyState : AnimationState;
private var faceContemptState : AnimationState;
private var faceDisgustState : AnimationState;
private var faceFearState : AnimationState;
private var faceAngerState : AnimationState;
private var facePainState : AnimationState;
private var faceSadState : AnimationState;
private var faceSurpriseState : AnimationState;

private var currentFaceState : AnimationState;
private var lastCurrentFaceState : AnimationState;

private var expressionNeutralTimer : float = 0; // allows expressions to fade after certain time

private var bodyNeutralState : AnimationState;
private var bodyHappyState : AnimationState;
private var bodyContemptState : AnimationState;
private var bodyDisgustState : AnimationState;
private var bodyFearState : AnimationState;
private var bodyAngerState : AnimationState;
private var bodyPainState : AnimationState;
private var bodySadState : AnimationState;
private var bodySurpriseState : AnimationState;

private var nodNoState : AnimationState;
private var nodYesState : AnimationState;

private var currentBodyState : AnimationState;
private var lastCurrentBodyState : AnimationState;	

private var animNames = new Array ();  // NOT YET USED
private var shapeList = new Array ();
private var lastShape : AnimationClip;

static var defaultEmotion: String;

// the speed at which a person must be moving in the real world for the characters eyes to temporarily lock on.
var speedThreshold: float = .8;

//private var isWebplayer : boolean = false;

function Awake() {
	#if UNITY_WEBPLAYER
		//isWebPlayer = true;
		GetComponent(VoiceSpeaker).enabled = false; //because you cannot play voices through the Web player
		defaultEmotion = ("Happy2");
	#endif
	#if UNITY_STANDALONE_WIN
		//isWebPlayer = false;
		//GetComponent(VoiceSpeaker).enabled = true;
		defaultEmotion = ("Happy2");
	#endif
	
	
}

function Start () {
	
	// a way to catch all the animation names, but script still uses vars for all
	
	// for (var state : AnimationState in animation) {
		// animNames.Add(state.name);
		// print("state: "+state.name);
	// }
	
	// set up the expression states for additive blending
	faceHappyState = GetComponent.<Animation>()[faceHappy.name];
	faceHappyState.layer = 10;
	faceHappyState.blendMode = AnimationBlendMode.Additive;
	faceHappyState.wrapMode = WrapMode.ClampForever;
	
	faceContemptState = GetComponent.<Animation>()[faceContempt.name];
	faceContemptState.layer = 10;
	faceContemptState.blendMode = AnimationBlendMode.Additive; 
	faceContemptState.wrapMode = WrapMode.ClampForever;
	
	faceDisgustState = GetComponent.<Animation>()[faceDisgust.name];
	faceDisgustState.layer = 10;
	faceDisgustState.blendMode = AnimationBlendMode.Additive;
	faceDisgustState.wrapMode = WrapMode.ClampForever;
	
	faceFearState = GetComponent.<Animation>()[faceFear.name];
	faceFearState.layer = 10;
	faceFearState.blendMode = AnimationBlendMode.Additive;
	faceFearState.wrapMode = WrapMode.ClampForever;
	
	faceAngerState = GetComponent.<Animation>()[faceAnger.name];
	faceAngerState.layer = 10;
	faceAngerState.blendMode = AnimationBlendMode.Additive;
	faceAngerState.wrapMode = WrapMode.ClampForever;

	facePainState = GetComponent.<Animation>()[facePain.name];
	facePainState.layer = 10;
	facePainState.blendMode = AnimationBlendMode.Additive;
	facePainState.wrapMode = WrapMode.ClampForever;
	
	faceSadState = GetComponent.<Animation>()[faceSad.name];
	faceSadState.layer = 10;
	faceSadState.blendMode = AnimationBlendMode.Additive;
	faceSadState.wrapMode = WrapMode.ClampForever;
	
	faceSurpriseState = GetComponent.<Animation>()[faceSurprise.name];
	faceSurpriseState.layer = 10;
	faceSurpriseState.blendMode = AnimationBlendMode.Additive;
	faceSurpriseState.wrapMode = WrapMode.ClampForever;
	
	// SET UP NOD STATES
	nodYesState = GetComponent.<Animation>()[nodYes.name];
	nodYesState.layer = 12;
	nodYesState.blendMode = AnimationBlendMode.Blend; //Blend
	nodYesState.wrapMode = WrapMode.Once;
	
	nodNoState = GetComponent.<Animation>()[nodNo.name];
	nodNoState.layer = 12;
	nodNoState.blendMode = AnimationBlendMode.Blend; //Blend
	nodNoState.wrapMode = WrapMode.Once;
	
	// SET UP BODY STATES
	bodyNeutralState = GetComponent.<Animation>()[bodyNeutral.name];
	bodyNeutralState.layer = 11;
	bodyNeutralState.blendMode = AnimationBlendMode.Blend; //Blend
	bodyNeutralState.wrapMode = WrapMode.Loop;

	bodyHappyState = GetComponent.<Animation>()[bodyHappy.name];
	bodyHappyState.layer = 11;
	bodyHappyState.blendMode = AnimationBlendMode.Blend;
	bodyHappyState.wrapMode = WrapMode.Loop;

	bodyContemptState = GetComponent.<Animation>()[bodyContempt.name];
	bodyContemptState.layer = 11;
	bodyContemptState.blendMode = AnimationBlendMode.Blend;
	bodyContemptState.wrapMode = WrapMode.Loop;
	
	bodyDisgustState = GetComponent.<Animation>()[bodyDisgust.name];
	bodyDisgustState.layer = 11;
	bodyDisgustState.blendMode = AnimationBlendMode.Blend;
	bodyDisgustState.wrapMode = WrapMode.Loop;
	
	bodyFearState = GetComponent.<Animation>()[bodyFear.name];
	bodyFearState.layer = 11;
	bodyFearState.blendMode = AnimationBlendMode.Blend;
	bodyFearState.wrapMode = WrapMode.Loop;
	
	bodyAngerState = GetComponent.<Animation>()[bodyAnger.name];
	bodyAngerState.layer = 11;
	bodyAngerState.blendMode = AnimationBlendMode.Blend;
	bodyAngerState.wrapMode = WrapMode.Loop;
	
	bodyPainState = GetComponent.<Animation>()[bodyPain.name];
	bodyPainState.layer = 11;
	bodyPainState.blendMode = AnimationBlendMode.Blend;
	bodyPainState.wrapMode = WrapMode.Loop;
	
	bodySadState = GetComponent.<Animation>()[bodySad.name];
	bodySadState.layer = 11;
	bodySadState.blendMode = AnimationBlendMode.Blend;
	bodySadState.wrapMode = WrapMode.Loop;

	bodySurpriseState = GetComponent.<Animation>()[bodySurprise.name];
	bodySurpriseState.layer = 11;
	bodySurpriseState.blendMode = AnimationBlendMode.Blend;
	bodySurpriseState.wrapMode = WrapMode.Loop;
	
	// SET CURRENT FACE STATE
	currentFaceState = faceHappyState;
	lastCurrentFaceState = currentFaceState;
	currentFaceState.enabled = true;
	currentFaceState.normalizedTime = 0.0;
	currentFaceState.weight = 0.0;
	
	// SET CURRENT BODY STATE
	currentBodyState = bodyHappyState;
	lastCurrentBodyState = currentBodyState;
	currentBodyState.enabled = true;
	currentBodyState.normalizedTime = 0.0; // The normalized time of the animation. A value of 1 is the end of the animation. A value of 0.5 is the middle of the animation.
	currentBodyState.weight = 0.25;
	
	lastCurrentBodyState.weight = 0.0;
	bodyNeutralState.enabled = true;
	bodyNeutralState.weight = 0.75;
	
	print("STARTING");
	ExpressEmotion(defaultEmotion);
	
	
	// INIT EYE STUFF
	initialLeftPos = leftEyeLid.localPosition; // position relative to parent of leftEyeLid
	initialRightPos = rightEyeLid.localPosition; // position relative to parent of leftEyeLid
	
	//create eyeLookAtTarget
	eyesLookAtTarget = new GameObject("eyesLookAtTarget").transform; // create new gameObject for chracter to look at
	eyesLookAtTarget.position = GameObject.Find("camTarget").transform.position; // set the position of the GO to camtarget object, which contains the main camera
	eyesLookAtTarget.parent = GameObject.Find("camTarget").transform; //parent new GO to camtarget object
	
	headAimPoint = new GameObject("headAimPoint").transform;// create new GO for head to follow when not looking at the main camera
	neckJoint = GameObject.Find("Head").transform; // set neckJoint equal to head joint in Covington.  ***Problem arises because blockman also has a head. (Solved b/c new blockman joints have numbers)****
	headAimPoint.parent = neckJoint;// set neckJoint as the parent of headAimPoint
	headAimPoint.localPosition = Vector3(0,0,-Vector3.Distance(headAimPoint.position, GameObject.Find("camTarget").transform.position));//set the z-value of the headAimPoint position equal to the distance to the camtarget. This way, it's relatively close to the camTarget
	eyeTimer = eyePacing;
	
	kinectHeadAimRotationRef = new GameObject("kinectHeadAimRotationRef").transform; // set Kinect references
	kinectHeadAimRotationRef.position = GameObject.Find("Neck").transform.position;
	kinectHeadAimRotationRef.parent = GameObject.Find("Neck").transform.parent;
	
	// END INIT EYE STUFF
}

// private var bodyEase : float = 0;  // NOT YET USED - presumably so blends aren't so abrupt
// private var faceEase : float = 0; // NOT YET USED - presumably so blends aren't so abrupt

private var switchTimer : float = 0.0;
private var selectExpression : int = 0;


function Update () {

//print ("expressionNeutralTimer = " + expressionNeutralTimer);

	
	// ONLY FOR DEMO TESTING - TURN CYCLEEMOTIONTEST ON IN INSPECTOR
	if (cycleEmotionTest) {
		// switchTimer += Time.deltaTime;
		// if (switchTimer > 9.0) {
			// selectExpression++;
		if (Input.GetKeyDown("left")) { selectExpression--; switchTimer = 1.0; }
		if (Input.GetKeyDown("right")) { selectExpression++; switchTimer = 1.0; }
		if (switchTimer > 0.9) {
			if (selectExpression == 33) selectExpression = 1;
			if (selectExpression == 0) selectExpression = 32;
			if (selectExpression == 1) ExpressEmotion("Happy1");
			if (selectExpression == 2) ExpressEmotion("Happy2");
			if (selectExpression == 3) ExpressEmotion("Happy3");
			if (selectExpression == 4) ExpressEmotion("Happy4");			
			
			if (selectExpression == 5) ExpressEmotion("Pain1");
			if (selectExpression == 6) ExpressEmotion("Pain2");
			if (selectExpression == 7) ExpressEmotion("Pain3");
			if (selectExpression == 8) ExpressEmotion("Pain4");
			
			if (selectExpression == 9) ExpressEmotion("Sad1");
			if (selectExpression == 10) ExpressEmotion("Sad2");
			if (selectExpression == 11) ExpressEmotion("Sad3");
			if (selectExpression == 12) ExpressEmotion("Sad4");
			
			if (selectExpression == 13) ExpressEmotion("Contempt1");
			if (selectExpression == 14) ExpressEmotion("Contempt2");
			if (selectExpression == 15) ExpressEmotion("Contempt3");
			if (selectExpression == 16) ExpressEmotion("Contempt4");
			
			if (selectExpression == 17) ExpressEmotion("Fear1");
			if (selectExpression == 18) ExpressEmotion("Fear2");
			if (selectExpression == 19) ExpressEmotion("Fear3");
			if (selectExpression == 20) ExpressEmotion("Fear4");
			
			if (selectExpression == 21) ExpressEmotion("Surprise1");
			if (selectExpression == 22) ExpressEmotion("Surprise2");
			if (selectExpression == 23) ExpressEmotion("Surprise3");
			if (selectExpression == 24) ExpressEmotion("Surprise4");
			
			if (selectExpression == 25) ExpressEmotion("Disgust1");
			if (selectExpression == 26) ExpressEmotion("Disgust2");
			if (selectExpression == 27) ExpressEmotion("Disgust3");
			if (selectExpression == 28) ExpressEmotion("Disgust4");
			
			if (selectExpression == 29) ExpressEmotion("Anger1");
			if (selectExpression == 30) ExpressEmotion("Anger2");
			if (selectExpression == 31) ExpressEmotion("Anger3");
			if (selectExpression == 32) ExpressEmotion("Anger4");
			
			switchTimer = 0;
		}
	}
	
	GetComponent.<Animation>().CrossFade(mmm.name); // retain mouth pose anim so face expressions remain additive
	
	// BRING FACE UP TO TARGET WEIGHT AND DROP PREVIOUS WEIGHT (ALWAYS BALANCED BY MMM ANIMATION)
	currentFaceState.weight = Mathf.Lerp(currentFaceState.weight, faceStateWeight, Time.deltaTime);
	// if (currentFaceState.weight < faceStateWeight) currentFaceState.weight += Time.deltaTime * faceTransitionSpeed;
	// if (currentFaceState.weight > faceStateWeight) currentFaceState.weight -= Time.deltaTime * faceTransitionSpeed;
	if (currentFaceState != lastCurrentFaceState && lastCurrentFaceState.weight > 0) lastCurrentFaceState.weight -= Time.deltaTime * faceTransitionSpeed;
	
	// BRING BODY UP TO TARGET WEIGHT AND DROP PREVIOUS WEIGHT (ALWAYS BALANCED BY NEUTRAL BODY)
	currentBodyState.weight = Mathf.Lerp(currentBodyState.weight, bodyStateWeight, Time.deltaTime)   ; // transitionCurve.Evaluate( 0.5 );
	//if (currentBodyState.weight < bodyStateWeight) currentBodyState.weight += Time.deltaTime * bodyTransitionSpeed;
	//if (currentBodyState.weight > bodyStateWeight) currentBodyState.weight -= Time.deltaTime * bodyTransitionSpeed;
	if (currentBodyState != lastCurrentBodyState && lastCurrentBodyState.weight > 0.0) {
		lastCurrentBodyState.weight -= Time.deltaTime * bodyTransitionSpeed;
		bodyNeutralState.weight = 1.0 - (currentBodyState.weight + lastCurrentBodyState.weight);
	}
	else bodyNeutralState.weight = 1.0 - (currentBodyState.weight);
	
	//fade expressions
	if (expressionNeutralTimer > 0.0) expressionNeutralTimer -= Time.deltaTime;
	if (expressionNeutralTimer < 0.0) {
		ExpressEmotion(defaultEmotion);
		expressionNeutralTimer = 0.0;
	}
	
	//if (Input.GetKey("y")) animation.CrossFade(nodYes.name);
	//if (Input.GetKey("n")) animation.CrossFade(nodNo.name);
	
		
	// DO EYE STUFF
	
	if (!LockEyesOnCam) {
		eyeTimer -= Time.deltaTime;
		
				if (eyeTimer < 0) {
					// if the distance between the headAimPosition and the camTarget (where the eyes should be locked) is less than the value defined by lookAtCamDistance, look at the KinectMan 
					// so increasing value of lookAtCamDistanceLimit should increase the amount of time spend staring at the camera
					if (UnityEngine.Random.Range(0,2) == 1 && Vector3.Distance( headAimPoint.position, GameObject.Find("camTarget").transform.position) < lookAtCamDistanceLimit) {
						eyesLookAtTarget.position = GameObject.Find("camTarget").transform.position;
						}
					// if the distance is greater, have the eyes randomly at an area close to where the head is pointing
					else eyesLookAtTarget.position = headAimPoint.position + (UnityEngine.Random.insideUnitSphere * eyeMovementRange);
					
					eyeTimer = UnityEngine.Random.Range(eyePacing,eyePacing+1.0);
				} 
				// if head rotates away past limit while eyes are on camera
				if ( Vector3.Distance( eyesLookAtTarget.position, GameObject.Find("camTarget").transform.position) < 0.1 &&
					Vector3.Distance( headAimPoint.position, GameObject.Find("camTarget").transform.position) > lookAtCamDistanceLimit) eyesLookAtTarget.position = headAimPoint.position;
			}
	// END DO EYE STUFF

}

// QUESTIONS WITH EMOTION TAGS
// Do you ever need an eye opener  No &&Surprise2
// Do you feel guilty about your drinking No &&Surprise1
// Anyone thought you have an alcohol problem No, never &&Disgust2
// Ever felt annoyed by people confronting you about your alcohol use  No &&Anger1
// Do you have a boyfriend No &&Contempt4
// What bothers you the most It seems to be getting worse and I can't work &&Pain3
// Are you currently working No, not since the pain began &&Sad1
// Do you use drugs [No, never &&Contempt4] [No way&&Contempt4] [Absolutely not &&Contempt4]
// * bath salts  People do that? I certainly don't &&Surprise2


// THIS FUNCTION SETS THE WEIGHTS FOR FACE AND BODY EXPRESSION. IT IS CALLED FROM THE CHATSOCKET SCRIPT AND FROM THIS SCRIPTS UPDATE FUNCTION.

private var currentEmotion : String ; // used for GUI display
private var lastEmotion : String;

function ExpressEmotion(emotion : String) {
	expressionNeutralTimer = 7.0;
	if (emotion == lastEmotion) return;

	lastCurrentFaceState = currentFaceState;
	lastCurrentBodyState = currentBodyState;
	
	var emotionFound : boolean = false;
	
	if (emotion.Contains("Happy1")) 	{ currentBodyState = bodyHappyState; 	bodyStateWeight = 0.0; 	currentFaceState = faceHappyState; 		faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Happy2")) 	{ currentBodyState = bodyHappyState; 	bodyStateWeight = 0.33; currentFaceState = faceHappyState; 		faceStateWeight = 0.33; emotionFound = true; }
	if (emotion.Contains("Happy3")) 	{ currentBodyState = bodyHappyState; 	bodyStateWeight = 0.66; currentFaceState = faceHappyState; 		faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Happy4")) 	{ currentBodyState = bodyHappyState; 	bodyStateWeight = 1.0; 	currentFaceState = faceHappyState; 		faceStateWeight = 1.0;  emotionFound = true; }
	if (emotion.Contains("Contempt1")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.0; 	currentFaceState = faceContemptState; 	faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Contempt2")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.33; currentFaceState = faceContemptState; 	faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Contempt3")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.66; currentFaceState = faceContemptState; 	faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Contempt4")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 1.0; 	currentFaceState = faceContemptState; 	faceStateWeight = 1.0; 	emotionFound = true; } 
	if (emotion.Contains("Disgust1")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.0; 	currentFaceState = faceDisgustState; 	faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Disgust2")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.33; currentFaceState = faceDisgustState; 	faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Disgust3")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.66; currentFaceState = faceDisgustState; 	faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Disgust4")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 1.0; 	currentFaceState = faceDisgustState; 	faceStateWeight = 1.0; 	emotionFound = true; } 
	if (emotion.Contains("Sad1")) 		{ currentBodyState = bodySadState; 		bodyStateWeight = 0.0; 	currentFaceState = faceSadState; 		faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Sad2")) 		{ currentBodyState = bodySadState; 		bodyStateWeight = 0.33; currentFaceState = faceSadState; 		faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Sad3")) 		{ currentBodyState = bodySadState; 		bodyStateWeight = 0.66; currentFaceState = faceSadState; 		faceStateWeight = 0.66; emotionFound = true; } 
	if (emotion.Contains("Sad4"))		{ currentBodyState = bodySadState; 		bodyStateWeight = 1.0;  currentFaceState = faceSadState; 		faceStateWeight = 1.0;  emotionFound = true; } 
	if (emotion.Contains("Pain1")) 		{ currentBodyState = bodyPainState; 	bodyStateWeight = 0.0; 	currentFaceState = facePainState; 		faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Pain2")) 		{ currentBodyState = bodyPainState; 	bodyStateWeight = 0.33; currentFaceState = facePainState; 		faceStateWeight = 0.33; emotionFound = true; }
	if (emotion.Contains("Pain3")) 		{ currentBodyState = bodyPainState; 	bodyStateWeight = 0.66; currentFaceState = facePainState; 		faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Pain4")) 		{ currentBodyState = bodyPainState; 	bodyStateWeight = 1.0; 	currentFaceState = facePainState; 		faceStateWeight = 1.0; 	emotionFound = true; }
	if (emotion.Contains("Fear1")) 		{ currentBodyState = bodyFearState; 	bodyStateWeight = 0.0; 	currentFaceState = faceFearState; 		faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Fear2")) 		{ currentBodyState = bodyFearState; 	bodyStateWeight = 0.33; currentFaceState = faceFearState; 		faceStateWeight = 0.33; emotionFound = true; }
	if (emotion.Contains("Fear3")) 		{ currentBodyState = bodyFearState; 	bodyStateWeight = 0.66; currentFaceState = faceFearState; 		faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Fear4")) 		{ currentBodyState = bodyFearState; 	bodyStateWeight = 1.0; 	currentFaceState = faceFearState; 		faceStateWeight = 1.0; 	emotionFound = true; }
	if (emotion.Contains("Anger1")) 	{ currentBodyState = bodyAngerState; 	bodyStateWeight = 0.0; 	currentFaceState = faceAngerState; 		faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Anger2")) 	{ currentBodyState = bodyAngerState; 	bodyStateWeight = 0.33; currentFaceState = faceAngerState; 		faceStateWeight = 0.33; emotionFound = true; }
	if (emotion.Contains("Anger3")) 	{ currentBodyState = bodyAngerState; 	bodyStateWeight = 0.66; currentFaceState = faceAngerState; 		faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Anger4")) 	{ currentBodyState = bodyAngerState; 	bodyStateWeight = 1.0; 	currentFaceState = faceAngerState; 		faceStateWeight = 1.0; 	emotionFound = true; }
	if (emotion.Contains("Surprise1"))	{ currentBodyState = bodySurpriseState; bodyStateWeight = 0.0; 	currentFaceState = faceSurpriseState; 	faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Surprise2")) 	{ currentBodyState = bodySurpriseState; bodyStateWeight = 0.33; currentFaceState = faceSurpriseState; 	faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Surprise3")) 	{ currentBodyState = bodySurpriseState; bodyStateWeight = 0.66; currentFaceState = faceSurpriseState; 	faceStateWeight = 0.66; emotionFound = true; } 
	if (emotion.Contains("Surprise4")) 	{ currentBodyState = bodySurpriseState; bodyStateWeight = 1.0; 	currentFaceState = faceSurpriseState; 	faceStateWeight = 1.0; 	emotionFound = true; } 
	
	if (emotionFound) { 
		currentFaceState.enabled = true;
		currentFaceState.normalizedTime = 1.0;
		if (currentFaceState != lastCurrentFaceState) currentFaceState.weight = 0;
		currentBodyState.enabled = true;
		if (currentBodyState != lastCurrentBodyState) currentBodyState.weight = 0;
		lastEmotion = emotion;
		print("emotion to display: "+ emotion);
		currentEmotion = emotion;
	}
	else print("requested emotion not found: "+ emotion);
}

function NodNo() {
	GetComponent.<Animation>().CrossFade(nodNo.name);
}
function NodYes() {
	GetComponent.<Animation>().CrossFade(nodYes.name);
}


function LateUpdate() {
	// body corrections;
	for (var i = 0; i < joints.length; i++) {
		joints[i].eulerAngles.x += jointTweak[i].x;
		joints[i].eulerAngles.y += jointTweak[i].y;
		joints[i].eulerAngles.z += jointTweak[i].z;

	}
	// DO EYE STUFF
	blinkTimer -= Time.deltaTime;
	if (blinkTimer <= 0) {
		leftEyeLid.localPosition = initialLeftPos + Vector3(0.3,0,0) ;
		rightEyeLid.localPosition = initialRightPos + Vector3(0.3,0,0) ;
		if (blinkTimer < -0.1) {
			blinkTimer = UnityEngine.Random.Range(0.5, 4.0); 
			leftEyeLid.localPosition = initialLeftPos ;
			rightEyeLid.localPosition = initialRightPos ;
		}
		
	}
	
		// THIS GETS THE NECK TO POINT THE SAME DIRECTION THAT THE EYES ARE LOOKING AT
		kinectHeadAimRotationRef.LookAt(GameObject.Find("camTarget").transform);
    	var rotation = kinectHeadAimRotationRef.localEulerAngles + kinectHeadAimOffset;
    	GameObject.Find("Neck").transform.localRotation *= Quaternion.Euler(rotation);
    
    	// keep the headAimPoint reference at the same plane as the camera

		var newPos = GameObject.Find("camTarget").transform.InverseTransformPoint( headAimPoint.position );  
		newPos.z = 0; 
		headAimPoint.position = GameObject.Find("camTarget").transform.TransformPoint( newPos );
  
    if (CameraControls.camLocked || webBuild){
    
		leftEyeJoint.LookAt(eyesLookAtTarget);
		leftEyeJoint.eulerAngles -= eyeCorrection;
		rightEyeJoint.LookAt(eyesLookAtTarget);
		rightEyeJoint.eulerAngles -= eyeCorrection;
	}  	
	
	if (CameraControls.headTargetVelocity > speedThreshold){
		LockEyes(true);
	}
	else {
		LockEyes(false);
	}
}

function LockEyes(state : boolean) {
	if (state) {
		if (CameraControls.camLocked){
			LockEyesOnCam = true;
			eyesLookAtTarget.localPosition = Vector3.zero;
		}
	}
	else {
		LockEyesOnCam = false;
	}
}



private var fps : int;
function TrackFrameRate() {
	while (true) {
		var frameCount : int = 0;
		var timer : float = 0.0;
		while (timer < 1.0) {
			timer += Time.deltaTime;
			frameCount++;
			yield;
		}
		fps = frameCount;
		yield;
	}
}

private var nonLetter : boolean = false;

function BuildShapeList(input : String) {
	shapeList.Clear();
	input = input.ToLower();
	for (var i = 0; i < input.Length; i++) {
		//print( input.Substring(i,1) );
		var letter = input.Substring(i,1);

		
		nonLetter = false;
		switch (letter) {
				case "a": shapeList.Add(aaa); break;
				case "b": shapeList.Add(mmm); break;
				case "c": shapeList.Add(ess); break;
				case "d": shapeList.Add(eee); break;
				case "e": shapeList.Add(eee); break;
				case "f": shapeList.Add(fuh); break;
				case "g": shapeList.Add(eee); break;
				case "h": shapeList.Add(iye); break;
				case "i": shapeList.Add(iye); break;
				case "j": shapeList.Add(aaa); break;
				case "k": shapeList.Add(aaa); break;
				case "l": shapeList.Add(laa); break;
				case "m": shapeList.Add(mmm); break;
				case "n": shapeList.Add(ess); break;
				case "o": shapeList.Add(oh); break;
				case "p": shapeList.Add(eee); break;
				case "q": shapeList.Add(ooh); break;
				case "r": shapeList.Add(aaa); break;
				case "s": shapeList.Add(ess); break;
				case "t": shapeList.Add(eee); break;
				case "u": shapeList.Add(ooh); break;
				case "v": shapeList.Add(fuh); break;
				case "w": shapeList.Add(ooh); break;
				case "x": shapeList.Add(ess); break;
				case "y": shapeList.Add(ooh); break;
				case "z": shapeList.Add(ess); break;
				case ",": 
					// THIS METHOD NEEDS IMPROVEMENT
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); 
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); 
				
					break;
				case ".": 
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); 
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); 
				
					break;
				case "!": 
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm);  
					shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm); shapeList.Add(mmm);  
					break;
				case " ": shapeList.Add(lastShape); break;
				default: nonLetter = true; 
				//print (nonLetter);
		}
		if (!nonLetter) lastShape = shapeList[shapeList.length-1];
	}
	//print(shapeList);
	PlayShapeList();

	if (useAudioFiles)	{
		var audioClipFound : boolean = false;
		input = Regex.Replace(input,"[.,']","");
		input = Regex.Replace(input, "( )+", "");
		//print("look for: "+input.ToLower());
		var matchLength = Mathf.Min( input.length, 12);
		var subInput = input.Substring(0,matchLength);
		//print("look for sub: "+subInput);
		for (i = 0; i < audioFiles.length; i++) {
			//print(audioFiles[i].ToString());
			var subAudioFile = audioFiles[i].ToString();
			subAudioFile = subAudioFile.Substring(0,matchLength);
			if (subInput == subAudioFile) {
				audioClipFound = true;
				GetComponent.<AudioSource>().clip = audioFiles[i];
			}
		}
		if (audioClipFound) GetComponent.<AudioSource>().Play();
		else if (useTTS_asBackup) SendMessage ("SpeakThis", input);
	}
	else SendMessage ("SpeakThis", input);

	LockEyes(true);

}

function PlayShapeList() {
	var t : float = 0;
	lastShape = aaa;

	var shapeTime : float = 0.0;
	var shapeElement : int = 0; 
	var duration : float = shapeList.length * speakingSpeed;
	print("start speaking");
	while ( shapeTime < duration ) {
		// close the mouth
		if (currentFaceState == faceSurpriseState ) currentFaceState.weight = 0; //currentFaceState.weight -= Time.deltaTime * faceTransitionSpeed;
		
		shapeElement = Mathf.Round(shapeList.length * (shapeTime/duration));
		//print ((shapeTime/duration) +" "+ shapeElement );
		//if (shapeElement < shapeList.length) GetComponent.<Animation>().CrossFade(shapeList[  shapeElement ].name, blendSpeed * Time.deltaTime  );
		shapeTime += Time.deltaTime;
		yield;	
	}
	print("done speaking");
	t = 0;
	while (t < 0.5) {
		GetComponent.<Animation>().CrossFade(mmm.name);
		t += Time.deltaTime;
		yield;
	}

	LockEyes(false);

}

function OnGUI() {
	if (showEmotionBlendValues) {
		GUI.Label (Rect (10, Screen.height - 60, 400, 22), currentEmotion );
		GUI.Label (Rect (10, Screen.height - 40, 400, 22), currentFaceState.name+": "+currentFaceState.weight.ToString("0.00") ); 
		GUI.Label (Rect (10, Screen.height - 20, 540, 22), currentBodyState.name+": "+currentBodyState.weight.ToString("0.00")+" (neutral: "+bodyNeutralState.weight.ToString("0.00")+")");
	}

}

function OnDrawGizmos() {
	
	if (eyesLookAtTarget) {
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(eyesLookAtTarget.position, 1.0);
		Gizmos.DrawLine ( leftEyeJoint.position, eyesLookAtTarget.position);
		Gizmos.DrawLine ( rightEyeJoint.position, eyesLookAtTarget.position);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(headAimPoint.position, 1.0);
		//Gizmos.DrawLine ( neckJoint.position, headAimPoint.position);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(headAimPoint.position, eyeMovementRange);
		
		if (Vector3.Distance( headAimPoint.position,  GameObject.Find("camTarget").transform.position) > lookAtCamDistanceLimit) Gizmos.color = Color.red;
		Gizmos.DrawLine ( headAimPoint.position, GameObject.Find("camTarget").transform.position);
	}


}

