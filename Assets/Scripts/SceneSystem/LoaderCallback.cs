using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abyss.SceneSystem
{
    public class LoaderCallback : MonoBehaviour
    {
        private bool isFirstUpdate = true;

        private void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;
                SceneLoader.Instance.LoaderCallback();
            }
        }
    }
}