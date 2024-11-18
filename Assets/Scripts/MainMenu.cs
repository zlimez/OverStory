using System.Collections;
using System.Collections.Generic;
using Abyss.SceneSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void StartButtonOnClick() 
	{
		Debug.Log("Stating New Game...");
		SceneManager.LoadScene(AbyssScene.Lab.ToString());
	}
	
	public void QuitButtonOnClick() 
	{
		Debug.Log("Closing Game...");
		Application.Quit();
	}
}
