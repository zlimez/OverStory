using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    public Animation anim;
    public void ReturnToMM()
    {
        anim.Play("CreditsFadeOut");
        StartCoroutine(waiterShort());
    }

    IEnumerator waiterShort()
    {
        //Wait for 15 seconds this is harded coded to when the animation end
        yield return new WaitForSecondsRealtime(.3f);
        SceneManager.LoadScene("MainMenu");
    }
    IEnumerator waiter(string scene)
    {
        //Wait for 15 seconds this is harded coded to when the animation end
        yield return new WaitForSecondsRealtime(15f);
        ReturnToMM();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waiter("MainMenu"));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMM();
        }
    }
}
