using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class DirectionIcons : MonoBehaviour
    {
        [SerializeField] Button btmLeft;
        [SerializeField] Button btmRight;

        [SerializeField] GameObject antiCIcon_1;
        [SerializeField] GameObject antiCIcon_2;
        [SerializeField] GameObject cIcon_1;
        [SerializeField] GameObject cIcon_2;

        public void DirectionIconToggle(bool isClockWise)
        {
            if (isClockWise)
            {
                antiCIcon_1.SetActive(false);
                antiCIcon_2.SetActive(false);
                cIcon_1.SetActive(true);
                cIcon_2.SetActive(true);
            }
            else
            {
                cIcon_1.SetActive(false);
                cIcon_2.SetActive(false);
                antiCIcon_1.SetActive(true);
                antiCIcon_2.SetActive(true);
            }
        }

        public bool Interactable
        {
            get { return btmLeft.interactable; }
            set
            {
                btmLeft.interactable = value;
                btmRight.interactable = value;
            }
        }
    }
}




