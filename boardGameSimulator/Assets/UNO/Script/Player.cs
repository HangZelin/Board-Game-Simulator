using System.Collections;
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
            } 
        }
        [SerializeField] GameObject currentHand;

        public void Initialize(string playerName)
        {
            this.playerName = playerName;

            cards = new List<GameObject>();

            currentHand = GameObject.Find("CurrentHand");
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
        }

        public void PlaceCards()
        {
            if (cards[0].GetComponent<Card>().IsFace != isCurrentPlayer)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isCurrentPlayer;

            if (!isCurrentPlayer)
                Hand.GetComponent<Hand>().PlaceCards(cards);
            else
            {
                currentHand.GetComponent<CurrentHand>().PlaceCards(cards);
            }

            cards = new List<GameObject>();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
