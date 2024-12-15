using Abyss.EventSystem;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class BGChangePost : Interactable
    {
        [SerializeField] public Texture CameraBG; 

        protected override void PlayerEnterAction(Collider2D collider)
        {
            EventManager.InvokeEvent(SystemEvents.ChangeCameraBG, CameraBG);
        }

    }
}
