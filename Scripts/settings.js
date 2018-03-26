
import System.IO; 
import System;
import System.Text;

//var selGridInt : int = 0;
//var selStrings : String[] = ["Neutral", "Happy", "Sad", "Anger", "Fear", "Contempt", "Surprise", "Disgust", "Pain"];

private var defaultLevel : int = 0;
private static var firstLoad : boolean = true;
//private var needLoad : boolean = false;

private var showSettingsGUI : boolean = false; // settings GUI initially off
private var showPatientGUI : boolean = false;

// images for different avatars
public var OMTex : Texture;
public var OFTex : Texture;
public var YMTex : Texture;
public var YFTex : Texture;


private var fov : float; // field of view
private var camPos : Vector3; // camera position
private var camRot : Vector3; // camera rotation
private var commentCount : int = 0; // ???
private var toggleChatSocketReady = false;

// Kinect Control Variables
private var worldScale: Vector3; //scale of Kinect PointMan Character
private var xOffset:float;
private var yOffset: float;
private var zOffset: float;
private var screenHeight: float;// controls position of the frame through which the camera points to
private var kinFOV: float; //frustum adjust

private var voiceListA = new Array();
private var voiceList : String[];
private var selectedVoice : int;
private var subtitleSize : int; // subtitle size variable
private var textLine = "subtitle text";
private var inputLine = "input text";
private var inputFieldHeight:int;

private var seeCursor : boolean = true;

//VISUALS
var wallColor: Material;
private var wallRed: float;
private var wallGreen: float;
private var wallBlue: float;
var eyeLight: Light;
private var eyeLightInt: float;
var leftHighKey: Light;
private var leftHighKeyInt: float;
var leftHighRim: Light;
private var leftHighRimInt: float;
var rightHighFill:Light;
private var rightHighFillInt: float;

// CONFIG FILE STUFF
var allowGUI : boolean = true;
var config = "vp_configuration.txt";
private var displayTimer : float = 0.0;
private var wwwprogress : float = 0.0;

private var hostReset : String;
private var patientReset : String;
private var emotionReset : String;

private var isWebPlayer : boolean = true;

