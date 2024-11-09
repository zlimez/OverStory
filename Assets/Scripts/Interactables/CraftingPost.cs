using Abyss.EventSystem;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class CraftingPost : Interactable
    {
        public override void Interact() => EventManager.InvokeEvent(PlayEvents.CraftingPostEntered);
    }
}
