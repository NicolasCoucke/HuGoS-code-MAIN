using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Drawing;

public class Launcher : MonoBehaviourPunCallbacks
{

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    [SerializeField]
    private GameObject AccesCodeField;
    [SerializeField]
    private GameObject CodeErrorText;
    [SerializeField]
    private GameObject RoomFull;
    [SerializeField]
    private GameObject ClosedErrorText;


    [SerializeField]
    private GameObject Initiator;


    //[SerializeField]
    //private GameObject LobbyUI;

    //[SerializeField]
    //public static Launcher instance;


    #region Private Serializable Fields
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    //[SerializeField]
    private byte maxPlayersPerRoom = 17;

    #endregion


    #region Private Fields
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    bool gamebusy = false;

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    private float[] AccessCodes = { 70704, 11424, 58070, 83492, 85403, 55778, 63964, 99713, 2464, 19068, 71439, 77783, 48424, 83522, 31571, 76641, 76292, 84824, 47101, 72449, 21234, 58891, 78238, 45768, 42963, 67557, 27157, 14093, 46091, 21791, 31086, 25254, 42325, 42173, 97570, 33398, 86697, 28727, 77619, 36274, 28194, 16627, 7845, 48457, 28990, 71982, 76389, 69249, 43747, 18928, 52355, 14106, 663, 41005, 56000, 85491, 11928, 89685, 20897, 32553, 63396, 65241, 11246, 18989, 17019, 14828, 9174, 23007, 87085, 2115, 65705, 24588, 56351, 47687, 90182, 59556, 14666, 35269, 8366, 35733, 84720, 32037, 65854, 98558, 9170, 85988, 91553, 17520, 31287, 31058, 88769, 65497, 48227, 48263, 95982, 93259, 5612, 66678, 14717, 28296, 79089, 73532, 93144, 47192, 64769, 67619, 29197, 96518, 7587, 30942, 49569, 73428, 47717, 57279, 51256, 83253, 80798, 4683, 36710, 94050, 39490, 33280, 96156, 33512, 10108, 45801, 19724, 52826, 51420, 49819, 34009, 86287, 76919, 55567, 22434, 20557, 46199, 2501, 42265, 54238, 8610, 19920, 34621, 13352, 73872, 47567, 84015, 26678, 55122, 28695, 79694, 92849, 49228, 90228, 18027, 79681, 26510, 77614, 82299, 47244, 1250, 81956, 23154, 24808, 92763, 33092, 46026, 36892, 37387, 48909, 75976, 70706, 31541, 91168, 58417, 3068, 6331, 7695, 67762, 97010, 86855, 86255, 38822, 43444, 99658, 77196, 21026, 44865, 79737, 49027, 92976, 93532, 26899, 10746, 16432, 85252, 48678, 45049, 28358, 27394, 71184, 39174, 91496, 99325, 7842, 59118, 58613, 71919, 56891, 10946, 87034, 90189, 18007, 69231, 2457, 6550, 62249, 27764, 7485, 88515, 45042, 54521, 55973, 17177, 10988, 99584, 93584, 74030, 26807, 18310, 53832, 54475, 47577, 49484, 47887, 87216, 88866, 10300, 23459, 44021, 47081, 76942, 99067, 70936, 39058, 79791, 78946, 25459, 78418, 65856, 78434, 25055, 16948, 69188, 66498, 91368, 30394, 61661, 63776, 21681, 61350, 77343, 60599, 8616, 37243, 56675, 35499, 40772, 95452, 14206, 14684, 28690, 61433, 5390, 59821, 6904, 35193, 10381, 10927, 43660, 54967, 18083, 1089, 51231, 67592, 67790, 2217, 20348, 56004, 68522, 5912, 64337, 23645, 98034, 17194, 34326, 92841, 61919, 28989, 67580, 70258, 28188, 76634, 56966, 81385, 44444, 47174, 90522, 72113, 89706, 7575, 46091, 77558, 59206, 44918, 19047, 45889, 51307, 58723, 95452, 37331, 98893, 76344, 4941, 47893, 73167, 31444, 19389, 12589, 64760, 7822, 61801, 84931, 41754, 48270, 72978, 36670, 17919, 47904, 49974, 2557, 84378, 40940, 96822, 19370, 98307, 71268, 7779, 95117, 51263, 55026, 99429, 58502, 16310, 47986, 27496, 20820, 22715, 55699, 99333, 78512, 18391, 98185, 76151, 78301, 13645, 74669, 67213, 11469, 86126, 40155, 66091, 11335, 8351, 45452, 76220, 18480, 39207, 85356, 26411, 80992, 85752, 39534, 78598, 23653, 34067, 64353, 79965, 25759, 5833, 6577, 95095, 49074, 64067, 26211, 69942, 69924, 76408, 6404, 84719, 9238, 89068, 74282, 96545, 64944, 99519, 67404, 84573, 95902, 59790, 54377, 39871, 72475, 57592, 80997, 39330, 66975, 56058, 44173, 19200, 47195, 15885, 44708, 31916, 57295, 36606, 16265, 51254, 89533, 77202, 92504, 54223, 87243, 42792, 75195, 40675, 88928, 79507, 27965, 27885, 46172, 46530, 7045, 43363, 36715, 73763, 55446, 73336, 72861, 25679, 50061, 52208, 85198, 36189, 55743, 29244, 11775, 51668, 47089, 29399, 57853, 83309, 88230, 82709, 35091, 79852, 50242, 54016, 80094, 43989, 11255, 24799, 5023, 65130, 19886, 56530, 68667, 41349, 97441, 76241, 63828, 82604, 35868, 59612, 27787, 19010, 93149, 99679, 90859, 22305, 90278, 28776, 43728, 43406, 34984, 70802, 61114, 28671, 325, 30333 };
    private string[] AccessStrings = new string[501];