function Start() {

	// LOAD CONFIG FILE
	var configString : String;
	#if UNITY_WEBPLAYER
		isWebPlayer = true;
		var url : String = Application.dataPath + "/" + config; // url path to the location of the config file (must always be in the same location)
		var www : WWW = new WWW (url); // download the contents of the config file and store it in www variable
		yield www;
		configString = www.text;
		
	#endif
	#if UNITY_STANDALONE_WIN
		isWebPlayer = false;
		selectedVoice = GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voice_nb; // use voices if not WebPLayer, retrieve specific voice number being used
		MakeVoiceList();
		print("loading config");
		try {
			var sr = new StreamReader(config); // "StreamReader - MS .NET class that implements a TextReader to read characters from a byte stream in a particular encoding
			configString = sr.ReadToEnd();  
			sr.Close();
			sr = null;
		}
		catch (e) {
			Debug.Log("internal error!");
			return null;
		}

	#endif

	// read and load the config data
	var lineResult = configString.Split(["\n"], StringSplitOptions.None);
	for (var i = 0; i <= lineResult.length-1; i++) {
		if (i == 1) ChatSocket.Host = lineResult[i];
		if (i == 4) GetComponent.<Camera>().main.fieldOfView = parseFloat(lineResult[i]);
		if (i == 6) {
			var lineSep = lineResult[i].Split([" "], StringSplitOptions.None);
			GetComponent.<Camera>().main.transform.localPosition = Vector3(parseFloat(lineSep[0]),parseFloat(lineSep[1]),parseFloat(lineSep[2]));
		}
		if (i == 8) {
			lineSep = lineResult[i].Split([" "], StringSplitOptions.None);
			GetComponent.<Camera>().main.transform.localEulerAngles = Vector3(parseFloat(lineSep[0]),parseFloat(lineSep[1]),parseFloat(lineSep[2]));
		}
		if (i == 11) { GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voice_nb = parseInt(lineResult[i]); selectedVoice = parseInt(lineResult[i]);}
		if (i == 17) ChatSocket.endSentenceTime = parseFloat(lineResult[i]);
		if (i == 19) {
			if (lineResult[i] == "True") ChatSocket.showReplies = true;
			else ChatSocket.showReplies = false;
		}
		if (i == 21) GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize = parseInt(lineResult[i]);//set fontSize variable in Chatsocket to config
		if (i == 24){
			lineSep = lineResult[i].Split([" "], StringSplitOptions.None);
			GameObject.Find("KinectPointMan").transform.localScale = Vector3(parseFloat(lineSep[0]),parseFloat(lineSep[1]),parseFloat(lineSep[2]));
			}
		if (i == 26) GameObject.Find("KinectPointMan").transform.position.x = parseFloat(lineResult[i]);
		if (i == 28) GameObject.Find("KinectPointMan").transform.position.y = parseFloat(lineResult[i]);
		if (i == 30) GameObject.Find("KinectPointMan").transform.position.z = parseFloat(lineResult[i]);
		if (i == 32) GameObject.Find("targetFrame").transform.position.y = parseFloat(lineResult[i]);
		if (i == 34) GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist= parseFloat(lineResult[i]);
		if (i == 37) wallColor.color.r = parseFloat(lineResult[i]);
		if (i == 39) wallColor.color.g = parseFloat(lineResult[i]);
		if (i == 41) wallColor.color.b = parseFloat(lineResult[i]);
		if (i == 43) eyeLight.intensity = parseFloat(lineResult[i]);
		if (i == 45) leftHighKey.intensity = parseFloat(lineResult[i]);
		if (i == 47) leftHighRim.intensity = parseFloat(lineResult[i]);
		if (i == 49) rightHighFill.intensity = parseFloat(lineResult[i]);
		if (i == 51) GameObject.FindWithTag("Player").GetComponent(ChatSocket).inputFieldHeight = parseInt(lineResult[i]);
		if (i == 53) ChatSocket.PatientChoice = lineResult[i];
		if (i == 55) defaultLevel = parseInt(lineResult[i]);
		if (i == 57) GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = lineResult[i];

		print(lineResult[i]);
	}
	print("chatscript url: "+ChatSocket.Host);
	// END LOAD CONFIG
	
	fov = GetComponent.<Camera>().main.fieldOfView;
	camPos = GetComponent.<Camera>().main.transform.localPosition;
	camRot = GetComponent.<Camera>().main.transform.localEulerAngles;
	subtitleSize = GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
	worldScale =  GameObject.Find("KinectPointMan").transform.localScale;
	xOffset = GameObject.Find("KinectPointMan").transform.position.x;
	yOffset =  GameObject.Find("KinectPointMan").transform.position.y;
	zOffset =  GameObject.Find("KinectPointMan").transform.position.z;
	screenHeight = GameObject.Find("targetFrame").transform.position.y;
	kinFOV =  GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist;
	wallRed = wallColor.color.r;
	wallGreen = wallColor.color.g;
	wallBlue = wallColor.color.b;
	eyeLightInt = eyeLight.intensity;
	leftHighKeyInt = leftHighKey.intensity;
	leftHighRimInt = leftHighRim.intensity;
	rightHighFillInt = rightHighFill.intensity;
	inputFieldHeight = GameObject.FindWithTag("Player").GetComponent(ChatSocket).inputFieldHeight;
	
	hostReset = ChatSocket.Host;
	patientReset = ChatSocket.PatientChoice;
	emotionReset = GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion;
	
	if ( firstLoad &&Application.loadedLevel != defaultLevel) Application.LoadLevel(defaultLevel);
	firstLoad = false;
}

function MakeVoiceList() {
	yield WaitForSeconds(1.0);
	for (var i = 0; i < VoiceSpeaker.GetVoiceCount(); i++) {
		//voiceListA.Add( VoiceSpeaker.GetVoiceName(i) );
		//print (VoiceSpeaker.GetVoiceName(i) );
		yield;
	}
	voiceList = voiceListA.ToBuiltin(String);
}

