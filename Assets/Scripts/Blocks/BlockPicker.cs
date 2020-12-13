using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlockPicker : MonoBehaviourPunCallbacks
{

    BlockPickUp PickedBlock;

    [SerializeField]
    public Transform CarryTransform;

    bool HasBlock;
    public int HasBlockID;
    // Start is called before the first frame update
    void Start()
    {
        this.HasBlock = false;
        this.HasBlockID = 0;
    }
      
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
          
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                if (HasBlock == false)
                {

                    TryPickUp();

                }
                else
                {
                    DoPutDown();
                }

            }
        }
       
    }

    private void DoPutDown()
    {
        PickedBlock.IsPutDown();
        HasBlock = false;
        Debug.Log("falsepick");
        HasBlockID = 0;
    }

   private void TryPickUp()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Debug.Log(this.gameObject.transform.position);
        Debug.Log(transform.TransformDirection(Vector3.forward));
        if (Physics.Raycast(this.gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hit, 5))
        {
            Debug.Log("trycatch");
            Debug.DrawRay(this.gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.yellow);
            if (hit.transform.gameObject.tag == "Block")
            {
                PickedBlock = hit.transform.gameObject.GetComponent<BlockPickUp>();
                PickedBlock.IsPickedUp(photonView.OwnerActorNr);
                HasBlock = true;
                Debug.Log("truepick");
                HasBlockID = PickedBlock.BlockID;
            }
        }
        else
        {
            HasBlock = false;
            HasBlockID = 0;
        }
    }

}
