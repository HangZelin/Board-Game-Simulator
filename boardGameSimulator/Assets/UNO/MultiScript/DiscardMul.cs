using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class DiscardMul : MonoBehaviour, IPointerClickHandler, IContainer
    {
        // References
        [SerializeField] GameObject currentHand;
        [SerializeField] AudioSource playCard;

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }

        // On click event
        public delegate void DiscardOnClick();
        public event DiscardOnClick DiscardOnClickHandler;

        public void Initialize(GameObject currentHand)
        {
            this.currentHand = currentHand;
            cards = new List<GameObject>();

            name = ToString();
        }

        #region Card Methods

        /// <summary>
        /// Put a card onto the top of discard.
        /// </summary>
        /// <remarks>
        /// Card's transform is set to discard; card's reference is added to the end of discard card list.
        /// </remarks>
        /// <param name="card">Card to put in discard.</param>
        public void CardToPile(GameObject card)
        {
            card.transform.SetParent(gameObject.transform);
            card.GetComponent<CardReaction>().enabled = false;
            card.GetComponent<Card>().IsFace = true;
            
            card.transform.rotation = Quaternion.identity;
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            cards.Add(card);

            // Play audio
            playCard.Play();
            // Disable outline
            GetComponent<Outline>().enabled = false;
        }

        #endregion

        #region Button Callbacks

        public void OnPointerClick(PointerEventData eventData)
        {
            // For Enforce Rules
            if (DiscardOnClickHandler != null)
            {
                DiscardOnClickHandler();
                return;
            }

            // For default hotseat mode without rules
            currentHand.GetComponent<CurrentHandMul>().PlayCard();
        }

        #endregion

        #region Helpers

        public void RemoveAll()
        {
            this.cards = new List<GameObject>();
        }

        public override string ToString()
        {
            return "Discard";
        }

        #endregion

        #region IContainer Implementation

        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards)
        {
            foreach (GameObject card in cards)
                card.transform.SetParent(parent);

            transferedCards = new List<GameObject>(cards);

            cards = new List<GameObject>();
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
            foreach (GameObject card in cards)
                card.transform.SetParent(transform);
        }

        #endregion
    }
}
