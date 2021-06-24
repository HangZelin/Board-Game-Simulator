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

        public void Initialize(bool isFace)
        {
            IsFace = isFace;
        }

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.GetComponent<Draw4Card>().Initialize(isFace);
            copy.name = ToString();
            return copy;
        }

        public override string ToString()
        {
            return "Draw4";
        }
    }
}
