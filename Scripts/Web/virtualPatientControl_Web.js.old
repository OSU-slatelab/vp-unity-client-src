import System;
import System.Text; 

var showEmotionBlendValues : boolean = true;
var cycleEmotionTest : boolean = true;  // for testing
var useAudioFiles : boolean = false;	// voice recordings are played
var useTTS_asBackup : boolean = true;	// text to speech is used (and as backup if a voice recording is not found)
var defaultEmotion: String = "Happiness1"; // set the default emotion of the character

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
var faceConfusion: AnimationClip; 
var faceWorried: AnimationClip; 

var faceTransitionSpeed : float = 2.0;  // length of blend btwn face expressions
private var faceStateWeight : float = 1.0;

var nodYes : AnimationClip;
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
var bodyConfusion : AnimationClip;
var bodyWorried : AnimationClip;

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
var eyeCorrection : Vector3 = Vector3(-270,0,-90);
var eyeMovementRange : float = 3.0;
var eyePacing : float = 0.5;
var lookAtCamDistanceLimit : float = 3.0;
var LockEyesOnCam : boolean = false; 
private var eyesLookAtTarget : Transform; //will use with Kinect
private var headAimPoint : Transform; //will use with Kinect
private var neckJoint : Transform; //will use with Kinect
private var blinkTimer : float = 1.0;
private var initialLeftPos : Vector3;
private var initialRightPos : Vector3;
private var eyeTimer : float;
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
private var faceConfusionState : AnimationState;
private var faceWorriedState : AnimationState;

private var currentFaceState : AnimationState;
private var lastCurrentFaceState : AnimationState;

private var nodYesState : AnimationState;
private var nodNoState : AnimationState;

private var expressionNeutralTimer : float = 0;

private var bodyNeutralState : AnimationState;
private var bodyHappyState : AnimationState;
private var bodyContemptState : AnimationState;
private var bodyDisgustState : AnimationState;
private var bodyFearState : AnimationState;
private var bodyAngerState : AnimationState;
private var bodyPainState : AnimationState;
private var bodySadState : AnimationState;
private var bodySurpriseState : AnimationState;
private var bodyConfusionState : AnimationState;
private var bodyWorriedState : AnimationState;

private var currentBodyState : AnimationState;
private var lastCurrentBodyState : AnimationState;	

private var animNames = new Array ();  // NOT YET USED
private var shapeList = new Array ();
private var lastShape : AnimationClip;

//private var isWebplayer : boolean = false;


// END VARIABLES  -------------------------------------------------------


function Awake() {
//	#if UNITY_WEBPLAYER
//		GetComponent(VoiceSpeaker).enabled = false;
//	#endif
//	#if UNITY_STANDALONE_WIN
//		GetComponent(VoiceSpeaker).enabled = true;
//	#endif
}

// END AWAKE()  -------------------------------------------------------


