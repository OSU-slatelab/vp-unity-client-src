import System.IO;
import System;
import System.Text;

private var showSettingsGUI : boolean = false;

private var fov : float;
private var camPos : Vector3;
private var camRot : Vector3;
private var commentCount : int = 0;
private var toggleChatSocketReady = false;

private var voiceListA = new Array();
private var voiceList : String[];
private var selectedVoice : int;
private var subtitleSize : int;
private var textLine = "subtitle text";

// CONFIG FILE STUFF
var allowGUI : boolean = true;
var config = "vp_configuration.txt";
private var chatURL : String;
private var displayTimer : float = 0.0;
private var wwwprogress : float = 0.0;

private var isWebPlayer : boolean = true;

function Start() {

	// LOAD CONFIG FILE
	var configString : String;
	#if UNITY_WEBPLAYER
		isWebPlayer = true;
		var url : String = Application.dataPath + "/" + config;
		var www : WWW = new WWW (url);
		yield www;
		configString = www.text;
		//print("IT'S A WEBPLAYER!!!!");
	#endif
	
	#if UNITY_STANDALONE_WIN
		isWebPlayer = false;
		selectedVoice = GameObject.FindWithTag("Player").GetComponent(VoiceSpeaker).voice_nb;
//		MakeVoiceList();
		print("loading config");
		try {
			var sr = new StreamReader(config);
			configString = sr.ReadToEnd();  
			sr.Close();
			sr = null;
		}
		catch (e) {
			Debug.Log("internal error!");
			return null;
		}
	#endif

	var lineResult = configString.Split(["\n"], StringSplitOptions.None);
	for (var i = 0; i <= lineResult.length-1; i++) {
		if (i == 1) chatURL = lineResult[i];
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
		if (i == 21) GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize = parseInt(lineResult[i]);
		print(lineResult[i]);
	}
	print("chatscript url: "+ChatSocket.Host);
	// END LOAD CONFIG

	fov = GetComponent.<Camera>().main.fieldOfView;
	camPos = GetComponent.<Camera>().main.transform.localPosition;
	camRot = GetComponent.<Camera>().main.transform.localEulerAngles;
	subtitleSize = GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
	



}

//function MakeVoiceList() {
//	yield WaitForSeconds(1.0);
//	for (var i = 0; i < VoiceSpeaker.GetVoiceCount(); i++) {
//		voiceListA.Add( VoiceSpeaker.GetVoiceName(i) );
//		print (VoiceSpeaker.GetVoiceName(i) );
//		yield;
//	}
//	voiceList = voiceListA.ToBuiltin(String);
//}

function Update () {
	//if (isWebPlayer) return;
	if (Input.GetKeyDown("escape")) Application.Quit();
	
	if (Input.GetKeyDown (KeyCode.F1) && allowGUI) {
		if (!showSettingsGUI) {
			textLine = "subtitle text";
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
			}
		}
	}
	
}

function SaveConfig() {
	print("saving config");
	var text = "// the chatscript ip address";
	text += "\n";
	text += "128.146.170.194";
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
	text += "0.047";
	text += "\n";
	text += "// smooth";
	text += "\n";
	text += "9";
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

}

private var listVoices : boolean = false;
private var showKinectGUI : boolean = false;
private var useKinect : boolean = false;
private var kinectCentering : float = 0;
private var kinectRange : float = 12.0;

function OnGUI() {

	if (showSettingsGUI && !showKinectGUI) {
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)-40,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("chatscript IP:");
						ChatSocket.Host = GUILayout.TextField (ChatSocket.Host);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						ChatSocket.showInputBox = GUILayout.Toggle(ChatSocket.showInputBox, "show input text", GUILayout.Width(140));
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
		GUILayout.BeginArea (Rect ((Screen.width/2)-(205),(Screen.height/2)+50,200,600));
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
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();
		// SAVE  & RESET
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)-40,200,100));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("config:");
						if (GUILayout.Button("reset")) {
							print("reset");
							GetComponent.<Camera>().main.fieldOfView = fov;
							GetComponent.<Camera>().main.transform.localPosition = camPos;
							GetComponent.<Camera>().main.transform.localEulerAngles = camRot;
							GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize = subtitleSize;
						}
						//if (!isWebPlayer) {
							if (GUILayout.Button("save")) SaveConfig();
						//}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();				
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();
		// KINECT
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2),200,100));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("kinect")) showKinectGUI = true;
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();				
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();		
		// SPEECH
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)+50,200,600));
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
					GUILayout.BeginHorizontal();
						 ChatSocket.showReplies = GUILayout.Toggle(ChatSocket.showReplies, "subtitles", GUILayout.Width(70));
						 GUILayout.Label("size:", GUILayout.Width(35));
						 GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize =  
							GUILayout.HorizontalSlider (GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize, 10, 60);
						 
					GUILayout.EndHorizontal();
		
				GUILayout.EndVertical();				
			GUILayout.EndHorizontal();;
		GUILayout.EndArea();

		if (ChatSocket.showReplies && !listVoices) {
			var fontsize = GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI.fontSize;
			GUILayout.BeginArea (new Rect( Mathf.Max( 20, (Screen.width/2)-(textLine.Length * (fontsize/4))) , (Screen.height/2)+220, Mathf.Min( Screen.width-40, (textLine.Length * (fontsize/2)) ) , 500));
				GUILayout.BeginHorizontal("box");
					GUILayout.Label(textLine, GameObject.FindWithTag("Player").GetComponent(ChatSocket).customGUI );
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		
	}
	
	if (displayTimer > 0) {
		GUI.Label(  Rect (10, 40, 800, 20), "the chatscript IP: "+ chatURL);
		//GUI.Label(  Rect (10, 60, 800, 20), "progress: "+ wwwprogress.ToString()   );
		displayTimer -= Time.deltaTime;
	}
	
	if (showKinectGUI) {
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
				GUILayout.Label ("center point:", GUILayout.Width(80));
				kinectCentering = GUILayout.HorizontalSlider (kinectCentering, -12.0, 12.0);
			GUILayout.EndHorizontal();	
			GUILayout.BeginHorizontal();
				GUILayout.Label ("angle range:", GUILayout.Width(80));
				kinectRange = GUILayout.HorizontalSlider (kinectRange, 1.0, 45.0);
			GUILayout.EndHorizontal();	

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();	
		GUILayout.EndArea();
		
		GUILayout.BeginArea (Rect ((Screen.width/2)+(5),(Screen.height/2)-50,200,600));
			GUILayout.BeginHorizontal("box");	
				GUILayout.BeginVertical();	
					GUILayout.Label ("kinect texture here");				
				GUILayout.EndVertical(); 
				 
			GUILayout.EndHorizontal();	
		GUILayout.EndArea();						
	}


}