using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlockPickUp : MonoBehaviourPunCallbacks, IPunObservable
{

    Transform ParentTransform;
    bool IsHeld;
    public int BlockID;
    // Start is called before the first frame update
    void Start()
    {
        this.IsHeld = false;
        int counter = 0;
        //PhotonView[] View_array = PhotonNetwork.PhotonViews;
        //foreach (PhotonView view in View_array)
        //{
        //        if (view.gameObject.tag == "Block")
        //        {
        //            counter += 1;
        //        }
            
        //}   
        
        //this.photonView.RPC("DefineBlockID", RpcTarget.All, counter + 1);
        this.GetComponent<Rigidbody>().useGravity = true;
    }

    public void SubmitBlockID(int ID)
    {
        this.photonView.RPC("DefineBlockID", RpcTarget.All, ID);
    }

    [PunRPC]
    void DefineBlockID(int ID)
    {
        this.BlockID = ID;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.IsHeld)
        {
            //this.transform.parent = ParentTransform;
            this.photonView.transform.position = ParentTransform.position;
            this.transform.rotation = ParentTransform.rotation;
        }

    }

    public void IsPickedUp(int ParentPlayer)
    {
        photonView.RPC("SyncPickUp", RpcTarget.All, ParentPlayer);
        Debug.Log("try block");
     

    }

    [PunRPC]
    void SyncPickUp(int ParentPlayer)
    {
        PhotonView[] View_array = PhotonNetwork.PhotonViews;
        foreach (PhotonView view in View_array)
        {
            if (view.gameObject.tag == "Player")
            {
                if (view.OwnerActorNr == ParentPlayer)
                {
                    ParentTransform = view.gameObject.GetComponent<BlockPicker>().CarryTransform;
                }
            }

        }
        this.transform.position = ParentTransform.position;
       // this.transform.parent = ParentTransform;

        //this.GetComponent<Rigidbody>().useGravity = false;
       // this.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("Blocktrue");
        this.IsHeld = true;
    }

    public void IsPutDown()
    {
        photonView.RPC("SyncPutDown", RpcTarget.All);

    }

    [PunRPC]
    void SyncPutDown()
    {
       // this.transform.parent = null;
        //this.GetComponent<Rigidbody>().useGravity = true;
        //this.GetComponent<BoxCollider>().enabled = true;
        Debug.Log("Blockfalse");
        this.IsHeld = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
   

        if (stream.IsWriting)
        {
           
        }
        else
        {
          
        }


    }


}
