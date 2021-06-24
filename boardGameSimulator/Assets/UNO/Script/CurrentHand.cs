using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
   public class CurrentHand : MonoBehaviour
    {
        List<GameObject> cards;

        public void Initialize()
        {
            cards = new List<GameObject>();
        }

        public void PlaceCards(List<GameObject> cards)
        {
            float x = -((cards.Count-1f) / 2f) * 20f;
            float y = 10f;
            foreach (GameObject card in cards)
            {
                GameObject a_Card = card.GetComponent<Card>().Copy(transform);
                this.cards.Add(a_Card);
                a_Card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                x += 20f;
            }
        }
    }
}

