using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class SelectColorTabMul : MonoBehaviour
    {
        [SerializeField] GameObject yellow;
        [SerializeField] GameObject green;
        [SerializeField] GameObject blue;
        [SerializeField] GameObject red;

        [SerializeField] GameObject nextTurnButton;
        [SerializeField] RulesMul rules;

        public CardColor color;

        private void OnEnable()
        {
            nextTurnButton.GetComponent<Button>().interactable = false;
        }

        private void OnDisable()
        {
            DisableOutlines();
        }

        public void SetLastCardColor()
        {
            DisableOutlines();
            gameObject.SetActive(false);
        }

        public void YellowOnClick()
        {
            DisableOutlines();
            yellow.GetComponent<Outline>().enabled = true;
            nextTurnButton.GetComponent<Button>().interactable = true;
            color = CardColor.yellow;
        }

        public void GreenOnClick()
        {
            DisableOutlines();
            green.GetComponent<Outline>().enabled = true;
            nextTurnButton.GetComponent<Button>().interactable = true;
            color = CardColor.green;
        }

        public void BlueOnClick()
        {
            DisableOutlines();
            blue.GetComponent<Outline>().enabled = true;
            nextTurnButton.GetComponent<Button>().interactable = true;
            color = CardColor.blue;
        }

        public void RedOnClick()
        {
            DisableOutlines();
            red.GetComponent<Outline>().enabled = true;
            nextTurnButton.GetComponent<Button>().interactable = true;
            color = CardColor.red;
        }

        // helper

        void DisableOutlines()
        {
            yellow.GetComponent<Outline>().enabled = false;
            green.GetComponent<Outline>().enabled = false;
            blue.GetComponent<Outline>().enabled = false;
            red.GetComponent<Outline>().enabled = false;
        }
    }
}
