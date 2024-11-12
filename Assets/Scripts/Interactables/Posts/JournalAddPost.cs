using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class JournalAddPost : Interactable
    {
        [SerializeField] JournalItem journal; 
        [SerializeField] float IntellLevelUp; 

        void OnEnable()
        {
            if (GameManager.Instance == null)
                EventManager.StartListening(SystemEvents.SystemsReady, initAddPost);
            else
            {
                tryCloseThis();
            }
        }
        void initAddPost(object input)
        {
            tryCloseThis();
            EventManager.StopListening(SystemEvents.SystemsReady, initAddPost);
        }

        void tryCloseThis()
        {
            if(GameManager.Instance.Inventory.MaterialCollection.Contains(journal)) 
                gameObject.SetActive(false);
        }

        protected override void PlayerEnterAction(Collider2D collider)
        {
            if(!GameManager.Instance.Inventory.MaterialCollection.Contains(journal)) 
            {
                GameManager.Instance.Inventory.MaterialCollection.Add(journal);
                GameManager.Instance.PlayerPersistence.PlayerAttr.Intelligence += IntellLevelUp;
                if (IntellLevelUp != 0) EventManager.InvokeEvent(PlayEvents.PlayerIntelligenceChange, IntellLevelUp);
            }
            gameObject.SetActive(false);
        }

    }
}
