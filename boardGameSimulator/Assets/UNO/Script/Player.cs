using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public class Player : MonoBehaviour, ISaveable, IContainer
    {
        string playerName;
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

        public bool isCurrentPlayer;

        /// <summary>
        /// Initialize Player. Add PlaceCards to TurnStartHandler. Add GetCardsFromHand to TurnEndHandler.
        /// </summary>
        /// <param name="playerName">Name of this player.</param>
        /// <param name="currentHand">Reference of current hand.</param>
        /// <param name="unoInfo">Reference of uno info script.</param>
        public void Initialize(string playerName, GameObject currentHand, UNOInfo unoInfo)
        {
            this.playerName = playerName;
            this.currentHand = currentHand;
            this.unoInfo = unoInfo;
            
            cards = new List<GameObject>();
            name = playerName;
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
        }

        public void PlaceCards()
        {
            if (cards.Count == 0) return;

            // If is current player, toggle card to face. Otherwise toggle card to back.
            if (cards[0].GetComponent<Card>().IsFace != isCurrentPlayer)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isCurrentPlayer;

            // Transfer all cards to hand/currenthand. 
            List<GameObject> transferedCards;
            if (isCurrentPlayer)
            {
                TransferAllCards(currentHand.transform, out transferedCards);
                currentHand.GetComponent<CurrentHand>().Cards = transferedCards;
            }
            else
            {
                TransferAllCards(hand.transform, out transferedCards);
                hand.GetComponent<Hand>().Cards = transferedCards;
            }

            // Remove the cards list.
            cards = new List<GameObject>();
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

        public void SetCurrentHandName()
        {
            // Set the name of current hand.
            if (isCurrentPlayer) currentHand.GetComponent<CurrentHand>().PlayerName = name;
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

        // IContainer Method
        public void TransferAllCards(Transform parent, out List<GameObject> transferedCards)
        {
            foreach (GameObject card in cards)
                card.transform.SetParent(parent);

            transferedCards = new List<GameObject>();
            transferedCards.AddRange(cards);

            cards = new List<GameObject>();
        }

        // Save load methods

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

        public override string ToString()
        {
            return playerName;
        }
    }
}