function Update () {

	//  show or hide cursor
	if (Input.GetKeyDown (KeyCode.C) && seeCursor==true) {
		Cursor.visible = false;
		seeCursor = false;
	}
	
	else if (Input.GetKeyDown (KeyCode.C)){
		Cursor.visible = true;
		seeCursor = true;
	}
	

	//if (isWebPlayer) return;
	
	if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); 
	
	if (Input.GetKeyDown (KeyCode.F1) && allowGUI) {
		if (!showSettingsGUI) {
			textLine = "subtitle text";
			inputLine = "input text";
			showSettingsGUI = true;
			if (ChatSocket.readyToConnect) {
				ChatSocket.readyToConnect = false;
				toggleChatSocketReady = true;
			}
		}
		else {
			showSettingsGUI = false;
			if (toggleChatSocketReady) {
				ChatSocket.readyToConnect = true;
				toggleChatSocketReady = false;
				
				GameObject.FindWithTag("Player").GetComponent(ChatSocket).inputFieldHeight = inputFieldHeight;
			}
		}
	}
	
}

function SaveConfig() {
	print("saving config");
	var text = "// the chatscript ip address";
	text += "\n";
	text += ChatSocket.Host;
	text += "\n";
	text += "// camera settings - use the in-app GUI by pressing F1";
	text += "\n";
	text += "// fov";
	text += "\n";
	text += GetComponent.<Camera>().main.fieldOfView;
	text += "\n";
	text += "// position";
	text += "\n";
	text += GetComponent.<Camera>().main.transform.localPosition.x +" "+ GetComponent.<Camera>().main.transform.localPosition.y +" "+ GetComponent.<Camera>().main.transform.localPosition.z;
	text += "\n";
	text += "// rotation";
	text += "\n";
	text += GetComponent.<Camera>().main.transform.localEulerAngles.x +" "+ GetComponent.<Camera>().main.transform.localEulerAngles.y +" "+ GetComponent.<Camera>().main.transform.localEulerAngles.z;
	text += "\n";
	text += "// speech settings - use the in-app GUI by pressing F1";
	text += "\n";
	text += "// system voice number";
	text += "\n";
	text += GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voice_nb;
	text += "\n";
	text += "// length";
	text += "\n";
	text += "0.047"; //length hardcoded?
	text += "\n";
	text += "// smooth";
	text += "\n";
	text += "9"; //smooth hardcoded?
	text += "\n";
	text += "// voice recognition end time delay to send question";
	text += "\n";
	text += ChatSocket.endSentenceTime;
	text += "\n";
	text += "// subtitles on/off (True/False)";
	text += "\n";
	text += ChatSocket.showReplies;
	text += "\n";
	text += "// subtitle size";
	text += "\n";
	text += GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
	text += "\n";
	text += "//Kinect Settings";
	text += "\n";
	text += "//world scale";
	text += "\n";
	text += GameObject.Find("KinectPointMan").transform.localScale.x + " " + GameObject.Find("KinectPointMan").transform.localScale.y + " " + GameObject.Find("KinectPointMan").transform.localScale.z;
	text += "\n";
	text += "//x offset";
	text += "\n";
	text += GameObject.Find("KinectPointMan").transform.position.x;
	text += "\n";
	text += "//y offset";
	text += "\n";
	text += GameObject.Find("KinectPointMan").transform.position.y;
	text += "\n";
	text += "//z offset";
	text += "\n";
	text += GameObject.Find("KinectPointMan").transform.position.z;
	text += "\n";
	text += "//screen height";
	text += "\n";
	text += GameObject.Find("targetFrame").transform.position.y;
	text += "\n";
	text += "//field of view";
	text += "\n";
	text += GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist;
	text += "\n";
	text += "//Visual Settings";
	text += "\n";
	text += "//wall red value";
	text += "\n";
	text += wallColor.color.r;
	text += "\n";
	text += "//wall green value";
	text += "\n";
	text += wallColor.color.g;
	text += "\n";
	text += "//wall blue value";
	text += "\n";
	text += wallColor.color.b;
	text += "\n";
	text += "//eye light intensity";
	text += "\n";
	text += eyeLight.intensity;
	text += "\n";
	text += "//key light intensity";
	text += "\n";
	text += leftHighKey.intensity;
	text += "\n";
	text += "//rim light intensity";
	text += "\n";
	text += leftHighRim.intensity;
	text += "\n";
	text += "//fill light intensity";
	text += "\n";
	text += rightHighFill.intensity;
	text += "\n";
	text += "//input text height";
	text += "\n";
	text += inputFieldHeight;
	text += "\n";
	text += "//patient selection choice";
	text += "\n";
	text += ChatSocket.PatientChoice;
	text += "\n";
	text += "//Default level";
	text += "\n";
	text += Application.loadedLevel;
	text += "\n";
	text += "default emotion";
	text += "\n";
	text += GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion;

   try {
      //var sw = new StreamWriter( Application.dataPath+"/"+config);
	  var sw = new StreamWriter(config);
      sw.WriteLine(text);
      sw.Close();
      sw = null;
	}
	catch (e) {
		Debug.Log("internal error!");
		return null;
	}
	// update the vars so reset is same as saved config

	fov = GetComponent.<Camera>().main.fieldOfView;
	camPos = GetComponent.<Camera>().main.transform.localPosition;
	camRot = GetComponent.<Camera>().main.transform.localEulerAngles;
	subtitleSize = GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
	worldScale =  GameObject.Find("KinectPointMan").transform.localScale;
	xOffset = GameObject.Find("KinectPointMan").transform.position.x;
	yOffset =  GameObject.Find("KinectPointMan").transform.position.y;
	zOffset =  GameObject.Find("KinectPointMan").transform.position.z;
	screenHeight = GameObject.Find("targetFrame").transform.position.y;
	kinFOV =  GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist;
	wallRed = wallColor.color.r;
	wallGreen = wallColor.color.g;
	wallBlue = wallColor.color.b;
	eyeLightInt = eyeLight.intensity;
	leftHighKeyInt = leftHighKey.intensity;
	leftHighRimInt = leftHighRim.intensity;
	rightHighFillInt = rightHighFill.intensity;
	GameObject.FindWithTag("Player").GetComponent(ChatSocket).inputFieldHeight = inputFieldHeight;
	
	hostReset = ChatSocket.Host;
	patientReset = ChatSocket.PatientChoice;
	emotionReset = GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion;
}