function Start () {

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

	// facePainState = animation[facePain.name];
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

	faceConfusionState = GetComponent.<Animation>()[faceConfusion.name];
	faceConfusionState.layer = 10;
	faceConfusionState.blendMode = AnimationBlendMode.Additive;
	faceConfusionState.wrapMode = WrapMode.ClampForever;

	faceWorriedState = GetComponent.<Animation>()[faceWorried.name];
	faceWorriedState.layer = 10;
	faceWorriedState.blendMode = AnimationBlendMode.Additive;
	faceWorriedState.wrapMode = WrapMode.ClampForever;
	
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

	bodyConfusionState = GetComponent.<Animation>()[bodyConfusion.name];
	bodyConfusionState.layer = 11;
	bodyConfusionState.blendMode = AnimationBlendMode.Blend;
	bodyConfusionState.wrapMode = WrapMode.Loop;

	bodyWorriedState = GetComponent.<Animation>()[bodyWorried.name];
	bodyWorriedState.layer = 11;
	bodyWorriedState.blendMode = AnimationBlendMode.Blend;
	bodyWorriedState.wrapMode = WrapMode.Loop;

	currentFaceState = faceHappyState;
	lastCurrentFaceState = currentFaceState;
	currentFaceState.enabled = true;
	currentFaceState.normalizedTime = 0.0;
	currentFaceState.weight = 0.0;
	
	currentBodyState = bodyHappyState;
	lastCurrentBodyState = currentBodyState;
	currentBodyState.enabled = true;
	currentBodyState.normalizedTime = 0.0;
	currentBodyState.weight = 0.25;
	
	lastCurrentBodyState.weight = 0.0;
	bodyNeutralState.enabled = true;
	bodyNeutralState.weight = 0.75;
	
	ExpressEmotion(defaultEmotion);
		
	// INIT EYE STUFF
	
	initialLeftPos = leftEyeLid.localPosition;
	initialRightPos = rightEyeLid.localPosition;
	
	//create eyeLookTarget
	eyesLookAtTarget = new GameObject("eyesLookAtTarget").transform; // create new gameObject for chracter to look at
	eyesLookAtTarget.position = GetComponent.<Camera>().main.transform.position; //set position of GO equal to main camera
	eyesLookAtTarget.parent = GetComponent.<Camera>().main.transform; //parent new GO to camera
	headAimPoint = new GameObject("headAimPoint").transform;// create new GO for head to follow
	neckJoint = GameObject.Find("Head").transform; // set neckJoint equal to head joint in Covington.  ***Problem arises because blockman also has a head.****
	headAimPoint.parent = neckJoint;
	headAimPoint.localPosition = Vector3(0,0,-Vector3.Distance(headAimPoint.position, GetComponent.<Camera>().main.transform.position)/2);
	eyeTimer = eyePacing;
	
	// END INIT EYE STUFF
}

// END START()  -------------------------------------------------------

private var switchTimer : float = 0.0;
private var selectExpression : int = 0;

function Update () {
	
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
			if (selectExpression == 1) ExpressEmotion("Happiness1");
			if (selectExpression == 2) ExpressEmotion("Happiness2");
			if (selectExpression == 3) ExpressEmotion("Happiness3");
			if (selectExpression == 4) ExpressEmotion("Happiness4");			
			
			if (selectExpression == 5) ExpressEmotion("Pain1");
			if (selectExpression == 6) ExpressEmotion("Pain2");
			if (selectExpression == 7) ExpressEmotion("Pain3");
			if (selectExpression == 8) ExpressEmotion("Pain4");
			
			if (selectExpression == 9) ExpressEmotion("Sadness1");
			if (selectExpression == 10) ExpressEmotion("Sadness2");
			if (selectExpression == 11) ExpressEmotion("Sadness3");
			if (selectExpression == 12) ExpressEmotion("Sadness4");
			
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

			if (selectExpression == 33) ExpressEmotion("Confusion1");
			if (selectExpression == 34) ExpressEmotion("Confusion2");
			if (selectExpression == 35) ExpressEmotion("Confusion3");
			if (selectExpression == 36) ExpressEmotion("Confusion4");

			if (selectExpression == 37) ExpressEmotion("Worried1");
			if (selectExpression == 38) ExpressEmotion("Worried2");
			if (selectExpression == 39) ExpressEmotion("Worried3");
			if (selectExpression == 40) ExpressEmotion("Worried4");
			
			switchTimer = 0;
		}
	}
	
	GetComponent.<Animation>().CrossFade(mmm.name); // retain mouth pose anim so face expressions remain additive
	
	// BRING FACE UP TO TARGET WEIGHT AND DROP PREVIOUS WEIGHT (ALWAYS BALANCED BY MMM ANIMATION)
	currentFaceState.weight = Mathf.Lerp(currentFaceState.weight, faceStateWeight, Time.deltaTime)   ;
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
	
	if (expressionNeutralTimer > 0.0) expressionNeutralTimer -= Time.deltaTime;
	
	if (expressionNeutralTimer < 0.0) {
		ExpressEmotion(defaultEmotion);
		expressionNeutralTimer = 0.0;
	}
	
	if (Input.GetKey("y")) GetComponent.<Animation>().CrossFade(nodYes.name);
	if (Input.GetKey("n")) GetComponent.<Animation>().CrossFade(nodNo.name);
		
	// DO EYE STUFF
	// keep the headAimPoint reference at the same plane as the camera
	var newPos = GetComponent.<Camera>().main.transform.InverseTransformPoint( headAimPoint.position );  // find postion in local space
	newPos.z = 0; 
	headAimPoint.position = GetComponent.<Camera>().main.transform.TransformPoint( newPos ); // set new position back into world space
	
	if (!LockEyesOnCam) {
		eyeTimer -= Time.deltaTime;
		if (eyeTimer < 0) {
			//var averagedPosition = (camera.main.transform.position + headAimPoint.position) / 2.0;
			// look at camera unless head is pointed too far away from camera
			if (UnityEngine.Random.Range(0,2) == 1 && Vector3.Distance( headAimPoint.position, GetComponent.<Camera>().main.transform.position) < lookAtCamDistanceLimit) eyesLookAtTarget.position = GetComponent.<Camera>().main.transform.position;
			else eyesLookAtTarget.position = headAimPoint.position + (UnityEngine.Random.insideUnitSphere * eyeMovementRange);
			eyeTimer = UnityEngine.Random.Range(eyePacing,eyePacing+1.0);
		} 
		// if head rotates away past limit while eyes are on camera
		if ( Vector3.Distance( eyesLookAtTarget.position, GetComponent.<Camera>().main.transform.position) < 0.1 &&
			Vector3.Distance( headAimPoint.position, GetComponent.<Camera>().main.transform.position) > lookAtCamDistanceLimit) eyesLookAtTarget.position = headAimPoint.position;
	}
	// END DO EYE STUFF

}

