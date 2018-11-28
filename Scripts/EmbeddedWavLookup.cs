using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class EmbeddedWavLookup : SpeechProducer	{

	AudioClip scream1;
	AudioClip scream2;
	System.Random rng;

	void Start () {
		print ("Loading test audio.");
		scream1 = Resources.Load ("Wilhelm_Scream") as AudioClip;
		scream2 = Resources.Load ("The_Howie_Long_Scream") as AudioClip;
		rng = new System.Random ();
	}

	void Update () {

	}


	public override void Say (string text){
		AudioClip clip;
		if (rng.NextDouble() < 0.5f) {
			clip = scream1;
		} else {
			clip = scream2;
		}

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
		
}

