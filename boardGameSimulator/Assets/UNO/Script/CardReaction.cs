using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UNO
{
    public class CardReaction : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject currentHand;

        [SerializeField] float y = 10f;
        [SerializeField] bool isHighlight = false;
        [SerializeField] int siblingsIndex;

        public void Initialize()
        {
            siblingsIndex = transform.GetSiblingIndex();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isHighlight)
            {
                currentHand.GetComponent<CurrentHand>().HighlightedCard = gameObject;
                Vector2 v = GetComponent<RectTransform>().anchoredPosition;
                v.y += this.y;
                GetComponent<RectTransform>().anchoredPosition = v;
                gameObject.transform.SetAsLastSibling();
                isHighlight = true;
            }
            else
            {
                currentHand.GetComponent<CurrentHand>().HighlightedCard = null;
                PutBack();
            }
        }

        public void PutBack()
        {
            Vector2 v = GetComponent<RectTransform>().anchoredPosition;
            v.y -= this.y;
            GetComponent<RectTransform>().anchoredPosition = v;
            gameObject.transform.SetSiblingIndex(siblingsIndex);
            isHighlight = false;
        }
    }
}
