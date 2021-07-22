using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public interface Card
    {
        /** 
         * <summary>Assign true/false to toggle card to face/back.</summary>
         */
        public bool IsFace { get; set; }

        public CardInfo cardInfo { get; }
    }

    public interface IContainer
    {
        public List<GameObject> Cards { get; }

        /**
         * <summary>
         * Transfer all cards from current transform to target transform.
         * </summary>
         * <param name="parent">Target transform</param>
         * <param name="transferedCards">List of transfered cards</param>
         */
        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards);

        /// <summary>
        /// Add the cards at the end of container's card list, set the cards' transform to container.
        /// </summary>
        /// <remarks>
        /// Hand and currrentHand will call PlaceCards() after setting cards' transform.
        /// </remarks>
        /// <param name="cards">List of cards to take.</param>
        public void TakeCards(List<GameObject> cards);
    }

    public interface ICurrentHand : IContainer
    {
        public GameObject HighlightedCard { get; set; }

        /// <summary>
        /// Put back highlighted cards, disable card reactions.
        /// </summary>
        public void SkipTurn();
    }

    interface IHand : IContainer
    {
        void SetTextPosition(float zRotation);
    }
}