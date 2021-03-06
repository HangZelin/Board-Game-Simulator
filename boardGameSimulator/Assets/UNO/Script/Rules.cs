using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO 
{
    public class Rules : MonoBehaviour, ISaveable
    {
        Game gameScript;
        GameObject currentHand;
        Discard discardScript;
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

        CardInfo lastCardInfo;
        public CardColor lastCardColor { set { lastCardInfo.cardColor = value; } }

        public void Initialize(GameObject currentHand, GameObject discard, GameObject deck)
        {
            gameScript = GetComponent<Game>();
            this.currentHand = currentHand;
            discardScript = discard.GetComponent<Discard>();
            discard.GetComponent<Discard>().DiscardOnClickHandler += CheckValid;
            deck.GetComponent<Deck>().drawACardButton.onClick.AddListener(delegate { DeckDraw(); });

            List<GameObject> discardCards = discard.GetComponent<Discard>().Cards;
            if (GameStatus.isNewGame)
            {
                List<GameObject> firstCard = deck.GetComponent<Deck>().DrawCards(1, gameObject.transform);
                discard.GetComponent<Discard>().CardToPile(firstCard[0]);
                lastCardInfo = firstCard[0].GetComponent<Card>().cardInfo;
            }
            else
            {
                CardInfo info = discardCards[discardCards.Count - 1].GetComponent<Card>().cardInfo;
                lastCardInfo.cardType = info.cardType;
                lastCardInfo.num = info.num;

                if (info.cardType == CardType.draw4 || info.cardType == CardType.wild)
                {
                    gameUI.AddLog(string.Format("Last player chose <color={0}>{1}</color>.",
                        Colors.ColorToHex(lastCardInfo.cardColor), lastCardInfo.cardColor));
                }
            }

            unoButton.transform.SetAsLastSibling();
            cardDrawed = GameStatus.isNewGame;
            unoButtonClicked = false;
            isCheckUno = false;
        }

        void OnEnable()
        {
            SaveLoadManager.OnSaveHandler += PopulateSaveData;
            SaveLoadManager.OnLoadHandler += LoadFromSaveData;
        }

        private void OnDisable()
        {
            SaveLoadManager.OnSaveHandler -= PopulateSaveData;
            SaveLoadManager.OnLoadHandler -= LoadFromSaveData;

            discardScript.DiscardOnClickHandler -= CheckValid;

            Game.TurnStartHandler -= OnDraw2Draw4Played_Start;
            Game.TurnEndHandler -= OnDraw2Draw4Played_End;
            Game.TurnEndHandler -= IsNextTurn;
        }

        public void CheckValid()
        {
            CurrentHand cHScript = currentHand.GetComponent<CurrentHand>();
            GameObject currCard = cHScript.HighlightedCard;
            if (currCard == null) return;

            Card cardScript = currCard.GetComponent<Card>();
            if (isAllowed(cardScript.cardInfo))
            {
                discardScript.PlayCard(currCard);
                lastCardInfo = cardScript.cardInfo;

                switch (lastCardInfo.cardType)
                {
                    case CardType.reverse: gameScript.ToggleDirection(); break;
                    case CardType.wild: selectColorTab.SetActive(true); break;
                    case CardType.draw2: cardDrawed = false; break;
                    case CardType.draw4: cardDrawed = false; selectColorTab.SetActive(true); break;
                }

                cHScript.SkipTurn();

                if (cHScript.Cards.Count == 0)
                {
                    gameUI.AddLog(cHScript.PlayerName + " wins!");
                    return;
                }
                else if (cHScript.Cards.Count == 1)
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

            gameUI.AddLog(currentHand.GetComponent<CurrentHand>().PlayerName + ": Invalid play.");
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
                    player.GetComponent<Player>().TakeCards(gameScript.DealCards(2, player));
                else if (lastCardInfo.cardType == CardType.draw4)
                    player.GetComponent<Player>().TakeCards(gameScript.DealCards(4, player));
            }
        }

        /// <summary>
        /// If a draw2/draw4 card is played last turn, skip this turn.
        /// </summary>
        public void OnDraw2Draw4Played_Start()
        {
            if (!cardDrawed && (lastCardInfo.cardType == CardType.draw2 || lastCardInfo.cardType == CardType.draw4))
            {
                currentHand.GetComponent<CurrentHand>().SkipTurn();
                nextTurnButton.interactable = true;
                cardDrawed = true;
            }
        }

        IEnumerator CheckUno()
        {
            CurrentHand cHScript = currentHand.GetComponent<CurrentHand>();
            string player = cHScript.PlayerName;
            isNextTurn = false;
            unoButtonClicked = false;

            isCheckUno = true;
            unoButton.SetActive(true);

            gameUI.AddLog(currentHand.GetComponent<CurrentHand>().PlayerName + " has 1 card left.");
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

        public void IsNextTurn()
        {
            if (isCheckUno)
            {
                isNextTurn = true;
                GameObject player = gameScript.CurrentPlayer;
                player.GetComponent<Player>().TakeCards(gameScript.DealCards(2, player));
                isCheckUno = false;
            }
        }

        #region Button Callbacks

        public void UNOButtonOnClick()
        {
            unoButtonClicked = true;
        }

        void DeckDraw()
        {
            CurrentHand cHScript = currentHand.GetComponent<CurrentHand>();
            cHScript.SkipTurn();
            cHScript.Cards[cHScript.Cards.Count - 1].GetComponent<CardReaction>().enabled = true;
            nextTurnButton.interactable = true;
        }

        #endregion

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData sd)
        {
            sd.unoSaveData.lastCardColor = lastCardInfo.cardColor.ToString();
        }

        public void LoadFromSaveData(SaveData sd)
        {
            Enum.TryParse<CardColor>(sd.unoSaveData.lastCardColor, out lastCardInfo.cardColor);
        }

        #endregion
    }
}


