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

            hands = new List<GameObject>();
            for (int i = 0; i < numOfPlayer - 1; i++)
            {
                GameObject a_Hand = Instantiate(hand, Canvas.transform);
                a_Hand.GetComponent<Hand>().Initialize(i);
                hands.Add(a_Hand);
            }
            Hand.SetPositions(hands);

            players = new List<GameObject>();
            for(int i = 0; i < numOfPlayer; i++)
            {
                GameObject a_Player = Instantiate(player, playersObj.transform);
                a_Player.GetComponent<Player>().Initialize("Player" + (i + 1));
                players.Add(a_Player);
            }
            currentPlayerIndex = 0;

            foreach (GameObject player in players)
                player.GetComponent<Player>().TakeCards(DealCards(7, player));
        }

        private void TurnStart()
        {

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
                cards.Add(CopyCard(card, player.transform));
            return cards;
        }

        // helpers

        GameObject CopyCard(GameObject origin, Transform transform)
        {
            if (origin.GetComponent<NumCard>() != null)
                return origin.GetComponent<NumCard>().Copy(transform);
            else if (origin.GetComponent<SkipCard>() != null)
                return origin.GetComponent<SkipCard>().Copy(transform);
            else if (origin.GetComponent<ReverseCard>() != null)
                return origin.GetComponent<ReverseCard>().Copy(transform);
            else if (origin.GetComponent<Draw2Card>() != null)
                return origin.GetComponent<Draw2Card>().Copy(transform);
            else if (origin.GetComponent<Draw4Card>() != null)
                return origin.GetComponent<Draw4Card>().Copy(transform);
            else if (origin.GetComponent<WildCard>() != null)
                return origin.GetComponent<WildCard>().Copy(transform);
            else return null;
        }
    
        GameObject NextPlayer()
        {
            if (antiClockWise)
                return currentPlayerIndex == (numOfPlayer - 1) 
                    ? players[0] 
                    : players[currentPlayerIndex + 1];
            else
                return currentPlayerIndex == 0 
                    ? players[numOfPlayer - 1] 
                    : players[currentPlayerIndex - 1];
        }
    }

    public interface Card
    {
        public GameObject Copy(Transform transform);
    }

}


