using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ChatSocket : MonoBehaviour {
	// Configuration constants
	public Boolean useAutoEntry = true;
	public int inputFieldVerticalOffset;
	public int inputFieldHeight;
	public int inputOffSet = 2;
	public int inputFieldWidth;
	public int scoreButtonWidth = Screen.width / 6;
	public float subTitleHeightAdjust = 0.0f;
	public float scoringButtonPositionAdjust = 0.0f;
	public GUIStyle customLabelGUI;
	public static Boolean showReplies = false;
	public static Boolean showInputBox = true;
	private bool _voice = true;
	private bool _guiVoice = true;
	public Boolean ScoringToggle = false;
	private static string instructions = "Please interview Mr. Jack Wilson, who is here for back pain.\n" +
	                                     "\n" +
	                                     "You should obtain a complete medical history, including History of Present Illness, " +
	                                     "Past Medical History, Family History, and Social History. The interview should take 10-15 minutes.\n\n" +
	                                     "This app uses speech recognition and generation, so you can simply ask your questions, and Mr. Wilson " +
	                                     "will respond. Because of this, you should use the app in a quiet environment, or with a headset. Mr. Wilson " +
	                                     "is also likely to have difficulty understanding questions interrupted by long pauses.\n\n" +
	                                     "By using this app, you consent to have your interactions with the patient recorded for research purposes. " +
	                                     "No personally identifying information is captured.";


	// WEBSERVICE CONSTANTS
	private static string clientID = "iOS-1.1";
	private static string backendRootURL = "https://boulder.cse.ohio-state.edu/";
	//private static string backendRootURL = "http://127.0.0.1:5000/";

	//STATE VARIABLES
	// - old auto-entry 
	public static float endSentenceTime = 200.0f;
	private float endSentenceTimer = 0.0f;
	private string lineCompare = "";
	private float displayTimer = 0;

	// - comms
	private string reply = "";
	private string lineToSend = "";
	private string firstName = "";
	private string lastName = "";
	private int conversationNum = -1;
	private string _uuid = "";
	private string _json = "";
	private string _audio_json = "";
	private string receivedText = "";
	private Queue<string> _queryResources = new Queue<string> ();
	private Queue<byte[]> _audioFiles = new Queue<byte[]> ();

	// - process flow
	public static Boolean readyToConnect = true;
	private Boolean connected = false;
	private bool _conversationStarted = false;
	private bool _game_over = false;
	private Boolean error = false;
	private bool _checkListening = false;
	private bool _muting = false;
	private float _muteTimer = 0.0f;

	// Component references
	private SpeechSynthesizer _synth = null;
	private SpeechListener _listener = null;


	// NEWER VARIABLES FOR USE WITH WEBSOCKETS
	//internal Boolean socketReady = false;   
	//WebSocket mySocket;


	// SERVERS HOSTING CHATSCRIPT - (Port designations no longer required for Unity Web plugin versions, still required for WebGL versions)
	
	// public static string Host = "128.146.170.201"; // Dedicated Server
	// public static string Host = "128.146.170.200"; // Harmony Server - Jim Wilkins  / Cred: DougDanforth, D0uglas6557
	// public static string Host = "128.146.170.194"; // Old AI Server - Jack Wilson
	// public static string Host = "128.146.170.198"; // JIBE Server
	// public static string Host = "128.146.170.195"; // TEMP SERVER
	// public static string Host = "ws://52.24.145.185:27016"; // Amazon Web Services Server

	public static string Host = "ws://128.146.170.201:27016"; // Local Development Server Running Websockify

	// public static string Host = "ws://127.0.0.1:27016"; // LocalHost Test
	// public static string Host = "ws://10.98.8.20:27016"; // DMZ Server

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

	public void ShowRecognitionResult(string recognized){
		lineToSend = Regex.Replace(recognized, @"\b\S+\b", "\u2022");
	}

	public void SendQuery(string query){
		reply = "";
		StartCoroutine (postQuery (query));
		return;
	}

	public void SendAudio(byte[] compressedBytes){
		_audioFiles.Enqueue (compressedBytes);
	}

	public bool IsConversationStarted(){
		return _conversationStarted;
	}


	//  NEW START FUNCTION FOR THE WEBSOCKIFY VERSION

	IEnumerator Start() {
		_synth = GetComponent<SpeechSynthesizer>();
		_listener = GetComponent<SpeechListener> ();
		_synth.speaking.AddListener (MuteListener);
		yield return new WaitForSeconds (2);
		firstName = "iOSuser";
		lastName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
		try {
			//mySocket = new WebSocket(new Uri(Host)); // Connect socket on startup

			//mySocket = new WebSocket(new Uri(Host),['binary', 'base64']); // Base template
			//mySocket = new WebSocket("ws://localhost:27016", ["base64"]); // Tried this
			//mySocket = new WebSocket(new Uri(Host), ("binary")); // Tried This - says WebSocket constructor does not take two arguments
				

			//socketReady = true;
		}
		catch (Exception e) {
			Debug.Log("Socket error: " + e);
			reply = "Socket error: " + e;
			error = true;
		}
	}
	
	void Update() {
		if (!_checkListening && connected) {
			StartCoroutine (CheckListening());
		}
		if (_muting) {
			if (_muteTimer > 0) {
				_muteTimer -= Time.deltaTime;
			} else {
				_listener.Mute = false;
				_muting = false;
			}
		}
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
				//ScoreMe();
			}
			else {
				// tell the mouthcontrol script to do its thing
				print ("Building mouth shape list...");
				SendMessage("BuildShapeList", reply);
				displayTimer = Mathf.Max(7.0f,reply.Length /15 );
				//Application.ExternalCall("speak", reply);

				//NOTE: we mute recognition using an event callback (MuteListener)
				_synth.Synthesize(reply);
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
			StartCoroutine(postQuery(lineToSend));
			//lineToSend = "";
			endSentenceTimer = 0.0f;	
		}

		if (_voice && _audioFiles.Count > 0 && _queryResources.Count > 0) {
			//assuming (bigly) that audio files and queries come in in the same order.
			print("Prepping postAudio");
			string queryURL = _queryResources.Dequeue();
			string[] toks = queryURL.Split (new Char[]{'/'});
			string convNum = toks [toks.Length - 4];
			string qNum = toks [toks.Length - 2];
			string filename = convNum + "-" + qNum + ".wav";
			string audioURL =  backendRootURL.TrimEnd(new Char[]{'/'}) + queryURL + "audio";
			StartCoroutine(postAudio(audioURL, _audioFiles.Dequeue(), filename));
		}

		if (Input.GetKeyDown("up")) inputFieldVerticalOffset--;
		if (Input.GetKeyDown("down")) inputFieldVerticalOffset++;
	}
	
	void OnGUI() {
		//if (readyToConnect && socketReady){
		if (readyToConnect){
			GUI.skin.textField.fontSize = 28;
			GUI.skin.button.fontSize = 28;
			GUI.skin.label.fontSize = 28;
			int boxwidth = 600;
			int boxheight = 800;
			int labelwidth = 150;
			GUILayout.BeginArea (new Rect((Screen.width/2)-(boxwidth/2) , (Screen.height/4), boxwidth , boxheight));
			GUILayout.BeginVertical("box");
// Introductory text
			GUILayout.BeginHorizontal();
			GUILayout.Label (instructions);
			GUILayout.EndHorizontal ();

// Name capture
/*			GUILayout.BeginHorizontal();
			GUILayout.Label("first name: ");
			GUILayoutOption[] textopts = new GUILayoutOption[2];
			textopts [0] = GUILayout.Width (boxwidth - labelwidth - 10);
			textopts [1] = GUILayout.Height (boxheight / 3 - 5);
			firstName = GUILayout.TextField (firstName, labelwidth, textopts);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("last name: ");
			lastName = GUILayout.TextField (lastName, labelwidth, textopts);
			GUILayout.EndHorizontal(); */
//			GUILayout.BeginHorizontal ();
//			_guiVoice = GUILayout.Toggle (_guiVoice, "Voice?");
//			GUILayout.EndHorizontal ();
			GUILayoutOption[] buttonopts = new GUILayoutOption[2];
			buttonopts [0] = GUILayout.Width (boxwidth - 10);
			buttonopts [1] = GUILayout.Height (50);
			if (lastName.Length > 0 && firstName.Length > 0) {
				if ( GUILayout.Button("Interview the patient", buttonopts)) {
					// clean names to facilitate pdf generation.
					firstName = firstName.Trim ();
					lastName = lastName.Trim ();
					print ("Name recorded...");
					lineToSend = "";
					StartCoroutine(startConversation(firstName, lastName, clientID, "far", _guiVoice));
					readyToConnect = false;
					connected = true;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		// subtitles centered on screen
		if (connected && reply != "" && displayTimer > 0 && showReplies){
			GUILayout.BeginArea( new Rect(Mathf.Max(20,(Screen.width/2) - (GUI.skin.label.fontSize * reply.Length/4)),
				((Screen.height -Mathf.Ceil((GUI.skin.label.fontSize/2*reply.Length /(Screen.width - 40))+1)* (GUI.skin.label.fontSize + 5)- 40)/2)+subTitleHeightAdjust,
				Mathf.Min(Screen.width - 20,reply.Length *GUI.skin.label.fontSize/2 ), 600));
			GUILayout.BeginHorizontal("box");
			GUILayout.Label(reply, customLabelGUI);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}	
		
		if (error) {
			GUILayout.BeginArea (new Rect( 
				Mathf.Max( 20, (Screen.width/2)-(reply.Length * (GUI.skin.label.fontSize/4)) ) , 
			                              (Screen.height/2), 
				Mathf.Min( Screen.width-40, (reply.Length * (GUI.skin.label.fontSize/2)) ) , 
			                              500));
			GUILayout.BeginHorizontal("box");
			GUILayout.Label(reply );
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		if (connected && !_game_over) {
			GUILayout.BeginArea (new Rect( (inputOffSet), inputFieldVerticalOffset, (Screen.width-inputOffSet*2),inputFieldHeight));			
			GUILayout.BeginHorizontal("box");
			GUI.SetNextControlName("inputField");
			lineToSend = GUILayout.TextField (lineToSend);
			GUILayout.EndHorizontal();
			string str = "\n";
			if (Event.current.type == EventType.KeyDown && Event.current.character == str[0] && lineToSend.Length > 0) {
				reply = "";
				StartCoroutine(postQuery(lineToSend));
			}
			GUILayout.EndArea();
			
			if (GUI.GetNameOfFocusedControl() != "inputField") GUI.FocusControl("inputField");
			
			if (ScoringToggle){
				// SCORING STUFF
				GUILayout.BeginArea (new Rect( Screen.width - inputOffSet - scoreButtonWidth, inputFieldHeight + inputFieldVerticalOffset, scoreButtonWidth , inputFieldHeight));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Score Me")){
					reply = "";
					StartCoroutine(postQuery("score me"));
					GameOver ();
				}
				
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
		if (_game_over) {
			int boxwidth = 500;
			int boxheight = 250;
			int labelwidth = 150;
			GUILayout.BeginArea (new Rect((Screen.width/2)-(boxwidth/2) , (Screen.height/2), boxwidth , boxheight));
			GUILayout.BeginVertical("box");

//			GUILayout.BeginHorizontal();
//			GUILayout.Label("Would you like to interview the patient again?", customLabelGUI);
//			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayoutOption[] buttonopts = new GUILayoutOption[2];
			buttonopts [0] = GUILayout.Width (boxwidth - 10);
			buttonopts [1] = GUILayout.Height (boxheight / 4 - 5);

			if ( GUILayout.Button("Get Summary Report", buttonopts)) {
				Application.OpenURL(backendRootURL+"score/?convo_num="+conversationNum.ToString()+"&secret="+_uuid);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if ( GUILayout.Button("Get Expert Answers", buttonopts)) {
				Application.OpenURL("http://128.146.170.201/Downloads/ExpertAnswers.pdf");
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if ( GUILayout.Button("Restart", buttonopts)) {
				Restart ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			buttonopts [0] = GUILayout.Width (boxwidth - 10);
			buttonopts [1] = GUILayout.Height (boxheight / 4 - 5);

			if ( GUILayout.Button("Quit", buttonopts)) {
				Application.Quit();
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical();
			GUILayout.EndArea();

		}
	}

	private IEnumerator CheckListening(){
		_checkListening = true;
		if (_listener != null && _listener.enabled && !_listener.Active) {
			_listener.Active = true;
		}
		yield return new WaitForSeconds (1);
		_checkListening = false;
	}

/*	private IEnumerator LaunchScorePage(){
		yield return new WaitForSeconds(1);
		Application.OpenURL(backendRootURL+"score/?convo_num="+conversationNum.ToString()+"&secret="+_uuid);
		GameOver ();

	}
*/
	private void Restart(){
		Time.timeScale = 1;
		_listener.enabled = true;
		_synth.enabled = true;
		_game_over = false;
		readyToConnect = true;
		conversationNum = -1;
		_uuid = "";
//		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void MuteListener(float duration){
		_listener.Mute = true;
		_muting = true;
		_muteTimer = Math.Max (_muteTimer, duration);
	}

/*	private IEnumerator MuteRecognition(float duration){
		_listener.Mute = true;
		yield return new WaitForSeconds (duration);
		_listener.Mute = false;
		print ("Un-muted.");
	}
*/
	private void GameOver(){
		_listener.enabled = false;
		_synth.enabled = false;
		Time.timeScale = 0;
		_game_over = true;
		connected = false;
	}

	private IEnumerator PostJSON (string url, byte[] bytes){
		using (UnityWebRequest request = new UnityWebRequest (url)) {
			request.uploadHandler = new UploadHandlerRaw (bytes);
			request.downloadHandler = new DownloadHandlerBuffer ();
			request.chunkedTransfer = false;
			request.method = UnityWebRequest.kHttpVerbPOST;
			request.SetRequestHeader ("Content-type", "application/json");
			yield return request.SendWebRequest ();
			if (request.isNetworkError || request.isHttpError) {
				print (request.error);
			} else {
				_json = request.downloadHandler.text;
			}
		}


	}

	private IEnumerator PostForm (string url, WWWForm form){
		using (UnityWebRequest request = new UnityWebRequest(url)) {
			request.uploadHandler = new UploadHandlerRaw (form.data);
			request.downloadHandler = new DownloadHandlerBuffer ();
			request.method = UnityWebRequest.kHttpVerbPOST;
			foreach (string key in form.headers.Keys) {
				request.SetRequestHeader (key, form.headers [key]);
			}
			request.chunkedTransfer = false;
			yield return request.SendWebRequest ();
			if (request.isNetworkError || request.isHttpError) {
				print (request.error);
			} else {
				_audio_json = request.downloadHandler.text;
			}
		}


	}


	/*
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
	*/

	[Serializable]
	private class QueryOut
	{
		public string query;
	}

	[Serializable]
	private class QueryIn
	{
		public string status;
		public string resource;
		public string reply;
	}

	IEnumerator postQuery(string sendLine)
	{
		QueryOut parms = new QueryOut ();
		sendLine = sendLine.Replace ("%HESITATION", "");
		parms.query = sendLine;
		byte[] bytes = Encoding.UTF8.GetBytes (JsonUtility.ToJson(parms));
		//Dictionary<String, String> headers = new Dictionary<String, String>();
		//headers.Add ("Content-type", "application/json");
		string url = backendRootURL + "conversations/" + conversationNum.ToString() + "/query/";
		print ("Sending \"" + sendLine + "\"");
		//WWW response = new WWW(url, bytes, headers);
		//yield return response;
		yield return PostJSON(url, bytes);
		QueryIn reply = JsonUtility.FromJson<QueryIn>(_json);
		_json = "";
		receivedText = reply.reply;
		if (_voice) {
			_queryResources.Enqueue (reply.resource);			
		}
		lineToSend = "";
	}

	[Serializable]
	private class AudioReceipt{
		string status;
		string resource;
		string info;
	}

	IEnumerator postAudio(string url, byte[] file, string filename){
		print ("postAudio called.");
		WWWForm form = new WWWForm ();
		form.AddBinaryData ("file", file, filename);
		//WWW response = new WWW (url, form);
		yield return PostForm(url, form);
		AudioReceipt reply = JsonUtility.FromJson<AudioReceipt> (_audio_json);
		_audio_json = "";
		yield break;
	}

	[Serializable]
	private class NewConvOut
	{
		public string client; //currently limited to 8 bytes; that may be dumb.
		public string first;
		public string last;
		public int patient;
		public string input;
		public string mic;
	}

	[Serializable]
	private class NewConvIn
	{
		public string status;
		public string resource;
		public int conversation_num;
		public string greeting;
		public string uuid;
	}

	IEnumerator startConversation(string first, string last, string client, string mic, bool voice)
	{
		if (voice) {
			_voice = true;
		}
		NewConvOut parms = new NewConvOut ();
		parms.first = first;
		parms.last = last;
		parms.client = client;
		parms.mic = mic;
		parms.input = (voice ? "voice" : "text");
		parms.patient = 2;
		print (JsonUtility.ToJson (parms));
		byte[] bytes = Encoding.UTF8.GetBytes (JsonUtility.ToJson(parms));
//		Dictionary<String, String> headers = new Dictionary<String, String>();
//		headers.Add ("Content-type", "application/json");
		string url = backendRootURL + "conversations/";
		print ("Creating conversation...");
//		WWW response = new WWW(url, bytes, headers);
//		yield return response;
		yield return PostJSON(url, bytes);
		NewConvIn reply = JsonUtility.FromJson<NewConvIn>(_json);
		_json = "";
		//TODO handle case where backend is down!
		conversationNum = reply.conversation_num;
		_uuid = reply.uuid;
		receivedText = reply.greeting;
		_conversationStarted = true;
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
