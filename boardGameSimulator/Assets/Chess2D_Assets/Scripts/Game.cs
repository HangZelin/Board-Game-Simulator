using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace BGS.Chess_2D
{
    public class Game : MonoBehaviour, ISaveable
    {
        public GameObject chesspiece;
        public GameObject gameUI;
        public GameObject Board;
        SettingsUI settings;

        //Positions and team for other chesspiece
        private GameObject[,] positions = new GameObject[8, 8];
        private GameObject[] playerBlack = new GameObject[16];
        private GameObject[] playerWhite = new GameObject[16];

        private string Player1 = GameStatus.GetNameOfPlayer(1);
        private string Player2 = GameStatus.GetNameOfPlayer(2);

        private string currentPlayer;

        private bool gameOver = false;

        // Start is called before the first frame update
        void Start()
        {
            settings = gameUI.GetComponent<SettingsUI>();

            if (!GameStatus.isNewGame)
            {
                LoadFromSaveData(SaveLoadManager.tempSD);
                GameStatus.isNewGame = true;
                settings.AddLog(GameStatus.GetNameOfGame() + ": Load Complete.");
                return;
            }

            settings.Initialize();

            //Gamemode Initialization;
            Debug.Log("Hotseat Mode");

            Initialized();
        }

        public void Initialized()
        {

            Board.SetActive(true);

            //Instantiate the chesspiece when the game starts.

            currentPlayer = Player1;
            CreatePiecesforP1();
            CreatePiecesforP2();



            //Set all pieces positions on the position board
            for (int i = 0; i < playerBlack.Length; i++)
            {
                SetPosition(playerBlack[i]);
                SetPosition(playerWhite[i]);
            }

            // Log
            settings.AddLog(GameStatus.GetNameOfGame() + ": New Game.");
        }

        public GameObject Create(string name, int x, int y)
        {
            GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);

            Chessman cm = obj.GetComponent<Chessman>();
            cm.name = name;
            cm.SetXBoard(x);
            cm.SetYBoard(y);
            cm.Activate();

            return obj;
        }

        public void CreatePiecesforP1()
        {
            playerBlack = new GameObject[]
          {
            Create("black_rook", 0, 7),Create("black_knight", 1, 7),Create("black_bishop", 2, 7), Create("black_queen", 3, 7), Create("black_king", 4, 7),
            Create("black_bishop", 5, 7),Create("black_knight", 6, 7),Create("black_rook", 7, 7),Create("black_pawn", 0, 6),Create("black_pawn", 1, 6),
            Create("black_pawn", 2, 6),Create("black_pawn", 3, 6),Create("black_pawn", 4, 6),Create("black_pawn", 5, 6),Create("black_pawn", 6, 6),
            Create("black_pawn", 7, 6)
          };
        }

        public void CreatePiecesforP2()
        {
            playerWhite = new GameObject[]
           {
            Create("white_rook", 0, 0),Create("white_knight", 1, 0),Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0),Create("white_knight", 6, 0),Create("white_rook", 7, 0),Create("white_pawn", 0, 1),Create("white_pawn", 1, 1),
            Create("white_pawn", 2, 1),Create("white_pawn", 3, 1),Create("white_pawn", 4, 1),Create("white_pawn", 5, 1),Create("white_pawn", 6, 1),
            Create("white_pawn", 7, 1)
           };
        }
        public void SetPosition(GameObject obj)
        {
            Chessman cm = obj.GetComponent<Chessman>();

            positions[cm.GetXBoard(), cm.GetYBoard()] = obj;


        }

        public void SetPositionEmpty(int x, int y)
        {
            positions[x, y] = null;
        }

        public GameObject GetPosition(int x, int y)
        {
            return positions[x, y];
        }

        public bool PositionOnBoard(int x, int y)
        {
            if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1))
            {
                return false;
            }
            return true;
        }

        public string GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public bool IsGameOver()
        {
            return gameOver;
        }

        public void NextTurn()
        {
            if (currentPlayer == Player1)
            {
                currentPlayer = Player2;
            }
            else
            {
                currentPlayer = Player1;
            }

            // Log 

            settings.AddLog("<b>" + currentPlayer + "</b>'s turn!");
        }

        public void Update()
        {
            if (gameOver == true && Input.GetMouseButtonDown(0))
            {
                gameOver = false;
                GameStatus.isNewGame = true;

                SceneManager.LoadScene("Chess (2D)");//Reset the game for us.
            }
        }

        public void GameWinner(string winner)
        {
            gameOver = true;

            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = winner + " is the winner";

            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

            settings.AddLog("<b>" + Player1 + "</b> is the winner! " + "Tap to restart.");
        }

        public void Winner1()
        {
            gameOver = true;

            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = Player1 + " is the winner";

            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

            settings.AddLog("<b>" + Player1 + "</b> is the winner! " + "Tap to restart.");
        }

        public void Winner2()
        {
            gameOver = true;

            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = Player2 + " is the winner";

            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

            settings.AddLog("<b>" + Player2 + "</b> is the winner! " + "Tap to restart.");
        }

        // Save and load

        public void PopulateSaveData(SaveData sd)
        {
            List<MarkerPosition> markerPositions = new List<MarkerPosition>();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (positions[i, j] != null)
                    {
                        MarkerPosition mp = new MarkerPosition();
                        mp.marker = positions[i, j].name;
                        mp.num = i * 8 + j;
                        markerPositions.Add(mp);
                    }
                }

            sd.playerInTurn = currentPlayer.Equals(Player1) ? 1 : 2;
            sd.markerPositions = markerPositions;
        }

        public void LoadFromSaveData(SaveData sd)
        {
            currentPlayer = GameStatus.GetNameOfPlayer(sd.playerInTurn);
            foreach (MarkerPosition mp in sd.markerPositions)
            {
                GameObject go = Create(mp.marker, mp.num / 8, mp.num % 8);
                SetPosition(go);
            }
        }

        // 

        void OnEnable()
        {
            SceneLoader.backToGame += ActivateChessman;
        }

        void OnDisable()
        {

            SceneLoader.backToGame -= ActivateChessman;
        }

        // helper for IngameLoad

        public void DeactivateChessman()
        {
            foreach (GameObject go in positions)
                if (go != null) go.SetActive(false);

            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            foreach (GameObject go in movePlates)
                go.SetActive(false);
        }

        public void ActivateChessman()
        {
            foreach (GameObject go in positions)
                if (go != null) go.SetActive(true);

            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            foreach (GameObject go in movePlates)
                go.SetActive(true);
        }
    }
}
