using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class BlockSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

   // public GameObject Lava;
    // public Player Player;
    public GameObject Block;
    int Block_counter = 0;
    Vector3 SpawnPosition = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateBlocks();
            InvokeRepeating("CheckBlockReplacement", 10f, 10f);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
      

    }

    void InstantiateBlocks()
    {
       
        for (int i = -4; i <= 4; i = i+2)
        {
            for (int j = -4; j <= 4; j = j+2)
            {
                SpawnPosition = new Vector3(i, 0.75f, j);
                GameObject SpawnedBlock = PhotonNetwork.Instantiate(Block.name, SpawnPosition, Quaternion.identity);
                Block_counter += 1;
                SpawnedBlock.GetComponent<BlockPickUp>().SubmitBlockID(Block_counter);
            }
             
        }
       
    }

    void CheckBlockReplacement()
    {
        for (int i = -4; i <= 4; i = i + 2)
        {
            for (int j = -4; j <= 4; j = j + 2)
            {
                SpawnPosition = new Vector3(i, 0.75f, j);
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(SpawnPosition + 10*Vector3.up, Vector3.down,  out hit, 9.9f))
                {
                    
                }
                else
                {
                    GameObject SpawnedBlock = PhotonNetwork.Instantiate(Block.name, SpawnPosition, Quaternion.identity);
                    Block_counter += 1;
                    SpawnedBlock.GetComponent<BlockPickUp>().SubmitBlockID(Block_counter);
                }
            }

        }
    }

   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


    }


  



}