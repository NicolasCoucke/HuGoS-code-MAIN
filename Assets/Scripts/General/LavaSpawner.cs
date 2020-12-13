using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LavaSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    public GameObject Lava;
   // public Player Player;

    public static int HoleCounter = 0;

    public static List<int> Hole_wait_times;
    public static List<Vector3> Hole_positions;
    public static List<int> Hole_durations;

    public static List<GameObject> Holes;
    public static List<float> Single_Scores;
    public static float[] Single_Scores_array;

    public static float Total_Score;

    public static List<int> OffHoles;
    public static string string_scores;

    [SerializeField]
    public Text Scoretext;

    [SerializeField]
    private GameObject leadingtext;

    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        leadingtext.SetActive(false);
        #region defineSequence
        Holes = new List<GameObject>();
        Hole_positions = new List<Vector3>();
        Hole_positions.Add(new Vector3(10f, 0f, 10f));
        Hole_positions.Add(new Vector3(-10f, 0f, -10f));
        Hole_positions.Add(new Vector3(-10f, 0f, 10f));
        Hole_positions.Add(new Vector3(10f, 0f, -10f));

        Hole_durations = new List<int>();
        Hole_durations.Add(10);
        Hole_durations.Add(10);
        Hole_durations.Add(10);
        Hole_durations.Add(10);

        Hole_wait_times = new List<int>();
        Hole_wait_times.Add(5); 
        Hole_wait_times.Add(10);
        Hole_wait_times.Add(10);
        Hole_wait_times.Add(10);
        #endregion 
        Single_Scores = new List<float>();
        Single_Scores_array = new float[4];
        OffHoles = new List<int>();
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
        for(int i = 0; i < Hole_wait_times.Count; i++)
        {
            yield return new WaitForSeconds(Hole_wait_times[i]);
            Spawn_next_lava(Hole_durations[i], Hole_positions[i]);
        }      

    }

    void Spawn_next_lava(int duration, Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Lava.name, position, Quaternion.identity);
            coroutine = EndLava(duration, 0);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator EndLava(int Duration, int hole)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(Duration);
            OffHoles.Add(hole);
            Holes[hole].GetComponent<LavaGenerator>().StartShrinking();
           
        }

    }


  

    public void Calculate_Total_Score()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Total_Score = 0;
            for (int i = 0; i < Single_Scores.Count; i++)
            {
                Total_Score += Single_Scores[i];
            }
           //Debug.Log("total score " + Total_Score);
            
        }
        //Total_Score = Mathf.Round(Total_Score);

        Scoretext.text = Mathf.Round(Total_Score).ToString();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      
        if (stream.IsWriting)
        {
            stream.SendNext(Total_Score);
            stream.SendNext(string_scores);

        }
        else
        {

            Total_Score = (float)stream.ReceiveNext();
            string_scores = (string)stream.ReceiveNext();
            //Single_Scores_array = (float[])stream.ReceiveNext();
        }


    }

    public static void Update_Score(float Single_Score, int hole)
    {
        if (!OffHoles.Contains(hole))
        {
            //Debug.Log("Update single score " + Single_Score);
            Single_Scores[hole] = Single_Score;
            Single_Scores_array[hole] = Single_Score;
        }

        
        string_scores = null;

        if (Single_Scores != null)
        {
            foreach (float score in Single_Scores)
            {
                string_scores = string_scores + "," + score.ToString("f2");

            }
        }
    }


    public static int Register_Hole(GameObject Lava)
    {
        HoleCounter = HoleCounter + 1;
        Holes.Add(Lava);
        Single_Scores.Add(0f);

        return HoleCounter - 1;


    }

    public static Vector3 Get_Position(int hole)
    {
        return Hole_positions[hole];
    }

    public void toggleLeadingText(bool value)
    {
        leadingtext.SetActive(value);
    }
}
