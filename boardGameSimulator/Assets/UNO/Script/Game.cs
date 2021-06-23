using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNO
{

    public class Game : MonoBehaviour
    {
        [SerializeField] GameObject deck;
        Deck deckScript;

        [SerializeField] GameObject playerHand;

        private void Start()
        {
            deckScript = deck.GetComponent<Deck>();
            deckScript.Initialize();
        }

        public void DrawCard(int num)
        {
            List<GameObject> drawCards = deckScript.DrawCards(num);
            GameObject card = CopyCard(drawCards[0], playerHand.transform);
        }

        public GameObject CopyCard(GameObject origin, Transform transform)
        {
            if (origin.GetComponent<NumCard>() != null)
                return origin.GetComponent<NumCard>().Copy(transform);
            else if (origin.GetComponent<SkipCard>() != null)
                return origin.GetComponent<SkipCard>().Copy(transform);
            else if (origin.GetComponent<ReverseCard>() != null)
                return origin.GetComponent<ReverseCard>().Copy(transform);
            else if (origin.GetComponent<Draw2Card>() != null)
                return origin.GetComponent<Draw2Card>().Copy(transform);
            else if (origin.GetComponent<Draw4Card>() != null)
                return origin.GetComponent<Draw4Card>().Copy(transform);
            else if (origin.GetComponent<WildCard>() != null)
                return origin.GetComponent<WildCard>().Copy(transform);
            else return null;
        }
    }

    public interface Card
    {
        public GameObject Copy(Transform transform);
    }

}


