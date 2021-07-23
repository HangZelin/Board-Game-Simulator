using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGS.Chess_3D
{
    public class BoardHighlights_mul : MonoBehaviour
    {

        public static BoardHighlights_mul Instance { set; get; }

        public GameObject highlightPrefab;
        private List<GameObject> highlights;

        private void Start()
        {
            Instance = this;
            highlights = new List<GameObject>();
        }

        private GameObject GetHighLightObject()
        {
            GameObject go = highlights.Find(g => !g.activeSelf);

            if (go == null)
            {
                go = Instantiate(highlightPrefab);
                highlights.Add(go);
            }

            return go;
        }

        public void HighLightAllowedMoves(bool[,] moves)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (moves[i, j])
                    {
                        GameObject go = GetHighLightObject();
                        go.SetActive(true);
                        go.transform.position = new Vector3(i + 0.5f, 0.0001f, j + 0.5f);
                    }
                }

            }
        }

        public void HideHighlights()
        {
            foreach (GameObject go in highlights)
                go.SetActive(false);
        }
    }
}
