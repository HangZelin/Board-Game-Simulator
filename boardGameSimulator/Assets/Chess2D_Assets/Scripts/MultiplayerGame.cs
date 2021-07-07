using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGame : Game, IOnEventCallback
{
	private const int SET_GAME_STATE_EVENT_CODE = 1;
	private NetworkManager networkManager;
	private string localPlayer;

	public void SetNetworkManager(NetworkManager networkManager)
	{
		this.networkManager = networkManager;
	}

	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void SetLocalPlayer(Team team)
	{
		localPlayer = team == Team.P1 ? GameStatus.GetNameOfPlayer(1): GameStatus.GetNameOfPlayer(2);
	}

	public bool IsLocalPlayersTurn()
	{
		return localPlayer == GetCurrentPlayer();
	}


	public void OnEvent(EventData photonEvent)
	{
		byte eventCode = photonEvent.Code;
		if (eventCode == SET_GAME_STATE_EVENT_CODE)
		{
			object[] data = (object[])photonEvent.CustomData;
		}
	}

	public void TryToStartThisGame()
	{

		if (networkManager.IsRoomFull())
		{
			Initialized();
		}

	}


	public bool CanPerformMove()
	{
		if (!IsLocalPlayersTurn())
			return false;
		return true;
	}
}
