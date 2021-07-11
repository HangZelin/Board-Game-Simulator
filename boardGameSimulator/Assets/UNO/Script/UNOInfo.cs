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
    }

    public struct CardInfo
    {
        public CardType cardType;
        public CardColor cardColor;
        public int num;
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

        // helper
        public static List<string> CardToList(GameObject card)
        {
            if (card.GetComponent<Card>() == null)
            {
                Debug.LogError("Invalid input.");
                return new List<string>();
            }

            CardInfo cardInfo = card.GetComponent<Card>().cardInfo;
            List<string> list = new List<string>();
            list.Add(cardInfo.cardType.ToString());
            list.Add(cardInfo.cardColor.ToString());
            list.Add(cardInfo.num.ToString());
            return list;
        }

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
            GameObject card = null;

            switch (type)
            {
                case CardType.draw2:
                    card = Instantiate(draw2Card, temp.transform);
                    card.GetComponent<Draw2Card>().Initialize(color, true);
                    break;
                case CardType.num:
                    card = Instantiate(numCard, temp.transform);
                    card.GetComponent<NumCard>().Initialize(color, num, true);
                    break;
                case CardType.draw4:
                    card = Instantiate(draw4Card, temp.transform);
                    card.GetComponent<Draw4Card>().Initialize(true);
                    break;
                case CardType.reverse:
                    card = Instantiate(reverseCard, temp.transform);
                    card.GetComponent<ReverseCard>().Initialize(color, true);
                    break;
                case CardType.skip:
                    card = Instantiate(skipCard, temp.transform);
                    card.GetComponent<SkipCard>().Initialize(color, true);
                    break;
                case CardType.wild:
                    card = Instantiate(wildCard, temp.transform);
                    card.GetComponent<WildCard>().Initialize(true);
                    break;
                default:
                    Debug.LogError("Invalid Card Type");
                    break;
            }

            card.name = card.GetComponent<Card>().ToString();
            return card;
        }
    }
}

