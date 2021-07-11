using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class ReverseCard : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;

        [SerializeField] Sprite redCard;
        [SerializeField] Sprite blueCard;
        [SerializeField] Sprite yellowCard;
        [SerializeField] Sprite greenCard;
        [SerializeField] Sprite cardBack;
        Sprite cardFace;

        [SerializeField] GameObject topIcon;
        [SerializeField] GameObject midIcon;
        [SerializeField] GameObject btmIcon;

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

                topIcon.SetActive(isFace);
                midIcon.SetActive(isFace);
                btmIcon.SetActive(isFace);
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

            a_CardInfo.cardType = CardType.reverse;
            a_CardInfo.cardColor = color;
            a_CardInfo.num = -1;
        }

        public override string ToString()
        {
            return "Reverse_" + color;
        }
    }
}
