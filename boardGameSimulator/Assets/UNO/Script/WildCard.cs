using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class WildCard : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Sprite blackCard;
        [SerializeField] Sprite cardBack;

        [SerializeField] GameObject topIcon;
        [SerializeField] GameObject midIcon;
        [SerializeField] GameObject btmIcon;

        [SerializeField] bool isFace;
        public bool IsFace
        {
            get { return isFace; }
            set
            {
                isFace = value;
                if (isFace)
                    GetComponent<Image>().sprite = blackCard;
                else
                    GetComponent<Image>().sprite = cardBack;

                topIcon.SetActive(isFace);
                midIcon.SetActive(isFace);
                btmIcon.SetActive(isFace);
            }
        }

        CardInfo a_CardInfo;
        public CardInfo cardInfo { get { return a_CardInfo; } }

        public void Initialize(bool isFace)
        {
            IsFace = isFace;
            
            a_CardInfo.cardType = CardType.wild;
            a_CardInfo.cardColor = CardColor.black;
            a_CardInfo.num = -1;
        }

        public override string ToString()
        {
            return "Wild";
        }
    }
}
