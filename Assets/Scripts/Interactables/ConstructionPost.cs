using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using Tuples;
using System.Collections.Generic;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class ConstructionPost : Interactable
    {
        [SerializeField] ConstructionItem construction;
        [SerializeField] GameObject ConstructionPrefab;

        private GameObject constructionPanel;
        private bool isPanelOpen;

        void Start()
        {
            constructionPanel = Instantiate(ConstructionPrefab, transform);
            ConstructionSystem sconstructionController = constructionPanel.GetComponent<ConstructionSystem>();
            if (sconstructionController != null) sconstructionController.InitializePanel(construction, transform.position);
            else Debug.LogError("ConstructionSystem 组件未找到!");
            isPanelOpen = false;
            constructionPanel.SetActive(isPanelOpen);
        }

        public override void Interact()
        {
            isPanelOpen = !isPanelOpen;
            constructionPanel.SetActive(isPanelOpen);
            
        } 
    }
}
