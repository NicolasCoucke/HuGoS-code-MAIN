using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
//using System.Numerics;

public class TUTGameManager : MonoBehaviourPunCallbacks
{

    

    private int Game_duration = 60;
    static List<string> CommonList = new List<string>();

    static string Commonoutput = null;
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    public static Dictionary<int,int> prestigeList = new Dictionary<int,int>();
    public static Dictionary<int,int> voteList = new Dictionary<int,int>();



    //Dictionary<int, GameObject> Player_objects;

    private void Start()
    {

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Debug.Log("menr man " + PhotonNetwork.PlayerList[i].ActorNumber);
            int actnumber = PhotonNetwork.PlayerList[i].ActorNumber;
            //if (actnumber == 1)
            //    actnumber = 0; // hacky fix for the actornumber of the first player being zero instead of 1
            //prestigeList.Add(actnumber, 0);
            //voteList.Add(actnumber, 0);
            if (PhotonNetwork.PlayerList[i].NickName != "spectator")
            {
                prestigeList.Add(actnumber, 0);
                voteList.Add(actnumber, 0);
            }
        }
        //PhotonNetwork.PlayerList[0].GetPlayerNumber()


        if (PhotonNetwork.NickName == "spectator")
        {
            //ScoreMenu.instance._canvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;



        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        CommonList = new List<string>();
        CommonList.Add("999999999999999999999999999999999999999999999999999999999999999");

        List<Vector3> SpawnPositions = new List<Vector3>();
        SpawnPositions.Add(new Vector3(-10, 0, 10));
        SpawnPositions.Add(new Vector3(10, 0, -10));
        SpawnPositions.Add(new Vector3(10, 0, 10));
        SpawnPositions.Add(new Vector3(-10, 0, -10));

        SpawnPositions.Add(new Vector3(0, 0, 10));
        SpawnPositions.Add(new Vector3(0, 0, -10));
        SpawnPositions.Add(new Vector3(10, 0, 0));
        SpawnPositions.Add(new Vector3(-10, 0, 0));
        SpawnPositions.Add(new Vector3(-15, 0, 0));
        SpawnPositions.Add(new Vector3(15, 0, 0));

        List<int> SpawnRotations = new List<int>();
        SpawnRotations.Add(135);
        SpawnRotations.Add(-45);
        SpawnRotations.Add(-135);
        SpawnRotations.Add(45);

        SpawnRotations.Add(180);
        SpawnRotations.Add(0);
        SpawnRotations.Add(-90);
        SpawnRotations.Add(90);
        SpawnRotations.Add(90);
        SpawnRotations.Add(-90);


        int PlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (PhotonNetwork.NickName != "spectator")
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    GameObject spawned_player = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPositions[PlayerNumber - 1], Quaternion.Euler(0, SpawnRotations[PlayerNumber - 1], 0), 0);
                    Debug.Log("playerid " + PlayerNumber);
                }
                else
                {
                }
            }
        } 


        StartCoroutine("EndGame");
    }

    IEnumerator EndGame()
    {

        yield return new WaitForSeconds(Game_duration);

        Debug.Log("total score + " + LavaSpawnerv2.Total_Score.ToString());

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("RoomLobbyScene");
        }
        

    }


    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    #endregion


    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    #region Private Methods




    #endregion

    #region Photon Callbacks


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            //LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //LoadArena();
        }
    }





    //public static void OutputPlayervars(int ID, float tempvar)
    //{
    //    if (ID == 1)
    //    {
    //        CommonList[0] = tempvar.ToString();
    //    }
    //    else
    //    {
 
        
    //        CommonList[1] = tempvar.ToString();
      
    //    }


    //}

    void OutputCommondata()
    {

        var JointStrings = String.Join(",", CommonList);


        byte evCode = 2; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        object[] content = new object[] { JointStrings }; // Array contains the target position and the IDs of the selected units
        var flags = new WebFlags(WebFlags.HttpForwardConst);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        Debug.Log(JointStrings);

    }

    #endregion
}
