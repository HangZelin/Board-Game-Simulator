using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
   public class CurrentHand : MonoBehaviour, IHand
    {
        GameObject discard;
        GameObject deck;

        [SerializeField] Text playerName;
        public string PlayerName
        {
            get { return playerName.text; }
            set { playerName.text = value; }
        }

        List<GameObject> cards;

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

        public void Initialize(GameObject discard, GameObject deck)
        {
            this.discard = discard;
            this.deck = deck;

            cards = new List<GameObject>();
            name = "CurrentHand";
        }

        void PlaceCards()
        {
            if (cards.Count == 0) return;

            float width = GetComponent<RectTransform>().rect.width;
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

            float d = cards.Count <= 1 ? 0f : (width - cardWidth) / cards.Count - 1;
            d = d > cardWidth + 10f ? cardWidth + 10f : d;

            float x = -(cardWidth + ((cards.Count - 1) * d)) / 2f + 0.5f * cardWidth;
            float y = 10f;
            foreach (GameObject card in cards)
            {
                // Initialize Card Reaction
                card.GetComponent<CardReaction>().enabled = true;
                card.GetComponent<CardReaction>().Initialize(gameObject);

                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = Quaternion.identity;
                x += d;
            }
        }

        public void GiveCards(GameObject player, out List<GameObject> cards)
        {
            cards = new List<GameObject>(this.cards);
            foreach (GameObject card in this.cards)
                card.transform.SetParent(player.transform);
            this.cards = new List<GameObject>();
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
    }
}

