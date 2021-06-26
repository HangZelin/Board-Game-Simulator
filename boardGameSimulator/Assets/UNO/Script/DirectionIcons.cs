using UnityEngine;

namespace UNO
{
    public class DirectionIcons : MonoBehaviour
    {
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
    }

}

