/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate; //The new Location.
    public GameObject stepDialog;//stepWindow UI

    public float aspectRatio = Screen.width * 1.0f / Screen.height;

    //Positions
    private int xBoard = -1;
    private int yBoard = -1;
    private int is_attack = -1;
    private bool is_captured = false;

    //Keep track of "black" player or "White" player
    private string player;

    //Sprites for chesspieces
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    //Sounds
    public AudioClip Sound_Move;
    public AudioClip Sound_Eat;
    public AudioClip Sound_Capture;
    private AudioSource audio_source;

    //Functions

    void Start()
    {
        audio_source = GetComponent<AudioSource>();
    }
    public void Activate()
    {
        //Getting access to controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Take the instantiated location and adjust the transform
        SetCoords();

        //Chesspiece switch to a correct image used
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = GameStatus.GetNameOfPlayer(2); break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = GameStatus.GetNameOfPlayer(2); break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = GameStatus.GetNameOfPlayer(2); break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = GameStatus.GetNameOfPlayer(2); break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = GameStatus.GetNameOfPlayer(2); break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = GameStatus.GetNameOfPlayer(2); break;

            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = GameStatus.GetNameOfPlayer(1); break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = GameStatus.GetNameOfPlayer(1); break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = GameStatus.GetNameOfPlayer(1); break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = GameStatus.GetNameOfPlayer(1); break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = GameStatus.GetNameOfPlayer(1); break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = GameStatus.GetNameOfPlayer(1); break;
        }
    }

    public void Update()
    {
        if (is_captured)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int pos_x = (int)(Input.mousePosition.x / 35.0);
                int pos_y = (int)((Input.mousePosition.y - 115.0) / 35.0);
                PointMovePlate(pos_x, pos_y);
                is_captured = false;
            }
        }
        else
        {

        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x = x * 0.57f;
        y = y * 0.57f;

        x -= 2f;
        y -= 2f;

        this.transform.position = new Vector3(x, y, -1.0f);//In front of the board. 
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver())
        {
            if (controller.GetComponent<Game>().GetCurrentPlayer() == player && !is_captured)
            {
                is_captured = true;

                DestroyMovePlate();


                audio_source.PlayOneShot(Sound_Capture, 0.7F);

                InitiateMovePlate();
            }
        }

    }

    public void DestroyMovePlate()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        if (is_attack == 0)
        {
            audio_source.PlayOneShot(Sound_Move, 0.7F);
            is_attack = -1;
        }
        else if (is_attack == 1)
        {
            audio_source.PlayOneShot(Sound_Eat, 0.7F);
            is_attack = -1;
        }
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlate()
    {


    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }
    public void MovePlateSpawn(int BoardX, int BoardY)
    {
        float x = BoardX;
        float y = BoardY;

        x = x * 0.57f;
        y = y * 0.57f;

        x -= 2f;
        y -= 2f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        //Keep Track of everything in the script.
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        is_attack = 0;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(BoardX, BoardY);
    }

    public void MovePlateAttackSpawn(int BoardX, int BoardY)
    {
        float x = BoardX;
        float y = BoardY;

        x = x * 0.57f;
        y = y * 0.57f;

        x -= 2f;
        y -= 2f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        //Keep Track of everything in the script.
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        is_attack = 1;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(BoardX, BoardY);

        *//*        //toggle the window to ask user what to do
                if (!stepDialog.activeInHierarchy)
                {
                   Debug.Log("Toggled!");
                   stepDialog.SetActive(true);
                   Time.timeScale = 0f;
                }
                else
                {
                stepDialog.SetActive(false);
                Time.timeScale = 1f;
                }*//*
    }
}
*/