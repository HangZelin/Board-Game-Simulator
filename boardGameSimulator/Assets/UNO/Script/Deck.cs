using System;
using System.Collections.Generic;
using UnityEngine;

namespace UNO 
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] GameObject unused;
        [SerializeField] GameObject discard;

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

        [SerializeField] GameObject numCard;
        [SerializeField] GameObject skipCard;
        [SerializeField] GameObject reverseCard;
        [SerializeField] GameObject draw2Card;
        [SerializeField] GameObject draw4Card;
        [SerializeField] GameObject wildCard;

        public void Initialize(GameObject discard)
        {
            unused = GameObject.Find("Canvas/Unused");
            this.discard = discard;

            cards = new List<GameObject>();

            // Add cards to the deck

            // Color cards
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
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

            foreach (GameObject card in cards)
                card.name = card.GetComponent<Card>().ToString();

            Shuffle(cards);

            Debug.Log(cards.Count);
        }

        public List<GameObject> DrawCards(int num, Transform transform)
        {
            if (num > cards.Count)
            {
                discard.GetComponent<Discard>().PileToDeck();
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
    }

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
                default:
                    Debug.LogError("Failed to get color");
                    return Color.white;
            }
        }
    }

    public enum CardColor { red, yellow, blue, green };
}

