using UnityEngine;
using Steamworks;
using System.Collections;

public class MatchmakingManager_Steam : MonoBehaviour 
{
	private CSteamID m_Lobby;

	// Steam Matchmaking Callbacks
	protected Callback<FavoritesListChanged_t> m_FavoritesListChanged;
	protected Callback<LobbyInvite_t> m_LobbyInvite;
	protected Callback<LobbyEnter_t> m_LobbyEnter;
	protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;
	protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
	protected Callback<LobbyChatMsg_t> m_LobbyChatMsg;
	protected Callback<LobbyGameCreated_t> m_LobbyGameCreated;
	protected Callback<LobbyKicked_t> m_LobbyKicked;
	protected Callback<FavoritesListAccountsUpdated_t> m_FavoritesListAccountsUpdated;

	// Steam Matchmaking Call Results
	private CallResult<LobbyEnter_t> OnLobbyEnterCallResult;
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;
	private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;
	
	public void OnEnable() 
	{
		// Create all the callback instances
		//m_FavoritesListChanged = Callback<FavoritesListChanged_t>.Create(OnFavoritesListChanged);
		m_LobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
		m_LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
		m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
		m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
		//m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);
		m_LobbyGameCreated = Callback<LobbyGameCreated_t>.Create(OnLobbyGameCreated);
		m_LobbyKicked = Callback<LobbyKicked_t>.Create(OnLobbyKicked);
		//m_FavoritesListAccountsUpdated = Callback<FavoritesListAccountsUpdated_t>.Create(OnFavoritesListAccountsUpdated);
		
		OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
		OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
	}

	void OnLobbyInvite(LobbyInvite_t pCallback) 
	{
		Debug.Log("[" + LobbyInvite_t.k_iCallback + " - LobbyInvite] - " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulGameID);
	}

	void OnLobbyEnter(LobbyEnter_t pCallback) 
	{
		Debug.Log("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);
		m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
	}
	
	void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure) 
	{
		Debug.Log("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);
		m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
	}
	
	void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback) 
	{
		Debug.Log("[" + LobbyDataUpdate_t.k_iCallback + " - LobbyDataUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDMember + " -- " + pCallback.m_bSuccess);
	}
	
	void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback) 
	{
		Debug.Log("[" + LobbyChatUpdate_t.k_iCallback + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUserChanged + " -- " + pCallback.m_ulSteamIDMakingChange + " -- " + pCallback.m_rgfChatMemberStateChange);
	}
	
	void OnLobbyGameCreated(LobbyGameCreated_t pCallback) 
	{
		Debug.Log("[" + LobbyGameCreated_t.k_iCallback + " - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDGameServer + " -- " + pCallback.m_unIP + " -- " + pCallback.m_usPort);
	}
	
	void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure) 
	{
		Debug.Log("[" + LobbyMatchList_t.k_iCallback + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching);
	}
	
	void OnLobbyKicked(LobbyKicked_t pCallback) 
	{
		Debug.Log("[" + LobbyKicked_t.k_iCallback + " - LobbyKicked] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDAdmin + " -- " + pCallback.m_bKickedDueToDisconnect);
	}
	
	void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure) 
	{
		Debug.Log("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
		m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
