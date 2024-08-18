using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Michsky.UI.Shift;


// TODO: extract common logic
namespace Chronellium.MiniGames
{
    public class InfoMiniGameModal : ModalWindowManager
    {
        [Header("Resources")]
        public TextMeshProUGUI gameMode;
        public TextMeshProUGUI difficulty;
        public Button firstButton;
        public Button secondButton;
        public SelectedMiniGame selectedMiniGame;

        new private void Start()
        {
            base.Start();
            firstButton.onClick.AddListener(ExitMiniGame);
            secondButton.onClick.AddListener(ContinueMiniGame);
            selectedMiniGame.OnSelectedMiniGameChanged += UpdateModalTexts;
            // ModalWindowIn();
        }

        private void UpdateModalTexts(MiniGameUnitBase newSelectedMiniGame)
        {
            windowTitle.text = newSelectedMiniGame.title;
            windowDescription.text = newSelectedMiniGame.description;
            gameMode.text = newSelectedMiniGame.gameMode.ToString();
            difficulty.text = newSelectedMiniGame.difficulty.ToString();

            ModalWindowIn();
            PauseMiniGame();
        }

        private void ExitMiniGame()
        {
            // Implement logic for exiting the miniGame scene
            SceneManager.LoadScene("Main Menu (Desktop)");
        }

        public void ContinueMiniGame()
        {
            selectedMiniGame.ResumeSelectedMiniGame();
            ModalWindowOut();
        }

        public void PauseMiniGame()
        {
            selectedMiniGame.PauseSelectedMiniGame();
        }

        private void OnDestroy()
        {
            selectedMiniGame.OnSelectedMiniGameChanged -= UpdateModalTexts;
        }
    }
}