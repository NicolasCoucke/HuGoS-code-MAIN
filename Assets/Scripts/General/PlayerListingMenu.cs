using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{

    //public Player player { get; private set };
    [SerializeField]
    private Transform _content;
    [SerializeField]
    public PlayerListing _playerListing;


    private List<PlayerListing> _listings = new List<PlayerListing>();

    private void Awake()
    {
        GetCurrentRoomPlayers();
    }

    private void GetCurrentRoomPlayers()
    {
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {

            StartCoroutine("DelayedPlayerListing", playerInfo.Value);
            //if (PhotonNetwork.NickName != "spectatorr")
            //{
            //    AddPlayerListing(playerInfo.Value);
            //}
            
        }
    }

    private void AddPlayerListing(Player player)
    {
        
        
        PlayerListing listing = Instantiate(_playerListing, _content);
        if (listing != null)
        {

        }
        listing.SetPlayerInfo(player);
        _listings.Add(listing);
    }

    public void MakePlayerReady(int actornumber)
    {
        Player readyPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actornumber);
        int index = _listings.FindIndex(x => x.Player == readyPlayer);
        _listings[index].PlayerReady();

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartCoroutine("DelayedPlayerListing", newPlayer);
        
    }

    IEnumerator DelayedPlayerListing(Player newPlayer)
    {
        yield return new WaitForSeconds(2);
        if (newPlayer.NickName != "spectator")
        {
            AddPlayerListing(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
        }
    }

}