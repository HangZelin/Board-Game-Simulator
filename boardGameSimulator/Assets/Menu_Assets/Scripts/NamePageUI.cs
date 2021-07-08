using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace NamePage
{
    public class NamePageUI : MonoBehaviour
    {
        [SerializeField] Text nameOfGame;
        int numOfPlayers;

        [SerializeField] GameObject nameInputBarPrefab;
        List<GameObject> nameInputBars;

        [SerializeField] GameObject continueButtonPrefab;
        GameObject continueButton;

        [SerializeField] GameObject ruleToggleBar;
        [SerializeField] Toggle ruleToggle;

        [SerializeField] Vector2 initialPosition;
        [SerializeField] float nameBarHeight;

        private void Start()
        {
            nameOfGame.text = GameStatus.GetNameOfGame();
            numOfPlayers = GameStatus.NumOfPlayers;

            // Initialize Name bars

            nameInputBars = new List<GameObject>();
            Vector2 pos = new Vector2(initialPosition.x, initialPosition.y);
            for (int i = 1; i <= numOfPlayers; i++)
            {
                GameObject go = Instantiate(nameInputBarPrefab, gameObject.transform);
                go.name = "NameInputBar" + i;
                go.GetComponent<RectTransform>().anchoredPosition = pos;
                go.transform.Find("EnterPlayerName").gameObject
                    .GetComponent<Text>().text = "Enter Player " + i + "'s name:";
                
                nameInputBars.Add(go);
                pos.y -= nameBarHeight;
            }

            //Initialize Game Mode
            GameStatus.is_Multiplayer = PlayerPrefs.GetInt(GameStatus.GetNameOfGame() + "_isMultiplayer") == 1;

            // Initialize rule toggle
            if (GameStatus.hasRules)
            {
                ruleToggleBar.SetActive(true);
                ruleToggleBar.GetComponent<RectTransform>().anchoredPosition = pos;
                pos.y -= 62f;

                ruleToggle.isOn = PlayerPrefs.GetInt(GameStatus.GetNameOfGame() + "_useRule", 0) == 1;
            }

            // Initialize continue button

            continueButton = Instantiate(continueButtonPrefab, gameObject.transform);
            continueButton.name = "ContinueButton";
            continueButton.GetComponent<RectTransform>().anchoredPosition = pos;
            continueButton.GetComponent<Button>().onClick.AddListener(delegate { OnContinueClicked(); });
        }

        void OnContinueClicked()
        {
            // Change the names from inputs to GameStatus
            ChangeName();

            // Memorize user preference on rules, Change userules
            if (GameStatus.hasRules)
                PlayerPrefs.SetInt(GameStatus.GetNameOfGame() + "_useRule", ruleToggle.isOn ? 1 : 0);
            GameStatus.useRules = ruleToggle.isOn;


            // Check if all names are unique
            if (GameStatus.IsNameUnique())
            {
                // Load the game scene
                SceneManager.LoadScene("HomeLoading");
                return;
            }

            continueButton.transform.Find("InvalidInput").gameObject.SetActive(true);
        }

        void ChangeName()
        {
            for (int i = 0; i < nameInputBars.Count; i++)
                GameStatus.SetNameOfPlayer(i + 1, 
                    nameInputBars[i].transform.Find("InputField/Placeholder/Text").gameObject.GetComponent<Text>().text);

            // Fill the PlayerOfNames with default names
            GameStatus.FillNameOfPlayer();
            GameStatus.PrintLog();
        }
    }
}

