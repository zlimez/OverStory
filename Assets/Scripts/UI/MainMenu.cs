using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void StartButtonOnClick()
	{
		Debug.Log("Stating New Game...");
		SceneManager.LoadScene(Abyss.Settings.Scene.Lab.ToString());
	}

	public void QuitButtonOnClick()
	{
		Debug.Log("Closing Game...");
		Application.Quit();
	}
}
