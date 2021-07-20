using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class RulesMul : MonoBehaviourPun, IPunObservable
    {
        MultiGame gameScript;

        [Header("Containers")]
        [SerializeField] DeckMul deckScript;
        [SerializeField] CurrentHandMul cHandScript;
        [SerializeField] GameObject currentHand;
        [SerializeField] DiscardMul discardScript;

        [Header("UIs")]
        [SerializeField] Button deckButton;
        [SerializeField] SettingsUI gameUI;
        [SerializeField] GameObject selectColorTab;
        [SerializeField] Button nextTurnButton;
        [SerializeField] GameObject unoButton;

        /// <summary>
        /// Draw2/Draw4 cards drawed
        /// </summary>
        bool cardDrawed;
        bool unoButtonClicked;
        bool isNextTurn;
        bool isCheckUno;
        public bool firstCardDrawed;

        CardInfo lastCardInfo;
        public CardColor lastCardColor { set { lastCardInfo.cardColor = value; } }

        public void Initialize()
        {
            discardScript.DiscardOnClickHandler += CheckValid;

            deckButton.onClick.AddListener(delegate { DeckDraw(); });

            this.gameScript = GetComponent<MultiGame>();

            unoButton.transform.SetAsLastSibling();
            cardDrawed = true;
            unoButtonClicked = false;
            isCheckUno = false;
            firstCardDrawed = false;

            StartCoroutine(DrawFirstCard());
        }

        IEnumerator DrawFirstCard()
        {
            while (!deckScript.cardsInitialized)
                yield return new WaitForSeconds(0.1f);
            List<GameObject> firstCard = deckScript.DrawCards(1, gameObject.transform);
            discardScript.CardToPile(firstCard[0]);
            lastCardInfo = firstCard[0].GetComponent<Card>().cardInfo;
            firstCardDrawed = true;
        }

        private void OnDisable()
        {
            discardScript.DiscardOnClickHandler -= CheckValid;

            MultiGame.TurnStartHandler -= OnDraw2Draw4Played_Start;
            MultiGame.TurnEndHandler -= OnDraw2Draw4Played_End;
            MultiGame.TurnEndHandler -= UpdateLastCardInfo;
            MultiGame.TurnEndHandler -= IsNextTurn;
        }

        public void CheckValid()
        {
            GameObject currCard = cHandScript.HighlightedCard;
            if (currCard == null) return;

            Card cardScript = currCard.GetComponent<Card>();
            if (isAllowed(cardScript.cardInfo))
            {
                discardScript.PlayCard(currCard);

                switch (lastCardInfo.cardType)
                {
                    case CardType.reverse: gameScript.ToggleDirection(); break;
                    case CardType.wild: selectColorTab.SetActive(true); break;
                    case CardType.draw2: this.photonView.RPC("SetCardDrawed", gameScript.GetNextPunPlayer(), false); break;
                    case CardType.draw4: this.photonView.RPC("SetCardDrawed", gameScript.GetNextPunPlayer(), false); selectColorTab.SetActive(true); break;
                }

                cHandScript.SkipTurn();

                if (cHandScript.Cards.Count == 0)
                {
                    gameUI.AddLog(cHandScript.PlayerName + " wins!");
                    return;
                }
                else if (cHandScript.Cards.Count == 1)
                    StartCoroutine(CheckUno());

                if (lastCardInfo.cardType != CardType.wild && lastCardInfo.cardType != CardType.draw4)
                    nextTurnButton.interactable = true;
            }
        }

        bool isAllowed(CardInfo currCardInfo)
        {
            if (isAllowedHelper(currCardInfo))
            {
                lastCardInfo = currCardInfo;
                return true;
            }

            gameUI.AddLog(currentHand.GetComponent<CurrentHandMul>().PlayerName + ": Invalid play.");
            return false;
        }

        bool isAllowedHelper(CardInfo currCardInfo)
        {
            switch (currCardInfo.cardType)
            {
                case CardType.wild:
                case CardType.draw4:
                    return true;
                case CardType.draw2:
                case CardType.skip:
                case CardType.reverse:
                    return lastCardInfo.cardColor == currCardInfo.cardColor || lastCardInfo.cardType == currCardInfo.cardType;
                case CardType.num:
                    return lastCardInfo.cardColor == currCardInfo.cardColor || lastCardInfo.num == currCardInfo.num;
                default:
                    Debug.LogError("Invalid Card type.");
                    return false;
            }
        }

        /// <summary>
        /// If a draw2/draw4 card is played, deal 2/4 cards to the player
        /// </summary>
        public void OnDraw2Draw4Played_End()
        {
            if (!cardDrawed)
            {
                GameObject player = gameScript.CurrentPlayer;
                if (lastCardInfo.cardType == CardType.draw2)
                    player.GetComponent<PlayerMul>().TakeCards(gameScript.DealCards(2, player));
                else if (lastCardInfo.cardType == CardType.draw4)
                    player.GetComponent<PlayerMul>().TakeCards(gameScript.DealCards(4, player));
            }
        }

        /// <summary>
        /// If a draw2/draw4 card is played last turn, skip this turn.
        /// </summary>
        public void OnDraw2Draw4Played_Start()
        {
            if (!cardDrawed && (lastCardInfo.cardType == CardType.draw2 || lastCardInfo.cardType == CardType.draw4))
            {
                currentHand.GetComponent<CurrentHandMul>().SkipTurn();
                nextTurnButton.interactable = true;
                cardDrawed = true;
            }
        }

        void DeckDraw()
        {
            cHandScript.SkipTurn();
            cHandScript.Cards[cHandScript.Cards.Count - 1].GetComponent<CardReaction>().enabled = true;

            nextTurnButton.interactable = true;
        }

        IEnumerator CheckUno()
        {
            CurrentHandMul cHScript = currentHand.GetComponent<CurrentHandMul>();
            string player = cHScript.PlayerName;
            isNextTurn = false;
            unoButtonClicked = false;

            isCheckUno = true;
            unoButton.SetActive(true);

            gameUI.AddLog(cHandScript.PlayerName + " has 1 card left.");
            int i = 30;
            while (i > 0 && !unoButtonClicked && !isNextTurn)
            {
                if (i % 10 == 0) gameUI.AddLog((i / 10).ToString());
                yield return new WaitForSeconds(0.1f);
                i--;
            }
            unoButton.SetActive(false);

            if (!unoButtonClicked)
            {
                gameUI.AddLog(player + " did not say UNO. Draw 2 cards.");
                if (!isNextTurn)
                {
                    for (int j = 0; j < 2; j++) gameScript.DealCard();
                    cHScript.SkipTurn();
                }
            }
            else
                gameUI.AddLog(player + ": UNO!");

            isCheckUno = false;
        }

        public void UNOButtonOnClick()
        {
            unoButtonClicked = true;
        }

        public void IsNextTurn()
        {
            if (isCheckUno)
            {
                isNextTurn = true;
                GameObject player = gameScript.CurrentPlayer;
                player.GetComponent<PlayerMul>().TakeCards(gameScript.DealCards(2, player));
                isCheckUno = false;
            }
        }

        public void UpdateLastCardInfo()
        {
            this.photonView.RPC("SyncLastCardInfo", RpcTarget.Others, UNOInfo.CardInfoToArr(lastCardInfo));
        }

        #region RPCs

        [PunRPC]
        void SyncLastCardInfo(string[] cardInfoArr)
        {
            this.lastCardInfo = UNOInfo.ArrToCardInfo(cardInfoArr);
        }

        [PunRPC]
        void SetCardDrawed(bool cardDrawed)
        {
            this.cardDrawed = cardDrawed;
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