    #endregion


    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    /// 

    void Awake()
    {
        //////:!!!!!
        PhotonNetwork.UseAlternativeUdpPorts = true;

        PhotonNetwork.AutomaticallySyncScene = true;


        //if (instance != null)
        //{
        //    //Debug.LogError("More than one Launcher in scene.");
        //    Destroy(this);
        //    return;
        //}
        //else
        //{
        //    instance = this;
        //}

        //DontDestroyOnLoad(this.gameObject);

    }
    //void Awake()
    //{
    //    // #Critical
    //    // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
    //  
    //    

    //}


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        int i = 0;
        foreach (float code in AccessCodes)
        {
            AccessStrings[i] = code.ToString();
            i++;
        }



        //Debug.Log(gamebusy);
        //// Connect();
        //if(gamebusy)
        //{
        //    LobbyUI.SetActive(true);
        //    gamebusy = false;
        //}
        //else
        //{
        //    progressLabel.SetActive(false);
        //    controlPanel.SetActive(true);
        //}

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);



    }


    #endregion


    #region Public Methods


    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        bool found = false;
        //foreach(string code in AccessStrings)
        //{
        //    if(code == AccesCodeField.GetComponent<InputField>().text)
        //    {
        //        found = true;
        //    }
        //}

        string CodeInput = AccesCodeField.GetComponent<InputField>().text;
        if (CodeInput.Length > 0)
        {
            found = true;
            StaticVariables.ProlificID = CodeInput;
        }




        if (found == true)
        {
            CodeErrorText.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                //isConnecting = PhotonNetwork.ConnectToRegion("eu");
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
        }
        else
        {
            CodeErrorText.SetActive(true);
        }

    }


    #endregion

    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Cursor.lockState = CursorLockMode.None;
        if (StaticVariables.DoReconnect == true)
        {
            PhotonNetwork.ReconnectAndRejoin();
            StaticVariables.DoReconnect = true;
        }
        else
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            ClosedErrorText.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        //If a room is full, make sure that we're not able to create another room but show that we have to wait
        if (PhotonNetwork.CountOfRooms == 0)
        {
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        else
        {
            PhotonNetwork.Disconnect();
            RoomFull.SetActive(true);
        }

    }

    public override void OnJoinedRoom()
    {
        progressLabel.SetActive(false);
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    ////Debug.Log("We load the 'Room for 1' ");

        //    //LobbyUI.SetActive(true);
        //    // #Critical
        //    // Load the Room Level.
        //    //PhotonNetwork.LoadLevel("RoomLobbyScene");
        //    PhotonNetwork.LoadLevel("RoomLobbyScene");

        //}

        //

        bool spectatorPresent = false;
        if (AccesCodeField.GetComponent<InputField>().text == "spectatorr")
        {


            spectatorPresent = true;
            //Initiator.GetComponent<InitiateSpectator>().IsSpectator = true;
            StaticVariables.IsSpectator = true;

        }
        else
        {

            foreach (Player player in PhotonNetwork.PlayerList)
                if (player.NickName == "spectator")
                    spectatorPresent = true;
        }
        if (spectatorPresent == true)
        {
            PhotonNetwork.LoadLevel("PreRoomLobbyScene");

        }
        else
        {
            PhotonNetwork.Disconnect();


        }

     

        //PhotonNetwork.LoadLevel("CoordinationScene");

    }






    #endregion
}
