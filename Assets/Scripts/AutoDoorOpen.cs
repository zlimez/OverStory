using UnityEngine;
using UnityEngine.Playables;

public class AutoDoorOpen : MonoBehaviour
{
    public PlayableDirector timeline;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {


        timeline.Play();
    }


}
