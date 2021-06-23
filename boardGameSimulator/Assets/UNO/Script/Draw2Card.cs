using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class Draw2Card : MonoBehaviour, Card
    {
        public enum Color { Red, Blue, Yellow, Green };

        [SerializeField] GameObject prefab;

        [SerializeField] Sprite redCard;
        [SerializeField] Sprite blueCard;
        [SerializeField] Sprite yellowCard;
        [SerializeField] Sprite greenCard;
        [SerializeField] Sprite cardBack;

        [SerializeField] Image midIcon;

        public Color color;

        public void Initialize(Color color)
        {
            this.color = color;
            switch (color)
            {
                case Color.Red:
                    GetComponent<Image>().sprite = redCard;
                    midIcon.color = new UnityEngine.Color(1f, 0.3333333f, 0.3333333f);
                    break;
                case Color.Blue:
                    GetComponent<Image>().sprite = blueCard;
                    midIcon.color = new UnityEngine.Color(0.3372549f, 0.3372549f, 0.9803922f);
                    break;
                case Color.Yellow:
                    GetComponent<Image>().sprite = yellowCard;
                    midIcon.color = new UnityEngine.Color(1f, 0.6666667f, 0f);
                    break;
                case Color.Green:
                    GetComponent<Image>().sprite = greenCard;
                    midIcon.color = new UnityEngine.Color(0.3372549f, 0.6627451f, 0.3372549f);
                    break;
            }
        }

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.GetComponent<Draw2Card>().Initialize(color);
            copy.name = ToString();
            return copy;
        }

        public override string ToString()
        {
            switch (color)
            {
                case Color.Blue: return "Draw2_blue";
                case Color.Green: return "Draw2_green";
                case Color.Red: return "Draw2_red";
                case Color.Yellow: return "Draw2_yellow";
            }
            Debug.Log("Failed Card ToString Method");
            return "";
        }
    }
}
