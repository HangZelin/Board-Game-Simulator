using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class HandMul : MonoBehaviour, IContainer, IHand
    {
        // References
        [SerializeField] DiscardMul discardScript;
        [SerializeField] GameObject playerText;
        [SerializeField] GameObject playerTextLeft;
        [SerializeField] GameObject playerTextRight;

        /// <summary>
        /// Index of hand. Starts from 0. 
        /// </summary>
        int num;

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards
        {
            get { return cards; }
            set
            {
                cards = value;
                PlaceCards();
            }
        }

        Text playerName;
        public string PlayerName
        {
            get { return playerName.text; }
            set { playerName.text = value; }
        }

        public void Initialize(int num, DiscardMul discardScript)
        {
            this.num = num;
            cards = new List<GameObject>();

            name = ToString();

            this.discardScript = discardScript;
        }

        #region Card Methods

        // Set cards positions

        void PlaceCards()
        {
            float width = GetComponent<RectTransform>().rect.width;
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

            float d = cards.Count <= 1 ? 0f : (width - cardWidth) / cards.Count - 1;
            d = d > cardWidth + 10f ? cardWidth + 10f : d;

            float x = -(cardWidth + ((cards.Count - 1) * d)) / 2f + 0.5f * cardWidth;
            float y = 10f;
            Quaternion handRotation = GetComponent<RectTransform>().rotation;

            foreach (GameObject card in cards)
            {
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                card.GetComponent<RectTransform>().rotation = handRotation;
                x += d;

                card.GetComponent<CardReaction>().enabled = false;
            }
        }

        /// <summary>
        /// Place the card to discard, remove it from hand card list.
        /// </summary>
        /// <param name="cardIndex">Index of card to be played.</param>
        public void PlayCard(int cardIndex)
        {
            discardScript.CardToPile(cards[cardIndex]);
            cards.Remove(cards[cardIndex]);
            PlaceCards();
        }

        #endregion

        #region Helpers

        public override string ToString()
        {
            return "Hand" + num.ToString();
        }

        #endregion

        #region IHand Implementation

        public void SetTextPosition(float zRotation)
        {
            switch (zRotation)
            {
                case 90f: playerTextRight.SetActive(true); playerName = playerTextRight.GetComponent<Text>(); break;
                case 180f: playerText.SetActive(true); playerName = playerText.GetComponent<Text>(); break;
                case 270f: playerTextLeft.SetActive(true); playerName = playerTextLeft.GetComponent<Text>(); break;
                default: playerText.SetActive(true); playerName = playerText.GetComponent<Text>(); break;
            }
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
            PlaceCards();
        }

        #endregion
    }
}