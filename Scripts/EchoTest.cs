using System;
using UnityEngine; 
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class EchoTest : MonoBehaviour {
	public Boolean useAutoEntry = true;
	public int inputFieldHeight;
	public float subTitleHeightAdjust = 0.0f;
	public float scoringButtonPositionAdjust = 0.0f;
	public GUIStyle customGUI;
	private string reply = "";
	private string receivedText = "";
	private string lineToSend = "";
	private float displayTimer = 0;
	public static Boolean showReplies = true;
	public static Boolean showInputBox = true;
	public static float endSentenceTime = 200.0f;
	private float endSentenceTimer = 0.0f;
	private string lineCompare = "";
	public Boolean ScoringToggle = false;
	internal Boolean socketReady = false;   
//	WebSocket mySocket;
	public static Boolean readyToConnect = true;
	private Boolean connected = false;
	private string firstName = "";
	private string lastName = "";
	private Boolean error = false;

	public static string Host = "ws://128.146.170.201:27016"; // Websockify server

	public static string PatientChoice = "patient1a";  // Jack Wilson - older white male

	void Start() {
//		try {
//			mySocket = new WebSocket(new Uri(Host)); // Connect socket on startup
//			socketReady = true;
//		}
//		catch (Exception e) {
//			Debug.Log("Socket error: " + e);
//			reply = "Socket error: " + e;
//			error = true;
//		}
	}

	void Update() {
		if (receivedText != "" && receivedText != null) { // if some text has been received... 
			reply = receivedText; // ...set the reply to that text.
			receivedText = "";
			if ( reply.Contains("[") ) {
				string emotion = reply.Substring( reply.IndexOf("[")+1, reply.IndexOf("]")-1  );
				SendMessage("ExpressEmotion", emotion);
				reply = reply.Substring(reply.IndexOf("]")+1) ;
				print(emotion);
			}
			if (reply.Contains("/openCurly/")) {
				connected = false;
				ScoreMe();
			}
			else {
				// tell the mouthcontrol script to do its thing
				SendMessage("BuildShapeList", reply);
				displayTimer = Mathf.Max(7.0f,reply.Length /15 );
				
				if (reply.Contains("no") || reply.Contains("No") || reply.Contains("never") || reply.Contains("didn't")|| reply.Contains("Not") || reply.Contains("Never") || reply.Contains("not") ) SendMessage("NodNo");
				if (reply.Contains("yes") || reply.Contains("I am taking") || reply.Contains("of course")|| reply.Contains("Yes") ) SendMessage("NodYes");
			}
		}

		if (displayTimer > 0 && lineToSend.Length== 0) displayTimer -= Time.deltaTime;
		
		if (lineCompare != lineToSend) {
			lineCompare = lineToSend;
			endSentenceTimer = 0.0f;			
		}
		else endSentenceTimer += Time.deltaTime;

		if (useAutoEntry && endSentenceTimer > endSentenceTime && lineToSend.Length > 0) {
			reply = "";
			writeSocket(lineToSend);
			lineToSend = "";
			endSentenceTimer = 0.0f;	
		}
		
		if (Input.GetKeyDown("up")) inputFieldHeight--;
		if (Input.GetKeyDown("down")) inputFieldHeight++;
	}

	void OnGUI() {
		if (readyToConnect && socketReady) {
			GUILayout.BeginArea (new Rect((Screen.width/2)-150 , (Screen.height/2), 300 , 100));
			GUILayout.BeginVertical("box");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("first name: ");
			firstName = GUILayout.TextField (firstName, 50, GUILayout.Width(200));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("last name: ");
			lastName = GUILayout.TextField (lastName, 50, GUILayout.Width(200));
			GUILayout.EndHorizontal();
			if (lastName.Length > 0 && firstName.Length > 0) {
				if ( GUILayout.Button("Interview the patient", GUILayout.Width(290)) ) {
					lineToSend = "";
					writeSocket(lineToSend);
					readyToConnect = false;
					connected = true;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		// subtitles centered on screen
		if (connected && reply != "" && displayTimer > 0 && showReplies){
			GUILayout.BeginArea( new Rect(Mathf.Max(20,(Screen.width/2) - (customGUI.fontSize * reply.Length/4)),
			                     ((Screen.height -Mathf.Ceil((customGUI.fontSize/2*reply.Length /(Screen.width - 40))+1)* (customGUI.fontSize + 5)- 40)/2)+subTitleHeightAdjust,
			                     Mathf.Min(Screen.width - 40,reply.Length *customGUI.fontSize/2 ), 600));
			GUILayout.BeginHorizontal("box");
			GUILayout.Label(reply, customGUI);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}	
		
		if (error) {
			GUILayout.BeginArea (new Rect( 
			                              Mathf.Max( 20, (Screen.width/2)-(reply.Length * (customGUI.fontSize/4)) ) , 
			                              (Screen.height/2), 
			                              Mathf.Min( Screen.width-40, (reply.Length * (customGUI.fontSize/2)) ) , 
			                              500));
			GUILayout.BeginHorizontal("box");
			GUILayout.Label(reply, customGUI );
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		if (connected) {
			GUILayout.BeginArea (new Rect( 20, inputFieldHeight, Screen.width-40 , 100));			
			GUILayout.BeginHorizontal("box");
			GUI.SetNextControlName("inputField");
			lineToSend = GUILayout.TextField (lineToSend);
			GUILayout.EndHorizontal();
			string str = "\n";
			if (Event.current.type == EventType.keyDown && Event.current.character == str[0] && lineToSend.Length > 0) {
				reply = "";
				writeSocket(lineToSend);
				lineToSend = "";
			}
			GUILayout.EndArea();
			
			if (GUI.GetNameOfFocusedControl() != "inputField") GUI.FocusControl("inputField");
			
			if (ScoringToggle){
				// SCORING STUFF
				GUILayout.BeginArea (new Rect( (Screen.width-scoringButtonPositionAdjust), 60, Screen.width/6 , 100));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Score Me")){
					reply = "";
					lineToSend = "Score Me";
					writeSocket(lineToSend);
					lineToSend = "";
				}
				
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
	}

	IEnumerator writeSocket(string sendLine)
	{
		//yield return StartCoroutine(mySocket.Connect()); // Wait till connection is established

		string message = firstName+"_"+lastName+":"+PatientChoice +"\0\0"+sendLine+"\0";
		print ("Sending: " + sendLine);
		//mySocket.SendString(message);

		receivedText = null;
		while (receivedText == null) {
			receivedText = ""; //mySocket.RecvString();
			yield return 0;
		}

		print ("Received: " + receivedText);
		//mySocket.Close();
	}

	void ScoreMe () {
		StartCoroutine (GetFile ("https://urldefense.proofpoint.com/v2/url?u=http-3A__128.146.170.201_Scores_&d=BQIGAg&c=k9MF1d71ITtkuJx-PdWme51dKbmfPEvxwt8SFEkBfs4&r=plf_0nhsT82-Rnct6ZKJEHLGxgh5eePcapAP3pB2NKs&m=91hnWTeMIutQ7xSVrsnSPGG6zfFxKRrRazmBKDJ9y2Y&s=lGiaSFzXp8D4Y1zR4owDMAKsjRch-h4uF1w86JhfmEA&e= " + firstName + "_" + lastName + ".txt"));
	}

	IEnumerator GetFile (string url) {
		WWW scorefile = new WWW (url);
		yield return scorefile;
		Application.ExternalCall("score", scorefile.text);
	}
}
