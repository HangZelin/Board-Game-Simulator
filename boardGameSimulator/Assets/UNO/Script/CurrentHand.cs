using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
   public class CurrentHand : MonoBehaviour, IContainer
    {
        // References
        GameObject discard;
        GameObject deck;
        [SerializeField] Button cover;

        [SerializeField] Text playerName;
        public string PlayerName
        {
            get { return playerName.text; }
            set { playerName.text = value; }
        }

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards
        {
            get { return cards; }
            set
            {
                if (value[0].GetComponent<Card>() != null)
                    cards = value;
                PlaceCards();
            }
        }

        GameObject highlightedCard;
        public GameObject HighlightedCard
        {
            get { return highlightedCard; }
            set 
            {
                if (value == null)
                {
                    highlightedCard = null;
                    discard.GetComponent<Outline>().enabled = false;
                }
                else
                {
                    if (highlightedCard != null)
                        highlightedCard.GetComponent<CardReaction>().PutBack();
                    highlightedCard = value;
                    discard.GetComponent<Outline>().enabled = true;
                }  
            }
        }

        public void Initialize(GameObject discard, GameObject deck, Action onTurnStart)
        {
            this.discard = discard;
            this.deck = deck;
            cover.onClick.AddListener(delegate { onTurnStart(); });

            cards = new List<GameObject>();
            name = ToString();
        }

        void OnEnable()
        {
            Game.TurnEndHandler += EnableCover;
        }

        void OnDisable()
        {
            Game.TurnEndHandler -= EnableCover;
        }

        void PlaceCards()
        {
            // If there is no cards to place, return
            if (cards.Count == 0) return;

            // If there is a highlighted card, put it back
            if (highlightedCard != null)
            {
                highlightedCard.GetComponent<CardReaction>().PutBack();
                highlightedCard = null;
            }

            float width = GetComponent<RectTransform>().rect.width;
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

            // Distance between cards
            float d = cards.Count <= 1 ? 0f : (width - cardWidth) / cards.Count - 1;
            d = d > cardWidth + 10f ? cardWidth + 10f : d;

            // Distance to left and right
            float x = -(cardWidth + ((cards.Count - 1) * d)) / 2f + 0.5f * cardWidth;
            
            // Distance to bottom
            float y = 10f;

            foreach (GameObject card in cards)
            {
                // Initialize Card Reaction
                card.GetComponent<CardReaction>().enabled = true;
                card.GetComponent<CardReaction>().Initialize(gameObject);

                // Place cards from left to right
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = Quaternion.identity;
                x += d;
            }
        }

        public void PlayCard()
        {
            cards.Remove(highlightedCard);
            highlightedCard = null;
            PlaceCards();
        }

        public void SkipTurn()
        {
            if (highlightedCard != null) 
                highlightedCard.GetComponent<CardReaction>().PutBack();
            foreach (GameObject card in cards)
                card.GetComponent<CardReaction>().enabled = false;
            deck.GetComponent<Deck>().Interactable = false;
        }

        public void EnableCover()
        {
            cover.gameObject.SetActive(true);
            cover.gameObject.transform.SetAsLastSibling();
        }

        public void DisableCover()
        {
            cover.gameObject.SetActive(false);
        }

        // IContainer Method

        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards)
        {
            foreach (GameObject card in cards)
                card.transform.SetParent(parent);

            transferedCards = new List<GameObject>();
            transferedCards.AddRange(cards);

            cards = new List<GameObject>();
        }

        public override string ToString()
        {
            return "CurrentHand";
        }
    }
}