private var listVoices : boolean = false;
private var showKinectGUI : boolean = false;
private var useKinect : boolean = false;
private var kinectCentering : float = 0;
private var kinectRange : float = 12.0;

function OnGUI() {

	if (showSettingsGUI && !showKinectGUI && !showPatientGUI) {
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)-225,200,750));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("chatscript IP:");
						ChatSocket.Host = GUILayout.TextField (ChatSocket.Host);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("Use default chatscript server"))
						ChatSocket.Host = "128.146.170.194";
					GUILayout.EndHorizontal();
				/*	GUILayout.BeginHorizontal();
						GUILayout.Label ("Patient Choice:");
						ChatSocket.PatientChoice = GUILayout.TextField (ChatSocket.PatientChoice);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						if(GUILayout.Button("Use default patient"))
						ChatSocket.PatientChoice = "patient1a";
					GUILayout.EndHorizontal();*/
					GUILayout.BeginHorizontal();
						ChatSocket.showInputBox = GUILayout.Toggle(ChatSocket.showInputBox, "show input text", GUILayout.Width(110));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.Label ("input vert  position:");
						if (GUILayout.RepeatButton(" up  "))if(inputFieldHeight >0) inputFieldHeight--;
						if (GUILayout.RepeatButton("down")) if (inputFieldHeight < (Screen.height - 30)) inputFieldHeight++;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("voice recognition timer:");
						var voiceTime : String = ChatSocket.endSentenceTime.ToString("0.00") ;
						voiceTime = GUILayout.TextField (voiceTime ) ;
						ChatSocket.endSentenceTime = parseFloat(voiceTime);
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();				
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		// CAMERA
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)-90,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("camera:");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("zoom:", GUILayout.Width(50));
						if (GUILayout.RepeatButton(" << ")) GetComponent.<Camera>().main.fieldOfView += 0.05;
						if (GUILayout.RepeatButton(" >>" )) GetComponent.<Camera>().main.fieldOfView -= 0.05;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("dolly:", GUILayout.Width(50));
						if (GUILayout.RepeatButton(" << ")) GetComponent.<Camera>().main.transform.Translate(0,0,-0.05);
						if (GUILayout.RepeatButton(" >> ")) GetComponent.<Camera>().main.transform.Translate(0,0,0.05);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("height:", GUILayout.Width(50));
						if (GUILayout.RepeatButton("down")) GetComponent.<Camera>().main.transform.Translate(0,-0.05,0);
						if (GUILayout.RepeatButton(" up ")) GetComponent.<Camera>().main.transform.Translate(0,0.05,0);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("tilt:", GUILayout.Width(50));
						if (GUILayout.RepeatButton("down")) GetComponent.<Camera>().main.transform.Rotate(0.05,0,0);
						if (GUILayout.RepeatButton(" up ")) GetComponent.<Camera>().main.transform.Rotate(-0.05,0,0);
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();				
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)+50,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("default expression:");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						if (GUILayout.RepeatButton("neutral")) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = ("Happy1");
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Happy1");
							}
						if (GUILayout.RepeatButton("happy" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = ("Happy2");
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Happy2");
							}
						if (GUILayout.RepeatButton("sad" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Sad2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Sad2");
							}
        				//selGridInt = GUI.SelectionGrid (Rect (0, 35, 200, 75), selGridInt, selStrings, 3);
    				GUILayout.EndHorizontal();
    				GUILayout.BeginHorizontal();
						if (GUILayout.RepeatButton("anger")) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Anger2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Anger2");
						}
						if (GUILayout.RepeatButton("fear" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Fear2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Fear2");
							}
						if (GUILayout.RepeatButton("contempt" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Contempt2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Contempt2");
							}
    				GUILayout.EndHorizontal();
    				 GUILayout.BeginHorizontal();
						if (GUILayout.RepeatButton("surprise")) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Surprise2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Surprise2");
							}
						if (GUILayout.RepeatButton("disgust" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Disgust2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Disgust2");
							}
						if (GUILayout.RepeatButton("pain" )) {
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = "Pain2";
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).ExpressEmotion("Pain2");
							}
    				GUILayout.EndHorizontal();
    			GUILayout.EndVertical();
    		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		
		//  WALL COLOR
		GUILayout.BeginArea (Rect ((Screen.width/2)-205,(Screen.height/2)+165,220,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("wall color:", GUILayout.Width(192));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("red value:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) wallColor.color.r -= .005;
						if (GUILayout.RepeatButton("more")) wallColor.color.r += .005;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("green value:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) wallColor.color.g -= .005;
						if (GUILayout.RepeatButton("more")) wallColor.color.g += .005;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("blue value:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) wallColor.color.b -= .005;
						if (GUILayout.RepeatButton("more")) wallColor.color.b += .005;
					GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		
		// SAVE  & RESET
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)-225,200,100));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("config:");
						if (GUILayout.Button("reset")){
							print("reset");
							GetComponent.<Camera>().main.fieldOfView = fov;
							GetComponent.<Camera>().main.transform.localPosition = camPos;
							GetComponent.<Camera>().main.transform.localEulerAngles = camRot;
							GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize = subtitleSize;
							wallColor.color.r = wallRed;
							wallColor.color.g = wallGreen ;
							wallColor.color.b = wallBlue;
							eyeLight.intensity = eyeLightInt;
							leftHighKey.intensity = leftHighKeyInt;
							leftHighRim.intensity = leftHighRimInt;
							rightHighFill.intensity = rightHighFillInt;
							inputFieldHeight = GameObject.FindWithTag("Player").GetComponent(ChatSocket).inputFieldHeight;
							ChatSocket.Host = hostReset;
							GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).defaultEmotion = emotionReset;
						//	ChatSocket.PatientChoice = patientReset;
						}
						//if (!isWebPlayer) {
							if (GUILayout.Button("save")) SaveConfig();
						//}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();				
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();
		// KINECT
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2-190),200,100));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("kinect")) showKinectGUI = true;
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();				
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();
		//Patient Settings
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2-155),200,100));
			GUILayout.BeginHorizontal("box");
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("Patient Settings")) showPatientGUI = true;
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();	
		// SPEECH
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2-120),200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();		
					GUILayout.Label ("output:");
					GUILayout.BeginHorizontal();
						GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).useAudioFiles = 
							GUILayout.Toggle(GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).useAudioFiles, "recorded voice", GUILayout.Width(100));
						GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).useTTS_asBackup = 
							GUILayout.Toggle(GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).useTTS_asBackup, "TTS voice");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("select")) listVoices = true;;
						if (GUILayout.Button("test")) {
							if (commentCount == 0) textLine = "This is a test of the selected voice. The characters mouth should match reasonably well";
							if (commentCount == 1) textLine = "I have been feeling a sharp pain in my side for the past two days. It used to come and go but today it is constant";
							if (commentCount == 2) textLine = "How much would a wood chuck chuck if a wood chuck could chuck wood";
							if (GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).useAudioFiles) textLine = "Well, I can't move around very well so that is pretty limiting";
							commentCount++;
							if (commentCount > 2) commentCount = 0;
							GameObject.FindWithTag("Player").SendMessage("BuildShapeList", textLine);
						}
					GUILayout.EndHorizontal();
					if (listVoices) {
						//selectedVoice = GUILayout.SelectionGrid (selectedVoice, voiceList, 1, GUILayout.Width(190));
						for (var i = 0; i < voiceList.length; i++) {
							if (GUILayout.Button( voiceList[i], GUILayout.Width(190) )) {
								VoiceSpeaker.SetVoice(i);
								listVoices = false;
							}
						}
					}
					GUILayout.BeginHorizontal();
						GUILayout.Label("length:", GUILayout.Width(50));
						GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).speakingSpeed =  
							GUILayout.HorizontalSlider (GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).speakingSpeed, 0.01, 0.09);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label("smooth:", GUILayout.Width(50));
						GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).blendSpeed =  
							GUILayout.HorizontalSlider (GameObject.FindWithTag("Player").GetComponent(virtualPatientControl).blendSpeed, 0.0, 12.0);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(); // VOICE RATE CONTROLS - ONLY ADJUST IN INT INCREMENTS - DON'T SAVE TO CONFIG FILE YET
						GUILayout.Label("rate:", GUILayout.Width(50));
						GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voiceRate =  
							GUILayout.HorizontalSlider (GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voiceRate, 0.0, 12.0);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						 ChatSocket.showReplies = GUILayout.Toggle(ChatSocket.showReplies, "subtitles", GUILayout.Width(70));
						 GUILayout.Label("size:", GUILayout.Width(35));
						 GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize =  
							GUILayout.HorizontalSlider (GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize, 10, 60);
					GUILayout.EndHorizontal();
		
				GUILayout.EndVertical();				
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)+65,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();		
					GUILayout.Label ("light intensity:");
					GUILayout.BeginHorizontal();
						GUILayout.Label ("eye light:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) eyeLight.intensity -= .025;
						if (GUILayout.RepeatButton("more")) eyeLight.intensity += .025;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("key:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) leftHighKey.intensity -= .025;
						if (GUILayout.RepeatButton("more")) leftHighKey.intensity += .025;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("rim:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) leftHighRim.intensity -= .025;
						if (GUILayout.RepeatButton("more")) leftHighRim.intensity += .025;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("fill:", GUILayout.Width(100));
						if (GUILayout.RepeatButton("less")) rightHighFill.intensity -= .025;
						if (GUILayout.RepeatButton("more")) rightHighFill.intensity += .025;
					GUILayout.EndHorizontal();
					
					
					GUILayout.EndVertical();				
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		

		if (ChatSocket.showReplies && !listVoices) {
			var fontsize = GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
			GUILayout.BeginArea (new Rect( Mathf.Max( 20, (Screen.width/2)-(textLine.Length * (fontsize/4))) , Mathf.Min((Screen.height/2)+350, Screen.height - fontsize - 50), Mathf.Min( Screen.width-40, (textLine.Length * (fontsize/2)) ) , 500));
				GUILayout.BeginHorizontal("box");
					GUILayout.Label(textLine, GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI );
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		
		if (ChatSocket.showInputBox) {
			GUILayout.BeginArea (new Rect( 20, inputFieldHeight, Screen.width-40 , 100));			
				GUILayout.BeginHorizontal("box");
					inputLine = GUILayout.TextField (inputLine);
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		
	}
	if (displayTimer > 0) {
		GUI.Label(  Rect (10, 40, 800, 20), "the chatscript IP: "+ ChatSocket.Host);
		//GUI.Label(  Rect (10, 60, 800, 20), "progress: "+ wwwprogress.ToString()   );
		displayTimer -= Time.deltaTime;
	}
	
	if (showKinectGUI && showSettingsGUI) {
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)-50,200,600));
			GUILayout.BeginHorizontal("box");	
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.Label ("kinect:");
				if (GUILayout.Button("main settings")) showKinectGUI = false;
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				useKinect = GUILayout.Toggle(useKinect, "  use kinect tracking");
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				GUILayout.Label ("world scale:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("KinectPointMan").transform.localScale += Vector3(0.1,0.1,0.1); // BlockMan gets bigger, world gets smaller
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("KinectPointMan").transform.localScale -= Vector3(0.1,0.1,0.1);//  BlockMan gets smaller, world gets bigger
			GUILayout.EndHorizontal();			
			GUILayout.BeginHorizontal();
				GUILayout.Label ("offset x:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("KinectPointMan").transform.Translate(-0.1,0,0);  
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("KinectPointMan").transform.Translate(0.1,0,0);
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				GUILayout.Label ("offset y:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("KinectPointMan").transform.Translate(0,-0.1,0,0);  
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("KinectPointMan").transform.Translate(0,0.1,0);  
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				GUILayout.Label ("offset z:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("KinectPointMan").transform.Translate(0,0,-0.1);  
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("KinectPointMan").transform.Translate(0,0,0.1);  
			GUILayout.EndHorizontal();		
			GUILayout.BeginHorizontal();
				GUILayout.Label ("screen height:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("targetFrame").transform.Translate(0,0.1,0);
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("targetFrame").transform.Translate(0,-0.1,0);
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				GUILayout.Label ("field of view:", GUILayout.Width(100));
					if (GUILayout.RepeatButton(" << ")) GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist -= 0.1;
					if (GUILayout.RepeatButton(" >>" )) GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist += 0.1;
			GUILayout.EndHorizontal();	

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();	
		GUILayout.EndArea();
		
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)-50,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.Label ("kinect texture here");	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("config:");
						if (GUILayout.Button("reset")) {
							print("Kinect Reset");
								GameObject.Find("KinectPointMan").transform.localScale = worldScale;
								GameObject.Find("KinectPointMan").transform.position.x = xOffset;
								GameObject.Find("KinectPointMan").transform.position.y = yOffset;
								GameObject.Find("KinectPointMan").transform.position.z = zOffset;
								GameObject.Find("targetFrame").transform.position.y = screenHeight;
								GameObject.Find("camTarget").GetComponent(CameraControls).initHeightAtDist = kinFOV;
						}
						//if (!isWebPlayer) {
							if (GUILayout.Button("save")) SaveConfig();
						//}			
				GUILayout.EndVertical(); 
				
					GUILayout.EndHorizontal();
				 
			GUILayout.EndHorizontal();	
		GUILayout.EndArea();						
	}
	
	if (showPatientGUI && showSettingsGUI){
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)-125,200,600));
			GUILayout.BeginHorizontal("box");
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Patient Name:");
						ChatSocket.PatientChoice = GUILayout.TextField (ChatSocket.PatientChoice);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						if(GUILayout.Button("Use default patient"))
						ChatSocket.PatientChoice = "patient1a";
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)-125,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.Label("Patient:");
						if (GUILayout.Button( "main settings"))showPatientGUI = false;
					GUILayout.EndHorizontal();		
					GUILayout.BeginHorizontal();
						GUILayout.Label ("config:");
						if (GUILayout.Button("reset")) {
							print("Patient Reset");
							ChatSocket.PatientChoice = patientReset;
						}
						//if (!isWebPlayer) {
							if (GUILayout.Button("save")) SaveConfig();
						//}		
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();	
		GUILayout.EndArea();
		
		GUILayout.BeginArea(Rect((Screen.width/2) - 205, (Screen.height/2)-60, 410, 600));
			GUILayout.BeginHorizontal("box");
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.Label("Patient avatar:");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label("");
						if(GUILayout.Button(OMTex)) {//defaultLevel = 0; 
						ChatSocket.readyToConnect = true;
						Application.LoadLevel(0);};
						if(GUILayout.Button(YMTex)) {//defaultLevel = 1;
						ChatSocket.readyToConnect = true;
						 Application.LoadLevel(1);};
						if(GUILayout.Button(OFTex)) {//defaultLevel = 2;
						ChatSocket.readyToConnect = true;
						 Application.LoadLevel(2);};
						if(GUILayout.Button(YFTex)) {//defaultLevel = 3;
						ChatSocket.readyToConnect = true;
						 Application.LoadLevel(3);};
						 GUILayout.Label("");
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();			
	}
}