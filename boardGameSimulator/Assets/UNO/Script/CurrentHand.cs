using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
   public class CurrentHand : MonoBehaviour, IHand
    {
        List<GameObject> cards;

        [SerializeField] bool hasHighLight;

        GameObject highlightedCard;
        public GameObject HighlightedCard
        {
            get { return highlightedCard; }
            set 
            {
                if (value == null)
                {
                    hasHighLight = false;
                    highlightedCard = null;
                }
                else
                {
                    hasHighLight = true;
                    if (highlightedCard != null)
                        highlightedCard.GetComponent<CardReaction>().PutBack();
                    highlightedCard = value;
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

        public void Initialize()
        {
            cards = new List<GameObject>();
            name = "CurrentHand";
        }

        void PlaceCards()
        {
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
    }
}

