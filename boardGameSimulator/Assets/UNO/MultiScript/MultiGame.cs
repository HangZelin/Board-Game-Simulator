using BGS.MenuUI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BGS.UNO
{
    public class MultiGame : MonoBehaviourPun, IPunObservable
    {
        [Header("Basic")]
        [SerializeField] GameObject Canvas;
        [SerializeField] GameObject gameUI;
        SettingsUI uiScript;
        MultiplayerManager mulManager;

        [Header("Containers")]
        [SerializeField] GameObject deck;
        DeckMul deckScript;

        [SerializeField] GameObject discard;

        [SerializeField] GameObject currentHand;
        [SerializeField] GameObject handPrefab;
        List<GameObject> hands;

        [SerializeField] GameObject playersObj;
        [SerializeField] GameObject playerPrefab;
        List<GameObject> players;
        public GameObject CurrentPlayer { get { return players[currentPlayerIndex]; } }

        [Header("Button & Icons")]
        [SerializeField] GameObject directionIcons;
        [SerializeField] Button nextTurnButton;

        [Header("Other Properties")]
        [SerializeField] byte unusedViewID;

        string clientName;
        int clientIndex;
        int numOfPlayer;
        public int currentPlayerIndex;
        bool antiClockWise;

        List<bool> sceneLoaded;
        int[] indexToKey;

        public delegate void TurnStart();
        public static event TurnStart TurnStartHandler;

        public delegate void TurnEnd();
        public static event TurnEnd TurnEndHandler;

        private void Start()
        {
            mulManager = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            PhotonNetwork.LocalPlayer.NickName = mulManager.playerName;
            if (mulManager.isHost)
            {
                sceneLoaded = new List<bool>();
            }

            Screen.orientation = ScreenOrientation.Landscape;
            numOfPlayer = GameStatus.NumOfPlayers;
            uiScript = gameUI.GetComponent<SettingsUI>();
            
            clientName = mulManager.playerName;
            clientIndex = mulManager.playerIndex;

            if (!mulManager.isHost)
            {
                this.photonView.RPC("GuestSceneLoaded", RpcTarget.MasterClient);
            }
            StartCoroutine(Initialize());
        }

        IEnumerator Initialize()
        {
            while (mulManager.isHost && sceneLoaded.Count < numOfPlayer - 1)
                yield return new WaitForSeconds(0.1f);

            // Initialize Index to key
            indexToKey = new int[numOfPlayer];
            Dictionary<int, Photon.Realtime.Player> punPlayers = PhotonNetwork.CurrentRoom.Players;
            for (int i = 0; i < numOfPlayer; i++)
                indexToKey[i] = punPlayers.FirstOrDefault(p => p.Value.NickName == GameStatus.GetNameOfPlayer(i + 1)).Key;

            // Initialize deck, discard, currenthand;
            deckScript = deck.GetComponent<DeckMul>();
            deckScript.Initialize(DealCard);

            discard.GetComponent<DiscardMul>().Initialize(currentHand, GetComponent<UNOInfo>());

            currentHand.GetComponent<CurrentHandMul>().Initialize(discard, deck);

            // Initialize rules script
            GetComponent<RulesMul>().enabled = GameStatus.useRules;
            if (GameStatus.useRules)
                GetComponent<RulesMul>().Initialize();

            // Initialize hands
            hands = new List<GameObject>();
            for (int i = 0; i < numOfPlayer - 1; i++)
            {
                GameObject a_Hand = Instantiate(handPrefab, Canvas.transform);
                a_Hand.GetComponent<HandMul>().Initialize(i, discard.GetComponent<DiscardMul>());
                hands.Add(a_Hand);
            }
            Hand.SetPositions(hands);

            /////////
            // Initialize players
            players = new List<GameObject>();
            string s = GameStatus.GetNameOfPlayer(1);

            for (int i = 0; i < numOfPlayer; i++, s = GameStatus.GetNameOfPlayer(i + 1))
            {
                GameObject a_Player = Instantiate(playerPrefab, playersObj.transform);
                int num = i < clientIndex ? (i + numOfPlayer - clientIndex - 1) : (i - clientIndex - 1);
                a_Player.GetComponent<PlayerMul>().Initialize(GetComponent<UNOInfo>(), s, i == clientIndex, i,
                    i == clientIndex ? currentHand : hands[num]);
                a_Player.GetComponent<PhotonView>().ViewID = unusedViewID + i;
                players.Add(a_Player);
            }

            // Initialize settings ui
            gameUI.GetComponent<SettingsUI>().Initialize();

            currentPlayerIndex = 0;
            antiClockWise = true;

            // Deal 7 cards to each player
            if (mulManager.isHost)
            {
                foreach (GameObject player in players)
                    player.GetComponent<PlayerMul>().TakeCards(DealCards(7, player));
            }
            gameUI.GetComponent<SettingsUI>().AddLog("UNO: New game.");

            // Initialize direaction Icons
            directionIcons.GetComponent<DirectionIconsMul>().DirectionIconToggle(antiClockWise);

            /////////
            // Initialize Game start handler

            MultiGame.TurnStartHandler += currentHand.GetComponent<CurrentHandMul>().EnableCardReaction;
            if (GameStatus.useRules)
                TurnStartHandler += GetComponent<RulesMul>().OnDraw2Draw4Played_Start;

            /////////
            // Initialize Game end handler

            TurnEndHandler += GetComponent<RulesMul>().UpdateLastCardInfo;

            // If use rules, check isNextTurn
            if (GameStatus.useRules)
                TurnEndHandler += GetComponent<RulesMul>().IsNextTurn;

            // Set next currentPlayerIndex
            TurnEndHandler += SetNextPlayerIndex;

            // TurnEndHandler += SetCurrentPlayerIndex;
            if (GameStatus.useRules)
                TurnEndHandler += GetComponent<RulesMul>().OnDraw2Draw4Played_End;

            // Initialize first turn
            if (mulManager.isHost)
            {
                OnTurnStart();
            }
            else
            {
                currentHand.GetComponent<CurrentHandMul>().SkipTurn();
            }

            nextTurnButton.interactable = false;
            GameStatus.isNewGame = true;
        }

        private void OnDisable()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            TurnEndHandler -= SetNextPlayerIndex;
        }

        public void OnTurnEnd()
        {
            // Disable next turn button
            nextTurnButton.interactable = false;
            // Disable deck draw
            deck.GetComponent<DeckMul>().Interactable = false;

            if (TurnEndHandler != null)
                TurnEndHandler();
        }

        /**
         * <summary>
         * Deal cards from deck into a transform.
         * </summary>
         * <return>A list of cards.</return>
         * <param name="num">Number of cards to deal.</param>
         * <param name="player">Player who take the cards</param>
         */
        public List<GameObject> DealCards(int num, GameObject player)
        {
            this.photonView.RPC("SyncDealCards", RpcTarget.Others, num, player.GetComponent<PlayerMul>().playerIndex);
            return deckScript.DrawCards(num, player.transform);
        }

        /// <summary>
        /// Deal a card to currenthand.
        /// </summary>
        public void DealCard()
        {
            players[currentPlayerIndex].GetComponent<PlayerMul>().TakeCards(DealCards(1, players[currentPlayerIndex]));
        }

        #region Button Callbacks

        public void OnNextTurnClicked()
        {
            OnTurnEnd();
            this.photonView.RPC("OnTurnStart", GetPunPlayer(currentPlayerIndex));
        }

        #endregion

        #region Helpers

        public void SyncPlayCard(int cardIndex)
        {
            this.players[currentPlayerIndex].GetComponent<PlayerMul>()
                .photonView.RPC("PlayCard", RpcTarget.Others, cardIndex);
        }

        void SetNextPlayerIndex()
        {
            currentPlayerIndex = NextPlayer(currentPlayerIndex);
            this.photonView.RPC("SyncCurrentPlayerIndex", RpcTarget.Others, currentPlayerIndex);
        }

        public Photon.Realtime.Player GetPunPlayer(int playerIndex)
        {
            return PhotonNetwork.CurrentRoom.Players[indexToKey[playerIndex]];
        }

        public Photon.Realtime.Player GetNextPunPlayer()
        {
            return GetPunPlayer(NextPlayer(currentPlayerIndex));
        }

        public void ToggleDirection()
        {
            antiClockWise = !antiClockWise;
            this.photonView.RPC("SyncDirection", RpcTarget.Others, this.antiClockWise);
            directionIcons.GetComponent<DirectionIconsMul>().DirectionIconToggle(antiClockWise);
            uiScript.AddLog("Direction reversed.");
        }

        /// <summary>
        /// Get index of next player based on direction.
        /// </summary>
        /// <param name="index">Player index.</param>
        /// <returns>Index of next player based on direction.</returns>
        public int NextPlayer(int index)
        {
            if (antiClockWise)
                return index == (numOfPlayer - 1)
                    ? 0
                    : index + 1;
            else
                return index == 0
                    ? numOfPlayer - 1
                    : index - 1;
        }

        IEnumerator SyncDealCardsHelper(int num, int playerIndex)
        {
            while (!GetComponent<RulesMul>().firstCardDrawed)
                yield return new WaitForSeconds(0.1f);
            players[playerIndex].GetComponent<PlayerMul>().TakeCards(
                deckScript.DrawCards(num, players[playerIndex].transform));
        }

        #endregion


        #region RPCs

        [PunRPC]
        public void GuestSceneLoaded()
        {
            this.sceneLoaded.Add(true);
        }

        [PunRPC]
        public void OnTurnStart()
        {
            // If without rules, enable next turn button
            if (!GameStatus.useRules)
                nextTurnButton.interactable = true;
            // Enable deck draw
            deck.GetComponent<DeckMul>().Interactable = true;

            if (TurnStartHandler != null)
                TurnStartHandler();
        }

        [PunRPC]
        void SyncDealCards(int num, int playerIndex)
        {
            if (GetComponent<RulesMul>().firstCardDrawed)
            {
                players[playerIndex].GetComponent<PlayerMul>().TakeCards(
                    deckScript.DrawCards(num, players[playerIndex].transform));
            }
            else
                StartCoroutine(SyncDealCardsHelper(num, playerIndex));
        }

        [PunRPC]
        void SyncCurrentPlayerIndex(int index)
        {
            currentPlayerIndex = index;
        }

        [PunRPC]
        void SyncDirection(bool antiClockwise)
        {
            if (antiClockwise ^ this.antiClockWise)
                ToggleDirection();
        }

        #endregion


        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // throw new System.NotImplementedException();
        }

        #endregion
    }
}



