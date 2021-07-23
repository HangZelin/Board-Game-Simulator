using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class CardReaction : MonoBehaviour, IPointerClickHandler
    {
        // Reference
        GameObject currentHand;

        // When click, the increase of card.rect.y
        [SerializeField] float y = 10f;

        bool isHighlight;
        int siblingsIndex;

        public void Initialize(GameObject currentHand)
        {
            isHighlight = false;
            this.currentHand = currentHand;
            siblingsIndex = transform.GetSiblingIndex();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isHighlight)
            {
                currentHand.GetComponent<ICurrentHand>().HighlightedCard = gameObject;
                Vector2 v = GetComponent<RectTransform>().anchoredPosition;
                v.y += this.y;
                GetComponent<RectTransform>().anchoredPosition = v;
                gameObject.transform.SetAsLastSibling();
                isHighlight = true;
            }
            else
            {
                currentHand.GetComponent<ICurrentHand>().HighlightedCard = null;
                PutBack();
            }
        }

        public void PutBack()
        {
            if (isHighlight)
            {
                Vector2 v = GetComponent<RectTransform>().anchoredPosition;
                v.y -= this.y;
                GetComponent<RectTransform>().anchoredPosition = v;
                gameObject.transform.SetSiblingIndex(siblingsIndex);
                isHighlight = false;
            }
        }

        void OnEnable()
        {
            GetComponent<Outline>().enabled = true;
        }

        void OnDisable()
        {
            GetComponent<Outline>().enabled = false;
        }
    }
}
