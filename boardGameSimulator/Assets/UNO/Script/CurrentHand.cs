using System.Collections;
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
            float x = -((cards.Count - 1) / 2f) * 20f;
            float y = 10f;
            foreach (GameObject card in cards)
            {
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = Quaternion.identity;
                x += 20f;

                // Initialize Card Reaction
                card.GetComponent<CardReaction>().Initialize(gameObject);
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
            Debug.Log(cards.Count);
            cards.Remove(highlightedCard);
            highlightedCard = null;
            PlaceCards();
        }
    }
}

