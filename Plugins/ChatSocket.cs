using System;
using UnityEngine; 
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatSocket : MonoBehaviour {
	public Boolean useAutoEntry = true;
	public int inputFieldHeight;
	public int inputOffSet = 2;
	public int inputFieldWidth;
	public float subTitleHeightAdjust = 0.0f;
	public float scoringButtonPositionAdjust = 0.0f;
	public GUIStyle customGUI;
	private string reply = "";
	private string lineToSend = "";
	private float displayTimer = 0;
	public static Boolean showReplies = true;
	public static Boolean showInputBox = true;
	public static float endSentenceTime = 200.0f;
	private float endSentenceTimer = 0.0f;
	private string lineCompare = "";
	public Boolean ScoringToggle = false;
	public static Boolean readyToConnect = true;
	private Boolean connected = false;
	private string firstName = "";
	private string lastName = "";
	
	// NEWER VARIABLES FOR USE WITH WEBSOCKETS
	private string receivedText = "";
	internal Boolean socketReady = false;   
	WebSocket mySocket;
	private Boolean error = false;

	// SERVERS HOSTING CHATSCRIPT - (Port designations no longer required for Unity Web plugin versions, still required for WebGL versions)
	
	// public static string Host = "128.146.170.201"; // Dedicated Server
	// public static string Host = "128.146.170.200"; // Harmony Server - Jim Wilkins  / Cred: DougDanforth, D0uglas6557
	// public static string Host = "128.146.170.194"; // Old AI Server - Jack Wilson
	// public static string Host = "128.146.170.198"; // JIBE Server
	// public static string Host = "128.146.170.195"; // TEMP SERVER
	// public static string Host = "ws://52.24.145.185:27016"; // Amazon Web Services Server

	// public static string Host = "ws://128.146.170.201:27016"; // Local Development Server Running Websockify

	public static string Host = "ws://127.0.0.1:27016"; // LocalHost Test
	//public static string Host = "ws://10.98.8.20:27016"; // DMZ Server

	// PATIENT ID'S

	// public static string PatientChoice = "patient1";   // Jim Wilkins (Back Pain) - older white male
	// public static string PatientChoice = "patient2";   // Jack Wilson (Back Pain) - older white male
	// public static string PatientChoice = "patient3";   // Josh Coulson (Back Pain) - younger white male 
	 public static string PatientChoice = "patient4";   // Melissa Phillips (Abdominal Pain)  - younger white female 
	// public static string PatientChoice = "patient5";   // Greta Schmidt (Vaginal Discharge) - older white female 
	// public static string PatientChoice = "patient6";   // Carmen Flores (Headache) - younger latin female 
	// public static string PatientChoice = "patient7";   // Jasmine Zhang (Abdominal pain) - younger asian female 
	// public static string PatientChoice = "patient8";   // Marcus Robertson (Painful urination) - younger black male 
	// public static string PatientChoice = "patient9";   // Harold Washington (dizziness) - older black male 
	// public static string PatientChoice = "patient10";  // Alexander Spiros (sinus infection) - younger white male
	// public static string PatientChoice = "patient11";  // Arlinda Ashwell(Shortness of Breath) - 60YO white female
	// public static string PatientChoice = "patient12";  // Kathleen Michael(Depression) - 54YO black female
	// public static string PatientChoice = "patient13";  // Candy Carruthers (Joint Pain) - 70YO white female
	// public static string PatientChoice = "patient14";  // Paul Cannardley (Urination Difficulty) - 70YO white male
	// public static string PatientChoice = "patient15";  // Ann Tomlin (painful periods) - 33YO asian female
	// public static string PatientChoice = "patient16";  // Cesar Rodriguez (abdominal pain) - 39YO short, thin latin male
	// public static string PatientChoice = "patient17";  // Mr. Smith (left leg swelling) - 53YO overweight, gray bearded white male
	// public static string PatientChoice = "patient18";  // Jacob Farber (dull low back pain) - 55YO thin, tall balding white male
	//public static string PatientChoice = "patient20";    // Vanessa Watson (prenatal counseling)


	// Tab Navigator for UI
	// Single instance of this script per GUI
	// An alternative would be to use a next/previous setting on a single GUI item, which would mean one script per InputField - not ideal

	public class tabBehaviour : MonoBehaviour
	{
		private EventSystem system;

		private void Start()
		{
			system = EventSystem.current;
		}

		private void Update()
		{
			if (system.currentSelectedGameObject == null || !Input.GetKeyDown (KeyCode.Tab))
				return;

			Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
			if (current == null)
				return;

			bool up = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
			Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

			// We are at the end or the beginning, go to either, depends on the direction we are tabbing in
			// The previous version would take the logical 0 selector, which would be the highest up in your editor hierarchy
			// But not certainly the first item on your GUI, or last for that matter
			// This code tabs in the correct visual order
			if (next == null)
			{
				next = current;

				Selectable pnext;
				if(up) while((pnext = next.FindSelectableOnDown()) != null) next = pnext;
				else while((pnext = next.FindSelectableOnUp()) != null) next = pnext;
			}

			// Simulate Inputfield MouseClick
			InputField inputfield = next.GetComponent<InputField>();
			if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));

			// Select the next item in the taborder of our direction
			system.SetSelectedGameObject(next.gameObject);
		}
	}



	//  NEW START FUNCTION FOR THE WEBSOCKIFY VERSION

	void Start() {
		try {
			mySocket = new WebSocket(new Uri(Host)); // Connect socket on startup

			//mySocket = new WebSocket(new Uri(Host),['binary', 'base64']); // Base template
			//mySocket = new WebSocket("ws://localhost:27016", ["base64"]); // Tried this
			//mySocket = new WebSocket(new Uri(Host), ("binary")); // Tried This - says WebSocket constructor does not take two arguments
				

			socketReady = true;
		}
		catch (Exception e) {
			Debug.Log("Socket error: " + e);
			reply = "Socket error: " + e;
			error = true;
		}
	}
	
	void Update() {
		if (receivedText != "" && receivedText != null) { // if some text has been received... 
			print ("Received: " + receivedText);
			reply = receivedText; // ...set the reply to that text.
			receivedText = "";
			if ( reply.Contains("[") ) {
				string emotion = reply.Substring( reply.IndexOf("[")+1, reply.IndexOf("]")-1  );
				SendMessage("ExpressEmotion", emotion);
				reply = reply.Substring(reply.IndexOf("]")+1) ;
			}
			if (reply.Contains("/openCurly/")) {
				connected = false;
				ScoreMe();
			}
			else {
				// tell the mouthcontrol script to do its thing
				print ("Building mouth shape list...");
				SendMessage("BuildShapeList", reply);
				displayTimer = Mathf.Max(7.0f,reply.Length /15 );
				//Application.ExternalCall("speak", reply);
				
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
			StartCoroutine(writeSocket(lineToSend));
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
					print ("Name recorded...");
					lineToSend = "";
					StartCoroutine(writeSocket(lineToSend));
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
			                              Mathf.Min(Screen.width - 20,reply.Length *customGUI.fontSize/2 ), 600));
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
			GUILayout.BeginArea (new Rect( (inputOffSet), inputFieldHeight, (Screen.width-inputOffSet*2),100));			
			GUILayout.BeginHorizontal("box");
			GUI.SetNextControlName("inputField");
			lineToSend = GUILayout.TextField (lineToSend);
			GUILayout.EndHorizontal();
			string str = "\n";
			if (Event.current.type == EventType.keyDown && Event.current.character == str[0] && lineToSend.Length > 0) {
				reply = "";
				StartCoroutine(writeSocket(lineToSend));
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
					lineToSend = "score me";
					StartCoroutine(writeSocket(lineToSend));
					lineToSend = "";
				}
				
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
	}
	
	IEnumerator writeSocket(string sendLine)
	{
		print ("Connecting...");
		yield return StartCoroutine(mySocket.Connect()); // Wait till connection is established
		
		string message = firstName+"_"+lastName+":"+PatientChoice +"\0\0"+sendLine+"\0";
		print ("Sending: " + sendLine);
		mySocket.SendString(message);
		
		receivedText = null;
		while (receivedText == null) {
			receivedText = mySocket.RecvString();
			yield return 0;
		}
		mySocket.Close();
	}
	
	void ScoreMe () {
		//StartCoroutine (GetFile ("https://urldefense.proofpoint.com/v2/url?u=http-3A__128.146.170.201_Scores_&d=BQIGAg&c=k9MF1d71ITtkuJx-PdWme51dKbmfPEvxwt8SFEkBfs4&r=plf_0nhsT82-Rnct6ZKJEHLGxgh5eePcapAP3pB2NKs&m=91hnWTeMIutQ7xSVrsnSPGG6zfFxKRrRazmBKDJ9y2Y&s=lGiaSFzXp8D4Y1zR4owDMAKsjRch-h4uF1w86JhfmEA&e= " + firstName + "_" + lastName + ".txt"));
		StartCoroutine (GetFile ("http://127.0.0.1/Scores/" + firstName + "_" + lastName + ".txt"));

	}
	
	IEnumerator GetFile (string url) {
		WWW scorefile = new WWW (url);
		yield return scorefile;
		Application.ExternalCall("score", scorefile.text);
	}
}
