using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;


using System.Collections;

    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {


        #region Private Constants


        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {

        ////////
        // this retrieves the playername if you log in again (not important in our case)
        ///////

        //string defaultName = string.Empty;
        //InputField _inputField = this.GetComponent<InputField>();
        //if (_inputField != null)
        //{
        //    
        //    if (PlayerPrefs.HasKey(playerNamePrefKey))
        //    {
        //        defaultName = PlayerPrefs.GetString(playerNamePrefKey);
        //        _inputField.text = defaultName;
        //    }
        //}
        //PhotonNetwork.NickName = defaultName;
    }

    private void Update()
    {
        //string value = this.GetComponent<InputField>().text;
        //     // #Important
        //    if (string.IsNullOrEmpty(value))
        //{
        //    Debug.LogError("Player Name is null or empty");
        //    Debug.Log("name = " + value);

        //    return;
        //}
        //PhotonNetwork.NickName = value;


        //PlayerPrefs.SetString(playerNamePrefKey, value);
        //Debug.Log(value);
    }

    #endregion


    #region Public Methods


    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <paramname="value">The name of the Player</param>
    /// //not used
    public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                Debug.Log("name = " + value);
                
                return;
            }
            PhotonNetwork.NickName = "Player " + PhotonNetwork.LocalPlayer.ActorNumber;


            PlayerPrefs.SetString(playerNamePrefKey, value);
        Debug.Log(value);
        }


        #endregion
    }
