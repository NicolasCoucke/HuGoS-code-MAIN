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

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool IsPrestigeCondition;

    private int Game_duration = 300;
    static List<string> CommonList = new List<string>();

    static string Commonoutput = null;
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    public static Dictionary<int, int> prestigeList;
    public static Dictionary<int, int> voteList;

    [SerializeField]
    GameObject ChatBox;

    private List<string> GameData;
    public float score;
    private float[] single_scores;

    public string Player_String;
    public string Lava_String;

    public int NumberOfGames = 3;
    int PlayerNumber;

    //Dictionary<int, GameObject> Player_objects;

    private void Start()
    {
        
        if(StaticVariables.IsSpectator == true && PhotonNetwork.IsMasterClient == false)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        GameData = new List<string>();
        prestigeList = new Dictionary<int, int>();
        voteList = new Dictionary<int, int>();
        // ScoreMenu.instance._canvas.gameObject.SetActive(false);

        if (StaticVariables.IsSpectator)
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

        if (StaticVariables.Condition == 2)
        {
            ChatBox.SetActive(true);
        }
        else
        {
            ChatBox.SetActive(false);
        }

        //PhotonNetwork.PlayerList[0].GetPlayerNumber()


        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Debug.Log("menr man " + PhotonNetwork.PlayerList[i].ActorNumber);
            int actnumber = PhotonNetwork.PlayerList[i].ActorNumber;
            //if (actnumber == 1)
            //    actnumber = 0; // hacky fix for the actornumber of the first player being zero instead of 1
            if (PhotonNetwork.PlayerList[i].NickName != "spectator")
            {  
                prestigeList.Add(actnumber, 0);
                voteList.Add(actnumber, 0);
            }
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
        SpawnPositions.Add(new Vector3(0, 0, -15));
        SpawnPositions.Add(new Vector3(0, 0, 15));

        SpawnPositions.Add(new Vector3(-15, 0, 15));
        SpawnPositions.Add(new Vector3(15, 0, -15));
        SpawnPositions.Add(new Vector3(15, 0, 15));
        SpawnPositions.Add(new Vector3(-15, 0, -15));

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
        SpawnRotations.Add(0);
        SpawnRotations.Add(180);

        SpawnRotations.Add(135);
        SpawnRotations.Add(-45);
        SpawnRotations.Add(-135);
        SpawnRotations.Add(45);



        //List<Quaternion> SpawnRotations = new List<Quaternion>();
        //SpawnPositions.Add(new Quaternion.Euler(90, 0, 0));
        //SpawnPositions.Add(new Vector3(5, 5, -5));
        //SpawnPositions.Add(new Vector3(5, 5, 5));
        //SpawnPositions.Add(new Vector3(-5, 5, -5));

        PlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        //if (PhotonNetwork.NickName != "spectator")
        if (!StaticVariables.IsSpectator)
        {
            Debug.Log("spawningPlayer");
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

                    bool SpectatorExists = false;
                    foreach (Player player in PhotonNetwork.PlayerList)
                    {

                        if (player.NickName == "spectator")
                        {
                            SpectatorExists = true;
                        }
                    }

                    if (SpectatorExists == true)
                    {
                        GameObject spawned_player = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPositions[PlayerNumber - 2], Quaternion.Euler(0, SpawnRotations[PlayerNumber - 2], 0), 0);
                    }
                    else
                    { 
                        GameObject spawned_player = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPositions[PlayerNumber - 1], Quaternion.Euler(0, SpawnRotations[PlayerNumber - 1], 0), 0);
                    }
                    Debug.Log("playerid " + PlayerNumber);

                    //Player_objects.Add(PlayerNumber, spawned_player);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }



        StartCoroutine("EndGame");


        bool spectatorPresent = false;
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == "spectator")
                spectatorPresent = true;

    }



    IEnumerator EndGame()
    {

        //for(int i = 0; i < Game_duration/10; i++)
        //{
        //    yield return new WaitForSeconds(10);

        Debug.Log("start end");


        //}


        yield return new WaitForSeconds(Game_duration);
      



        if (PhotonNetwork.IsMasterClient)
        {
            //PlayerManager.SendGameData();
            // photonView.RPC("NetSendGameData", RpcTarget.All);
            Debug.Log("readyto end");

            if (ScoreMenu.Gamecounter < NumberOfGames)
            {

                PhotonNetwork.LoadLevel("RoomLobbyScene");
            }
            else
            {
                PhotonNetwork.LoadLevel("EndScene");
            }
        }


    }

    [PunRPC]
    void ScoreUpdate(float Total_Score)
    {
        ScoreMenu.Update_Score();

    }
    //[PunRPC]
    //void NetSendGameData()
    //{
    //    PlayerManager.SendGameData();
    //}





    private void Update()
    {

        // fix numbers and circles from top_view
        if (PhotonNetwork.NickName == "spectator")
        {
            PhotonView[] View_array = PhotonNetwork.PhotonViews;
            foreach (PhotonView view in View_array)
            {


                if (!view.IsMine)
                {
                    GameObject Player_object = view.gameObject;//

                    if (Player_object.tag == "Player")
                    {
                        if (view.gameObject.transform.GetChild(1).gameObject.activeSelf == false)
                            view.gameObject.transform.GetChild(1).gameObject.SetActive(true);

                        view.gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = (view.OwnerActorNr - 1).ToString();

                        if (view.gameObject.transform.GetComponent<PlayerManager>().dominanceON == true) 
                        {
                            view.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                        }
                        else
                        {
                            view.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                        }
                           


                    }
                }

            }
        }

        //fix prestige condition

        if (IsPrestigeCondition)
        {

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


    //void LoadArena()
    //{
    //    if (!PhotonNetwork.IsMasterClient)
    //    {
    //        Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
    //    }
    //    Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    //    PhotonNetwork.LoadLevel("Room for 1");// + PhotonNetwork.CurrentRoom.PlayerCount);
    //}

  

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

    private void StoreGameData()
    {


        string string_scores = LavaSpawnerv2.string_scores;

        string TheTime = System.DateTime.Now.ToString("HH:mm:ss");

        //float score = LavaSpawnerv2.Total_Score;
        //Vector3 PlayerPosition = GetComponent<Rigidbody>().transform.position;
        //string WriteLine = Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2");
        //GameData.Add(new KeyValuePair<string, string>(Time.time.ToString("f2"), PlayerPosition.ToString("f2")));
        // GameData.Add(Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2"));

        // personal data + general game data



        string DataToAdd = TheTime + "," + score.ToString("f2") + string_scores; //"," + PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString; ;
        // personal data for other players to send with redundancy
       // Temp_player_data = PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString;
        PhotonView[] View_array = PhotonNetwork.PhotonViews;
        foreach (PhotonView view in View_array)
        {
            if (!view.IsMine)
            {
                GameObject Player_object = view.gameObject;//

                if (Player_object.tag == "Player")
                {
                    DataToAdd = DataToAdd + "," + "P" + (view.OwnerActorNr - 1).ToString() + "," + Player_object.GetComponent<PlayerManager>().Temp_player_data;

                }
            }

        }


        GameData.Add(DataToAdd);
        //Debug.Log(DataToAdd);
        //GameData.Add(TheTime + "," + PlayerPosition.ToString("f2") + "," + score.ToString("f2") + string_scores + "," + Player_String + "," + Lava_String);
    }

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogWarningFormat(this, "OnFailedToConnectToPhoton, cause {0}", cause);
    }

    void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogWarningFormat(this, "OnConnectionFail, cause {0}", cause);
    }




    public void OutputCommondata()
    {

        //use this space to add player info to data array


        string[] GameArray = GameData.ToArray();


        //clear data
        GameData = new List<string>();


        // Raise custom room event
        // Replace 15 with any custom event code of your choice [0..299]
        //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        //SendOptions sendOptions = new SendOptions { Reliability = true };
        //SendOptions sendOptions = new SendOptions { Reliability = true };
        //PhotonNetwork.RaiseEvent(15, "nicolastesting", RaiseEventOptions.Default, sendOptions: Sendreliable);// WebFlags.Default);
        byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        //object[] content = new object[] { "testing" };
        object[] content = new object[] { GameArray }; // Array contains the target position and the IDs of the selected units
        var flags = new WebFlags(WebFlags.HttpForwardConst);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }




    //void OutputCommondata()
    //{

    //    var JointStrings = String.Join(",", CommonList);


    //    byte evCode = 2; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
    //    object[] content = new object[] { JointStrings }; // Array contains the target position and the IDs of the selected units
    //    var flags = new WebFlags(WebFlags.HttpForwardConst);
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
    //    SendOptions sendOptions = new SendOptions { Reliability = true };
    //    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    //    Debug.Log(JointStrings);

    //}

    #endregion
}
