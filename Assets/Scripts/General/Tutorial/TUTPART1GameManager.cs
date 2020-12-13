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

public class TUTPART1GameManager : MonoBehaviourPunCallbacks
{

    public bool DominanceCondition;

    public Camera MainCamera;

    public int Game_duration = 60;

   public GameObject truck1;
   public GameObject truck2;
   public GameObject truck3;
   public GameObject truck4;
   public GameObject truck5;
   public GameObject truck6;

    [SerializeField]
    GameObject DominanceText;

    List<GameObject> trucks;
    List<Vector3> Endpositions;
    List<Vector3> Startpositions;
    Vector3 CameraStartPosition;
    Vector3 currentAngle;
    bool StartMove = false;

    public GameObject Crown1;

    int t =0;
    int tcam = 0;
    private void Start()
    {
        trucks = new List<GameObject>();
        trucks.Add(truck1);
        trucks.Add(truck2);
        trucks.Add(truck3);
        trucks.Add(truck4);
        trucks.Add(truck5);
        trucks.Add(truck6);

        Endpositions = new List<Vector3>();
        Endpositions.Add(new Vector3(-5f, 0, 3f));
        Endpositions.Add(new Vector3(5f, 0, -3f));
        Endpositions.Add(new Vector3(5f, 0, 3f));
        Endpositions.Add(new Vector3(-5, 0, -3f));
        Endpositions.Add(new Vector3(0f, 0, 5.3f));
        Endpositions.Add(new Vector3(0f, 0, -5.3f));

        Startpositions = new List<Vector3>();
        for (int i = 0; i < trucks.Count(); i++)
        {
            Startpositions.Add(trucks[i].transform.position);
        }

        
        StartCoroutine("EndGame");
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(15);
        //StartMove = true;
        InvokeRepeating("MoveTrucks",0,0.01f);

        yield return new WaitForSeconds(Game_duration - 15 - 2);
        CancelInvoke("MoveTrucks");


        if(StaticVariables.Condition == 3)
        {
            StartCoroutine("DominancePart");
            yield return new WaitForSeconds(12);

        }

        if (PhotonNetwork.IsMasterClient)
        {

            // PhotonNetwork.LoadLevel("TutorialScene");
            PhotonNetwork.LoadLevel("TutorialScene");

        }

        

    }

    void MoveTrucks()
    {
        t++;

        for (int i = 0; i < trucks.Count(); i++)
        {
            GameObject truck = trucks[i];

            Rigidbody rb = truck.GetComponent<Rigidbody>();
            rb.transform.position = Vector3.Lerp(Startpositions[i], Endpositions[i], t *0.002f);
       
        }
  
    }

    IEnumerator DominancePart()
    {
        CameraStartPosition = new Vector3(0, 130, 0); //MainCamera.transform.position;
        currentAngle = new Vector3(90, 0, 0);// MainCamera.transform.eulerAngles;
        
        
       
       InvokeRepeating("FlyCamera", 0, 0.01f);

        yield return new WaitForSeconds(2f);
        DominanceText.SetActive(true);
        yield return new WaitForSeconds(1f);
        Crown1.SetActive(true);
        yield return new WaitForSeconds(1f);
        t = 0;
        InvokeRepeating("MoveAway", 0, 0.01f);
        yield return new WaitForSeconds(2f);
        InvokeRepeating("FollowAway", 0, 0.01f);

    }

    void MoveAway()
    {
        t++;


        for (int i = 0; i < 1; i++)
        {
            Vector3 tempvec = Endpositions[i];
            tempvec.x -= 10f;
            tempvec.z += 10f;
            
            GameObject truck = trucks[i];
        

            Rigidbody rb = truck.GetComponent<Rigidbody>();
            rb.transform.position = Vector3.Lerp(Endpositions[i], tempvec, t * 0.002f);

        }
    }

    void FollowAway()
    {
        tcam++;

        for (int i = 1; i < trucks.Count(); i++)
        {
            Vector3 tempvec = Endpositions[i];
            tempvec.x -= 10f;
            tempvec.z += 10f;


            GameObject truck = trucks[i];
         
            Rigidbody rb = truck.GetComponent<Rigidbody>();
            rb.transform.LookAt(trucks[0].transform.position);
            rb.transform.position += rb.transform.forward * 0.02f;
        }
    }


    void FlyCamera()
    {
        tcam++;

     
            MainCamera.transform.position = Vector3.Lerp(CameraStartPosition, new Vector3(-20f, 2f, 0f), tcam * 0.01f);
            Vector3 targetAngle = new Vector3(0, 80.4f, 0);
            
      

        Vector3 NextAngle = new Vector3(
           Mathf.LerpAngle(currentAngle.x, targetAngle.x, tcam * 0.01f),
           Mathf.LerpAngle(currentAngle.y, targetAngle.y, tcam * 0.01f),
           Mathf.LerpAngle(currentAngle.z, targetAngle.z, tcam * 0.01f));
        MainCamera.transform.eulerAngles = NextAngle;

  

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


    #endregion
}
