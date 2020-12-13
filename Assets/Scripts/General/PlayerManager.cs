using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;
using Photon.Pun.UtilityScripts;
using TMPro;

//[RequireComponent(typeof(PlayGraph))]
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject Graphics;
    #region IPunObservable implementation
    //Color PlayerColor = Color.blue;
    string PlayerColor = "blue";
    string other_color_old = null;
    private int PlayerID = 0;
    public static float score;
    private float[] single_scores;

    public string Player_String;
    public string Lava_String; 
    [SerializeField] private Camera MyCamera;

    public List<int> Viewed_Players;
    public List<int> Viewed_Lavas;
    public int[] Viewed_Players_array;
    public int[] Viewed_Lavas_array;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    private static List<string> GameData;
    //private List<string> GameData;
    int testvar = 2;

    public bool dominanceON = false;
    public int vote = 0;
    public int prestige = 0;
    public int PingTime = 0;

    public string Temp_player_data;

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        //DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Graphics.GetComponent<Renderer>().material.color = Color.blue;
        //var NumberOfPlayers = PhotonNetwork.CurrentRoom.Players.Count;
        PlayerID = PhotonNetwork.LocalPlayer.ActorNumber-1;
        GameData = new List<string>();
        GameData.Add("Game " + ScoreMenu.Gamecounter.ToString());
        InvokeRepeating("LatencyChecker", 0f, 10f); 
        InvokeRepeating("StoreGameData", 0f, 0.1f);
        
        if(!photonView.IsMine)
        { 
            MyCamera.enabled = false;
        }
        else
        {
            if(ScoreMenu.instance != null)
                ScoreMenu.instance._canvas.gameObject.SetActive(false);
            //this.GetComponentInChildren<TMP_Text>().text = 
        }

        this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = (this.photonView.OwnerActorNr-1).ToString();

        //TextMesh t = this.gameObject.AddComponent<TextMesh>();
        //t.text = "1";
        //t.fontSize = 30;
        //t.transform.localPosition += new Vector3(0, 0.66f, 0.82f);

        if(StaticVariables.Condition == 3)
        {
            this.GetComponent<DominancePlayer>().enabled = true;
        }
        //if (StaticVariables.Condition == 4)
        //{
        //    this.GetComponent<PrestigePlayer>().enabled = true;
        //}


    }
 
    
    public void Update()
    {

        
        if (photonView.IsMine)
        {
            testvar += Random.Range(0,2);
            //GameManager.OutputPlayervars(PlayerID, testvar);


            if (Input.GetKeyDown(KeyCode.G))
            {

                SwitchColor();

            }

            if (Input.GetKeyDown(KeyCode.M))
            {

                //SendData();

            }

        }

        if (other_color_old != PlayerColor)
        {

            if (PlayerColor == "blue")
            {
                Graphics.GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                Graphics.GetComponent<Renderer>().material.color = Color.red;
            }

            other_color_old = PlayerColor;
        }


    }

    public void Detect_players()
    {
        this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = (this.photonView.OwnerActorNr - 1).ToString();

        if (StaticVariables.IsSpectator)
        {
            if (this.gameObject.transform.GetChild(1).gameObject.activeSelf == false)
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);

            this.gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = (this.photonView.OwnerActorNr - 1).ToString();
        }


        // Check visible players
        Player[] Player_array = PhotonNetwork.PlayerListOthers;
        PhotonView[] View_array  = PhotonNetwork.PhotonViews;
        
        //Player_array.Remove(_playerID); //do not count self
        Viewed_Players.Clear(); // remove storage of view
        foreach (PhotonView view in View_array)
        {


            if (view != this.gameObject.GetPhotonView())
            {
                GameObject Player_object = view.gameObject;//

                if (Player_object.tag == "Player")
                {
                    //extract the visible part
                    Renderer Player_renderer = Player_object.GetComponentInChildren<Renderer>();

                    //PlayerManager.Graphics.GetComponent<Renderer>();

                    //Debug.Log(other_player.Key);
                    //add to visible players
                    if (IsVisibleFrom(Player_renderer, MyCamera))
                    {
                        //Debug.Log("visible");
                        Viewed_Players.Add(view.OwnerActorNr-1);
                       // Debug.Log(view.OwnerActorNr.ToString());
                    }
                }
            }
            
        }
        Viewed_Players_array = Viewed_Players.ToArray();
       
      
    }



    public void Detect_Lavas()
    {
        //this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = (this.photonView.OwnerActorNr - 1).ToString();
        //// Check visible players
        //Player[] Player_array = PhotonNetwork.PlayerListOthers;
        PhotonView[] View_array = PhotonNetwork.PhotonViews;

        //Player_array.Remove(_playerID); //do not count self
        Viewed_Lavas.Clear(); // remove storage of view
        foreach (PhotonView view in View_array)
        {
            GameObject Lava_object = view.gameObject;//
            if (Lava_object.tag == "Lava")
            {
                //extract the visible part
                Renderer Lava_renderer = Lava_object.GetComponentInChildren<Renderer>();

                //PlayerManager.Graphics.GetComponent<Renderer>();

                //Debug.Log(other_player.Key);
                //add to visible players

                if (IsVisibleFrom(Lava_renderer, MyCamera))
                {
                    
                    //Debug.Log("visible");
                    Viewed_Lavas.Add(Lava_object.GetComponent<LavaGenerator>().hole); // start at one
                                                          // Debug.Log(view.OwnerActorNr.ToString());
                }
            }


        }
        Viewed_Lavas_array = Viewed_Lavas.ToArray();

        Lava_String = string.Join("_", Viewed_Lavas);

        //Debug.Log("lava size " + Lava_list.Count); //OK
        //Debug.Log(Lava_String);



    }

    public void Detect_block_Lavas()
    {
        //this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = (this.photonView.OwnerActorNr - 1).ToString();
        //// Check visible players
        //Player[] Player_array = PhotonNetwork.PlayerListOthers;
        PhotonView[] View_array = PhotonNetwork.PhotonViews;

        //Player_array.Remove(_playerID); //do not count self
        Viewed_Lavas.Clear(); // remove storage of view
        foreach (PhotonView view in View_array)
        {
            GameObject Lava_object = view.gameObject;//
            if (Lava_object.tag == "Lava")
            {
                //extract the visible part
                Renderer Lava_renderer = Lava_object.GetComponentInChildren<Renderer>();

                //PlayerManager.Graphics.GetComponent<Renderer>();

                //Debug.Log(other_player.Key);
                //add to visible players

                if (IsVisibleFrom(Lava_renderer, MyCamera))
                {

                    //Debug.Log("visible");
                    Viewed_Lavas.Add(Lava_object.GetComponent<BLOCKLavaGenerator>().hole); // start at one
                                                                                      // Debug.Log(view.OwnerActorNr.ToString());
                }
            }


        }
        Viewed_Lavas_array = Viewed_Lavas.ToArray();

        Lava_String = string.Join("_", Viewed_Lavas);

        //Debug.Log("lava size " + Lava_list.Count); //OK
        //Debug.Log(Lava_String);



    }
    //public void Detect_Lavas()
    //{
    //    if (LavaSpawnerv2.Spawned_Lavas != null)
    //    {
    //        // Check visible players
    //        List<GameObject> Lava_list = LavaSpawnerv2.Spawned_Lavas;
    //        Debug.Log("List length " + Lava_list.Count); 
    //        Viewed_Lavas.Clear(); // remove storage of view
    //        for (int i = 0; i < Lava_list.Count; i++)
    //        {

    //            GameObject Lava_object = Lava_list[i];



    //            if (Lava_object.tag == "Lava")
    //            {
    //                //extract the visible part
    //                Renderer Lava_renderer = Lava_object.GetComponentInChildren<Renderer>();

    //                //PlayerManager.Graphics.GetComponent<Renderer>();

    //                //Debug.Log(other_player.Key);
    //                //add to visible players
    //                if (IsVisibleFrom(Lava_renderer, MyCamera))
    //                {
    //                    //Debug.Log("visible");
    //                    Viewed_Lavas.Add((i + 1).ToString()); // start at one
    //                    // Debug.Log(view.OwnerActorNr.ToString());
    //                }
    //            }


    //        }
    //        Lava_String = string.Join("_", Viewed_Lavas);

    //        //Debug.Log("lava size " + Lava_list.Count); //OK
    //        Debug.Log(Lava_String);
    //    }
    //}

    // This function checks whether something is visible from a particular camera
    public static bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    void SwitchColor()
    {
        //gameObject.renderer.material.color = Color.blue;
        if (Graphics.GetComponent<Renderer>().material.color == Color.blue)
        {
           // Graphics.GetComponent<Renderer>().material.color = Color.red;

            //PlayerColor = Color.red;
            PlayerColor = "red";

        }

        else
        {
            //Graphics.GetComponent<Renderer>().material.color = Color.blue;
            PlayerColor = "blue";
            //PlayerColor = Color.blue;
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
            stream.SendNext(PlayerColor);
            stream.SendNext(dominanceON);
           // stream.SendNext(Temp_player_data);
            
           // stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            this.PlayerColor = (string)stream.ReceiveNext();
            this.dominanceON = (bool)stream.ReceiveNext();
            //this.Temp_player_data = (string)stream.ReceiveNext();
            //this.Health = (float)stream.ReceiveNext();
        }


    }

    private void StoreGameData()
    {
        if (this.photonView.IsMine)
        {
            Detect_players();
            if(StaticVariables.Condition == 4)
            {
                Detect_block_Lavas();
            }
            else
            {
                Detect_Lavas();
            }
            

            string string_scores = LavaSpawnerv2.string_scores;

            string TheTime = System.DateTime.Now.ToString("HH:mm:ss");

            score = LavaSpawnerv2.Total_Score;
            Vector3 PlayerPosition = GetComponent<Rigidbody>().transform.position;
            float PlayerRotation = GetComponent<Rigidbody>().transform.rotation.eulerAngles.y;
            //string WriteLine = Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2");
            //GameData.Add(new KeyValuePair<string, string>(Time.time.ToString("f2"), PlayerPosition.ToString("f2")));
            // GameData.Add(Time.time.ToString("f2") + "," + PlayerPosition.ToString("f2"));

            // personal data + general game data

            string prestigeString = "V_" + vote.ToString() + "P_" + prestige.ToString();


            string dominanceString = "off";

            if (dominanceON == true)
            { dominanceString = "on"; }

            int block = 0;
            block = this.gameObject.GetComponent<BlockPicker>().HasBlockID;

            // string DataToAdd = TheTime + "," + score.ToString("f2") + string_scores + "," + PlayerPosition.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString; ;
            // personal data for other players to send with redundancy
            // Temp_player_data = PlayerPosition.ToString("f2") + "," + PlayerRotation.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString;

            this.photonView.RPC("SyncTempData", RpcTarget.MasterClient, Viewed_Players_array, Viewed_Lavas_array, vote, prestige, dominanceON, block, PingTime);

            //PhotonView[] View_array = PhotonNetwork.PhotonViews;
            //foreach (PhotonView view in View_array)
            //{


            //    if (!view.IsMine)
            //    {
            //        GameObject Player_object = view.gameObject;//

            //        if (Player_object.tag == "Player")
            //        {
            //            DataToAdd = DataToAdd + "," + "P" + (view.OwnerActorNr - 1).ToString() + "," + Player_object.GetComponent<PlayerManager>().Temp_player_data;

            //        }
            //    }

            //}


            //GameData.Add(DataToAdd);
            //Debug.Log(DataToAdd);
            //GameData.Add(TheTime + "," + PlayerPosition.ToString("f2") + "," + score.ToString("f2") + string_scores + "," + Player_String + "," + Lava_String);
        }
    }

    void LatencyChecker()
    {
       PingTime = PhotonNetwork.GetPing();

    }

    [PunRPC]
    public void SyncTempData(int[] Playersviewed, int[] Lavasviewed, int vote, int prestige, bool dominanceON, int _block, int pingTime)
    {
        
      //  Debug.Log(Lava_String);
        string prestigeString = "V_" + vote.ToString() + "P_" + prestige.ToString();

        Vector3 PlayerPosition = GetComponent<Rigidbody>().transform.position; // already synced by 
        float PlayerRotation = GetComponent<Rigidbody>().transform.rotation.eulerAngles.y;

        string dominanceString = "off";

        List<int> playerlist = new List<int>(Playersviewed);
        Player_String = string.Join("_", playerlist);

        List<int> lavalist = new List<int>(Lavasviewed);
        Lava_String = string.Join("_", lavalist);

        if (dominanceON == true)
        { dominanceString = "on"; }
        // personal data for other players to send with redundancy
        Temp_player_data = pingTime.ToString() + "," + PlayerPosition.ToString("f2") + "," + PlayerRotation.ToString("f2") + "," + Player_String + "," + Lava_String + "," + dominanceString + "," + prestigeString + "," + _block.ToString();
    }

   public static void SendGameData()
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
        object[] content = new object[] {GameArray}; // Array contains the target position and the IDs of the selected units
        var flags = new WebFlags(WebFlags.HttpForwardConst);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }


    #endregion

    //FOR TESTING WITH KEYBOARD PRESS
    //void SendData()
    //{
    //    var data = new Dictionary<string, object>() {
    //{ "Hello" , "World" }
    //};

    //    // Raise custom room event
    //    // Replace 15 with any custom event code of your choice [0..299]
    //    //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
    //    //SendOptions sendOptions = new SendOptions { Reliability = true };
    //    //SendOptions sendOptions = new SendOptions { Reliability = true };
    //    //PhotonNetwork.RaiseEvent(15, "nicolastesting", RaiseEventOptions.Default, sendOptions: Sendreliable);// WebFlags.Default);
    //    byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
    //    object[] content = new object[] { "nicolastesting" }; // Array contains the target position and the IDs of the selected units
    //    var flags = new WebFlags(WebFlags.HttpForwardConst);
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
    //    SendOptions sendOptions = new SendOptions { Reliability = true };
    //    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

    }
