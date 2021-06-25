using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class NumCard : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;

        [SerializeField] Sprite redCard;
        [SerializeField] Sprite blueCard;
        [SerializeField] Sprite yellowCard;
        [SerializeField] Sprite greenCard;
        [SerializeField] Sprite cardBack;
        Sprite cardFace;

        [SerializeField] GameObject topNum;
        [SerializeField] GameObject midNum;
        [SerializeField] GameObject btmNum;

        [SerializeField] GameObject underline1;
        [SerializeField] GameObject underline2;
        [SerializeField] GameObject underline3;

        private int num;
        public int Num
        {
            get { return num; }
            set
            {
                if (value >= 0 && value <= 9) num = value;
                else num = 0;
            }
        }

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
                midNum.SetActive(isFace);
                btmNum.SetActive(isFace);

                SetUnderline(isFace);
            }
        }

        public void Initialize(CardColor color, int num, bool isFace)
        {
            this.color = color;
            midNum.GetComponent<Text>().color = Colors.GetColor(color);
            switch (color)
            {
                case CardColor.red: cardFace = redCard; break;
                case CardColor.blue: cardFace = blueCard; break;
                case CardColor.yellow: cardFace = yellowCard; break;
                case CardColor.green: cardFace = greenCard; break;
            }
            GetComponent<Image>().sprite = cardFace;

            Num = num;
            topNum.GetComponent<Text>().text = num.ToString();
            midNum.GetComponent<Text>().text = num.ToString();
            btmNum.GetComponent<Text>().text = num.ToString();

            SetUnderline(true);

            IsFace = isFace;
        }

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.GetComponent<NumCard>().Initialize(color, num, isFace);
            copy.name = ToString();
            return copy;
        }

        public void SetUnderline(bool isActive)
        {
            if (num == 6 || num == 9)
            {
                underline1.SetActive(isActive);
                underline2.SetActive(isActive);
                underline3.SetActive(isActive);
            }
        }

        public override string ToString()
        {
            return num + "_" + color;
        }
    }
}
