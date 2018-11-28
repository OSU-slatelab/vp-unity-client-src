using System;
using UnityEngine; 
using UnityEngine.Events;


[System.Serializable]
public class SpeakingEvent : UnityEvent<float>
{
}

public abstract class SpeechProducer : MonoBehaviour, ISpeaker {


	public SpeakingEvent speaking = new SpeakingEvent();

	public abstract void Say (string text);


}


