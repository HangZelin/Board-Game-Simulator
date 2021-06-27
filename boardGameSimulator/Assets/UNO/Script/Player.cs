using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
    public class Player : MonoBehaviour, ISaveable
    {
        string playerName;
        List<GameObject> cards;
        public bool isCurrentPlayer;

        [SerializeField] GameObject hand;
        public GameObject Hand { get { return hand; }
            set {
                if (value.GetComponent<Hand>() != null) hand = value;
                hand.GetComponent<Hand>().PlayerName = name;
            } 
        }
        GameObject currentHand;
        UNOInfo unoInfo;

        public void Initialize(string playerName, GameObject currentHand, UNOInfo unoInfo)
        {
            this.unoInfo = unoInfo;
            this.playerName = playerName;
            name = playerName;
            this.currentHand = currentHand;

            cards = new List<GameObject>();

            Game.TurnStartHandler += PlaceCards;

            Game.TurnEndHandler += GetCardsFromHand;
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
        }

        public void PlaceCards()
        {
            if (isCurrentPlayer) currentHand.GetComponent<CurrentHand>().PlayerName = name;
            if (cards.Count == 0) return;

            if (cards[0].GetComponent<Card>().IsFace != isCurrentPlayer)
                foreach (GameObject card in cards)
                    card.GetComponent<Card>().IsFace = isCurrentPlayer;

            if (!isCurrentPlayer)
            {
                foreach (GameObject card in cards)
                    card.transform.SetParent(hand.transform);
                hand.GetComponent<Hand>().Cards = cards;
            }
            else
            {
                foreach (GameObject card in cards)
                    card.transform.SetParent(currentHand.transform);
                currentHand.GetComponent<CurrentHand>().Cards = cards;
            }

            cards = new List<GameObject>();
        }

        public void GetCardsFromHand()
        {
            if (isCurrentPlayer)
            {
                currentHand.GetComponent<CurrentHand>().GiveCards(gameObject, out cards);
                isCurrentPlayer = false;
            }
            else
                hand.GetComponent<Hand>().GiveCards(gameObject, out cards);
        }

        void OnEnable()
        {
            SaveLoadManager.OnSaveHandler += PopulateSaveData;
            SaveLoadManager.OnLoadHandler += LoadFromSaveData;
        }

        private void OnDisable()
        {
            Game.TurnStartHandler -= PlaceCards;
            Game.TurnEndHandler -= GetCardsFromHand;
            SaveLoadManager.OnSaveHandler -= PopulateSaveData;
            SaveLoadManager.OnLoadHandler -= LoadFromSaveData;
        }

        public override string ToString()
        {
            return playerName;
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
    }
}
