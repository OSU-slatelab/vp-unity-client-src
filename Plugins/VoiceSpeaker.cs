// Voice Speaker  //
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class VoiceSpeaker : MonoBehaviour 
{
	[DllImport ("Voice_speaker.dll", EntryPoint="VoiceAvailable")] private static extern int    VoiceAvailable();
	[DllImport ("Voice_speaker.dll", EntryPoint="InitVoice")]      private static extern void   InitVoice();
	[DllImport ("Voice_speaker.dll", EntryPoint="WaitUntilDone")]  private static extern int    WaitUntilDone(int millisec);
	[DllImport ("Voice_speaker.dll", EntryPoint="FreeVoice")]      private static extern void   FreeVoice();
	[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceCount")]  public static extern int    GetVoiceCount();

	// Unity V4.x.x
	[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceName")]   private static extern IntPtr GetVoiceName(int index);
	
	//  other Unity version	
	//[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceName")]   private static extern string GetVoiceName(int index);
	
	[DllImport ("Voice_speaker.dll", EntryPoint="SetVoice")]       public static extern void   SetVoice(int index);
	[DllImport ("Voice_speaker.dll", EntryPoint="Say")]            public static extern void   Say(string ttospeak);
	[DllImport ("Voice_speaker.dll", EntryPoint="SayAndWait")]     private static extern void   SayAndWait(string ttospeak);
	[DllImport ("Voice_speaker.dll", EntryPoint="SpeakToFile")]    private static extern int    SpeakToFile(string filename, string ttospeak);
	[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceState")]  private static extern int    GetVoiceState();
	[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceVolume")] private static extern int    GetVoiceVolume();
	[DllImport ("Voice_speaker.dll", EntryPoint="SetVoiceVolume")] private static extern void   SetVoiceVolume(int volume);
	[DllImport ("Voice_speaker.dll", EntryPoint="GetVoiceRate")]   private static extern int    GetVoiceRate();
	[DllImport ("Voice_speaker.dll", EntryPoint="SetVoiceRate")]   private static extern void   SetVoiceRate(int rate);
	[DllImport ("Voice_speaker.dll", EntryPoint="PauseVoice")]     private static extern void   PauseVoice();
	[DllImport ("Voice_speaker.dll", EntryPoint="ResumeVoice")]    private static extern void   ResumeVoice();

	public int voice_nb = 0; 
	public int voiceRate = 0;
	private int curVoiceRate = 0;

	void Start ()
	{
        if( VoiceAvailable()>0 )
        {
            InitVoice(); // init the engine

			if (voice_nb > GetVoiceCount()) voice_nb = 0; //NEW STUFF
			if (voice_nb < 0) voice_nb = 0; //NEW STUFF

			//Unity V4.x.x *******************************************
			IntPtr pStr = GetVoiceName(voice_nb);
			string str = Marshal.PtrToStringAnsi(pStr);	
			Debug.Log ("Voice name : "+str); // Voice Name
			
			 //Unity V4.x.x *******************************************
			Debug.Log ("Voice name : "+GetVoiceName(voice_nb)); // Voice Name other Unity version
		
			Debug.Log ("Number of voice : "+GetVoiceCount()); // Number of voice
			SetVoice(voice_nb); // 0 to voiceCount - 1
			SetVoiceRate(voiceRate);

			//Say("Hello. I can speak now. My name is "+GetVoiceName(voice_nb)+". Welcome to Unity");
			//Say("Hello.");
        }
	}
	
	void Update ()
	{
		if (voiceRate != curVoiceRate) {
			SetVoiceRate(voiceRate);
			curVoiceRate = voiceRate;
		}
	}
	
	void SpeakThis (String toSay)
	{
		Say(toSay);
	}
	
	
    void OnDisable()
	{ 
        if( VoiceAvailable()>0 )
        {
            FreeVoice();
        }
    } 
}