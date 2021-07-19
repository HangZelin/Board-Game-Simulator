using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public class PlayerMul : MonoBehaviourPun, IContainer, IPunObservable
    {
        UNOInfo unoInfo;
        GameObject currentHand;
        GameObject hand;
        public GameObject Hand
        {
            get { return hand; }
            set
            {
                if (value.GetComponent<Hand>() != null)
                {
                    hand = value;
                    hand.GetComponent<Hand>().PlayerName = name;
                }
            }
        }

        string playerName;
        bool isClient;
        public bool isCurrentPlayer;
        public int playerIndex;

        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }

        /// <summary>
        /// Initialize a player object. 
        /// <remarks>
        /// Assign references and properties. Set names in respective hands.
        /// </remarks>
        /// </summary>
        /// <param name="playerName">Name of this player.</param>
        /// <param name="isClient">Is it the client?</param>
        /// <param name="hand">Hand of this player.</param>
        public void Initialize(UNOInfo unoInfo, string playerName, bool isClient, int playerIndex, GameObject hand)
        {
            this.unoInfo = unoInfo;
            this.playerName = playerName;
            this.isClient = isClient;
            this.playerIndex = playerIndex;
            if (isClient)
            {
                this.currentHand = hand;
                currentHand.GetComponent<CurrentHandMul>().PlayerName = playerName;
            }
            else
            {
                this.hand = hand;
                hand.GetComponent<HandMul>().PlayerName = playerName;
            }

            cards = new List<GameObject>();
            name = playerName;
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
            PlaceCards();
        }

        public void PlaceCards()
        {
            if (cards.Count == 0) return;

            // If is current player, toggle card to face. Otherwise toggle card to back.
            if (cards[0].GetComponent<Card>().IsFace != isClient)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isClient;

            // Transfer all cards to hand/currenthand. 
            List<GameObject> transferedCards;
            if (isClient)
            {
                List<GameObject> temp = new List<GameObject>(currentHand.GetComponent<CurrentHandMul>().Cards);
                temp.AddRange(this.cards);
                this.cards = new List<GameObject>(temp);

                TransferAllCards(currentHand.transform, out transferedCards);
                currentHand.GetComponent<CurrentHandMul>().Cards = transferedCards;
            }
            else
            {
                List<GameObject> temp = new List<GameObject>(currentHand.GetComponent<CurrentHandMul>().Cards);
                temp.AddRange(this.cards);
                this.cards = new List<GameObject>(temp);

                TransferAllCards(hand.transform, out transferedCards);
                hand.GetComponent<HandMul>().Cards = transferedCards;
            }

            // Remove the cards list.
            cards = new List<GameObject>();
        }

        private void OnDisable()
        {
            Game.TurnEndHandler -= PlaceCards;
        }

        #region RPCs

        [PunRPC]
        public void PlayCard(int cardIndex)
        {
            if (!isClient)
                hand.GetComponent<HandMul>().PlayCard(cardIndex);
            else
                Debug.LogError("Wrong player called.");
        }

        #endregion

        #region Helpers

        public override string ToString()
        {
            return playerName;
        }

        #endregion

        #region IContainer Implementation

        // IContainer Method
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
            // throw new NotImplementedException();
        }

        #endregion
    }
}
