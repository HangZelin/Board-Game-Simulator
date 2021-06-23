using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{
    public class Player : MonoBehaviour
    {
        string playerName;
        List<GameObject> cards;

        public void Initialize(string playerName)
        {
            this.playerName = playerName;

            cards = new List<GameObject>();
        }

        public void TakeCards(List<GameObject> cards)
        {
            this.cards.AddRange(cards);
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
