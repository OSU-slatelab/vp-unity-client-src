using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Events;

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;

public class WatsonSpeechSynthesizer : SpeechProducer {

	private AuthenticationToken _authenticationToken;
	private TextToSpeech _textToSpeech;

	[Serializable]
	private class CredText
	{
		public string url;
		public string username;
		public string password;
	}

	void Start () {
		TextAsset jsonCredText = Resources.Load ("tts_creds") as TextAsset;
		CredText creds = JsonUtility.FromJson<CredText>(jsonCredText.text);

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(creds.username, creds.password, creds.url);
		_textToSpeech = new TextToSpeech(credentials);
		_textToSpeech.Voice = VoiceType.en_US_Michael;
	
	}
	
	void Update () {
		
	}

	public override void Say (string text){

		Synthesize (text);

	}

	private void Synthesize (string text){
		text = SecurityElement.Escape (text);
		text = "<voice-transformation type=\"Custom\" glottal_tension=\"+80%\" breathiness=\"-80%\" pitch=\"-100%\" pitch_range=\"-25%\">" + text;
		text += "</voice-transformation>";
		print (text);
		if(!_textToSpeech.ToSpeech(OnSynthesize, OnFail, text, true))
			Log.Debug("SpeechSynthesizer.Synthesize()", "Failed to synthesize!");
	}

	private void OnGetToken(AuthenticationToken authenticationToken, string customData)
	{
		_authenticationToken = authenticationToken;
		Log.Debug("SpeechSynthesizer.OnGetToken()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
	}

	private void OnSynthesize(AudioClip clip, Dictionary<string, object> customData)
	{
		speaking.Invoke (clip.length + 0.25f);
		Log.Debug("SpeechSynthesizer.OnSynthesize()", " called.");
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			Destroy(audioObject, clip.length);
		}
	}

	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
	}

}
