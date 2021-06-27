using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO 
{
    public class Rules : MonoBehaviour
    {
        GameObject currentHand;
        GameObject discard;
        GameObject deck;
        Game gameScript;
        [SerializeField] SettingsUI gameUI;
        [SerializeField] GameObject selectColorTab;
        [SerializeField] GameObject nextTurnButton;
        [SerializeField] GameObject directionIcons;
        [SerializeField] GameObject unoButton;
        bool unoButtonClicked;
        bool isNextTurn;

        CardInfo lastCardInfo;
        public CardColor lastCardColor { set { lastCardInfo.cardColor = value; } }

        public void Initialize(GameObject currentHand, GameObject discard, GameObject deck, Game gameScript)
        {
            this.currentHand = currentHand;
            this.discard = discard;
            this.deck = deck;
            deck.transform.Find("DrawACardButton").GetComponent<Button>().onClick.AddListener(delegate { DeckDraw(); });

            this.gameScript = gameScript;
            directionIcons.GetComponent<DirectionIcons>().Interactable = false;

            List<GameObject> discardCards = discard.GetComponent<Discard>().Cards;
            if (GameStatus.isNewGame || discardCards.Count == 0)
            {
                List<GameObject> firstCard = deck.GetComponent<Deck>().DrawCards(1, gameObject.transform);
                discard.GetComponent<Discard>().CardToPile(firstCard[0]);
                lastCardInfo = firstCard[0].GetComponent<Card>().cardInfo;
            }
            else
                lastCardInfo = discardCards[discardCards.Count - 1].GetComponent<Card>().cardInfo;

            unoButton.transform.SetAsLastSibling();
            unoButtonClicked = false;
        }

        private void OnEnable()
        {
            Game.TurnStartHandler += OnTurnStart;
            discard.GetComponent<Discard>().DiscardOnClickHandler += CheckValid;
        }

        private void OnDisable()
        {
            Game.TurnStartHandler -= OnTurnStart;
            discard.GetComponent<Discard>().DiscardOnClickHandler -= CheckValid;
        }

        public void CheckValid()
        {
            GameObject currCard = currentHand.GetComponent<CurrentHand>().HighlightedCard;
            if (currCard == null) return;

            Card cardScript = currCard.GetComponent<Card>();
            if (isAllowed(cardScript.cardInfo))
            {
                discard.GetComponent<Discard>().PlayCard(currCard);
                lastCardInfo = cardScript.cardInfo;

                switch (lastCardInfo.cardType)
                {
                    case CardType.reverse: gameScript.ToggleDirection(); break;
                    case CardType.wild: selectColorTab.SetActive(true); break;
                    case CardType.draw2: Game.TurnStartHandler += OnDraw2Draw4Played; break;
                    case CardType.draw4: selectColorTab.SetActive(true); Game.TurnStartHandler += OnDraw2Draw4Played; break;
                }

                currentHand.GetComponent<CurrentHand>().SkipTurn();

                if (currentHand.GetComponent<CurrentHand>().Cards.Count == 0)
                {
                    gameUI.AddLog(currentHand.GetComponent<CurrentHand>().PlayerName + "wins!");
                    return;
                }
                else if (currentHand.GetComponent<CurrentHand>().Cards.Count == 1)
                    StartCoroutine(CheckUno());
                
                if (lastCardInfo.cardType != CardType.wild && lastCardInfo.cardType != CardType.draw4)
                    nextTurnButton.GetComponent<Button>().interactable = true;
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

        void OnDraw2Draw4Played()
        {
            if (lastCardInfo.cardType == CardType.draw2)
                for (int i = 0; i < 2; i++) gameScript.DealCard();
            else if (lastCardInfo.cardType == CardType.draw4)
                for (int i = 0; i < 4; i++) gameScript.DealCard();
            currentHand.GetComponent<CurrentHand>().SkipTurn();
            Game.TurnStartHandler -= OnDraw2Draw4Played;

            nextTurnButton.GetComponent<Button>().interactable = true;
        }

        void DeckDraw()
        {
            CurrentHand cHScript = currentHand.GetComponent<CurrentHand>();
            cHScript.SkipTurn();
            cHScript.Cards[cHScript.Cards.Count - 1].GetComponent<CardReaction>().enabled = true;

            nextTurnButton.GetComponent<Button>().interactable = true;
        }

        void OnTurnStart()
        {
            nextTurnButton.GetComponent<Button>().interactable = false;
            deck.GetComponent<Deck>().Interactable = true;
        }

        IEnumerator CheckUno()
        {
            CurrentHand cHScript = currentHand.GetComponent<CurrentHand>();
            string player = currentHand.GetComponent<CurrentHand>().PlayerName;
            isNextTurn = false;
            unoButtonClicked = false;

            Game.TurnEndHandler += IsNextTurn;
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
                Game.TurnEndHandler -= IsNextTurn;
            }
            else
            {
                gameUI.AddLog(player + ": UNO!");
                Game.TurnEndHandler -= IsNextTurn;
            }
        }

        public void UNOButtonOnClick()
        {
            unoButtonClicked = true;
        }

        void IsNextTurn()
        {
            isNextTurn = true;
            GameObject player = gameScript.CurrentPlayer;
            player.GetComponent<Player>().TakeCards(gameScript.DealCards(2, player));
            Game.TurnEndHandler -= IsNextTurn;
        }
    }
}


