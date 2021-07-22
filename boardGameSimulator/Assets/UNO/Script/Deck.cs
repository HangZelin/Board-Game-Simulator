using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO 
{
    public class Deck : MonoBehaviour, ISaveable, IContainer
    {
        [SerializeField] GameObject unused;
        [SerializeField] GameObject discard;

        [SerializeField] GameObject numCard;
        [SerializeField] GameObject skipCard;
        [SerializeField] GameObject reverseCard;
        [SerializeField] GameObject draw2Card;
        [SerializeField] GameObject draw4Card;
        [SerializeField] GameObject wildCard;

        [SerializeField] public Button drawACardButton;
        [SerializeField] Outline buttonOutline;
        public bool Interactable
        {
            get { return drawACardButton.interactable; }
            set
            {
                // ??? why is outline destroyed when I load from save
                if (buttonOutline == null)
                    buttonOutline = gameObject.transform.Find("Image3").GetComponent<Outline>();

                if (value)
                    buttonOutline.effectColor = Color.blue;
                else
                    buttonOutline.effectColor = Color.black;
                drawACardButton.interactable = value;
            }
        }

        [SerializeField] GameObject Note;

        [SerializeField] UNOInfo unoInfo;

        [SerializeField] AudioSource drawCardAudio;

        [SerializeField] bool enableCardsCount;
        [SerializeField] GameObject cardsCount;

        List<GameObject> cards;
        public List<GameObject> Cards
        {
            get { return cards; }
            set
            {
                if (value != null && value[0].GetComponent<Card>() != null)
                {
                    cards = value;
                    foreach (GameObject card in value)
                        card.transform.SetParent(unused.transform);
                }
            }
        }

        public void Initialize(GameObject discard, Action onclick, UNOInfo unoInfo)
        {
            name = ToString();
            this.unoInfo = unoInfo;

            unused = GameObject.Find("Canvas/Unused");
            this.discard = discard;

            cards = new List<GameObject>();

            drawACardButton.onClick.AddListener(delegate { onclick(); });
            Interactable = true;

            if (!GameStatus.isNewGame)
            {
                cardsCount.SetActive(enableCardsCount);
                return;
            }

            // Initialize cards in the deck

            // Color cards
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color == CardColor.black) continue;

                // 19 number cards in each color
                for (int i = 0; i <= 9; i++)
                {
                    GameObject card = Instantiate(numCard, unused.transform);
                    card.GetComponent<NumCard>().Initialize(color, i, true);
                    cards.Add(card);
                    if (i != 0)
                    {
                        card = Instantiate(numCard, unused.transform);
                        card.GetComponent<NumCard>().Initialize(color, i, true);
                        cards.Add(card);
                    }
                }

                // 2 Skip cards in each color
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_SkipCard = Instantiate(skipCard, unused.transform);
                    a_SkipCard.GetComponent<SkipCard>().Initialize(color, true);
                    cards.Add(a_SkipCard);
                }

                // 2 Reverse cards in each color
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_ReverseCard = Instantiate(reverseCard, unused.transform);
                    a_ReverseCard.GetComponent<ReverseCard>().Initialize(color, true);
                    cards.Add(a_ReverseCard);
                }

                // 2 Draw 2 cards in each color
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_Draw2Card = Instantiate(draw2Card, unused.transform);
                    a_Draw2Card.GetComponent<Draw2Card>().Initialize(color, true);
                    cards.Add(a_Draw2Card);
                }
            }

            // 4 Draw 4 cards, 4 wild cards
            for (int i = 0; i < 4; i++)
            {
                GameObject a_Draw4Card = Instantiate(draw4Card, unused.transform);
                a_Draw4Card.GetComponent<Draw4Card>().Initialize(true);
                cards.Add(a_Draw4Card);

                GameObject a_WildCard = Instantiate(wildCard, unused.transform);
                a_WildCard.GetComponent<WildCard>().Initialize(true);
                cards.Add(a_WildCard);
            }

            // Rename the cards
            foreach (GameObject card in cards)
                card.name = card.GetComponent<Card>().ToString();

            Shuffle(cards);
            while (cards[0].GetComponent<Card>().cardInfo.cardType == CardType.draw4 || cards[0].GetComponent<Card>().cardInfo.cardType == CardType.wild)
                Shuffle(cards);

            cardsCount.SetActive(enableCardsCount);
        }

        /** <summary>
         * Draw a number of cards from head of the list.
         * </summary> 
         * <returns>List of cards drawed.</returns>
         * <param name="num">Number of cards to draw</param>
         * <param name="transform">Target transform.</param>
         */
        public List<GameObject> DrawCards(int num, Transform transform)
        {
            if (num > cards.Count)
            {
                // Transfer all cards from discard
                List<GameObject> transferedCards;
                discard.GetComponent<IContainer>().TransferAllCards(unused.transform, out transferedCards);
                cards.AddRange(transferedCards);

                Shuffle(Cards);

                if (num > cards.Count)
                {
                    Debug.LogError("No more cards to draw.");
                    return null;
                }
            }

            List<GameObject> drawCards = new List<GameObject>();
            for (int i = 0; i < num; i++)
            {
                cards[i].transform.SetParent(transform);
                drawCards.Add(cards[i]);
            }
            cards.RemoveRange(0, num);

            drawCardAudio.Play();

            return drawCards; 
        }

        public void Shuffle(List<GameObject> list)
        {
            System.Random rng = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameObject value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void DeactiveNote()
        {
            Note.SetActive(false);
        }

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

        // Save load methods

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
                card.transform.SetParent(unused.transform);
                this.cards.Add(card);
                i += listCount;
            }
        }

        public override string ToString()
        {
            return "Deck";
        }
    }
}

