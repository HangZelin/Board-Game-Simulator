using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNO 
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] GameObject unused;
        [SerializeField] GameObject numCard;
        [SerializeField] GameObject skipCard;
        [SerializeField] GameObject reverseCard;
        [SerializeField] GameObject draw2Card;
        [SerializeField] GameObject draw4Card;
        [SerializeField] GameObject wildCard;

        List<GameObject> cards;

        public void Initialize()
        {
            cards = new List<GameObject>();

            for (int i = 0; i <= 9; i++)
                foreach (NumCard.Color color in Enum.GetValues(typeof(NumCard.Color)))
                {
                    GameObject card = Instantiate(numCard, unused.transform);
                    card.GetComponent<NumCard>().Initialize(color, i);
                    cards.Add(card);
                    if (i != 0)
                    {
                        card = Instantiate(numCard, unused.transform);
                        card.GetComponent<NumCard>().Initialize(color, i);
                        cards.Add(card);
                    }
                }

            foreach (SkipCard.Color color in Enum.GetValues(typeof(NumCard.Color)))
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_SkipCard = Instantiate(skipCard, unused.transform);
                    a_SkipCard.GetComponent<SkipCard>().Initialize(color);
                    cards.Add(a_SkipCard);
                }

            foreach (ReverseCard.Color color in Enum.GetValues(typeof(NumCard.Color)))
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_SkipCard = Instantiate(reverseCard, unused.transform);
                    a_SkipCard.GetComponent<ReverseCard>().Initialize(color);
                    cards.Add(a_SkipCard);
                }

            foreach (Draw2Card.Color color in Enum.GetValues(typeof(NumCard.Color)))
                for (int i = 0; i < 2; i++)
                {
                    GameObject a_Draw2Card = Instantiate(draw2Card, unused.transform);
                    a_Draw2Card.GetComponent<Draw2Card>().Initialize(color);
                    cards.Add(a_Draw2Card);
                }

            for (int i = 0; i < 4; i++)
            {
                cards.Add(Instantiate(draw4Card, unused.transform));
                cards.Add(Instantiate(wildCard, unused.transform));
            }

            Shuffle(cards);
            Debug.Log(cards.Count);
        }

        public List<GameObject> DrawCards(int num)
        {
            if (num > cards.Count)
            {
                Debug.Log("Draw cards from an empty deck.");
                return new List<GameObject>();
            }
            List<GameObject> drawCards = new List<GameObject>();
            for (int i = 0; i < num; i++)
            {
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
}

