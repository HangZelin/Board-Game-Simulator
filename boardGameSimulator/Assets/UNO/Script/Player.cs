using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
    public class Player : MonoBehaviour
    {
        string playerName;
        List<GameObject> cards;
        public bool isCurrentPlayer;

        [SerializeField] GameObject hand;
        public GameObject Hand { get { return hand; }
            set {
                if (value.GetComponent<Hand>() != null) hand = value;
                hand.GetComponent<Hand>().PlayerName = name;
            } 
        }
        GameObject currentHand;

        public void Initialize(string playerName, GameObject currentHand)
        {
            this.playerName = playerName;
            name = playerName;
            this.currentHand = currentHand;

            cards = new List<GameObject>();

            Game.TurnStartHandler += PlaceCards;

            Game.TurnEndHandler += GetCardsFromHand;
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
        }

        public void PlaceCards()
        {
            if (isCurrentPlayer) currentHand.GetComponent<CurrentHand>().PlayerName = name;
            if (cards.Count == 0) return;

            if (cards[0].GetComponent<Card>().IsFace != isCurrentPlayer)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isCurrentPlayer;

            if (!isCurrentPlayer)
            {
                foreach (GameObject card in cards)
                    card.transform.SetParent(hand.transform);
                hand.GetComponent<Hand>().Cards = cards;
            }
            else
            {
                foreach (GameObject card in cards)
                    card.transform.SetParent(currentHand.transform);
                currentHand.GetComponent<CurrentHand>().Cards = cards;
            }

            cards = new List<GameObject>();
        }

        public void GetCardsFromHand()
        {
            if (isCurrentPlayer)
            {
                currentHand.GetComponent<CurrentHand>().GiveCards(gameObject, out cards);
                isCurrentPlayer = false;
            }
            else
                hand.GetComponent<Hand>().GiveCards(gameObject, out cards);
        }

        private void OnDisable()
        {
            Game.TurnStartHandler -= PlaceCards;
            Game.TurnEndHandler -= GetCardsFromHand;
        }

        public override string ToString()
        {
            return playerName;
        }
    }
}
