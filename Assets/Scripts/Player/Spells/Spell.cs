using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Player.Spells
{
    public class Spell : MonoBehaviour
    {
        [SerializeField] SpellItem spellItem;
        [SerializeField] protected float purityPenalty;

        public virtual void Cast(bool toLeft)
        {
            EventManager.InvokeEvent(PlayEvents.PlayerActionPurityChange, -purityPenalty);
            GameManager.Instance.Inventory.MaterialCollection.RemoveStock(spellItem.rottenFlesh, spellItem.rottenFleshNeeded);
        }
    }
}
