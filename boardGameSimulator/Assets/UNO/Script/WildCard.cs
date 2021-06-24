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

        public void Initialize(bool isFace)
        {
            IsFace = isFace;
        }

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.GetComponent<WildCard>().Initialize(isFace);
            copy.name = ToString();
            return copy;
        }

        public override string ToString()
        {
            return "Wild";
        }
    }
}