// END UPDATE()  -------------------------------------------------------


// QUESTIONS FOR ELICITING EMOTIONAL RESPONSES

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
	// while(currentBodyState.weight > 0) {
		// currentBodyState.weight -= Time.deltaTime * bodyTransitionSpeed;
		// if (lastCurrentBodyState.weight < 1.0) lastCurrentBodyState.weight += Time.deltaTime * bodyTransitionSpeed;
		// yield;
	// }
	
	lastCurrentFaceState = currentFaceState;
	lastCurrentBodyState = currentBodyState;
	var emotionFound : boolean = false;
	
	if (emotion.Contains("Happiness1")) { currentBodyState = bodyHappyState; 	bodyStateWeight = 0.0; 	currentFaceState = faceHappyState; 		faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Happiness2")) { currentBodyState = bodyHappyState; 	bodyStateWeight = 0.33; currentFaceState = faceHappyState; 		faceStateWeight = 0.33; emotionFound = true; }
	if (emotion.Contains("Happiness3")) { currentBodyState = bodyHappyState; 	bodyStateWeight = 0.66; currentFaceState = faceHappyState; 		faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Happiness4")) { currentBodyState = bodyHappyState; 	bodyStateWeight = 1.0; 	currentFaceState = faceHappyState; 		faceStateWeight = 1.0;  emotionFound = true; }
	if (emotion.Contains("Contempt1")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.0; 	currentFaceState = faceContemptState; 	faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Contempt2")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.33; currentFaceState = faceContemptState; 	faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Contempt3")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 0.66; currentFaceState = faceContemptState; 	faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Contempt4")) 	{ currentBodyState = bodyContemptState; bodyStateWeight = 1.0; 	currentFaceState = faceContemptState; 	faceStateWeight = 1.0; 	emotionFound = true; } 
	if (emotion.Contains("Disgust1")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.0; 	currentFaceState = faceDisgustState; 	faceStateWeight = 0.25; emotionFound = true; }
	if (emotion.Contains("Disgust2")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.33; currentFaceState = faceDisgustState; 	faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Disgust3")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 0.66; currentFaceState = faceDisgustState; 	faceStateWeight = 0.66; emotionFound = true; }
	if (emotion.Contains("Disgust4")) 	{ currentBodyState = bodyDisgustState; 	bodyStateWeight = 1.0; 	currentFaceState = faceDisgustState; 	faceStateWeight = 1.0; 	emotionFound = true; } 
	if (emotion.Contains("Sadness1")) 	{ currentBodyState = bodySadState; 		bodyStateWeight = 0.0; 	currentFaceState = faceSadState; 		faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Sadness2")) 	{ currentBodyState = bodySadState; 		bodyStateWeight = 0.33; currentFaceState = faceSadState; 		faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Sadness3")) 	{ currentBodyState = bodySadState; 		bodyStateWeight = 0.66; currentFaceState = faceSadState; 		faceStateWeight = 0.66; emotionFound = true; } 
	if (emotion.Contains("Sadness4"))	{ currentBodyState = bodySadState; 		bodyStateWeight = 1.0;  currentFaceState = faceSadState; 		faceStateWeight = 1.0;  emotionFound = true; } 
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
	if (emotion.Contains("Confusion1"))	{ currentBodyState = bodyConfusionState; bodyStateWeight = 0.0; currentFaceState = faceConfusionState; 	faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Confusion2")) { currentBodyState = bodyConfusionState; bodyStateWeight = 0.33; currentFaceState = faceConfusionState; faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Confusion3")) { currentBodyState = bodyConfusionState; bodyStateWeight = 0.66; currentFaceState = faceConfusionState; faceStateWeight = 0.66; emotionFound = true; } 
	if (emotion.Contains("Confusion4")) { currentBodyState = bodyConfusionState; bodyStateWeight = 1.0; currentFaceState = faceConfusionState; 	faceStateWeight = 1.0; 	emotionFound = true; }
	if (emotion.Contains("Worried1")) { currentBodyState = bodyWorriedState; bodyStateWeight = 0.0; currentFaceState = faceWorriedState; 	faceStateWeight = 0.25; emotionFound = true; } 
	if (emotion.Contains("Worried2")) { currentBodyState = bodyWorriedState; bodyStateWeight = 0.33; currentFaceState = faceWorriedState; faceStateWeight = 0.33; emotionFound = true; } 
	if (emotion.Contains("Worried3")) { currentBodyState = bodyWorriedState; bodyStateWeight = 0.66; currentFaceState = faceWorriedState; faceStateWeight = 0.66; emotionFound = true; } 
	if (emotion.Contains("Worried4")) { currentBodyState = bodyWorriedState; bodyStateWeight = 1.0; currentFaceState = faceWorriedState; 	faceStateWeight = 1.0; 	emotionFound = true; }  

	
	if (emotionFound) {
		currentFaceState.enabled = true;
		currentFaceState.normalizedTime = 1.0;
		if (currentFaceState != lastCurrentFaceState) currentFaceState.weight = 0;
		currentBodyState.enabled = true;
		if (currentBodyState != lastCurrentBodyState) currentBodyState.weight = 0;
		lastEmotion = emotion;
		print("emotion to display: "+emotion);
		currentEmotion = emotion;
	}
	else print("requested emotion not found: "+emotion);
}

