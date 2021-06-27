using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UNO
{
    public class Discard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject deck;
        GameObject currentHand;

        List<GameObject> cards;

        [SerializeField] AudioSource playCard;

        public delegate void DiscardOnClick();
        public event DiscardOnClick DiscardOnClickHandler;

        public void Initialize(GameObject currentHand, GameObject deck)
        {
            this.currentHand = currentHand;
            this.deck = deck;

            cards = new List<GameObject>();
            name = ToString();
        }

        public void CardToPile(GameObject card)
        {
            card.transform.SetParent(gameObject.transform);
            card.transform.rotation = Quaternion.identity;
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<CardReaction>().enabled = false;

            this.cards.Add(card);
        }

        public void PileToDeck()
        {
            deck.GetComponent<Deck>().Cards = cards;
            this.cards = new List<GameObject>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (DiscardOnClickHandler != null)
            {
                DiscardOnClickHandler();
                return;
            }

            // For default hotseat mode without rules
            playCard.Play();
            GameObject card = currentHand.GetComponent<CurrentHand>().HighlightedCard;
            PlayCard(card);
        }

        public void PlayCard(GameObject card)
        {
            if (card != null)
            {
                CardToPile(card);
                currentHand.GetComponent<CurrentHand>().PlayCard();
            }

            GetComponent<Outline>().enabled = false;
        }

        public override string ToString()
        {
            return "Discard";
        }
    }
}
