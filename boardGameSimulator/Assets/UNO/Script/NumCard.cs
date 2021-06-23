using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class NumCard : MonoBehaviour, Card
    {
        public enum Color { Red, Blue, Yellow, Green };

        [SerializeField] GameObject prefab;

        [SerializeField] Sprite redCard;
        [SerializeField] Sprite blueCard;
        [SerializeField] Sprite yellowCard;
        [SerializeField] Sprite greenCard;
        [SerializeField] Sprite cardBack;

        [SerializeField] Text topNum;
        [SerializeField] Text midNum;
        [SerializeField] Text btmNum;

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
        public Color color;

        public void Initialize(Color color, int num)
        {
            this.color = color;
            switch (color)
            {
                case Color.Red: 
                    GetComponent<Image>().sprite = redCard;
                    midNum.color = new UnityEngine.Color(1f, 0.3333333f, 0.3333333f);
                    break;
                case Color.Blue: 
                    GetComponent<Image>().sprite = blueCard;
                    midNum.color = new UnityEngine.Color(0.3372549f, 0.3372549f, 0.9803922f);
                    break;
                case Color.Yellow: 
                    GetComponent<Image>().sprite = yellowCard;
                    midNum.color = new UnityEngine.Color(1f, 0.6666667f, 0f);
                    break;
                case Color.Green: 
                    GetComponent<Image>().sprite = greenCard;
                    midNum.color = new UnityEngine.Color(0.3372549f, 0.6627451f, 0.3372549f);
                    break;
            }

            Num = num;
            topNum.text = num.ToString();
            midNum.text = num.ToString();
            btmNum.text = num.ToString();

            if (num == 6 || num == 9)
            {
                underline1.SetActive(true);
                underline2.SetActive(true);
                underline3.SetActive(true);
            }
        }

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.GetComponent<NumCard>().Initialize(color, num);
            copy.name = ToString();
            return copy;
        }

        public override string ToString()
        {
            switch (color)
            {
                case Color.Blue: return num + "blue";
                case Color.Green: return num + "green";
                case Color.Red: return num + "red";
                case Color.Yellow: return num + "yellow";
            }
            Debug.Log("Failed Card ToString Method");
            return "";
        }
    }
}
