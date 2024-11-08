using UnityEngine;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class ConstructionPost : Interactable
    {
        [SerializeField] ConstructionItem construction; // TODO: make this a list for different tiers of same building
        [SerializeField] GameObject ConstructionPrefab;
        [SerializeField] Transform buildPoint, hoverPoint;

        private ConstructionSystem constructionSys;

        void Start()
        {
            constructionSys = Instantiate(ConstructionPrefab, transform).GetComponent<ConstructionSystem>();
            constructionSys.InitializePanel(construction, transform.position);
        }

        public override void Interact() => constructionSys.TryBuild(buildPoint, hoverPoint);

        // TODO: Add check whether player has unlocked the blueprint
        protected override void PlayerEnterAction(Collider2D collider)
        {
            base.PlayerEnterAction(collider);
            constructionSys.TryOpenPanel();
        }

        protected override void PlayerExitAction(Collider2D collider)
        {
            base.PlayerExitAction(collider);
            constructionSys.ClosePanel();
        }
    }
}
