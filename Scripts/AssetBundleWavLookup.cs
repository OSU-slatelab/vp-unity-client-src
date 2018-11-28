using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class AssetBundleWavLookup : SpeechProducer	{

	private AssetBundle _bundle = null;
	private IDictionary<string, List<string>> _map = null;
	System.Random rng;



	IEnumerator Start () {
		UnityWebRequest www = UnityWebRequest.GetAssetBundle("https://boulder.cse.ohio-state.edu/static/testaudio");
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			_bundle = DownloadHandlerAssetBundle.GetContent(www);
		}
		TextAsset mapJson = _bundle.LoadAsset ("answer_sound_map") as TextAsset;
//		_map = (IDictionary<string, IList<string>>) JsonConvert.DeserializeObject (mapJson.text, typeof(Dictionary < string, List<string>>));
		_map = JsonConvert.DeserializeObject<Dictionary < string, List<string>>> (mapJson.text);
		rng = new System.Random ();
	}

	void Update () {

	}


	public override void Say (string text){
		text = NormalizeText (text);
		AudioClip clip;
		string audioResource;
		int choice;

		try
		{
			choice = rng.Next (_map [text].Count);
			print(choice);
			audioResource = _map [text][choice];
			print(audioResource);
		} catch (KeyNotFoundException e){
			Debug.Log ("Key not found: " + text);
			choice = rng.Next (_map ["default"].Count);
			audioResource = _map ["default"] [choice];
		}

		clip = _bundle.LoadAsset (audioResource) as AudioClip;

		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;

			speaking.Invoke (clip.length + 0.25f);

			source.Play();

			Destroy(audioObject, clip.length);
		}

	}

	private string NormalizeText(string text){
		print (text);
		text = text.Trim ();
		print (text);
		text = text.Replace(".", null).Replace(",", null).Replace("?",null);
		print (text);
		text = text.ToLower ();
		print (text);
		string[] toks = text.Split (new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
		text = String.Join (" ", toks);
		print (text);
		return text;
	}
		
}

