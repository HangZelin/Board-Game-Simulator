using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UNO
{
    public class Discard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject deck;
        GameObject currentHand;

        List<GameObject> cards;

        public void Initialize(GameObject currentHand)
        {
            this.currentHand = currentHand;
            cards = new List<GameObject>();
        }

        public void CardToPile(GameObject card)
        {
            card.transform.SetParent(gameObject.transform);
            card.transform.rotation = Quaternion.identity;
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<CardReaction>().enabled = false;
        }

        public void PileToDeck()
        {
            deck.GetComponent<Deck>().Cards = cards;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject card = currentHand.GetComponent<CurrentHand>().HighlightedCard;
            if (card != null)
            {
                CardToPile(card);
                currentHand.GetComponent<CurrentHand>().PlayCard();
            }
        }
    }
}
