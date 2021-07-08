using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class Draw4Card : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Sprite blackCard;
        [SerializeField] Sprite cardBack;

        [SerializeField] GameObject topNum;
        [SerializeField] GameObject midIcon;
        [SerializeField] GameObject btmNum;

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

                topNum.SetActive(isFace);
                midIcon.SetActive(isFace);
                btmNum.SetActive(isFace);
            }
        }

        CardInfo a_CardInfo;
        public CardInfo cardInfo { get { return a_CardInfo; } }

        public void Initialize(bool isFace)
        {
            IsFace = isFace;

            a_CardInfo.cardType = CardType.draw4;
            a_CardInfo.cardColor = CardColor.black;
            a_CardInfo.num = -1;
        }

        public override string ToString()
        {
            return "Draw4";
        }
    }
}
