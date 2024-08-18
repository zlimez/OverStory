using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Michsky.UI.Shift;


// TODO: extract common logic
namespace Chronellium.MiniGames
{
    public class SelectedMiniGameModal : ModalWindowManager
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
            secondButton.onClick.AddListener(StartMiniGame);
            selectedMiniGame.OnSelectedMiniGameChanged += UpdateModalTexts;
        }

        private void UpdateModalTexts(MiniGameUnitBase newSelectedMiniGame)
        {
            windowTitle.text = newSelectedMiniGame.title;
            windowDescription.text = newSelectedMiniGame.description;
            gameMode.text = newSelectedMiniGame.gameMode.ToString();
            difficulty.text = newSelectedMiniGame.difficulty.ToString();

            ModalWindowIn();
        }

        public void StartMiniGame()
        {
            SceneManager.LoadScene("MiniGame");
        }

        private void OnDestroy()
        {
            selectedMiniGame.OnSelectedMiniGameChanged -= UpdateModalTexts;
        }
    }
}