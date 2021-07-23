using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class Discard : MonoBehaviour, IPointerClickHandler, ISaveable, IContainer
    {
        // References
        GameObject currentHand;
        UNOInfo unoInfo;
        [SerializeField] AudioSource playCardAudio;

        // Cards
        List<GameObject> cards;
        public List<GameObject> Cards { get { return cards; } }

        // On click event
        public delegate void DiscardOnClick();
        public event DiscardOnClick DiscardOnClickHandler;

        #region Initialize

        public void Initialize(GameObject currentHand, UNOInfo unoInfo)
        {
            this.currentHand = currentHand;
            this.unoInfo = unoInfo;
            cards = new List<GameObject>();

            name = ToString();
        }

        void OnEnable()
        {
            SaveLoadManager.OnSaveHandler += PopulateSaveData;
            SaveLoadManager.OnLoadHandler += LoadFromSaveData;
        }

        void OnDisable()
        {
            SaveLoadManager.OnSaveHandler -= PopulateSaveData;
            SaveLoadManager.OnLoadHandler -= LoadFromSaveData;
        }

        #endregion

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
            card.transform.rotation = Quaternion.identity;
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<CardReaction>().enabled = false;

            cards.Add(card);
            playCardAudio.Play();
        }

        public void PlayCard(GameObject card)
        {
            if (card != null)
            {
                CardToPile(card);
                currentHand.GetComponent<CurrentHand>().PlayCard();
            }

            GetComponent<Outline>().enabled = false;
        }

        #endregion

        #region IPointerClickHandler Implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            // For Enforce Rules
            if (GameStatus.useRules)
            {
                if (DiscardOnClickHandler != null)
                {
                    DiscardOnClickHandler();
                }
                return;
            }

            // For default hotseat mode without rules
            GameObject card = currentHand.GetComponent<CurrentHand>().HighlightedCard;
            PlayCard(card);
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

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData sd)
        {
            if (sd.playerCards == null) sd.playerCards = new List<PlayerCards>();
            sd.playerCards.Add(UNOInfo.CardsToSaveStruct(name, cards));
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
                CardToPile(card);
                i += listCount;
            }

            if (cards.Count != 0)
                cards[cards.Count - 1].transform.SetAsLastSibling();
        }

        #endregion

        #region Helpers

        public override string ToString()
        {
            return "Discard";
        }

        #endregion
    }
}
