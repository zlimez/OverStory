using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Michsky.UI.Shift;


// TODO: extract common logic
namespace Chronellium.MiniGames
{
    public class FailureMiniGameModal : ModalWindowManager
    {
        [Header("Resources")]
        public Button firstButton;
        public Button secondButton;
        public SelectedMiniGame selectedMiniGame;

        new private void Start()
        {
            base.Start();
            firstButton.onClick.AddListener(ExitMiniGame);
            secondButton.onClick.AddListener(Retry);
        }

        private void ExitMiniGame()
        {
            // Implement logic for exiting the miniGame scene
            SceneManager.LoadScene("Main Menu (Desktop)");
        }

        private void Retry()
        {
            // TODO: better to reload from selectedminigame SO?
            SceneManager.LoadScene("MiniGame");
        }
    }
}