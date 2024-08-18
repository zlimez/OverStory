using UnityEngine;
using UnityEngine.Rendering;
using OccaSoftware.LSPP.Runtime;

namespace OccaSoftware.LSPP.Demo
{
    public class ChangeLightScatteringColor : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(FindObjectOfType<Volume>().profile.TryGet(out LightScatteringPostProcess temp))
				{
                    temp.tint.overrideState = true;
                    temp.tint.value = Random.ColorHSV(0f, 1f);
                }
            }
        }
    }
}