using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class Game : MonoBehaviour
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

        [SerializeField] GameObject discardPrefab;
        GameObject discard;

        int numOfPlayer;
        int currentPlayerIndex;

        [SerializeField] GameObject directionIcons;
        bool antiClockWise;
        public bool AntiClockWise
        {
            get { return antiClockWise; }
            set 
            { 
                directionIcons.GetComponent<DirectionIcons>().DirectionIconToggle(!antiClockWise);
                antiClockWise = value;
            }
        }

        public delegate void TurnStart();
        public static event TurnStart TurnStartHandler;

        public delegate void TurnEnd();
        public static event TurnEnd TurnEndHandler;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            numOfPlayer = GameStatus.NumOfPlayers;
            uiScript = gameUI.GetComponent<SettingsUI>();

            // Initialize Turn Start

            TurnStartHandler += SetHand;

            // Initialize deck, discard, currenthand;
            deck = Instantiate(deckPrefab, Canvas.transform);
            discard = Instantiate(discardPrefab, Canvas.transform);
            currentHand = Instantiate(currentHandPrefab, Canvas.transform);

            deckScript = deck.GetComponent<Deck>();
            deckScript.Initialize(discard, DealCard);

            discard.GetComponent<Discard>().Initialize(currentHand, deck);

            currentHand.GetComponent<CurrentHand>().Initialize(discard);

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
            for(int i = 0; i < numOfPlayer; i++)
            {
                GameObject a_Player = Instantiate(player, playersObj.transform);
                a_Player.GetComponent<Player>().Initialize(GameStatus.GetNameOfPlayer(i + 1), currentHand);
                players.Add(a_Player);
            }

            currentPlayerIndex = 0;
            AntiClockWise = true;

            // Deal 7 cards to each player

            foreach (GameObject player in players)
                player.GetComponent<Player>().TakeCards(DealCards(7, player));


            OnTurnStart();
        }

        private void OnDisable()
        {
            TurnStartHandler -= SetHand;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        private void OnTurnStart()
        {
            if (TurnStartHandler != null)
                TurnStartHandler();
        }

        private void OnTurnEnd()
        {
            if (TurnEndHandler != null)
                TurnEndHandler();
            currentPlayerIndex = NextPlayer(currentPlayerIndex);
        }

        public void NextTurn()
        {
            OnTurnEnd();
            OnTurnStart();
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

        // helpers

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
        
        void SetHand()
        {
            players[currentPlayerIndex].GetComponent<Player>().isCurrentPlayer = true;
            int index = currentPlayerIndex;

            for (int i = 0; i < numOfPlayer - 1; i++)
            {
                index = NextPlayer(index);
                players[index].GetComponent<Player>().Hand = hands[i];
            }

            // For testing. Set current hand player name

/*            currentHand.transform.Find("PlayerName").gameObject
                .GetComponent<Text>().text = players[currentPlayerIndex].GetComponent<Player>().ToString();*/
        }

        // For testing
        public void DealCard()
        {
            List<GameObject> list = currentHand.GetComponent<CurrentHand>().Cards;
            list.AddRange(DealCards(1, currentHand));
            currentHand.GetComponent<CurrentHand>().Cards = list;
        }

        public void ToggleDirection()
        {
            AntiClockWise = !AntiClockWise;
            uiScript.AddLog("Direction: " + (antiClockWise ? "anticlockwise" : "clockwise"));
        }
    }

    public interface Card
    {
        public GameObject Copy(Transform transform);
        public bool IsFace { get; set; }
    }

    public interface IHand
    {
        public void GiveCards(GameObject player, out List<GameObject> cards);
        public List<GameObject> Cards { get; set; }
    } 
}


