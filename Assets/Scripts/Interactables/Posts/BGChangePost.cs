using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class BGChangePost : Interactable
    {
        public Texture CameraBG;

        protected override void PlayerEnterAction(Collider2D collider) => EventManager.InvokeEvent(SystemEvents.ChangeCameraBG, CameraBG);
    }
}
