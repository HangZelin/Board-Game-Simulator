using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public class Player : MonoBehaviour, ISaveable, IContainer
    {
        GameObject currentHand;
        UNOInfo unoInfo;

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

        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }

        string playerName;
        public bool isCurrentPlayer;

        #region Initialize

        /// <summary>
        /// Initialize Player.
        /// </summary>
        /// <param name="playerName">Name of this player.</param>
        /// <param name="currentHand">Reference of current hand.</param>
        /// <param name="unoInfo">Reference of uno info script.</param>
        public void Initialize(string playerName, GameObject currentHand, UNOInfo unoInfo)
        {
            this.currentHand = currentHand;
            this.unoInfo = unoInfo;
            cards = new List<GameObject>();

            this.playerName = playerName;
            isCurrentPlayer = false;
            name = playerName;
        }

        void OnEnable()
        {
            SaveLoadManager.OnSaveHandler += PopulateSaveData;
            SaveLoadManager.OnLoadHandler += LoadFromSaveData;
        }

        private void OnDisable()
        {
            Game.TurnEndHandler -= GetCardsFromHand;
            Game.TurnEndHandler -= PlaceCards;

            SaveLoadManager.OnSaveHandler -= PopulateSaveData;
            SaveLoadManager.OnLoadHandler -= LoadFromSaveData;
        }

        #endregion

        #region Card Methods

        public void PlaceCards()
        {
            if (cards.Count == 0) return;

            // If is current player, toggle card to face. Otherwise toggle card to back.
            if (cards[0].GetComponent<Card>().IsFace != isCurrentPlayer)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isCurrentPlayer;

            // Transfer all cards to hand/currenthand. 
            if (isCurrentPlayer)
            {
                currentHand.GetComponent<IContainer>().TakeCards(cards);
            }
            else
            {
                hand.GetComponent<IContainer>().TakeCards(cards);
            }
        }

        public void GetCardsFromHand()
        {
            if (isCurrentPlayer)
            {
                currentHand.GetComponent<IContainer>().TransferAllCards(gameObject.transform, out cards);
                isCurrentPlayer = false;
            }
            else
                hand.GetComponent<IContainer>().TransferAllCards(gameObject.transform, out cards);
        }

        #endregion

        #region Helpers

        public void SetCurrentHandName()
        {
            // Set the name of current hand.
            if (isCurrentPlayer) currentHand.GetComponent<CurrentHand>().PlayerName = name;
        }

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

            transferedCards = new List<GameObject>();
            transferedCards.AddRange(cards);

            cards = new List<GameObject>();
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
            foreach (GameObject card in cards)
                card.transform.SetParent(transform);
        }

        #endregion

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData sd)
        {
            if (sd.playerCards == null) sd.playerCards = new List<PlayerCards>();
            if (isCurrentPlayer)
                sd.playerCards.Add(UNOInfo.CardsToSaveStruct(name, currentHand.GetComponent<CurrentHand>().Cards));
            else
                sd.playerCards.Add(UNOInfo.CardsToSaveStruct(name, hand.GetComponent<Hand>().Cards));
        }

        public void LoadFromSaveData(SaveData sd)
        {
            List<int> listCounts = new List<int>();
            List<string> cardsString = new List<string>();

            foreach (PlayerCards playerCards in sd.playerCards)
            {
                if (playerCards.playerName.Equals(name))
                {
                    listCounts = playerCards.listCounts;
                    cardsString = playerCards.cards;
                    break;
                }
            }

            int i = 0;
            foreach (int listCount in listCounts)
            {
                List<string> list = cardsString.GetRange(i, listCount);
                GameObject card = unoInfo.ListToCard(list);
                card.transform.SetParent(gameObject.transform);
                this.cards.Add(card);
                i += listCount;
            }
        }

        #endregion
    }
}
