using System;
using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO.Compression;
using System.IO;

public class SpeechListener : MonoBehaviour {
	private AuthenticationToken _authenticationToken;
	private int _recordingRoutine = 0;
	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingBufferSize = 2;
	private int _recordingHZ = 22050;
	private bool _ongoingRecognition = false;
	private bool _save = false;
	private List<float> _utterance = new List<float> ();
	private bool _mute = true;
	private bool _startedMute = false;
	private string _to_send = "";
	private bool _maybe_done = false;
	private float _done_timer = 0f;
	private float _done_threshold = 0.75f;
	private Credentials _credentials;
	private ChatSocket _cs = null;
	private Dictionary<string, object> empty_dict = new Dictionary<string,object> ();
	//public Text ResultsField;

	private SpeechToText _speechToText;

	[Serializable]
	private class CredText
	{
		public string url;
		public string username;
		public string password;
	}

	IEnumerator Start()
	{
		_cs = GetComponent<ChatSocket> ();
		LogSystem.InstallDefaultReactors();
		TextAsset jsonCredText = Resources.Load ("stt_creds") as TextAsset;
		CredText creds = JsonUtility.FromJson<CredText>(jsonCredText.text);

		//  Create credential and instantiate service
		_credentials = new Credentials(creds.username, creds.password, creds.url);
		_speechToText = new SpeechToText(_credentials);

		yield return new WaitForSeconds (1);
		this.Active = true;
		StartRecording();
		Log.Debug("SpeechListener.Start()", "SpeechToText.isListening: {0}", _speechToText.IsListening);
	}


	public void Update(){
		if (_maybe_done) {
			if (_done_timer > _done_threshold) { //indeed done
				this.Mute = true;
				_cs.SendQuery (_to_send);
				_ongoingRecognition = false;
				_save = true;
				_to_send = "";
				_done_timer = 0f;
				_maybe_done = false;
			} else {
				_done_timer += Time.deltaTime;
			}
		}

	}

	public void OnDisable(){
		Log.Debug("SpeechListener.OnDisable()", "called", new object[]{});
		StopRecording ();
		this.Active = false;
		_speechToText = null;
	}

	public void OnEnable() {
		Log.Debug("SpeechListener.OnEnable()", "called", new object[]{});
		if (_credentials != null) {
			_speechToText = new SpeechToText (_credentials);
			this.Mute = true;
			this.Active = true;
			StartRecording ();
		}
	}

	public bool Active
	{
		get { return _speechToText.IsListening; }
		set
		{
			if (value && !_speechToText.IsListening)
			{
				Log.Debug("SpeechListener.Active", "set true", new object[]{});
				_speechToText.DetectSilence = true;
				_speechToText.EnableWordConfidence = true;
				_speechToText.EnableTimestamps = true;
				_speechToText.SilenceThreshold = 0.01f;
				_speechToText.MaxAlternatives = 0;
				_speechToText.EnableInterimResults = true;
				_speechToText.OnError = OnError;
				_speechToText.InactivityTimeout = -1;
				_speechToText.ProfanityFilter = false;
				_speechToText.SmartFormatting = true;
				_speechToText.SpeakerLabels = false;
				_speechToText.WordAlternativesThreshold = null;
				_speechToText.StartListening(OnRecognize, OnRecognizeSpeaker, empty_dict);
			}
			else if (!value && _speechToText.IsListening)
			{
				_speechToText.StopListening();
			}
		}
	}

	public bool Mute
	{
		get { return _mute; }
		set { _mute = value; }
	}
		
//	private void OnGetToken(AuthenticationToken authenticationToken, string customData)
//	{
//		_authenticationToken = authenticationToken;
//		Log.Debug("SpeechListener.OnGetToken()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
//	}

	private void StartRecording()
	{
		Log.Debug("SpeechListener.StartRecording()", "called", new object[]{});
		if (_recordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			_recordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	private void StopRecording()
	{
		Log.Debug("SpeechListener.StopRecording()", "called", new object[]{});
		if (_recordingRoutine != 0)
		{
			Microphone.End(_microphoneID);
			Runnable.Stop(_recordingRoutine);
			_recordingRoutine = 0;
		}
	}

	private void OnError(string error)
	{
		Active = false;

		Log.Debug("SpeechListener", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{
		Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
		_microphoneID = Microphone.devices [0];
		_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
		yield return null;      // let _recordingRoutine get set..

		if (_recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		//bool saveChecked = false;
		int midPoint = _recording.samples / 2;
		float[] samples = null;
		while (_recordingRoutine != 0 && _recording != null)
		{
			int writePos = Microphone.GetPosition(_microphoneID);
			//Log.Debug("ExampleStreaming.RecordingHandler()", "writePos: {0}", writePos);
			if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
			{
				Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

				StopRecording();
				yield break;
			}

			if ((bFirstBlock && writePos >= midPoint)
			    || (!bFirstBlock && writePos < midPoint)) {
				if (!_ongoingRecognition) {
					if (_save) {
						StartCoroutine (ConvertAndSend (_utterance.ToArray (), _recording.frequency, _recording.channels));
						_utterance.Clear ();
						_save = false;
					} 
					//buffer the most recent round of samples
					if (_utterance.Count >= _recording.samples) {
						int end = _utterance.Count - midPoint;
						_utterance.RemoveRange (0, end);
					}
				}
				if (!_mute && !_startedMute) {
					// front block is recorded, make a RecordClip and pass it onto our callback.
					samples = new float[midPoint];
					_recording.GetData (samples, bFirstBlock ? 0 : midPoint);
					_utterance.AddRange (samples);

					AudioData record = new AudioData ();
					record.MaxLevel = Mathf.Max (Mathf.Abs (Mathf.Min (samples)), Mathf.Max (samples));
					record.Clip = AudioClip.Create ("Recording", midPoint, _recording.channels, _recordingHZ, false);
					record.Clip.SetData (samples, 0);
					_speechToText.OnListen (record);
				}
				_startedMute = _mute;
				bFirstBlock = !bFirstBlock;
			} else {
				// calculate the number of samples remaining until we ready for a block of audio, 
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)_recordingHZ;
				yield return new WaitForSeconds (timeRemaining);
			}
		
		}

		yield break;
	}

	private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{
					if (res.final) {
						_cs.ShowRecognitionResult (alt.transcript);
						_maybe_done = true;
						_to_send += alt.transcript;
					} else {
						_maybe_done = false;
						_done_timer = 0f;
						_ongoingRecognition = true;
						_cs.ShowRecognitionResult (alt.transcript);
					}
					string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
					Log.Debug("ExampleStreaming.OnRecognize()", text);
					//ResultsField.text = text;
				}
			}
		}
	}

	private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
	{
		//if (result != null)
		//{
		//	foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
		//	{
		//		Log.Debug("ExampleStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
		//	}
		//}
	}


	IEnumerator ConvertAndSend(float[] samples, int hz, int channels){
		Log.Debug("SpeechListener.ConvertAndSend()", "samples: {0}", samples.Length);
		byte[] file;
		// convert samples to wav format (basically, add headers)
		using (MemoryStream wav = SavWav.CreateWav(samples, hz, channels)){
			yield return wav;
			file = wav.ToArray ();
		}
		// send the file through the chatsocket interface
		_cs.SendAudio(file);
	}
}
