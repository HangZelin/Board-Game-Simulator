using UnityEngine;
using UnityEngine.UI;

namespace UNO
{
    public class WildCard : MonoBehaviour, Card
    {
        [SerializeField] GameObject prefab;

        public GameObject Copy(Transform transform)
        {
            GameObject copy = Instantiate(prefab, transform);
            copy.name = ToString();
            return copy;
        }

        public override string ToString()
        {
            return "Wild";
        }
    }
}
