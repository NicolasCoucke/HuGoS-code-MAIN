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
using UnityEngine;
using System.IO;
using System.Diagnostics;


//using System.Numerics;

public class DataSender : MonoBehaviourPunCallbacks, IPunObservable
{

    public Stopwatch timer;

    private List<string> GameData;
    public float score;
    private float[] single_scores;

    public string Player_String;
    public string Lava_String;

    public int NumberOfGames = 6;

    public static string BasicPath;
    public static string Path;
    public static bool _definePath = false;

    Dictionary<int,float> Latencytimes;

    //Dictionary<int, GameObject> Player_objects;

    float Timer;

    private void Start()
    {
        Timer = 0f;
        LocalStart();

        timer = new Stopwatch();
        // InvokeRepeating("TurnoffCanvasBU", 1f, 2f);
    }

    void Update()
    {
        Timer += Time.deltaTime;

    }
    void ServerStart()
    {
        GameData = new List<string>();


        bool spectatorPresent = false;
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == "spectator")
                spectatorPresent = true;

        if (spectatorPresent == true)
        {
            if (PhotonNetwork.NickName == "spectator")
            {
                GameData.Add("Game " + ScoreMenu.Gamecounter.ToString());
                InvokeRepeating("StoreGameData", 1f, 0.1f);
                //send data to server every 10 seconds
                InvokeRepeating("OutputCommondata", 11f, 10f);



            }

        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating("StoreGameData", 1f, 0.1f);
                //send data to server every 10 seconds
                InvokeRepeating("OutputCommondata", 11f, 10f);
                GameData.Add("Game " + ScoreMenu.Gamecounter.ToString());

            }
        }

    }

    void LocalStart()
    {
        if (StaticVariables.IsSpectator)
        {

            if (StaticVariables.DefinedPath == false)
            {
                DefinePath();
            }
            else
            {
                Path = StaticVariables.Path;
            }

            



           // InvokeRepeating("LatencyChecker", 0f, 10f); 
            InvokeRepeating("AddDatatoFile", 0f, 0.1f);
        }




    }
    public static void DefinePath()
    {
        if (StaticVariables.DefinedPath == false)
        {
            int directoryCount = Directory.GetDirectories(@"C:\Users\Administrator\Documents\GAMEDATA").Length;
            int CurrentGame = directoryCount + 1;
            string DataString = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            Path = @"C:\Users\Administrator\Documents\GAMEDATA" + DataString + ".csv";
            StaticVariables.Path = Path;
            //string[] paths = { @"C:\Users\Administrator\Documents\Prototype_v1_Data\GameData", DateString};
            //BasicPath = Path.Combine(paths);
            ////Directory.CreateDirectory(BasicPath);
            //Debug.Log(BasicPath);
            StaticVariables.DefinedPath = true;
            //Debug.Log(StaticVariables.Path);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    // We own this player: send the others our data
        //    stream.SendNext(PlayerColor);
        //}
        //else
        //{
        //    // Network player, receive data
        //    this.PlayerColor = (string)stream.ReceiveNext();
        //}

        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(score);


            // stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            //this.score = (float)stream.ReceiveNext();


            //this.Health = (float)stream.ReceiveNext();
        }


    }


    private void StoreGameData()
    {


        //string string_scores = LavaSpawnerv2.string_scores;

        string TheTime = System.DateTime.Now.ToString("HH:mm:ss");

        float score = LavaSpawnerv2.Total_Score;
        float max_score = LavaSpawnerv2.Max_Total_Score;
        //Vector3 PlayerPosition = GetComponent<Rigidbody>().transform.position;
        //string WriteLine = Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2");
        //GameData.Add(new KeyValuePair<string, string>(Time.time.ToString("f2"), PlayerPosition.ToString("f2")));
        // GameData.Add(Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2"));

        // personal data + general game data

        List<string> BlockerList = LavaSpawnerv2.Holes_Blockers;
        string BlockerString = "_";
        if (BlockerList.Count() != 0)
        {
            BlockerString = BlockerList[0];
            for (int i = 1; i < BlockerList.Count; i++)
            {
                BlockerString = BlockerString + "_" + BlockerList[i];
            }
        }



        //    {
        //        if (SolidHoles.Contains(i))
        //        {
        //            //mark the event of closing by having that string there once 
        //            string_scores = string_scores + "_CLOSED";
        //            SolidHoles.Remove(i);
        //        }
        //        else
        //        {
        //            string_scores = string_scores + "_" + Single_Scores[i].ToString("f2");

        //        }



        string DataToAdd = TheTime + "," + score.ToString("f2") + "," + max_score.ToString("f2") + "," + BlockerString; //"," + PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString; ;
                                                                                                                        // personal data for other players to send with redundancy
                                                                                                                        // Temp_player_data = PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString;
        PhotonView[] View_array = PhotonNetwork.PhotonViews;
        foreach (PhotonView view in View_array)
        {

            GameObject Player_object = view.gameObject;//

            if (Player_object.tag == "Player")
            {
                DataToAdd = DataToAdd + "," + "P" + (view.OwnerActorNr - 1).ToString() + "," + Player_object.GetComponent<PlayerManager>().Temp_player_data + "," + Latencytimes[view.OwnerActorNr].ToString();

            }


        }


        GameData.Add(DataToAdd);
        //Debug.Log(DataToAdd);
        //GameData.Add(TheTime + "," + PlayerPosition.ToString("f2") + "," + score.ToString("f2") + string_scores + "," + Player_String + "," + Lava_String);
    }



    // Send data via Webhook
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





    // export data locally
    //public static void AddtoFile(int result)
    //{
    //    string filepath = BasicPath + "\\blues.csv";
    //    using (StreamWriter file = new StreamWriter(@filepath, true))
    //    {
    //        file.WriteLine(Time.time.ToString("f2") + " , " + result);
    //        //file.Close();
    //    }

    //}

    //public static void UpdatePlayerData(Vector3 Posit, string PlayerColor, string Viewed_Players, int percent_seen, string _playerID)
    //{
    //    //Debug.Log("ho" + players.Count);
    //    string filepath = BasicPath + "\\" + _playerID + ".csv";
    //    using (StreamWriter file = new StreamWriter(@filepath, true))
    //    {

    //        //Debug.Log(_playerID + Time.time.ToString("f2") + "," + Posit.ToString("f2") + "," + Viewed_Players + "," + percent_seen.ToString());
    //        //write line all at once
    //        Write_Dic[_playerID] = Time.time.ToString("f2") + "," + Posit.ToString("f2") + "," + PlayerColor + "," + Viewed_Players + "," + percent_seen.ToString();
    //        //file.Close();
    //    }
    //}


    void AddDatatoFile()
    {


        float score = LavaSpawnerv2.Total_Score;
        float max_score = LavaSpawnerv2.Max_Total_Score;
        //string string_scores = LavaSpawnerv2.string_scores;

        //  string TheTime = System.DateTime.Now.ToString("HH:mm:ss");

        string BlockerString = null;
        List<string> BlockerList = LavaSpawnerv2.Holes_Blockers;
        if (BlockerList.Count() != 0)
        {
            BlockerString = BlockerList[0];
            for (int i = 1; i < BlockerList.Count; i++)
            {
                BlockerString = BlockerString + "," + BlockerList[i];
            }
        }

        //Debug.Log(Timer);

        string DataToAdd = Timer + "," + score.ToString("f2") + "," + max_score.ToString("f2") + "," + BlockerString;

        //"," + PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString; ;
        // personal data for other players to send with redundancy
        // Temp_player_data = PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString;
        PhotonView[] View_array = PhotonNetwork.PhotonViews;
        foreach (PhotonView view in View_array)
        {

            GameObject Player_object = view.gameObject;//

            if (Player_object.tag == "Player")
            {
                DataToAdd = DataToAdd + "," + "P" + (view.OwnerActorNr - 1).ToString() + "," + Player_object.GetComponent<PlayerManager>().Temp_player_data; //+ "," + Latencytimes[view.OwnerActorNr].ToString();

            }

        }

        //string filepath = BasicPath + "\\" + player_entry.Key + ".csv";
        using (StreamWriter file = new StreamWriter(@Path, true))
        {

            //write line all at once
            file.WriteLine(DataToAdd);
            //file.WriteLine("\n");
            //file.Close();
        }
    }


# region latency check
    void LatencyChecker()
    {
        
        Latencytimes = new Dictionary<int, float>();
        foreach (Player _player in PhotonNetwork.PlayerListOthers)
        {
            Latencytimes.Add(_player.ActorNumber, 0f);
        }

        timer.Stop();
        timer.Reset();
        timer.Start();
        if (StaticVariables.IsSpectator)
        {
            photonView.RPC("ClientTimeCheck", RpcTarget.All);

        }
    }

    [PunRPC]
    void ClientTimeCheck()
    {
        if (!StaticVariables.IsSpectator)
        {
            int actornumber = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("ReceiveTimeCheck", RpcTarget.All, actornumber);
        }
       

    }

    [PunRPC]
    void ReceiveTimeCheck(int actornumber)
    {
        if (StaticVariables.IsSpectator)
        {
            float elapsed = timer.ElapsedMilliseconds;
            Latencytimes[actornumber] = elapsed;

        }
   

    }

    #endregion

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



}
