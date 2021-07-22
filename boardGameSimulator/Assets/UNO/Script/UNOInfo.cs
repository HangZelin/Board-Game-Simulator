using System;
using System.Collections.Generic;
using UnityEngine;

namespace BGS.UNO
{
    public enum CardType { num, draw2, draw4, reverse, skip, wild }
    public enum CardColor { red, yellow, blue, green, black };

    public class Colors
    {
        [SerializeField] static Color red = new Color(1f, 0.3333333f, 0.3333333f);
        [SerializeField] static Color blue = new Color(0.3372549f, 0.3372549f, 0.9803922f);
        [SerializeField] static Color green = new Color(0.3372549f, 0.6627451f, 0.3372549f);
        [SerializeField] static Color yellow = new Color(1f, 0.6666667f, 0f);

        public static Color GetColor(CardColor cardColor)
        {
            switch (cardColor)
            {
                case CardColor.red: return red;
                case CardColor.blue: return blue;
                case CardColor.green: return green;
                case CardColor.yellow: return yellow;
                case CardColor.black: return Color.black;
                default:
                    Debug.LogError("Failed to get color");
                    return Color.white;
            }
        }

        public static string ColorToHex(CardColor color)
        {
            switch (color)
            {
                case CardColor.red: return "#FF5555";
                case CardColor.blue: return "#5555FD";
                case CardColor.green: return "#55AA55";
                case CardColor.yellow: return "#FFAA00";
                default:
                    Debug.LogError("Failed to get color");
                    return "#FFFFFF";
            }
        }
    }

    public struct CardInfo
    {
        public CardType cardType;
        public CardColor cardColor;
        public int num;

        public CardInfo(CardType cardType, CardColor cardColor, int num)
        {
            this.cardType = cardType;
            this.cardColor = cardColor;
            this.num = num;
        }
    }

    public class UNOInfo : MonoBehaviour
    {
        [SerializeField] GameObject numCard;
        [SerializeField] GameObject skipCard;
        [SerializeField] GameObject reverseCard;
        [SerializeField] GameObject draw2Card;
        [SerializeField] GameObject draw4Card;
        [SerializeField] GameObject wildCard;

        [SerializeField] GameObject temp;

        public static PlayerCards CardsToSaveStruct(string playerName, List<GameObject> cards)
        {
            PlayerCards playerCards;
            playerCards.playerName = playerName;
            playerCards.listCounts = new List<int>();
            playerCards.cards = new List<string>();

            foreach (GameObject card in cards)
            {
                List<string> list = CardToList(card);
                playerCards.listCounts.Add(list.Count);
                playerCards.cards.AddRange(list);
            }
            return playerCards;
        }

        /// <summary>
        /// Serialize a uno card into string list, which contains its card info.
        /// </summary>
        /// <param name="card">Card to serialize.</param>
        /// <returns>String list containing the card's card info.</returns>
        public static List<string> CardToList(GameObject card)
        {
            if (card.GetComponent<Card>() == null)
            {
                // Given gameobject is not a card
                Debug.LogError("Invalid input.");
                return new List<string>();
            }

            CardInfo cardInfo = card.GetComponent<Card>().cardInfo;
            return new List<string> { 
                cardInfo.cardType.ToString(), 
                cardInfo.cardColor.ToString(), 
                cardInfo.num.ToString() };
        }

        /// <summary>
        /// Instantiate a card by string list. Card's transform is set under temp.
        /// </summary>
        /// <param name="list">String list containing the card's card info.</param>
        /// <returns>Card instantiated by string list.</returns>
        public GameObject ListToCard(List<string> list)
        {
            if (list.Count != 3)
            {
                Debug.LogError("Invalid input.");
                return null;
            }

            CardType type;
            CardColor color;
            Enum.TryParse<CardType>(list[0], out type);
            Enum.TryParse<CardColor>(list[1], out color);
            int num = int.Parse(list[2]);

            return InfoToCard(new CardInfo(type, color, num), temp.transform);
        }

        /// <summary>
        /// Instantiate a card by card info, set its transform under target transform.
        /// </summary>
        /// <param name="info">Card info of card to be instantiated.</param>
        /// <param name="targetTransform">Target transform of instantiated card.</param>
        /// <returns></returns>
        public GameObject InfoToCard(CardInfo info, Transform targetTransform)
        {
            CardType type = info.cardType;
            CardColor color = info.cardColor;
            int num = info.num;
            GameObject card = null;

            switch (type)
            {
                case CardType.draw2:
                    card = Instantiate(draw2Card, targetTransform);
                    card.GetComponent<Draw2Card>().Initialize(color, true);
                    break;
                case CardType.num:
                    card = Instantiate(numCard, targetTransform);
                    card.GetComponent<NumCard>().Initialize(color, num, true);
                    break;
                case CardType.draw4:
                    card = Instantiate(draw4Card, targetTransform);
                    card.GetComponent<Draw4Card>().Initialize(true);
                    break;
                case CardType.reverse:
                    card = Instantiate(reverseCard, targetTransform);
                    card.GetComponent<ReverseCard>().Initialize(color, true);
                    break;
                case CardType.skip:
                    card = Instantiate(skipCard, targetTransform);
                    card.GetComponent<SkipCard>().Initialize(color, true);
                    break;
                case CardType.wild:
                    card = Instantiate(wildCard, targetTransform);
                    card.GetComponent<WildCard>().Initialize(true);
                    break;
                default:
                    Debug.LogError("Invalid Card Type");
                    break;
            }

            card.name = card.GetComponent<Card>().ToString();
            return card;
        }

        /// <summary>
        /// Serialize card info to string array.
        /// </summary>
        /// <param name="cardInfo">Card info to serialize.</param>
        /// <returns>String array that contains card info.</returns>
        public static string[] CardInfoToArr(CardInfo cardInfo)
        {
            return new string[] { cardInfo.cardType.ToString(), cardInfo.cardColor.ToString(), cardInfo.num.ToString()};
        }

        /// <summary>
        /// Deserialize string array to card info.
        /// </summary>
        /// <param name="arr">String array that contains card info.</param>
        /// <returns>Card info derived from string array.</returns>
        public static CardInfo ArrToCardInfo(string[] arr)
        {
            CardInfo cardInfo;
            Enum.TryParse(arr[0], out cardInfo.cardType);
            Enum.TryParse(arr[1], out cardInfo.cardColor);
            cardInfo.num = int.Parse(arr[2]);
            return cardInfo;
        }
    }
}

