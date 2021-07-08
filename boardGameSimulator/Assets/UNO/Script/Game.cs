using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class Game : MonoBehaviour, ISaveable
    {
        [SerializeField] GameObject Canvas;
        [SerializeField] GameObject gameUI;
        SettingsUI uiScript;

        [SerializeField] GameObject deckPrefab;
        GameObject deck;
        Deck deckScript;

        [SerializeField] GameObject currentHandPrefab;
        GameObject currentHand;
        [SerializeField] GameObject hand;
        List<GameObject> hands;

        [SerializeField] GameObject playersObj;
        [SerializeField] GameObject player;
        List<GameObject> players;
        public GameObject CurrentPlayer { get { return players[currentPlayerIndex]; } }

        [SerializeField] GameObject discardPrefab;
        GameObject discard;

        int numOfPlayer;
        int currentPlayerIndex;

        [SerializeField] GameObject directionIcons;
        bool antiClockWise;

        [SerializeField] Button nextTurnButton;

        public delegate void TurnStart();
        public static event TurnStart TurnStartHandler;

        public delegate void TurnEnd();
        public static event TurnEnd TurnEndHandler;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            numOfPlayer = GameStatus.NumOfPlayers;
            uiScript = gameUI.GetComponent<SettingsUI>();

            // Initialize deck, discard, currenthand;
            deck = Instantiate(deckPrefab, Canvas.transform);
            discard = Instantiate(discardPrefab, Canvas.transform);
            currentHand = Instantiate(currentHandPrefab, Canvas.transform);

            deckScript = deck.GetComponent<Deck>();
            deckScript.Initialize(discard, DealCard, GetComponent<UNOInfo>());

            discard.GetComponent<Discard>().Initialize(currentHand, GetComponent<UNOInfo>());

            currentHand.GetComponent<CurrentHand>().Initialize(discard, deck, OnTurnStart);

            // Initialize hands

            hands = new List<GameObject>();
            for (int i = 0; i < numOfPlayer - 1; i++)
            {
                GameObject a_Hand = Instantiate(hand, Canvas.transform);
                a_Hand.GetComponent<Hand>().Initialize(i);
                hands.Add(a_Hand);
            }
            Hand.SetPositions(hands);

            // Initialize players

            players = new List<GameObject>();
            for (int i = 0; i < numOfPlayer; i++)
            {
                GameObject a_Player = Instantiate(player, playersObj.transform);
                a_Player.GetComponent<Player>().Initialize(GameStatus.GetNameOfPlayer(i + 1), currentHand, GetComponent<UNOInfo>());
                players.Add(a_Player);
            }

            // Initialize settings ui

            gameUI.GetComponent<SettingsUI>().Initialize();

            if (GameStatus.isNewGame)
            {
                currentPlayerIndex = 0;
                antiClockWise = true;

                // Deal 7 cards to each player

                foreach (GameObject player in players)
                    player.GetComponent<Player>().Cards.AddRange(DealCards(7, player));

                gameUI.GetComponent<SettingsUI>().AddLog("UNO: New game.");
            }
            else
            {
                if (SaveLoadManager.OnLoadHandler != null)
                    SaveLoadManager.OnLoadHandler(SaveLoadManager.tempSD);

                gameUI.GetComponent<SettingsUI>().AddLog("UNO: Load complete.");
            }

            if (GameStatus.useRules)
            {
                GetComponent<Rules>().Initialize(currentHand, discard, deck, this);
                GetComponent<Rules>().enabled = true;
            }
            else
            {
                GetComponent<Rules>().enabled = false;
                directionIcons.GetComponent<DirectionIcons>().Interactable = true;
            }

            // Initialize direaction Icons

            directionIcons.GetComponent<DirectionIcons>().DirectionIconToggle(antiClockWise);

            // Initilialize Game end handler
            TurnEndHandler += currentHand.GetComponent<CurrentHand>().EnableCover;

            SetHand();
            currentHand.GetComponent<CurrentHand>().EnableCover();
        }

        void OnEnable()
        {
            SaveLoadManager.OnSaveHandler += PopulateSaveData;
            SaveLoadManager.OnLoadHandler += LoadFromSaveData;
        }

        private void OnDisable()
        {
            SaveLoadManager.OnSaveHandler -= PopulateSaveData;
            SaveLoadManager.OnLoadHandler -= LoadFromSaveData;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public void OnTurnStart()
        {
            if (TurnStartHandler != null)
                TurnStartHandler();
            if (!GameStatus.useRules)
            {
                nextTurnButton.interactable = true;
            }
        }

        public void OnTurnEnd()
        {
            nextTurnButton.interactable = false;

            if (TurnEndHandler != null)
                TurnEndHandler();
            currentPlayerIndex = NextPlayer(currentPlayerIndex);
            SetHand();
        }

        /**
         * <summary>
         * Deal cards from deck into a transform.
         * </summary>
         * <return>A list of cards.</return>
         * <param name="num">Number of cards to deal.</param>
         * <param name="player">Player who take the cards</param>
         */
        public List<GameObject> DealCards(int num, GameObject player)
        {
            return deckScript.DrawCards(num, player.transform);
        }

        /// <summary>
        /// Get index of next player based on direction.
        /// </summary>
        /// <param name="index">Player index.</param>
        /// <returns>Index of next player based on direction.</returns>
        int NextPlayer(int index)
        {
            if (antiClockWise)
                return index == (numOfPlayer - 1)
                    ? 0
                    : index + 1;
            else
                return index == 0
                    ? numOfPlayer - 1
                    : index - 1;
        }

        /// <summary>
        /// Assign player to hand/currentHand.
        /// </summary>
        void SetHand()
        {
            players[currentPlayerIndex].GetComponent<Player>().isCurrentPlayer = true;
            players[currentPlayerIndex].GetComponent<Player>().SetCurrentHandName();

            int index = currentPlayerIndex;

            if (antiClockWise)
                for (int i = 0; i < numOfPlayer - 1; i++)
                {
                    index = NextPlayer(index);
                    players[index].GetComponent<Player>().Hand = hands[i];
                }
            else
                for (int i = numOfPlayer - 2; i >= 0; i--)
                {
                    index = NextPlayer(index);
                    players[index].GetComponent<Player>().Hand = hands[i];
                }
        }

        /// <summary>
        /// Deal a card to currenthand.
        /// </summary>
        public void DealCard()
        {
            List<GameObject> list = currentHand.GetComponent<CurrentHand>().Cards;
            list.AddRange(DealCards(1, currentHand));
            currentHand.GetComponent<CurrentHand>().Cards = list;
        }

        public void ToggleDirection()
        {
            antiClockWise = !antiClockWise;
            directionIcons.GetComponent<DirectionIcons>().DirectionIconToggle(antiClockWise);
            uiScript.AddLog("Direction reversed.");
        }

        // Save load methods

        public void PopulateSaveData(SaveData sd)
        {
            sd.playerInTurn = currentPlayerIndex;
            sd.antiClockwise = this.antiClockWise;
            gameUI.GetComponent<SettingsUI>().AddLog("Save complete.");
        }

        public void LoadFromSaveData(SaveData sd)
        {
            currentPlayerIndex = sd.playerInTurn;
            this.antiClockWise = sd.antiClockwise;
        }
    }

    public interface Card
    {
        /** 
         * <summary>Assign true/false to toggle card to face/back.</summary>
         */
        public bool IsFace { get; set; }
        public CardInfo cardInfo { get; }
    }

    public interface IContainer
    {
        public List<GameObject> Cards { get; }

        /**
         * <summary>
         * Transfer all cards from current transform to target transform.
         * </summary>
         * <param name="parent">Target transform</param>
         * <param name="transferedCards">List of transfered cards</param>
         */
        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards);
    }
}



