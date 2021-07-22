using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class Draw2Card : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;

        [SerializeField] Sprite redCard;
        [SerializeField] Sprite blueCard;
        [SerializeField] Sprite yellowCard;
        [SerializeField] Sprite greenCard;
        [SerializeField] Sprite cardBack;
        Sprite cardFace;

        [SerializeField] GameObject topNum;
        [SerializeField] GameObject midIcon;
        [SerializeField] GameObject btmNum;

        public CardColor color;

        [SerializeField] bool isFace;
        public bool IsFace 
        { 
            get { return isFace; }
            set
            {
                isFace = value;
                if (isFace)
                    GetComponent<Image>().sprite = cardFace;
                else
                    GetComponent<Image>().sprite = cardBack;

                topNum.SetActive(isFace);
                midIcon.SetActive(isFace);
                btmNum.SetActive(isFace);
            }
        }

        CardInfo a_CardInfo;
        public CardInfo cardInfo { get { return a_CardInfo; } }

        public void Initialize(CardColor color, bool isFace)
        {
            this.color = color;
            midIcon.GetComponent<Image>().color = Colors.GetColor(color);
            switch (color)
            {
                case CardColor.red: cardFace = redCard; break;
                case CardColor.blue: cardFace = blueCard; break;
                case CardColor.yellow: cardFace = yellowCard; break;
                case CardColor.green: cardFace = greenCard; break;
            }
            GetComponent<Image>().sprite = cardFace;

            IsFace = isFace;

            a_CardInfo.cardType = CardType.draw2;
            a_CardInfo.cardColor = color;
            a_CardInfo.num = -1;
        }

        public override string ToString()
        {
            return "Draw2_" + color;
        }
    }
}