// NODDING FUNCTIONS

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
	leftEyeJoint.LookAt(eyesLookAtTarget);
	leftEyeJoint.eulerAngles -= eyeCorrection;
	rightEyeJoint.LookAt(eyesLookAtTarget);
	rightEyeJoint.eulerAngles -= eyeCorrection;
	
	// END DO EYE STUFF

}

function LockEyes(state : boolean) {
	if (state) {
		LockEyesOnCam = true;
		eyesLookAtTarget.localPosition = Vector3.zero;
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
	print("BuildShapeList Message received...");
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
		//else if (GetComponent(VoiceSpeaker).enabled && useTTS_asBackup) SendMessage ("SpeakThis", input);
	}
	//else if (GetComponent(VoiceSpeaker).enabled) SendMessage ("SpeakThis", input);

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
		if (shapeElement < shapeList.length)
		{
			var shapeAnim : AnimationClip;
			shapeAnim = shapeList[ shapeElement ];
			GetComponent.<Animation>().CrossFade(shapeAnim.name, blendSpeed * Time.deltaTime  );
		}
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
		//Gizmos.DrawLine ( leftEyeJoint.position, eyesLookAtTarget.position);
		//Gizmos.DrawLine ( rightEyeJoint.position, eyesLookAtTarget.position);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(headAimPoint.position, 1.0);
		//Gizmos.DrawLine ( neckJoint.position, headAimPoint.position);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(headAimPoint.position, eyeMovementRange);
		if (Vector3.Distance( headAimPoint.position,  GetComponent.<Camera>().main.transform.position) > lookAtCamDistanceLimit) Gizmos.color = Color.red;
		Gizmos.DrawLine ( headAimPoint.position, GetComponent.<Camera>().main.transform.position);
	}


}

