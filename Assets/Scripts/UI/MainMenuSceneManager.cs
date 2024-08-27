using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{
    public Animator anim;
    // Detaches the main menu completely by using the deafult scene manager
    public void StartPrologue()
    {
        StartCoroutine(waiter("StartScene"));
    }
    public void StartChapter1()
    {
        StartCoroutine(waiter("CityTop"));
    }
    public void StartChapter2()
    {
        StartCoroutine(waiter("WarehouseEntrance"));
    }
    public void StartCredits()
    {
        StartCoroutine(waiter("Credits"));
    }

    IEnumerator waiter(string scene)
    {
        anim.Play("In");

        //Wait for 2 seconds
        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(scene);
    }

    public void StartHackingGame()
    {
        SceneManager.LoadScene("TutPart1");
    }
}