using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class DiscardMul : MonoBehaviourPun, IPointerClickHandler, IContainer, IPunObservable
    {
        // References
        [SerializeField] GameObject currentHand;
        [SerializeField] UNOInfo unoInfo;
        [SerializeField] AudioSource playCard;

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }
        public GameObject LastCard { get 
            {
                if (cards != null && cards.Count >= 1)
                    return cards[cards.Count - 1];
                else
                {
                    Debug.LogError("Card list has not been initialized or is empty.");
                    return null;
                }
            } }

        // On click event
        public delegate void DiscardOnClick();
        public event DiscardOnClick DiscardOnClickHandler;

        public void Initialize(GameObject currentHand, UNOInfo unoInfo)
        {
            this.unoInfo = unoInfo;
            this.currentHand = currentHand;

            cards = new List<GameObject>();

            name = ToString();
        }

        // Put a card into the discard, place it in the end of the list
        public void CardToPile(GameObject card)
        {
            card.transform.SetParent(gameObject.transform);
            card.transform.rotation = Quaternion.identity;
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<CardReaction>().enabled = false;

            cards.Add(card);
            playCard.Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // For Enforce Rules
            if (DiscardOnClickHandler != null)
            {
                DiscardOnClickHandler();
                return;
            }

            // For default hotseat mode without rules
            GameObject card = currentHand.GetComponent<CurrentHandMul>().HighlightedCard;
            PlayCard(card);
        }

        public void PlayCard(GameObject card)
        {
            if (card != null)
            {
                CardToPile(card);
                currentHand.GetComponent<CurrentHandMul>().PlayCard();
            }

            GetComponent<Outline>().enabled = false;
        }


        #region RPCs



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

            transferedCards = new List<GameObject>();
            transferedCards.AddRange(cards);

            cards = new List<GameObject>();
        }

        #endregion


        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // throw new System.NotImplementedException();
        }

        #endregion
    }
}
