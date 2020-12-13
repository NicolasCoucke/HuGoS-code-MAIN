using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//Leaves out collision and makes player go into lava
//Checks for each vertex if it hits the player and if so lava stops expanding at that location
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Player))]
public class LavaGenerator : MonoBehaviourPunCallbacks, IPunObservable
{

    //initialize Mesh Variables

    private Mesh mesh;

    [SerializeField]
    private Material SolidLavaMaterial;



    //private Mesh mesh_old;

    private Vector3[] vertices;

    public int[] triangles;


    //circle variables
    private float val = 3.14285f / 180f;//one degree = val radians
    private float radius = 1.0f;
    private float theoretical_radius = 1.0f;
    private int deltaAngle = 5;
    private int maxradius = 1;
    //int trisize = 0;

    //grow and collision variabels
    private float increment = 0.0015f;
    private float Vertice_Dist = 0;
    //public List<Vector3> ContactPoints;

    private Dictionary<int, float> Stored_Vertices = new Dictionary<int, float>();
    private Vector3 GlobalPosition;
    //Vector3 PlayerPosition;
    //GameObject Player;
    //Rigidbody Player_rb;

    //[SyncVar] SYNCCCC
    public GameObject this_object;


    //Score variabels
    // [SyncVar] SYNCC
    public float Track_Surface = 0;

    public float Track_null_Surface = 0;
    //[SyncVar] SYNCC
    private float Track_Surface_old = 0;

    //[SyncVar] SYNCCCC
    public int hole;

    private float endradius;
    private float startradius;
    private float startsurface;

    private List<int> Blocked_Vertices = new List<int>();

    float Blockpercentage = 0.9f;
    float Proportional_increment = 0;

    List<int> Blockers = new List<int>();
    string BlockerString = null;

    int Players;
    private void Awake()
    {
        //if player is above swpan location, remove the player

    }


    private void Start()
    {
        startsurface = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            hole = LavaSpawnerv2.Register_Hole(gameObject);
            GlobalPosition = LavaSpawnerv2.Get_Position(hole);
            photonView.RPC("SyncHole", RpcTarget.All, hole);
        }

        mesh = new Mesh();
        //StartCoroutine("WaitforRadia");
        Createmesh();
        InvokeRepeating("MeshGrowing", 1f, 0.5f);
        InvokeRepeating("surface_calculator", 1f, 0.5f);
        InvokeRepeating("CheckHits", 1f, 1f);

