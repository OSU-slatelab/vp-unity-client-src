using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public partial class virtualPatientControl : MonoBehaviour
{
    public bool webBuild;
    public bool showEmotionBlendValues; //for testing
    public bool cycleEmotionTest; // for testing
    public bool useAudioFiles; // voice recordings are played
    public bool useTTS_asBackup; // text to speech is used (and as backup if a voice recording is not found)
    public float speakingSpeed; // lipsync anim tweak
    public float blendSpeed; // lipsync anim tweak
    public AnimationClip aaa; //phonemes
    public AnimationClip eee;
    public AnimationClip iye;
    public AnimationClip oh;
    public AnimationClip ooh;
    public AnimationClip fuh;
    public AnimationClip mmm;
    public AnimationClip laa;
    public AnimationClip ess;
    public AnimationClip rest;
    public AnimationClip faceHappy; //facial expressions
    public AnimationClip faceContempt;
    public AnimationClip faceDisgust;
    public AnimationClip faceFear;
    public AnimationClip faceAnger;
    public AnimationClip facePain;
    public AnimationClip faceSurprise;
    public AnimationClip faceSad;
    public float faceTransitionSpeed; // length of blend btwn face expressions
    private float faceStateWeight;
    public AnimationClip nodYes; // for yes/no responses
    public AnimationClip nodNo;
    public AnimationClip bodyNeutral; //body expressions
    public AnimationClip bodyHappy;
    public AnimationClip bodyContempt;
    public AnimationClip bodyDisgust;
    public AnimationClip bodyFear;
    public AnimationClip bodyAnger;
    public AnimationClip bodyPain;
    public AnimationClip bodySurprise;
    public AnimationClip bodySad;
    public float bodyTransitionSpeed; // length of blend btwn body expressions
    private float bodyStateWeight;
    //var switchRate : Vector2 = Vector2(7.0,15);
    public AnimationCurve transitionCurve; // NOT YET USED
    public AudioClip[] audioFiles;
    public Transform[] joints;
    public Vector3[] jointTweak;
    // BEGIN EYE BLINK AND GAZE VARS
    public Transform leftEyeLid;
    public Transform rightEyeLid;
    public Transform leftEyeJoint;
    public Transform rightEyeJoint;
    public Vector3 lookCorrection;
    public Vector3 eyeCorrection;
    public float eyeMovementRange;
    public float eyePacing;
    public float lookAtCamDistanceLimit;
    public bool LockEyesOnCam;
    private Transform eyesLookAtTarget; //using with Kinect
    private Transform headAimPoint; //using with Kinect
    private Transform neckJoint; //using with Kinect
    private float blinkTimer;
    private Vector3 initialLeftPos;
    private Vector3 initialRightPos;
    private float eyeTimer;
    //KINECT
    private Transform kinectHeadAimRotationRef;
    public Vector3 kinectHeadAimOffset;
    // END EYE BLINK AND GAZE VARS
    // facial expressions are set up as states for additive layers
    private AnimationState faceHappyState;
    private AnimationState faceContemptState;
    private AnimationState faceDisgustState;
    private AnimationState faceFearState;
    private AnimationState faceAngerState;
    private AnimationState facePainState;
    private AnimationState faceSadState;
    private AnimationState faceSurpriseState;
    private AnimationState currentFaceState;
    private AnimationState lastCurrentFaceState;
    private float expressionNeutralTimer; // allows expressions to fade after certain time
    private AnimationState bodyNeutralState;
    private AnimationState bodyHappyState;
    private AnimationState bodyContemptState;
    private AnimationState bodyDisgustState;
    private AnimationState bodyFearState;
    private AnimationState bodyAngerState;
    private AnimationState bodyPainState;
    private AnimationState bodySadState;
    private AnimationState bodySurpriseState;
    private AnimationState nodNoState;
    private AnimationState nodYesState;
    private AnimationState currentBodyState;
    private AnimationState lastCurrentBodyState;
    private object[] animNames; // NOT YET USED
    private List<AnimationClip> shapeList;
    private AnimationClip lastShape;
    public static string defaultEmotion;
    // the speed at which a person must be moving in the real world for the characters eyes to temporarily lock on.
    public float speedThreshold;
    //private var isWebplayer : boolean = false;
    public virtual void Awake()
    {
    }

    public virtual void Start()// END INIT EYE STUFF
    {
        // a way to catch all the animation names, but script still uses vars for all
        // for (var state : AnimationState in animation) {
        // animNames.Add(state.name);
        // print("state: "+state.name);
        // }
        // set up the expression states for additive blending
        this.faceHappyState = this.GetComponent<Animation>()[this.faceHappy.name];
        this.faceHappyState.layer = 10;
        this.faceHappyState.blendMode = AnimationBlendMode.Additive;
        this.faceHappyState.wrapMode = WrapMode.ClampForever;
        this.faceContemptState = this.GetComponent<Animation>()[this.faceContempt.name];
        this.faceContemptState.layer = 10;
        this.faceContemptState.blendMode = AnimationBlendMode.Additive;
        this.faceContemptState.wrapMode = WrapMode.ClampForever;
        this.faceDisgustState = this.GetComponent<Animation>()[this.faceDisgust.name];
        this.faceDisgustState.layer = 10;
        this.faceDisgustState.blendMode = AnimationBlendMode.Additive;
        this.faceDisgustState.wrapMode = WrapMode.ClampForever;
        this.faceFearState = this.GetComponent<Animation>()[this.faceFear.name];
        this.faceFearState.layer = 10;
        this.faceFearState.blendMode = AnimationBlendMode.Additive;
        this.faceFearState.wrapMode = WrapMode.ClampForever;
        this.faceAngerState = this.GetComponent<Animation>()[this.faceAnger.name];
        this.faceAngerState.layer = 10;
        this.faceAngerState.blendMode = AnimationBlendMode.Additive;
        this.faceAngerState.wrapMode = WrapMode.ClampForever;
        this.facePainState = this.GetComponent<Animation>()[this.facePain.name];
        this.facePainState.layer = 10;
        this.facePainState.blendMode = AnimationBlendMode.Additive;
        this.facePainState.wrapMode = WrapMode.ClampForever;
        this.faceSadState = this.GetComponent<Animation>()[this.faceSad.name];
        this.faceSadState.layer = 10;
        this.faceSadState.blendMode = AnimationBlendMode.Additive;
        this.faceSadState.wrapMode = WrapMode.ClampForever;
        this.faceSurpriseState = this.GetComponent<Animation>()[this.faceSurprise.name];
        this.faceSurpriseState.layer = 10;
        this.faceSurpriseState.blendMode = AnimationBlendMode.Additive;
        this.faceSurpriseState.wrapMode = WrapMode.ClampForever;
        // SET UP NOD STATES
        this.nodYesState = this.GetComponent<Animation>()[this.nodYes.name];
        this.nodYesState.layer = 12;
        this.nodYesState.blendMode = AnimationBlendMode.Blend; //Blend
        this.nodYesState.wrapMode = WrapMode.Once;
        this.nodNoState = this.GetComponent<Animation>()[this.nodNo.name];
        this.nodNoState.layer = 12;
        this.nodNoState.blendMode = AnimationBlendMode.Blend; //Blend
        this.nodNoState.wrapMode = WrapMode.Once;
        // SET UP BODY STATES
        this.bodyNeutralState = this.GetComponent<Animation>()[this.bodyNeutral.name];
        this.bodyNeutralState.layer = 11;
        this.bodyNeutralState.blendMode = AnimationBlendMode.Blend; //Blend
        this.bodyNeutralState.wrapMode = WrapMode.Loop;
        this.bodyHappyState = this.GetComponent<Animation>()[this.bodyHappy.name];
        this.bodyHappyState.layer = 11;
        this.bodyHappyState.blendMode = AnimationBlendMode.Blend;
        this.bodyHappyState.wrapMode = WrapMode.Loop;
        this.bodyContemptState = this.GetComponent<Animation>()[this.bodyContempt.name];
        this.bodyContemptState.layer = 11;
        this.bodyContemptState.blendMode = AnimationBlendMode.Blend;
        this.bodyContemptState.wrapMode = WrapMode.Loop;
        this.bodyDisgustState = this.GetComponent<Animation>()[this.bodyDisgust.name];
        this.bodyDisgustState.layer = 11;
        this.bodyDisgustState.blendMode = AnimationBlendMode.Blend;
        this.bodyDisgustState.wrapMode = WrapMode.Loop;
        this.bodyFearState = this.GetComponent<Animation>()[this.bodyFear.name];
        this.bodyFearState.layer = 11;
        this.bodyFearState.blendMode = AnimationBlendMode.Blend;
        this.bodyFearState.wrapMode = WrapMode.Loop;
        this.bodyAngerState = this.GetComponent<Animation>()[this.bodyAnger.name];
        this.bodyAngerState.layer = 11;
        this.bodyAngerState.blendMode = AnimationBlendMode.Blend;
        this.bodyAngerState.wrapMode = WrapMode.Loop;
        this.bodyPainState = this.GetComponent<Animation>()[this.bodyPain.name];
        this.bodyPainState.layer = 11;
        this.bodyPainState.blendMode = AnimationBlendMode.Blend;
        this.bodyPainState.wrapMode = WrapMode.Loop;
        this.bodySadState = this.GetComponent<Animation>()[this.bodySad.name];
        this.bodySadState.layer = 11;
        this.bodySadState.blendMode = AnimationBlendMode.Blend;
        this.bodySadState.wrapMode = WrapMode.Loop;
        this.bodySurpriseState = this.GetComponent<Animation>()[this.bodySurprise.name];
        this.bodySurpriseState.layer = 11;
        this.bodySurpriseState.blendMode = AnimationBlendMode.Blend;
        this.bodySurpriseState.wrapMode = WrapMode.Loop;
        // SET CURRENT FACE STATE
        this.currentFaceState = this.faceHappyState;
        this.lastCurrentFaceState = this.currentFaceState;
        this.currentFaceState.enabled = true;
        this.currentFaceState.normalizedTime = 0f;
        this.currentFaceState.weight = 0f;
        // SET CURRENT BODY STATE
        this.currentBodyState = this.bodyHappyState;
        this.lastCurrentBodyState = this.currentBodyState;
        this.currentBodyState.enabled = true;
        this.currentBodyState.normalizedTime = 0f; // The normalized time of the animation. A value of 1 is the end of the animation. A value of 0.5 is the middle of the animation.
        this.currentBodyState.weight = 0.25f;
        this.lastCurrentBodyState.weight = 0f;
        this.bodyNeutralState.enabled = true;
        this.bodyNeutralState.weight = 0.75f;
        MonoBehaviour.print("STARTING");
        this.ExpressEmotion(virtualPatientControl.defaultEmotion);
        // INIT EYE STUFF
        this.initialLeftPos = this.leftEyeLid.localPosition; // position relative to parent of leftEyeLid
        this.initialRightPos = this.rightEyeLid.localPosition; // position relative to parent of leftEyeLid
        //create eyeLookAtTarget
        this.eyesLookAtTarget = new GameObject("eyesLookAtTarget").transform; // create new gameObject for chracter to look at
        this.eyesLookAtTarget.position = GameObject.Find("camTarget").transform.position; // set the position of the GO to camtarget object, which contains the main camera
        this.eyesLookAtTarget.parent = GameObject.Find("camTarget").transform; //parent new GO to camtarget object
        this.headAimPoint = new GameObject("headAimPoint").transform;// create new GO for head to follow when not looking at the main camera
        this.neckJoint = GameObject.Find("Head").transform; // set neckJoint equal to head joint in Covington.  ***Problem arises because blockman also has a head. (Solved b/c new blockman joints have numbers)****
        this.headAimPoint.parent = this.neckJoint;// set neckJoint as the parent of headAimPoint
        this.headAimPoint.localPosition = new Vector3(0, 0, -Vector3.Distance(this.headAimPoint.position, GameObject.Find("camTarget").transform.position));//set the z-value of the headAimPoint position equal to the distance to the camtarget. This way, it's relatively close to the camTarget
        this.eyeTimer = this.eyePacing;
        this.kinectHeadAimRotationRef = new GameObject("kinectHeadAimRotationRef").transform; // set Kinect references
        this.kinectHeadAimRotationRef.position = GameObject.Find("Neck").transform.position;
        this.kinectHeadAimRotationRef.parent = GameObject.Find("Neck").transform.parent;
    }

    // private var bodyEase : float = 0;  // NOT YET USED - presumably so blends aren't so abrupt
    // private var faceEase : float = 0; // NOT YET USED - presumably so blends aren't so abrupt
    private float switchTimer;
    private int selectExpression;
    public virtual void Update()// END DO EYE STUFF
    {
        //print ("expressionNeutralTimer = " + expressionNeutralTimer);
        // ONLY FOR DEMO TESTING - TURN CYCLEEMOTIONTEST ON IN INSPECTOR
        if (this.cycleEmotionTest)
        {
            // switchTimer += Time.deltaTime;
            // if (switchTimer > 9.0) {
            // selectExpression++;
            if (Input.GetKeyDown("left"))
            {
                this.selectExpression--;
                this.switchTimer = 1f;
            }
            if (Input.GetKeyDown("right"))
            {
                this.selectExpression++;
                this.switchTimer = 1f;
            }
            if (this.switchTimer > 0.9f)
            {
                if (this.selectExpression == 33)
                {
                    this.selectExpression = 1;
                }
                if (this.selectExpression == 0)
                {
                    this.selectExpression = 32;
                }
                if (this.selectExpression == 1)
                {
                    this.ExpressEmotion("Happy1");
                }
                if (this.selectExpression == 2)
                {
                    this.ExpressEmotion("Happy2");
                }
                if (this.selectExpression == 3)
                {
                    this.ExpressEmotion("Happy3");
                }
                if (this.selectExpression == 4)
                {
                    this.ExpressEmotion("Happy4");
                }
                if (this.selectExpression == 5)
                {
                    this.ExpressEmotion("Pain1");
                }
                if (this.selectExpression == 6)
                {
                    this.ExpressEmotion("Pain2");
                }
                if (this.selectExpression == 7)
                {
                    this.ExpressEmotion("Pain3");
                }
                if (this.selectExpression == 8)
                {
                    this.ExpressEmotion("Pain4");
                }
                if (this.selectExpression == 9)
                {
                    this.ExpressEmotion("Sad1");
                }
                if (this.selectExpression == 10)
                {
                    this.ExpressEmotion("Sad2");
                }
                if (this.selectExpression == 11)
                {
                    this.ExpressEmotion("Sad3");
                }
                if (this.selectExpression == 12)
                {
                    this.ExpressEmotion("Sad4");
                }
                if (this.selectExpression == 13)
                {
                    this.ExpressEmotion("Contempt1");
                }
                if (this.selectExpression == 14)
                {
                    this.ExpressEmotion("Contempt2");
                }
                if (this.selectExpression == 15)
                {
                    this.ExpressEmotion("Contempt3");
                }
                if (this.selectExpression == 16)
                {
                    this.ExpressEmotion("Contempt4");
                }
                if (this.selectExpression == 17)
                {
                    this.ExpressEmotion("Fear1");
                }
                if (this.selectExpression == 18)
                {
                    this.ExpressEmotion("Fear2");
                }
                if (this.selectExpression == 19)
                {
                    this.ExpressEmotion("Fear3");
                }
                if (this.selectExpression == 20)
                {
                    this.ExpressEmotion("Fear4");
                }
                if (this.selectExpression == 21)
                {
                    this.ExpressEmotion("Surprise1");
                }
                if (this.selectExpression == 22)
                {
                    this.ExpressEmotion("Surprise2");
                }
                if (this.selectExpression == 23)
                {
                    this.ExpressEmotion("Surprise3");
                }
                if (this.selectExpression == 24)
                {
                    this.ExpressEmotion("Surprise4");
                }
                if (this.selectExpression == 25)
                {
                    this.ExpressEmotion("Disgust1");
                }
                if (this.selectExpression == 26)
                {
                    this.ExpressEmotion("Disgust2");
                }
                if (this.selectExpression == 27)
                {
                    this.ExpressEmotion("Disgust3");
                }
                if (this.selectExpression == 28)
                {
                    this.ExpressEmotion("Disgust4");
                }
                if (this.selectExpression == 29)
                {
                    this.ExpressEmotion("Anger1");
                }
                if (this.selectExpression == 30)
                {
                    this.ExpressEmotion("Anger2");
                }
                if (this.selectExpression == 31)
                {
                    this.ExpressEmotion("Anger3");
                }
                if (this.selectExpression == 32)
                {
                    this.ExpressEmotion("Anger4");
                }
                this.switchTimer = 0;
            }
        }
        this.GetComponent<Animation>().CrossFade(this.mmm.name); // retain mouth pose anim so face expressions remain additive
        // BRING FACE UP TO TARGET WEIGHT AND DROP PREVIOUS WEIGHT (ALWAYS BALANCED BY MMM ANIMATION)
        this.currentFaceState.weight = Mathf.Lerp(this.currentFaceState.weight, this.faceStateWeight, Time.deltaTime);
        // if (currentFaceState.weight < faceStateWeight) currentFaceState.weight += Time.deltaTime * faceTransitionSpeed;
        // if (currentFaceState.weight > faceStateWeight) currentFaceState.weight -= Time.deltaTime * faceTransitionSpeed;
        if ((this.currentFaceState != this.lastCurrentFaceState) && (this.lastCurrentFaceState.weight > 0))
        {
            this.lastCurrentFaceState.weight = this.lastCurrentFaceState.weight - (Time.deltaTime * this.faceTransitionSpeed);
        }
        // BRING BODY UP TO TARGET WEIGHT AND DROP PREVIOUS WEIGHT (ALWAYS BALANCED BY NEUTRAL BODY)
        this.currentBodyState.weight = Mathf.Lerp(this.currentBodyState.weight, this.bodyStateWeight, Time.deltaTime); // transitionCurve.Evaluate( 0.5 );
        //if (currentBodyState.weight < bodyStateWeight) currentBodyState.weight += Time.deltaTime * bodyTransitionSpeed;
        //if (currentBodyState.weight > bodyStateWeight) currentBodyState.weight -= Time.deltaTime * bodyTransitionSpeed;
        if ((this.currentBodyState != this.lastCurrentBodyState) && (this.lastCurrentBodyState.weight > 0f))
        {
            this.lastCurrentBodyState.weight = this.lastCurrentBodyState.weight - (Time.deltaTime * this.bodyTransitionSpeed);
            this.bodyNeutralState.weight = 1f - (this.currentBodyState.weight + this.lastCurrentBodyState.weight);
        }
        else
        {
            this.bodyNeutralState.weight = 1f - this.currentBodyState.weight;
        }
        //fade expressions
        if (this.expressionNeutralTimer > 0f)
        {
            this.expressionNeutralTimer = this.expressionNeutralTimer - Time.deltaTime;
        }
        if (this.expressionNeutralTimer < 0f)
        {
            this.ExpressEmotion(virtualPatientControl.defaultEmotion);
            this.expressionNeutralTimer = 0f;
        }
        //if (Input.GetKey("y")) animation.CrossFade(nodYes.name);
        //if (Input.GetKey("n")) animation.CrossFade(nodNo.name);
        // DO EYE STUFF
        if (!this.LockEyesOnCam)
        {
            this.eyeTimer = this.eyeTimer - Time.deltaTime;
            if (this.eyeTimer < 0)
            {
                // if the distance between the headAimPosition and the camTarget (where the eyes should be locked) is less than the value defined by lookAtCamDistance, look at the KinectMan 
                // so increasing value of lookAtCamDistanceLimit should increase the amount of time spend staring at the camera
                if ((UnityEngine.Random.Range(0, 2) == 1) && (Vector3.Distance(this.headAimPoint.position, GameObject.Find("camTarget").transform.position) < this.lookAtCamDistanceLimit))
                {
                    this.eyesLookAtTarget.position = GameObject.Find("camTarget").transform.position;
                }
                else
                {
                    // if the distance is greater, have the eyes randomly at an area close to where the head is pointing
                    this.eyesLookAtTarget.position = this.headAimPoint.position + (UnityEngine.Random.insideUnitSphere * this.eyeMovementRange);
                }
                this.eyeTimer = UnityEngine.Random.Range(this.eyePacing, this.eyePacing + 1f);
            }
            // if head rotates away past limit while eyes are on camera
            if ((Vector3.Distance(this.eyesLookAtTarget.position, GameObject.Find("camTarget").transform.position) < 0.1f) && (Vector3.Distance(this.headAimPoint.position, GameObject.Find("camTarget").transform.position) > this.lookAtCamDistanceLimit))
            {
                this.eyesLookAtTarget.position = this.headAimPoint.position;
            }
        }
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
    private string currentEmotion; // used for GUI display
    private string lastEmotion;
    public virtual void ExpressEmotion(string emotion)
    {
        this.expressionNeutralTimer = 7f;
        if (emotion == this.lastEmotion)
        {
            return;
        }
        this.lastCurrentFaceState = this.currentFaceState;
        this.lastCurrentBodyState = this.currentBodyState;
        bool emotionFound = false;
        if (emotion.Contains("Happy1"))
        {
            this.currentBodyState = this.bodyHappyState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceHappyState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Happy2"))
        {
            this.currentBodyState = this.bodyHappyState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceHappyState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Happy3"))
        {
            this.currentBodyState = this.bodyHappyState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceHappyState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Happy4"))
        {
            this.currentBodyState = this.bodyHappyState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceHappyState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Contempt1"))
        {
            this.currentBodyState = this.bodyContemptState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceContemptState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Contempt2"))
        {
            this.currentBodyState = this.bodyContemptState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceContemptState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Contempt3"))
        {
            this.currentBodyState = this.bodyContemptState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceContemptState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Contempt4"))
        {
            this.currentBodyState = this.bodyContemptState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceContemptState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Disgust1"))
        {
            this.currentBodyState = this.bodyDisgustState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceDisgustState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Disgust2"))
        {
            this.currentBodyState = this.bodyDisgustState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceDisgustState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Disgust3"))
        {
            this.currentBodyState = this.bodyDisgustState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceDisgustState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Disgust4"))
        {
            this.currentBodyState = this.bodyDisgustState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceDisgustState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Sad1"))
        {
            this.currentBodyState = this.bodySadState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceSadState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Sad2"))
        {
            this.currentBodyState = this.bodySadState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceSadState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Sad3"))
        {
            this.currentBodyState = this.bodySadState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceSadState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Sad4"))
        {
            this.currentBodyState = this.bodySadState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceSadState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Pain1"))
        {
            this.currentBodyState = this.bodyPainState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.facePainState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Pain2"))
        {
            this.currentBodyState = this.bodyPainState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.facePainState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Pain3"))
        {
            this.currentBodyState = this.bodyPainState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.facePainState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Pain4"))
        {
            this.currentBodyState = this.bodyPainState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.facePainState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Fear1"))
        {
            this.currentBodyState = this.bodyFearState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceFearState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Fear2"))
        {
            this.currentBodyState = this.bodyFearState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceFearState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Fear3"))
        {
            this.currentBodyState = this.bodyFearState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceFearState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Fear4"))
        {
            this.currentBodyState = this.bodyFearState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceFearState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Anger1"))
        {
            this.currentBodyState = this.bodyAngerState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceAngerState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Anger2"))
        {
            this.currentBodyState = this.bodyAngerState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceAngerState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Anger3"))
        {
            this.currentBodyState = this.bodyAngerState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceAngerState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Anger4"))
        {
            this.currentBodyState = this.bodyAngerState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceAngerState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotion.Contains("Surprise1"))
        {
            this.currentBodyState = this.bodySurpriseState;
            this.bodyStateWeight = 0f;
            this.currentFaceState = this.faceSurpriseState;
            this.faceStateWeight = 0.25f;
            emotionFound = true;
        }
        if (emotion.Contains("Surprise2"))
        {
            this.currentBodyState = this.bodySurpriseState;
            this.bodyStateWeight = 0.33f;
            this.currentFaceState = this.faceSurpriseState;
            this.faceStateWeight = 0.33f;
            emotionFound = true;
        }
        if (emotion.Contains("Surprise3"))
        {
            this.currentBodyState = this.bodySurpriseState;
            this.bodyStateWeight = 0.66f;
            this.currentFaceState = this.faceSurpriseState;
            this.faceStateWeight = 0.66f;
            emotionFound = true;
        }
        if (emotion.Contains("Surprise4"))
        {
            this.currentBodyState = this.bodySurpriseState;
            this.bodyStateWeight = 1f;
            this.currentFaceState = this.faceSurpriseState;
            this.faceStateWeight = 1f;
            emotionFound = true;
        }
        if (emotionFound)
        {
            this.currentFaceState.enabled = true;
            this.currentFaceState.normalizedTime = 1f;
            if (this.currentFaceState != this.lastCurrentFaceState)
            {
                this.currentFaceState.weight = 0;
            }
            this.currentBodyState.enabled = true;
            if (this.currentBodyState != this.lastCurrentBodyState)
            {
                this.currentBodyState.weight = 0;
            }
            this.lastEmotion = emotion;
            MonoBehaviour.print("emotion to display: " + emotion);
            this.currentEmotion = emotion;
        }
        else
        {
            MonoBehaviour.print("requested emotion not found: " + emotion);
        }
    }

    public virtual void NodNo()
    {
        this.GetComponent<Animation>().CrossFade(this.nodNo.name);
    }

    public virtual void NodYes()
    {
        this.GetComponent<Animation>().CrossFade(this.nodYes.name);
    }

    public virtual void LateUpdate()
    {
        // body corrections;
        int i = 0;
        while (i < this.joints.Length)
        {

            {
                float _11 = this.joints[i].eulerAngles.x + this.jointTweak[i].x;
                Vector3 _12 = this.joints[i].eulerAngles;
                _12.x = _11;
                this.joints[i].eulerAngles = _12;
            }

            {
                float _13 = this.joints[i].eulerAngles.y + this.jointTweak[i].y;
                Vector3 _14 = this.joints[i].eulerAngles;
                _14.y = _13;
                this.joints[i].eulerAngles = _14;
            }

            {
                float _15 = this.joints[i].eulerAngles.z + this.jointTweak[i].z;
                Vector3 _16 = this.joints[i].eulerAngles;
                _16.z = _15;
                this.joints[i].eulerAngles = _16;
            }
            i++;
        }
        // DO EYE STUFF
        this.blinkTimer = this.blinkTimer - Time.deltaTime;
        if (this.blinkTimer <= 0)
        {
            this.leftEyeLid.localPosition = this.initialLeftPos + new Vector3(0.3f, 0, 0);
            this.rightEyeLid.localPosition = this.initialRightPos + new Vector3(0.3f, 0, 0);
            if (this.blinkTimer < -0.1f)
            {
                this.blinkTimer = UnityEngine.Random.Range(0.5f, 4f);
                this.leftEyeLid.localPosition = this.initialLeftPos;
                this.rightEyeLid.localPosition = this.initialRightPos;
            }
        }
         // THIS GETS THE NECK TO POINT THE SAME DIRECTION THAT THE EYES ARE LOOKING AT
        this.kinectHeadAimRotationRef.LookAt(GameObject.Find("camTarget").transform);
        Vector3 rotation = this.kinectHeadAimRotationRef.localEulerAngles + this.kinectHeadAimOffset;
        GameObject.Find("Neck").transform.localRotation = GameObject.Find("Neck").transform.localRotation * Quaternion.Euler(rotation);
        // keep the headAimPoint reference at the same plane as the camera
        Vector3 newPos = GameObject.Find("camTarget").transform.InverseTransformPoint(this.headAimPoint.position);
        newPos.z = 0;
        this.headAimPoint.position = GameObject.Find("camTarget").transform.TransformPoint(newPos);
        if (CameraControls.camLocked || this.webBuild)
        {
            this.leftEyeJoint.LookAt(this.eyesLookAtTarget);
            this.leftEyeJoint.eulerAngles = this.leftEyeJoint.eulerAngles - this.eyeCorrection;
            this.rightEyeJoint.LookAt(this.eyesLookAtTarget);
            this.rightEyeJoint.eulerAngles = this.rightEyeJoint.eulerAngles - this.eyeCorrection;
        }
        if (CameraControls.headTargetVelocity > this.speedThreshold)
        {
            this.LockEyes(true);
        }
        else
        {
            this.LockEyes(false);
        }
    }

    public virtual void LockEyes(bool state)
    {
        if (state)
        {
            if (CameraControls.camLocked)
            {
                this.LockEyesOnCam = true;
                this.eyesLookAtTarget.localPosition = Vector3.zero;
            }
        }
        else
        {
            this.LockEyesOnCam = false;
        }
    }

    private int fps;
    public virtual IEnumerator TrackFrameRate()
    {
        while (true)
        {
            int frameCount = 0;
            float timer = 0f;
            while (timer < 1f)
            {
                timer = timer + Time.deltaTime;
                frameCount++;
                yield return null;
            }
            this.fps = frameCount;
            yield return null;
        }
    }

    private bool nonLetter;
    public virtual void BuildShapeList(string input)
    {
        this.shapeList.Clear();
        input = input.ToLower();
        int i = 0;
        while (i < input.Length)
        {
            //print( input.Substring(i,1) );
            string letter = input.Substring(i, 1);
            this.nonLetter = false;
            switch (letter)
            {
                case "a":
                    this.shapeList.Add(this.aaa);
                    break;
                case "b":
                    this.shapeList.Add(this.mmm);
                    break;
                case "c":
                    this.shapeList.Add(this.ess);
                    break;
                case "d":
                    this.shapeList.Add(this.eee);
                    break;
                case "e":
                    this.shapeList.Add(this.eee);
                    break;
                case "f":
                    this.shapeList.Add(this.fuh);
                    break;
                case "g":
                    this.shapeList.Add(this.eee);
                    break;
                case "h":
                    this.shapeList.Add(this.iye);
                    break;
                case "i":
                    this.shapeList.Add(this.iye);
                    break;
                case "j":
                    this.shapeList.Add(this.aaa);
                    break;
                case "k":
                    this.shapeList.Add(this.aaa);
                    break;
                case "l":
                    this.shapeList.Add(this.laa);
                    break;
                case "m":
                    this.shapeList.Add(this.mmm);
                    break;
                case "n":
                    this.shapeList.Add(this.ess);
                    break;
                case "o":
                    this.shapeList.Add(this.oh);
                    break;
                case "p":
                    this.shapeList.Add(this.eee);
                    break;
                case "q":
                    this.shapeList.Add(this.ooh);
                    break;
                case "r":
                    this.shapeList.Add(this.aaa);
                    break;
                case "s":
                    this.shapeList.Add(this.ess);
                    break;
                case "t":
                    this.shapeList.Add(this.eee);
                    break;
                case "u":
                    this.shapeList.Add(this.ooh);
                    break;
                case "v":
                    this.shapeList.Add(this.fuh);
                    break;
                case "w":
                    this.shapeList.Add(this.ooh);
                    break;
                case "x":
                    this.shapeList.Add(this.ess);
                    break;
                case "y":
                    this.shapeList.Add(this.ooh);
                    break;
                case "z":
                    this.shapeList.Add(this.ess);
                    break;
                case ",":
                    // THIS METHOD NEEDS IMPROVEMENT
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    break;
                case ".":
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    break;
                case "!":
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    this.shapeList.Add(this.mmm);
                    break;
                case " ":
                    this.shapeList.Add(this.lastShape);
                    break;
                default:
                    this.nonLetter = true;
                    break;
            }
            //print (nonLetter);
            if (!this.nonLetter)
            {
                this.lastShape = (AnimationClip) this.shapeList[this.shapeList.Count - 1];
            }
            i++;
        }
        //print(shapeList);
        this.StartCoroutine(this.PlayShapeList());
        if (this.useAudioFiles)
        {
            bool audioClipFound = false;
            input = System.Text.RegularExpressions.Regex.Replace(input, "[.,']", "");
            input = System.Text.RegularExpressions.Regex.Replace(input, "( )+", "");
            //print("look for: "+input.ToLower());
            int matchLength = Mathf.Min(input.Length, 12);
            string subInput = input.Substring(0, matchLength);
            //print("look for sub: "+subInput);
            i = 0;
            while (i < this.audioFiles.Length)
            {
                //print(audioFiles[i].ToString());
                string subAudioFile = this.audioFiles[i].ToString();
                subAudioFile = subAudioFile.Substring(0, matchLength);
                if (subInput == subAudioFile)
                {
                    audioClipFound = true;
                    this.GetComponent<AudioSource>().clip = this.audioFiles[i];
                }
                i++;
            }
            if (audioClipFound)
            {
                this.GetComponent<AudioSource>().Play();
            }
            else
            {
                if (this.useTTS_asBackup)
                {
                    this.SendMessage("SpeakThis", input);
                }
            }
        }
        else
        {
            this.SendMessage("SpeakThis", input);
        }
        this.LockEyes(true);
    }

    public virtual IEnumerator PlayShapeList()
    {
        float t = 0;
        this.lastShape = this.aaa;
        float shapeTime = 0f;
        int shapeElement = 0;
        float duration = this.shapeList.Count * this.speakingSpeed;
        MonoBehaviour.print("start speaking");
        while (shapeTime < duration)
        {
            // close the mouth
            if (this.currentFaceState == this.faceSurpriseState)
            {
                this.currentFaceState.weight = 0; //currentFaceState.weight -= Time.deltaTime * faceTransitionSpeed;
            }
            shapeElement = (int) Mathf.Round(this.shapeList.Count * (shapeTime / duration));
            //print ((shapeTime/duration) +" "+ shapeElement );
            //if (shapeElement < shapeList.length) GetComponent.<Animation>().CrossFade(shapeList[  shapeElement ].name, blendSpeed * Time.deltaTime  );
            shapeTime = shapeTime + Time.deltaTime;
            yield return null;
        }
        MonoBehaviour.print("done speaking");
        t = 0;
        while (t < 0.5f)
        {
            this.GetComponent<Animation>().CrossFade(this.mmm.name);
            t = t + Time.deltaTime;
            yield return null;
        }
        this.LockEyes(false);
    }

    public virtual void OnGUI()
    {
        if (this.showEmotionBlendValues)
        {
            GUI.Label(new Rect(10, Screen.height - 60, 400, 22), this.currentEmotion);
            GUI.Label(new Rect(10, Screen.height - 40, 400, 22), (this.currentFaceState.name + ": ") + this.currentFaceState.weight.ToString("0.00"));
            GUI.Label(new Rect(10, Screen.height - 20, 540, 22), ((((this.currentBodyState.name + ": ") + this.currentBodyState.weight.ToString("0.00")) + " (neutral: ") + this.bodyNeutralState.weight.ToString("0.00")) + ")");
        }
    }

    public virtual void OnDrawGizmos()
    {
        if (this.eyesLookAtTarget)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(this.eyesLookAtTarget.position, 1f);
            Gizmos.DrawLine(this.leftEyeJoint.position, this.eyesLookAtTarget.position);
            Gizmos.DrawLine(this.rightEyeJoint.position, this.eyesLookAtTarget.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.headAimPoint.position, 1f);
            //Gizmos.DrawLine ( neckJoint.position, headAimPoint.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.headAimPoint.position, this.eyeMovementRange);
            if (Vector3.Distance(this.headAimPoint.position, GameObject.Find("camTarget").transform.position) > this.lookAtCamDistanceLimit)
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(this.headAimPoint.position, GameObject.Find("camTarget").transform.position);
        }
    }

    public virtualPatientControl()
    {
        this.showEmotionBlendValues = true;
        this.cycleEmotionTest = true;
        this.useTTS_asBackup = true;
        this.speakingSpeed = 0.47f;
        this.blendSpeed = 0.2f;
        this.faceTransitionSpeed = 2f;
        this.faceStateWeight = 1f;
        this.bodyTransitionSpeed = 0.5f;
        this.bodyStateWeight = 1f;
        this.lookCorrection = new Vector3(-182, 0, -90);
        this.eyeCorrection = new Vector3(-270, 7.4f, -103.8f);
        this.eyeMovementRange = 2f;
        this.eyePacing = 0.5f;
        this.lookAtCamDistanceLimit = 1f;
        this.blinkTimer = 1f;
        this.animNames = new object[0];
        this.shapeList = new List<AnimationClip>();
        this.speedThreshold = 0.8f;
    }

}