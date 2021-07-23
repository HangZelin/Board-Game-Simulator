using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public class PlayerMul : MonoBehaviourPun, IContainer, IPunObservable
    {
        // Only one of these are assigned
        GameObject currentHand;
        GameObject hand;

        string playerName;
        bool isClient;
        public int playerIndex;

        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }

        /// <summary>
        /// Initialize a player object. 
        /// </summary>
        /// <param name="playerName">Name of this player.</param>
        /// <param name="isClient">Is it the client?</param>
        /// <param name="hand">Hand of this player.</param>
        public void Initialize(string playerName, bool isClient, int playerIndex, GameObject hand)
        {
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

        /// <summary>
        /// Place cards in hand/currentHand.
        /// </summary>
        public void PlaceCards()
        {
            if (cards.Count == 0) return;

            // If is client, toggle card to face. Otherwise toggle card to back.
            if (cards[0].GetComponent<Card>().IsFace != isClient)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isClient;

            if (isClient)
            {
                currentHand.GetComponent<IContainer>().TakeCards(this.cards);
            }
            else
            {
                hand.GetComponent<IContainer>().TakeCards(this.cards);
            }

            // Remove the cards list.
            cards = new List<GameObject>();
        }

        private void OnDisable()
        {
            Game.TurnEndHandler -= PlaceCards;
        }

        #region RPCs

        /// <summary>
        /// Play a card from the player's hand to discard.
        /// </summary>
        /// <remarks>
        /// When a client play card, this method is called on other clients to sync the scenario.
        /// </remarks>
        /// <param name="cardIndex">Index of played card in card list.</param>
        [PunRPC]
        public void PlayCard(int cardIndex)
        {
            Debug.Log(playerName + " play a card in playermul script.");
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

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // throw new NotImplementedException();
        }

        #endregion
    }
}
