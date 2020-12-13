using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class BlockTUTLavaSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    public GameObject Lava;
    // public Player Player;

    public static int HoleCounter = 0;

    public static List<int> Hole_wait_times;
    public static List<Vector3> Hole_positions;
    public static List<int> Hole_durations;
    public static List<float> Hole_StartSizes;
    public static List<GameObject> Spawned_Lavas;

    public static List<GameObject> Holes;
    public static List<float> Single_Scores;
    public static List<float> Single_Surfaces;
    public static List<float> Max_Scores;
    public static float[] Single_Scores_array;
    public static List<string> Holes_Blockers;

    public static float Max_Total_Score;
    public static float Total_Score;

    public static List<int> OffHoles;
    public static string string_scores;
    public static string string_blocks;
    public static List<int> SolidHoles;

    public float endradius;
    public float startradius;
    public float frontlength = 2.8f;

    public int Trial = 1;


    [SerializeField]
    public Text Scoretext;

    //[SerializeField]
    //private GameObject leadingtext;

    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        //  ScoreMenu.instance._canvas.gameObject.SetActive(false);

        Trial = ScoreMenu.Gamecounter;
        Debug.Log("Trial " + Trial);
        Spawned_Lavas = new List<GameObject>();
        // leadingtext.SetActive(false);

        DefineSequence(Trial);
        Single_Scores = new List<float>();
        Single_Surfaces = new List<float>();
        Max_Scores = new List<float>();
        Single_Scores_array = new float[10];
        Holes_Blockers = new List<string>();
        OffHoles = new List<int>();
        SolidHoles = new List<int>();
        Max_Total_Score = 0;
        Total_Score = 0;
        HoleCounter = 0;
        string_scores = null;
        StartCoroutine("SpawnLavas");
    }

    // Update is called once per frame
    void Update()
    {
        Calculate_Total_Score();

    }

    IEnumerator SpawnLavas()
    {
        for (int i = 0; i < Hole_wait_times.Count; i++)
        {
            int WaitTime = 0;
            if (i == 0)
                WaitTime = Hole_wait_times[i];
            else
                WaitTime = Hole_wait_times[i] - Hole_wait_times[i - 1];

            yield return new WaitForSeconds(WaitTime);
            Spawn_next_lava(Hole_positions[i], Hole_StartSizes[i]);
        }

    }

    void Spawn_next_lava(Vector3 position, float start_size)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);

            photonView.RPC("MovePlayer", RpcTarget.All, position);
            //Calculate_radia();
            endradius = startradius / start_size;
            GameObject SpawnedLava = PhotonNetwork.Instantiate(Lava.name, position, Quaternion.identity);
            SpawnedLava.GetComponent<BLOCKTUTLavaGenerator>().Submit_radia(startradius, endradius);
            Spawned_Lavas.Add(SpawnedLava);
            //photonView.RPC("AddspawnedLava", RpcTarget.All, SpawnedLava);
            //coroutine = EndLava(duration, 0);
            //StartCoroutine(coroutine);
        }
    }

    //[PunRPC]
    //public static void AddspawnedLava(GameObject _SpawnedLava)
    //{
    //    Spawned_Lavas.Add(_SpawnedLava);

    //}

    [PunRPC]
    void MovePlayer(Vector3 position)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(position, position + Vector3.up, out hit))
        {
            Debug.Log("hit of " + hit.transform.position);
            if (transform.gameObject.tag == "Player")
                transform.position = Vector3.zero;

        }
    }
    //IEnumerator EndLava(int Duration, int hole)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        yield return new WaitForSeconds(Duration);
    //        OffHoles.Add(hole);
    //        Holes[hole].GetComponent<LavaGenerator>().StartShrinking();

    //    }

    //}


    private void Calculate_radia()
    {
        float numPlays = 10;

        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == "spectator")
                numPlays -= 1;



        startradius = (4.5f * numPlays) / (2f * 3.14f);

        // Debug.Log("numplays " + numPlays);
        // Debug.Log(endradius);

    }



    private void Calculate_distributed_radia()
    {

        float numPlays = PhotonNetwork.PlayerList.Count();

        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == "spectator")
                numPlays -= 1;

        if (numPlays % 2 == 0)
            startradius = ((float)0.6 * frontlength * numPlays) / (2f * 3.14f);
        else
            startradius = ((float)0.6 * frontlength * (numPlays - 1)) / (2f * 3.14f);
        // Debug.Log("numplays " + numPlays);
        // Debug.Log(endradius);

    }

    public void Calculate_Total_Score()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Total_Score = 0;
            Max_Total_Score = 0;
            for (int i = 0; i < Single_Scores.Count; i++)
            {
                Total_Score += Single_Scores[i];
                Max_Total_Score += Max_Scores[i];
            }
            //Debug.Log("total score " + Total_Score);

        }
        //Total_Score = Mathf.Round(Total_Score);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ScoreChange", RpcTarget.All, Total_Score);
        }

    }

    //
    // photonView.RPC("MeshChange", RpcTarget.All, vertices, triangles);


    [PunRPC]
    void ScoreChange(float Total_Score)
    {
        Scoretext.text = Mathf.Round(Total_Score).ToString();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(Total_Score);
            //stream.SendNext(string_scores);

        }
        else
        {

            Total_Score = (float)stream.ReceiveNext();
            //string_scores = (string)stream.ReceiveNext();
            //Single_Scores_array = (float[])stream.ReceiveNext();
        }


    }


    public static void Hole_Solidified(int hole)
    {
        SolidHoles.Add(hole);

    }


    public static void Update_Score(float Single_score, float Single_surface, float MaxScore, string BlockerString, int hole)
    {
        //if (!OffHoles.Contains(hole))
        //{
        //Debug.Log("Update single score " + Single_Score);
        Single_Surfaces[hole] = Single_surface;
        Single_Scores[hole] = Single_score;
        //Single_Scores_array[hole] = Single_Score;
        Holes_Blockers[hole] = BlockerString;

        Max_Scores[hole] = MaxScore;
        //}



        //string_scores = null;

        //if (Single_Scores != null)
        //{
        //    for(int i = 0; i < Single_Scores.Count;i++)
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

        //    }
        //}


    }


    public static int Register_Hole(GameObject Lava)
    {
        HoleCounter = HoleCounter + 1;
        Holes.Add(Lava);
        Single_Scores.Add(0f);
        Single_Surfaces.Add(0f);
        Max_Scores.Add(0f);
        Holes_Blockers.Add("0");

        return HoleCounter - 1;


    }

    public static Vector3 Get_Position(int hole)
    {
        return Hole_positions[hole];
    }

    public void toggleLeadingText(bool value)
    {
        // leadingtext.SetActive(value);
    }

    public void DefineSequence(int Trial)
    {
    
            Calculate_radia();
  
            Holes = new List<GameObject>();
            Hole_positions = new List<Vector3>();
            Hole_positions.Add(new Vector3(20f, 0f, 20f));
            Hole_positions.Add(new Vector3(-20f, 0f, -20f));

            Hole_wait_times = new List<int>();
            Hole_wait_times.Add(5);
            Hole_wait_times.Add(20);



            Hole_StartSizes = new List<float>();
            Hole_StartSizes.Add(0.3f);
            Hole_StartSizes.Add(0.3f);



    }

}
