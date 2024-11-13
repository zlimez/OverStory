using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class MovementTutorial : MonoBehaviour
{
	// Statements
	[SerializeField] private RefPair<string, float> moveHorizontalSpeech = new("Press 'a'/'d' to move Left/Right", 4f);
	[SerializeField] private RefPair<string, float> jumpSpeech = new("Press 'w' or 'space' to Jump", 3.5f);
	[SerializeField] private RefPair<string, float> runSpeech = new("Press 'left ctrl' or 'left alt' while moving to Run", 5f);
	[SerializeField] private RefPair<string, float> dashSpeech = new("Press 'left shift' to Dash", 5f);
	
	// Start is called before the first frame update
	void Start()
	{
		Invoke("SayInstructions", 3f);
	}

	private void SayInstructions() 
	{
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, moveHorizontalSpeech);
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, jumpSpeech);
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, runSpeech);
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, dashSpeech);
	}
}
