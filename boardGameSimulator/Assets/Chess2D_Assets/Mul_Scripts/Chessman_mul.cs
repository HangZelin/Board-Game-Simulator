using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace BGS.Chess_2D
{
    public class Chessman_mul : MonoBehaviourPunCallbacks, IPunObservable
    {
        public GameObject controller;
        public GameObject movePlate; //The new Location.

        //Positions
        private int xBoard = -1;
        private int yBoard = -1;
        private int is_attack = -1;

        //Keep track of "black" player or "White" player
        private string player;
        private float ratio;


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

            ratio = (float)Screen.height / Screen.width;
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

        public void SetCoords()
        {
            float x = xBoard;
            float y = yBoard;

            if (ratio >= 1 && ratio <= 1.8)
            {
                x *= 0.66f;
                y *= 0.66f;

                x -= 2.3f;
                y -= 2.3f;
            }
            else
            {
                x *= 0.57f;
                y *= 0.57f;

                x -= 2f;
                y -= 2f;
            }

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
            if (!controller.GetComponent<Game_mul>().IsGameOver() && controller.GetComponent<Game_mul>().GetCurrentPlayer() == player
                   && controller.GetComponent<Game_mul>().IsLocalTurn())
            {
                DestroyMovePlates();
                InitiateMovePlates();
            }
        }


        #region RPCs
        [PunRPC]
        public void DestroyMovePlates()
        {
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate_mul");
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

        [PunRPC]
        public void InitiateMovePlates()
        {
            audio_source.PlayOneShot(Sound_Capture, 0.7F);
                switch (this.name)
                {
                    case "black_queen":
                    case "white_queen":
                        LineMovePlate(1, 0);
                        LineMovePlate(0, 1);
                        LineMovePlate(1, 1);
                        LineMovePlate(-1, 0);
                        LineMovePlate(0, -1);
                        LineMovePlate(-1, -1);
                        LineMovePlate(-1, 1);
                        LineMovePlate(1, -1);
                        break;
                    case "black_knight":
                    case "white_knight":
                        LMovePlate();
                        break;
                    case "black_bishop":
                    case "white_bishop":
                        LineMovePlate(1, 1);
                        LineMovePlate(1, -1);
                        LineMovePlate(-1, 1);
                        LineMovePlate(-1, -1);
                        break;
                    case "black_king":
                    case "white_king":
                        SurroundMovePlate();
                        break;
                    case "black_rook":
                    case "white_rook":
                        LineMovePlate(1, 0);
                        LineMovePlate(0, 1);
                        LineMovePlate(-1, 0);
                        LineMovePlate(0, -1);
                        break;
                    case "black_pawn":
                        PawnMovePlate(xBoard, yBoard - 1);
                        break;
                    case "white_pawn":
                        PawnMovePlate(xBoard, yBoard + 1);
                        break;
                }
        }
        #endregion

        public void LineMovePlate(int xIncr, int yIncr)
        {
            Game_mul sc = controller.GetComponent<Game_mul>();

            int x = xBoard + xIncr;
            int y = yBoard + yIncr;

            while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                x += xIncr;
                y += yIncr;
            }

            if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman_mul>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }

        public void LMovePlate()
        {
            PointMovePlate(xBoard + 1, yBoard + 2);
            PointMovePlate(xBoard - 1, yBoard + 2);
            PointMovePlate(xBoard + 2, yBoard + 1);
            PointMovePlate(xBoard + 2, yBoard - 1);
            PointMovePlate(xBoard + 1, yBoard - 2);
            PointMovePlate(xBoard - 1, yBoard - 2);
            PointMovePlate(xBoard - 2, yBoard + 1);
            PointMovePlate(xBoard - 2, yBoard - 1);
        }

        public void SurroundMovePlate()
        {
            PointMovePlate(xBoard, yBoard + 1);
            PointMovePlate(xBoard, yBoard - 1);
            PointMovePlate(xBoard - 1, yBoard - 1);
            PointMovePlate(xBoard - 1, yBoard);
            PointMovePlate(xBoard - 1, yBoard + 1);
            PointMovePlate(xBoard + 1, yBoard - 1);
            PointMovePlate(xBoard + 1, yBoard);
            PointMovePlate(xBoard + 1, yBoard + 1);
        }

        public void PointMovePlate(int x, int y)
        {
            Game_mul sc = controller.GetComponent<Game_mul>();

            if (sc.PositionOnBoard(x, y))
            {
                GameObject cp = sc.GetPosition(x, y);

                if (cp == null)
                {
                    MovePlateSpawn(x, y);
                }
                else if (cp.GetComponent<Chessman_mul>().player != player)
                {
                    MovePlateAttackSpawn(x, y);
                }
            }
        }

        public void PawnMovePlate(int x, int y)
        {
            Game_mul sc = controller.GetComponent<Game_mul>();
            if (sc.PositionOnBoard(x, y))
            {
                if (sc.GetPosition(x, y) == null)
                {
                    MovePlateSpawn(x, y);
                }
            }

            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman_mul>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman_mul>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
        public void MovePlateSpawn(int BoardX, int BoardY)
        {
            float x = BoardX;
            float y = BoardY;

            if (ratio >= 1 && ratio <= 1.8)
            {
                x *= 0.66f;
                y *= 0.66f;

                x -= 2.3f;
                y -= 2.3f;
            }
            else
            {
                x *= 0.57f;
                y *= 0.57f;

                x -= 2f;
                y -= 2f;
            }

            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

            //Keep Track of everything in the script.
            MovePlate_mul mpScript = mp.GetComponent<MovePlate_mul>();
            is_attack = 0;
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(BoardX, BoardY);
        }


        public void MovePlateAttackSpawn(int BoardX, int BoardY)
        {
            float x = BoardX;
            float y = BoardY;

            if (ratio >= 1 && ratio <= 1.8)
            {
                x *= 0.66f;
                y *= 0.66f;

                x -= 2.3f;
                y -= 2.3f;
            }
            else
            {
                x *= 0.57f;
                y *= 0.57f;

                x -= 2f;
                y -= 2f;
            }

            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

            //Keep Track of everything in the script.
            MovePlate_mul mpScript = mp.GetComponent<MovePlate_mul>();
            mpScript.attack = true;
            is_attack = 1;
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(BoardX, BoardY);
        }

        #region IPunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new System.NotImplementedException();
        }
        #endregion
    }
}
