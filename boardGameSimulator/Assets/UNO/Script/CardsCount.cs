using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class CardsCount : MonoBehaviour
    {
        [SerializeField] GameObject container;
        IContainer containerScript;
        [SerializeField] Text text;

        private void Start()
        {
            containerScript = container.GetComponent<IContainer>();
            text.text = containerScript.Cards.Count + " Left";
        }

        void Update()
        {
            text.text = containerScript.Cards.Count + " Left";
        }
    }
}


