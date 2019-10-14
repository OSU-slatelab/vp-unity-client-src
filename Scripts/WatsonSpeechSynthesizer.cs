using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Events;

using IBM.Watson.TextToSpeech.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;

public class WatsonSpeechSynthesizer : SpeechProducer {

	private AuthenticationToken _authenticationToken;
	private TextToSpeechService _textToSpeech;
    private string serviceVoice = null;
	private bool _done_loading = false;

	public override bool loaded {
		get {
			return _done_loading;
		}
	}


	[Serializable]
	private class CredText
	{
		public string url;
		public string username;
		public string password;
	}

	IEnumerator Start () {
        yield return voice;
        if (voice == "michaelV3") {
            serviceVoice = "en-US_MichaelV3Voice";
        } else if (voice == "michael") {
            serviceVoice = "en-US_MichaelVoice";
        }
        TextAsset jsonCredText = Resources.Load ("tts_creds") as TextAsset;
		CredText creds = JsonUtility.FromJson<CredText>(jsonCredText.text);

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(creds.username, creds.password, creds.url);
		_textToSpeech = new TextToSpeechService(credentials);
		//_textToSpeech.Voice = VoiceType.en_US_Michael;
		_done_loading = true;
	}
	
	void Update () {
		
	}

	public override void Say (string text){
   		text = SecurityElement.Escape (text);
		//text = "<voice-transformation type=\"Custom\" glottal_tension=\"+80%\" breathiness=\"-80%\" pitch=\"-100%\" pitch_range=\"-25%\">" + text;
		//text += "</voice-transformation>";
		print (text);
		if(!_textToSpeech.Synthesize(callback: OnSynthesize, text: text, voice: serviceVoice, accept: "audio/wav"))
			Log.Debug("SpeechSynthesizer.Synthesize()", "Failed to synthesize!");
	}

	private void OnGetToken(AuthenticationToken authenticationToken, string customData)
	{
		_authenticationToken = authenticationToken;
		Log.Debug("SpeechSynthesizer.OnGetToken()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
	}

	private void OnSynthesize(DetailedResponse<byte[]> response, IBMError error)
	{
        byte[] audioData = response.Result;
        AudioClip clip = WaveFile.ParseWAV("speech", audioData);
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

//	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
//	{
//		Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
//	}

}
