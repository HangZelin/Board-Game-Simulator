﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManager_mul : MonoBehaviourPunCallbacks, IPunObservable, ISaveable
{
    public static BoardManager_mul Instance { get; set; }
    public GameObject gameUI;
    SettingsUI settings;
    private bool[,] allowedMoves { get; set; }

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private Quaternion whiteOrientation = Quaternion.Euler(0, 270, 0);
    private Quaternion blackOrientation = Quaternion.Euler(0, 90, 0);

    public Chessplayer[,] Chessmans { get; set; }
    private Chessplayer selectedChessman;

    public bool isWhiteTurn = true;

    private Material previousMat;
    public Material selectedMat;

    //Sounds
    public AudioClip Sound_Move;
    public AudioClip Sound_Eat;
    public AudioClip Sound_Capture;
    private AudioSource audio_source;
    private string currentPlayer;
    private string localPlayer;

    private bool is_localPlayer;

    private string Player1 = GameStatus.GetNameOfPlayer(1);
    private string Player2 = GameStatus.GetNameOfPlayer(2);

    bool gameOver = false;

    public int[] EnPassantMove { set; get; }

    // Use this for initialization
    void Awake()
    {

        settings = gameUI.GetComponent<SettingsUI>();

        photonView.RPC(nameof(Initialized),RpcTarget.All);

        if (!GameStatus.isNewGame)
        {
            LoadFromSaveData(SaveLoadManager.tempSD);
            GameStatus.isNewGame = true;
            settings.AddLog(GameStatus.GetNameOfGame() + ": Load Complete.");
            return;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Chess (3D)");
        }


        if (!gameOver)
        {
            UpdateSelection();
            if (Input.GetMouseButtonDown(0))
            {
                if (selectionX >= 0 && selectionY >= 0)
                {
                    if (selectedChessman == null)
                    {
                        // Select the chessman
                        SelectChessman(selectionX, selectionY);
                    }
                    else
                    {
                        // Move the chessman
                        photonView.RPC(nameof(MoveChessman), RpcTarget.All, selectionX, selectionY);
                        photonView.RPC(nameof(NextTurn),RpcTarget.All);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void Initialized()
    {
        audio_source = GetComponent<AudioSource>();
        gameOver = false;
        Instance = this;
        SpawnAllChessmans();
        EnPassantMove = new int[2] { -1, -1 };
        currentPlayer = Player1;
        localPlayer = PhotonNetwork.LocalPlayer.NickName;

        if (currentPlayer == localPlayer)
        {
            is_localPlayer = true;
        } else
        {
            is_localPlayer = false;
        }

        //log

        settings.AddLog(GameStatus.GetNameOfGame() + ": New Game.");
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null) return;

        if (Chessmans[x, y].isWhite != isWhiteTurn || !is_localPlayer) return;

        bool hasAtLeastOneMove = false;
        bool[,] all_possible = new bool[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                all_possible[i, j] = true;
            } 
        }
        if (!GameStatus.useRules)
        {
            allowedMoves = all_possible;
        } else
        {
            allowedMoves = Chessmans[x, y].PossibleMoves();
        }
        audio_source.PlayOneShot(Sound_Capture, 0.7F);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                    i = 8;
                    break;
                }
            }
        }

        if (!hasAtLeastOneMove)
            return;

        selectedChessman = Chessmans[x, y];
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;

        BoardHighlights_mul.Instance.HighLightAllowedMoves(allowedMoves);
    }

    [PunRPC]
    private void MoveChessman(int x, int y)
    {
        bool Eat = false;
        if (allowedMoves[x, y])
        {
            Chessplayer c = Chessmans[x, y];

            if (c != null && c.isWhite != isWhiteTurn)
            {
                // Capture a piece
                Eat = true;
                if (GameStatus.useRules && c.GetType() == typeof(King))
                {
                    // End the game
                    if (isWhiteTurn)
                    {
                        Winner(Player1);
                    } else
                    {
                        Winner(Player2);
                    }
                    return;
                }
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);

            }
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                if (isWhiteTurn)
                    c = Chessmans[x, y - 1];
                else
                    c = Chessmans[x, y + 1];

                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (selectedChessman.GetType() == typeof(Pawn))
            {

                if(y == 7) // White Promotion
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(1, x, y, true);
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0) // Black Promotion
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(7, x, y, false);
                    selectedChessman = Chessmans[x, y];
                }
                EnPassantMove[0] = x;
                if (selectedChessman.CurrentY == 1 && y == 3)
                    EnPassantMove[1] = y - 1;
                else if (selectedChessman.CurrentY == 6 && y == 4)
                    EnPassantMove[1] = y + 1;
            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;

            if (Eat)
            {
                audio_source.PlayOneShot(Sound_Eat, 0.7F);
            }
            else
            {
                audio_source.PlayOneShot(Sound_Move, 0.7F);
            }
        }

        selectedChessman.GetComponent<MeshRenderer>().material = previousMat;

        BoardHighlights_mul.Instance.HideHighlights();
        selectedChessman = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main) return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    [PunRPC]
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
        isWhiteTurn = !isWhiteTurn;

        if (currentPlayer == localPlayer)
        {
            is_localPlayer = true; 
        } else
        {
            is_localPlayer = false;
        }

        //Log
        settings.AddLog("<b>" + currentPlayer + "</b>'s turn!");
    }
    private void SpawnChessman(int index, int x, int y, bool isWhite)
    {
        Vector3 position = GetTileCenter(x, y);
        GameObject go;

        if (isWhite)
        {
            go = Instantiate(chessmanPrefabs[index], position, whiteOrientation) as GameObject;
        }
        else
        {
            go = Instantiate(chessmanPrefabs[index], position, blackOrientation) as GameObject;
        }

        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessplayer>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessplayer[8, 8];

        /////// White ///////

        // King
        SpawnChessman(0, 3, 0, true);

        // Queen
        SpawnChessman(1, 4, 0, true);

        // Rooks
        SpawnChessman(2, 0, 0, true);
        SpawnChessman(2, 7, 0, true);

        // Bishops
        SpawnChessman(3, 2, 0, true);
        SpawnChessman(3, 5, 0, true);

        // Knights
        SpawnChessman(4, 1, 0, true);
        SpawnChessman(4, 6, 0, true);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1, true);
        }


        /////// Black ///////

        // King
        SpawnChessman(6, 4, 7, false);

        // Queen
        SpawnChessman(7, 3, 7, false);

        // Rooks
        SpawnChessman(8, 0, 7, false);
        SpawnChessman(8, 7, 7, false);

        // Bishops
        SpawnChessman(9, 2, 7, false);
        SpawnChessman(9, 5, 7, false);

        // Knights
        SpawnChessman(10, 1, 7, false);
        SpawnChessman(10, 6, 7, false);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6, false);
        }
    }

    public void Winner(string winner)
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().text = winner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

        //log
        settings.AddLog("<b>" + Player1 + "</b> is the winner! " + "Tap to restart.");
    }

    public void Winner1()
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().text = Player1 + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

        //log
        settings.AddLog("<b>" + Player1 + "</b> is the winner! " + "Tap to restart.");
    }

    public void Winner2()
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>().text = Player2 + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

        //log

        settings.AddLog("<b>" + Player2 + "</b> is the winner! " + "Tap to restart.");

    }

    public void PopulateSaveData(SaveData sd)
    {
        List<MarkerPosition_3D> markerPositions_3D = new List<MarkerPosition_3D>();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                if (Chessmans[i, j] != null)
                {
                    MarkerPosition_3D mp = new MarkerPosition_3D();
                    mp.is_white = Chessmans[i, j].isWhite;
                    mp.marker = Chessmans[i, j].name;
                    mp.num = i * 8 + j;
                    markerPositions_3D.Add(mp);
                }
            }

        sd.playerInTurn = currentPlayer.Equals(Player1) ? 1 : 2;
        sd.markerPositions_3D = markerPositions_3D;
    }

    public void LoadFromSaveData(SaveData sd)
    {
        currentPlayer = GameStatus.GetNameOfPlayer(sd.playerInTurn);
        foreach (MarkerPosition_3D mp in sd.markerPositions_3D)
        {
            switch (mp.marker)
            {
                //White chess
                case "White_King(Clone)": SpawnChessman(0, mp.num / 8, mp.num % 8, true); break;
                case "White_Queen(Clone)": SpawnChessman(1, mp.num / 8, mp.num % 8, true); break;
                case "White_Rook(Clone)": SpawnChessman(2, mp.num / 8, mp.num % 8, true); break;
                case "White_Bishop(Clone)": SpawnChessman(3, mp.num / 8, mp.num % 8, true); break;
                case "White_Kight(Clone)": SpawnChessman(4, mp.num / 8, mp.num % 8, true); break;
                case "White_Pawn(Clone)": SpawnChessman(5, mp.num / 8, mp.num % 8, true); break;

                //Black chess
                case "Black_King(Clone)": SpawnChessman(6, mp.num / 8, mp.num % 8, false); break;
                case "Black_Queen(Clone)": SpawnChessman(7, mp.num / 8, mp.num % 8, false); break;
                case "Black_Rook(Clone)": SpawnChessman(8, mp.num / 8, mp.num % 8, false); break;
                case "Black_Bishop(Clone)": SpawnChessman(9, mp.num / 8, mp.num % 8, false); break;
                case "Black_Kight(Clone)": SpawnChessman(10, mp.num / 8, mp.num % 8, false); break;
                case "Black_Pawn(Clone)": SpawnChessman(11, mp.num / 8, mp.num % 8, false); break;
            }
        }

    }

    new void OnEnable()
    {
        SaveLoadManager.OnSaveHandler += PopulateSaveData;
    }

    new void OnDisable()
    {

        SaveLoadManager.OnSaveHandler -= PopulateSaveData;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new NotImplementedException();
    }
}

