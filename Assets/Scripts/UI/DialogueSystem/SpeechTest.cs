using System.Collections;
using System.Collections.Generic;
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
		SpeechManager.Instance.EnqueueDialogue("Hello, world", 3f);
	}
}
