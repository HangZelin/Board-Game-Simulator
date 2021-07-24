using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

namespace BGS.Chess_2D
{
    public class MovePlate_mul : MonoBehaviourPunCallbacks, IPunObservable
    {
        public GameObject controller, stepDialog;

        GameObject reference = null;

        //Board Positions
        int BoardX;
        int BoardY;

        //false : movement, true: attacking
        public bool attack = false;



        //Functions
        public void Start()
        {
            if (attack)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        public void OnMouseUp()
        {
            controller = GameObject.FindGameObjectWithTag("GameController");
            PerformMoveorAttack();

            if (!controller.GetComponent<Game_mul>().IsGameOver())
                controller.GetComponent<Game_mul>().photonView.RPC("NextTurn", RpcTarget.AllBuffered);


        }

        #region RPCs
        [PunRPC]
        public void PerformMoveorAttack()
        {

            if (attack)
            {
                GameObject cp = controller.GetComponent<Game_mul>().GetPosition(BoardX, BoardY);
                if (GameStatus.useRules)
                {
                    if (cp.name == "white_king") controller.GetComponent<Game_mul>().photonView.RPC("GameWinner", RpcTarget.AllBuffered, GameStatus.GetNameOfPlayer(1));
                    if (cp.name == "black_king") controller.GetComponent<Game_mul>().photonView.RPC("GameWinner", RpcTarget.AllBuffered, GameStatus.GetNameOfPlayer(2));
                }
                Destroy(cp);
            }

            // Call the other to sync
            controller.GetComponent<Game_mul>().photonView.RPC("SyncMoveChessPiece", RpcTarget.Others,
                reference.GetComponent<Chessman_mul>().GetXBoard(), reference.GetComponent<Chessman_mul>().GetYBoard(), BoardX, BoardY);

            controller.GetComponent<Game_mul>().SetPositionEmpty(reference.GetComponent<Chessman_mul>().GetXBoard(),
                reference.GetComponent<Chessman_mul>().GetYBoard());

            reference.GetComponent<Chessman_mul>().SetXBoard(BoardX);
            reference.GetComponent<Chessman_mul>().SetYBoard(BoardY);
            reference.GetComponent<Chessman_mul>().SetCoords();

            controller.GetComponent<Game_mul>().SetPosition(reference);

            reference.GetComponent<Chessman_mul>().DestroyMovePlates();
        }
        #endregion


        public void SetCoords(int x, int y)
        {
            BoardX = x;
            BoardY = y;
        }

        public void SetReference(GameObject obj)
        {
            reference = obj;
        }

        public GameObject GetReference()
        {
            return reference;
        }

        #region IPunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new System.NotImplementedException();
        }
        #endregion
    }
}
