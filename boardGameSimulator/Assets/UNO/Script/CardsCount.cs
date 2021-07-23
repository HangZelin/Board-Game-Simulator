using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class CardsCount : MonoBehaviour
    {
        [SerializeField] Text text;
        
        public void Initialize(int cardsCount)
        {
            text.text = cardsCount + " Left";
        }

        /// <summary>
        /// Change display card count to given card count.
        /// </summary>
        /// <param name="cardsCount">Current card count.</param>
        public void OnCardsCountChanged(int cardsCount)
        {
            text.text = cardsCount + " Left";
        }
    }
}


