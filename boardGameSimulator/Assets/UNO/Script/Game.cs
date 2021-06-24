using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{

    public class Game : MonoBehaviour
    {
        [SerializeField] GameObject Canvas;

        [SerializeField] GameObject deck;
        Deck deckScript;

        [SerializeField] GameObject currentHand;
        [SerializeField] GameObject hand;
        List<GameObject> hands;

        [SerializeField] GameObject playersObj;
        [SerializeField] GameObject player;
        List<GameObject> players;

        public int numOfPlayer = 2;
        public bool antiClockWise = true;
        int currentPlayerIndex;


        private void Start()
        {
            deckScript = deck.GetComponent<Deck>();
            deckScript.Initialize();

            // Initialize current hand
            currentHand.GetComponent<CurrentHand>().Initialize();

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
                a_Player.GetComponent<Player>().Initialize("Player" + (i + 1));
                players.Add(a_Player);
            }

            currentPlayerIndex = 0;

            // Deal 7 cards to each player

            foreach (GameObject player in players)
                player.GetComponent<Player>().TakeCards(DealCards(7, player));

            TurnStart();
        }

        private void TurnStart()
        {
            SetHand();
            foreach (GameObject player in players)
                player.GetComponent<Player>().PlaceCards();
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
            List<GameObject> cards = new List<GameObject>();
            foreach (GameObject card in deckScript.DrawCards(num))
                cards.Add(card.GetComponent<Card>().Copy(player.transform));
            return cards;
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
        }

        // For testing
        public void DealCard()
        {
            List<GameObject> list = DealCards(1, currentHand);
            foreach (GameObject go in list)
                if (go.GetComponent<Card>() != null)
                    go.GetComponent<Card>().IsFace = false;
        }
    }

    public interface Card
    {
        public GameObject Copy(Transform transform);
        public bool IsFace { get; set; }
    }
}


