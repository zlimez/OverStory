using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class SpeechTest : MonoBehaviour
{
	// Start is called before the first frame update
	void Start() 
	{
		Invoke("Say", 2f);
	}

	void Say() 
	{
		RefPair<string, float> args = new("Hello World", 3f);
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, args);
	}
}