        if (StaticVariables.IsSpectator)
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            this.gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = hole.ToString();
        }

        if (ScoreMenu.Gamecounter == 3)
        {
            Players = (PhotonNetwork.CountOfPlayers - 1) / 2;
        }
        else
        {
            Players = (PhotonNetwork.CountOfPlayers - 1);
        }

    }

    [PunRPC]
    void SyncHole(int _hole)
    {
        hole = _hole;
    }

    //IEnumerator WaitforRadia()
    //{
    //    yield return new WaitForSeconds(1);
    //    Createmesh();
    //    InvokeRepeating("MeshGrowing", 1f, 0.2f);
    //    InvokeRepeating("surface_calculator", 1f, 0.2f);
    //}

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("ok");
            //MeshGrowing();
            //Updatemesh();
            Debug.DrawLine(vertices[0], vertices[1] * 5, Color.white, 5.0f);
        }




        //if the spill is almost completely encircled by players then make it solidify


        //if (radius > endradius)
        //{
        //    CancelInvoke("MeshGrowing");
        //}


        //if(startradius < (2.8 * 2.9) / (2f * 3.14f))
        //{
        //    Blockpercentage = 0.5f;
        //}

    }


    void CheckHits()
    {

        Blocked_Vertices = new List<int>();
        Blockers = new List<int>();
        for (int vertex = 1; vertex < vertices.Length; vertex++)
        {
            //Debug.DrawLine(vertices[0], vertices[vertex] * 5, Color.white, 5.0f);

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(vertices[0] + GlobalPosition, vertices[vertex], out hit, Vector3.Distance(vertices[0], vertices[vertex])))
            {
                Vector3 temp1 = vertices[0] + GlobalPosition;
                temp1.y = 1f;
                Vector3 temp2 = vertices[vertex] + GlobalPosition;
                temp2.y = 1f;
                Debug.DrawRay(temp1, temp2 * 1000, Color.white, 5.0f);
                //Debug.Log(hit.transform.position);
                //Debug.Log(temp1);
                //Debug.Log(temp2);
                //ContactPoints.Add(vertices[vertex]);
                Blocked_Vertices.Add(vertex);
                if (!Stored_Vertices.ContainsKey(vertex))
                {
                    Stored_Vertices.Add(vertex, radius);
                }

                //Debug.Log(contact.point);
                //if (hole == 0)
                //{
                //    Debug.Log(vertex);
                //}

                //check which player it is 
                if (hit.transform.gameObject.tag == "Player")
                {
                    int blocker = hit.transform.gameObject.GetPhotonView().OwnerActorNr - 1;
                    if (!Blockers.Contains(blocker))
                    {
                        Blockers.Add(blocker);
                    }
                }

            }




        }
    }
    //void OnCollisionStay(Collision collisionInfo) //checks where players interact with lava
    //{
    //    ContactPoints.Clear();
    //    foreach (ContactPoint contact in collisionInfo.contacts)
    //    {
    //        //Debug.Log(contact.point);
    //        ContactPoints.Add(contact.point);
    //        Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
    //    }


    //}


    private void Createmesh()
    {
        // Debug.Log("meshcreate");
        radius = startradius;
        theoretical_radius = startradius;
        #region define vertices
        int circlepoints = 360 / deltaAngle;
        vertices = new Vector3[maxradius * circlepoints + 1];
        vertices[0] = new Vector3(0, 0.2f, 0);
        int j = 1;


        for (int i = 0; i < 359; i = i + deltaAngle)
        {
            float x2 = radius * Mathf.Sin((i + deltaAngle) * val);
            float y2 = 0;
            float z2 = radius * Mathf.Cos((i + deltaAngle) * val);
            vertices[j] = new Vector3(x2, y2, z2);
            //Debug.Log(vertices[j]+GlobalPosition);
            j++;

        }
        //Debug.Log(j);
        //Debug.Log(vertices.Length);

        #endregion


        #region define triangles
        //create triangles
        //Debug.Log(vertices.Length);
        //j is for indexing in triangles
        //i is for accessing in vertices
        triangles = new int[3 * circlepoints];
        //Debug.Log(triangles.Length);
        j = 0;
        for (int i = 1; i < (vertices.Length - 1); i++)
        {
            //Debug.Log(j);

            triangles[j + 0] = 0;
            triangles[j + 1] = i;
            triangles[j + 2] = i + 1;
            //Debug.Log(triangles[j]);
            //Debug.Log(triangles[j + 1]);
            //Debug.Log(triangles[j + 2]);
            j = j + 3;

            if (i == vertices.Length - 2)
            {
                triangles[j + 0] = 0;
                triangles[j + 1] = i + 1;
                triangles[j + 2] = 1;
                //j = j+3;

            }





        }
        #endregion
        surface_calculator();
        UpdateMesh(true);
    }


    public void MeshGrowing()
    {

        Proportional_increment = (endradius - startradius) * increment;


        //update circle variables
        int circlepoints = 360 / deltaAngle;
        Vertice_Dist = 2f * radius * 3.14f / (float)circlepoints; //the distance between two vertices on the circle edge
        Vertice_Dist = Vertice_Dist * 2;
        radius = radius + Proportional_increment;

        //maak hier dan een dict van die opslaat welke radius ze hadden


        #region calculate vertices
        //Debug.Log(Blocked_Vertices.Count);
        for (int i = 1; i <= circlepoints; i++)
        {
            if (!Stored_Vertices.ContainsKey(i))
            {
                //all untouced vertices get updated
                float x2 = radius * Mathf.Sin((i + 1) * deltaAngle * val);
                float y2 = 0;
                float z2 = radius * Mathf.Cos((i + 1) * deltaAngle * val);
                vertices[i] = new Vector3(x2, y2, z2);
                //Debug.Log(vertices[j]);
                //j++;
            }
            else
            {
                if (!Blocked_Vertices.Contains(i))
                {
                    // if not vertice not touched anymore, let it quickly grow back but in steps
                    if (Stored_Vertices[i] < radius)
                    {
                        Stored_Vertices[i] = Stored_Vertices[i] + 3 * Proportional_increment;
                        float x2 = Stored_Vertices[i] * Mathf.Sin((i + 1) * deltaAngle * val);
                        float y2 = 0;
                        float z2 = Stored_Vertices[i] * Mathf.Cos((i + 1) * deltaAngle * val);
                        vertices[i] = new Vector3(x2, y2, z2);
                    }
                    else
                    {
                        // if it is grown back, let it go back with the others
                        Stored_Vertices.Remove(i);
                        float x2 = radius * Mathf.Sin((i + 1) * deltaAngle * val);
                        float y2 = 0;
                        float z2 = radius * Mathf.Cos((i + 1) * deltaAngle * val);
                        vertices[i] = new Vector3(x2, y2, z2);
                    }
                }

            }
        }

        #endregion


        #region calculate triangles
        //create triangles
        //Debug.Log(vertices.Length);
        //j is for indexing in triangles
        //i is for accessing in vertices
        triangles = new int[3 * circlepoints];
        //Debug.Log(triangles.Length);
        int j = 0;
        for (int i = 1; i < (vertices.Length - 1); i++)
        {
            //Debug.Log(j);

            triangles[j + 0] = 0;
            triangles[j + 1] = i;
            triangles[j + 2] = i + 1;
            //Debug.Log(triangles[j]);
            //Debug.Log(triangles[j + 1]);
            //Debug.Log(triangles[j + 2]);
            j = j + 3;

            if (i == vertices.Length - 2)
            {
                triangles[j + 0] = 0;
                triangles[j + 1] = i + 1;
                triangles[j + 2] = 1;
                //j = j+3;

            }





        }
        #endregion

        BlockerString = "_";
        foreach (int block in Blockers)
        {
            BlockerString = BlockerString + block + "_";
        }

        if (Blocked_Vertices.Count > Blockpercentage * vertices.Length)// && Blockers.Count == Players)
        {
            photonView.RPC("StartSolidify", RpcTarget.All);

            BlockerString = "T";
        }

        UpdateMesh(false);
    }

    public void surface_calculator()
    {

        theoretical_radius = theoretical_radius + Proportional_increment;
        //Debug.Log("radius" + radius);
        //Debug.Log("theorie radius" + theoretical_radius);
        Track_Surface_old = Track_Surface;
        Track_Surface = 0;
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            Vector3 A = vertices[triangles[i]];
            Vector3 B = vertices[triangles[i + 1]];
            Vector3 C = vertices[triangles[i + 2]];
            //Debug.Log(B);
            Track_Surface += Mathf.Abs((A.x * (B.z - C.z) + B.x * (C.z - A.z) + C.x * (A.z - B.z)) / 2);

        }

        if (startsurface == 0)
        {
            startsurface = Track_Surface;
        }
        //Debug.Log(Track_Surface);
        Track_null_Surface = 0;//2 * radius * (float)3.14;
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            Vector3 AA = new Vector3();
            AA.x = 0; AA.y = 0.2f; AA.z = 0;
            Vector3 BB = new Vector3();
            BB.x = theoretical_radius * Mathf.Sin(0); ; BB.y = 0f; BB.z = theoretical_radius * Mathf.Cos(0);
            Vector3 CC = new Vector3();
            CC.x = theoretical_radius * Mathf.Sin(deltaAngle * val); CC.y = 0f; CC.z = theoretical_radius * Mathf.Cos(deltaAngle * val);
            Track_null_Surface += Mathf.Abs((AA.x * (BB.z - CC.z) + BB.x * (CC.z - AA.z) + CC.x * (AA.z - BB.z)) / 2);

        }

        if (PhotonNetwork.IsMasterClient)
        {
            float MaxScore = Track_null_Surface - startsurface;
            //pass through what they managed to stop
            LavaSpawnerv2.Update_Score(Track_null_Surface - Track_Surface, MaxScore, BlockerString, hole);
          

        }
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    //pass through what they managed to stop
        //    LavaSpawnerv2.Update_Score(Track_null_Surface - Track_Surface, BlockerString, hole);
        //}
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

        //if (stream.IsWriting)
        //{
        //    // We own this player: send the others our data
        //    stream.SendNext(hole);
        //    stream.SendNext(GlobalPosition);

        //    //stream.SendNext(vertices);
        //    //stream.SendNext(triangles);

        //    //stream.SendNext(Track_Surface);

        //}
        //else
        //{
        //    // Network player, receive data
        //    //Track_Surface = (float)stream.ReceiveNext();
        //    hole = (int)stream.ReceiveNext();
        //    GlobalPosition = (Vector3)stream.ReceiveNext();
        //    //vertices = (Vector3[])stream.ReceiveNext();
        //    //triangles = (int[])stream.ReceiveNext();


        //    //stream.SendNext(vertices);
        //    //stream.SendNext(triangles);
        //    //this.Health = (float)stream.ReceiveNext();
        //}


    }

    //public void UpdateMesh(float Track_Surface_old, float Track_Surface)
    public void UpdateMesh(bool create)
    {

        this_object = gameObject;
        //if (isServer)
        //MeshChange(vertices, triangles);

        if (PhotonNetwork.IsMasterClient)
        {
            //float MaxScore = Track_null_Surface - startsurface;
            ////pass through what they managed to stop
            //LavaSpawnerv2.Update_Score(Track_null_Surface - Track_Surface, MaxScore, BlockerString, hole);
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("MeshChange", RpcTarget.All, vertices, triangles, create, startradius);


        }




    }


    [PunRPC]
    void MeshChange(Vector3[] vertices, int[] triangles, bool create, float radius)
    {




        Mesh tempmesh = new Mesh();
        tempmesh.Clear();
        tempmesh.vertices = vertices;
        tempmesh.triangles = triangles;
        this.GetComponent<MeshFilter>().mesh = tempmesh;

        if (create == true)
        {
            this.gameObject.transform.GetChild(0).GetComponent<CapsuleCollider>().radius = 0.90f * radius;
            //float localscale = 0.90f * radius * 3 * 2;
            //this.gameObject.transform.GetChild(2).gameObject.transform.localScale = new Vector3(localscale, localscale, localscale);
        }
           
        // this.GetComponentInChildren<CapsuleCollider>().radius = radius;

        //this.gameObject.transform.GetChild(0).GetComponent<Collider>().enabled = false;

    }



    public void StartShrinking()
    {
        CancelInvoke("MeshGrowing");
        InvokeRepeating("MeshShrinking", 1f, 0.2f);
        //photonView.RPC("ColliderOff", RpcTarget.All);
        this.gameObject.transform.GetChild(0).GetComponent<Collider>().enabled = false;
    }

    [PunRPC]
    public void StartSolidify()
    {
        CancelInvoke("MeshGrowing");
        this.gameObject.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        this.GetComponent<Renderer>().material = SolidLavaMaterial;
        //this.gameObject.transform.GetChild(2).GetComponentInChildren<MeshRenderer>().material = SolidLavaMaterial;
        UpdateMesh(false);
    }

    //[PunRPC]
    //void ColliderOff()
    //{
    //    this.gameObject.transform.GetChild(0).GetComponent<Collider>().enabled = false;
    //}

    public void Submit_radia(float _startradius, float _endradius)
    {
        endradius = _endradius;
        startradius = _startradius;
        //Debug.Log("radiussubmit");
        //Debug.Log(endradius);
    }

    public void MeshShrinking()
    {

        //update circle variables
        int circlepoints = 360 / deltaAngle;
        Vertice_Dist = 2f * radius * 3.14f / (float)circlepoints; //the distance between two vertices on the circle edge
        Vertice_Dist = Vertice_Dist * 2;
        radius = radius - 3 * increment;

        //maak hier dan een dict van die opslaat welke radius ze hadden


        #region calculate vertices
        //Debug.Log(Blocked_Vertices.Count);
        for (int i = 1; i <= circlepoints; i++)
        {
            if (!Stored_Vertices.ContainsKey(i))
            {
                //all untouced vertices get updated
                float x2 = radius * Mathf.Sin((i + 1) * deltaAngle * val);
                float y2 = 0;
                float z2 = radius * Mathf.Cos((i + 1) * deltaAngle * val);
                vertices[i] = new Vector3(x2, y2, z2);
                //Debug.Log(vertices[j]);
                //j++;
            }
            else
            {

                // if not vertice not touched anymore, let it quickly grow back but in steps
                if (Stored_Vertices[i] < radius)
                {
                    Stored_Vertices[i] = Stored_Vertices[i] + 2 * increment;
                    float x2 = Stored_Vertices[i] * Mathf.Sin((i + 1) * deltaAngle * val);
                    float y2 = 0;
                    float z2 = Stored_Vertices[i] * Mathf.Cos((i + 1) * deltaAngle * val);
                    vertices[i] = new Vector3(x2, y2, z2);
                }
                else
                {
                    // if it is grown back, let it go back with the others
                    Stored_Vertices.Remove(i);
                    float x2 = radius * Mathf.Sin((i + 1) * deltaAngle * val);
                    float y2 = 0;
                    float z2 = radius * Mathf.Cos((i + 1) * deltaAngle * val);
                    vertices[i] = new Vector3(x2, y2, z2);
                }


            }
        }

        #endregion


        #region calculate triangles
        //create triangles
        //Debug.Log(vertices.Length);
        //j is for indexing in triangles
        //i is for accessing in vertices
        triangles = new int[3 * circlepoints];
        //Debug.Log(triangles.Length);
        int j = 0;
        for (int i = 1; i < (vertices.Length - 1); i++)
        {
            //Debug.Log(j);

            triangles[j + 0] = 0;
            triangles[j + 1] = i;
            triangles[j + 2] = i + 1;
            //Debug.Log(triangles[j]);
            //Debug.Log(triangles[j + 1]);
            //Debug.Log(triangles[j + 2]);
            j = j + 3;

            if (i == vertices.Length - 2)
            {
                triangles[j + 0] = 0;
                triangles[j + 1] = i + 1;
                triangles[j + 2] = 1;
                //j = j+3;

            }





        }
        #endregion


        surface_calculator();
        UpdateMesh(false);


        //if(radius<0.5)
        //{
        //    CancelInvoke("MeshShrinking");
        //    photonView.RPC("MeshGone", RpcTarget.All);

        //}
    }

    [PunRPC]
    void MeshGone()
    {

        //Mesh tempmesh = new Mesh();
        //tempmesh.Clear();
        //tempmesh.vertices = vertices;
        //tempmesh.triangles = triangles;
        this.GetComponent<MeshFilter>().mesh = null;
        //Destroy(this.gameObject);
    }

}


