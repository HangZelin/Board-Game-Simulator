using BGS.MenuUI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class DeckMul : MonoBehaviourPun, IContainer, IPunObservable
    {
        [Header("References")]
        [SerializeField] GameObject unused;
        [SerializeField] GameObject discard;

        [Header("Card Prefabs")]
        [SerializeField] GameObject numCard;
        [SerializeField] GameObject skipCard;
        [SerializeField] GameObject reverseCard;
        [SerializeField] GameObject draw2Card;
        [SerializeField] GameObject draw4Card;
        [SerializeField] GameObject wildCard;

        [Header("Components")]
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

        [SerializeField] GameObject note;

        [SerializeField] UNOInfo unoInfo;

        [SerializeField] AudioSource drawCardAudio;

        [Header("Card count")]
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
        MultiplayerManager mulManager;

        public bool cardsInitialized;

        public void Initialize(Action onclick)
        {
            name = ToString();
            drawACardButton.onClick.AddListener(delegate { onclick(); });
            Interactable = true;
            mulManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            cardsInitialized = false;

            if (mulManager.isHost)
            {
                InitializeCards();
                BroadcastCards();
                cardsInitialized = true;
            }

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

                this.photonView.RPC("ClearDiscard", RpcTarget.Others);
                BroadcastCards();
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


        #region Button Callbacks

        public void OnDeckButtonClicked()
        {
            if (note.activeSelf)
                note.SetActive(false);
        }

        #endregion


        #region RPCs

        [PunRPC]
        void GetCards(string[] cardInfoArr)
        {
            this.cards = new List<GameObject>();
            List<string> cardInfoList = new List<string>(cardInfoArr);
            for (int i = 0; i < cardInfoList.Count; i += 3)
            {
                GameObject card = unoInfo.ListToCard(cardInfoList.GetRange(i, 3));
                card.transform.SetParent(unused.transform);
                this.cards.Add(card);
            }
            Debug.Log(cards.Count);
            cardsInitialized = true;
        }

        [PunRPC]
        void ClearDiscard()
        {
            discard.GetComponent<DiscardMul>().RemoveAll();
        }

        #endregion


        #region Helpers

        void BroadcastCards()
        {
            List<string> cardInfoList = new List<string>();
            foreach (GameObject card in this.cards)
            {
                List<string> list = UNOInfo.CardToList(card);
                cardInfoList.AddRange(list);
            }
            this.photonView.RPC("GetCards", RpcTarget.Others, cardInfoList.ToArray());
        }

        /// <summary>
        /// Initialize cards in the deck.
        /// </summary>
        public void InitializeCards()
        {
            cards = new List<GameObject>();

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

        public override string ToString()
        {
            return "Deck";
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
            // throw new NotImplementedException();
        }

        #endregion
    }
}

