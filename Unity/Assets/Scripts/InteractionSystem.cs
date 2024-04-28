using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InteractionSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Detection Fields")]
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;
    //Cached Trigger Object
    public GameObject detectedObject;
    [Header("Examine Fields")]
    //Examine window object
    public GameObject examineWindow;
    public GameObject grabbedObject;
    public float grabbedObjectYValue;
    public Transform grabPoint;
    public Image examineImage;
    public Text examineText;
    public bool isExamining;
    public bool isGrabbing;

    private ExamineRef examineRef;

    private void Awake()
    {
        examineRef = ExamineRef.instance;

        examineWindow = examineRef.examineWindow;
        examineImage = examineRef.examineImage;
        examineText = examineRef.examineText;
    }



    void Update()
    {
        if (DetectObject())
        {
            if (InteractInput())
            {
                //If we are grabbing something don't interact with other items, drop the grabbed item first
                if (isGrabbing)
                {
                    GrabDrop();
                    return;
                }

                detectedObject.GetComponent<Item>().Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {

        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);

        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public void ExamineItem(Item item)
    {
        if (isExamining)
        {
            examineWindow.SetActive(false);
            isExamining = false;
            photonView.RPC("StopExamine", RpcTarget.All);
        }
        else
        {
            photonView.RPC("StartExamine", RpcTarget.All, item.descriptionText, item.GetComponent<SpriteRenderer>().sprite);
        }
    }

    [PunRPC]
    void StartExamine(string descriptionText, Sprite sprite)
    {
        examineImage.sprite = sprite;
        examineText.text = descriptionText;
        examineWindow.SetActive(true);
        isExamining = true;
    }

    [PunRPC]
    void StopExamine()
    {
        examineWindow.SetActive(false);
        isExamining = false;
    }


    public void GrabDrop()
    {
        if (isGrabbing)
        {
            photonView.RPC("DropItem", RpcTarget.All);
        }
        else
        {
            photonView.RPC("GrabItem", RpcTarget.All);
        }
    }


    [PunRPC]
    void GrabItem()
    {
        //Check if we do have a grabbed object => drop it
        if (isGrabbing)
        {
            //make isGrabbing false
            isGrabbing = false;
            //unparent the grabbed object
            grabbedObject.transform.parent = null;
            //set the y position to its origin
            grabbedObject.transform.position =
                new Vector3(grabbedObject.transform.position.x, grabbedObjectYValue, grabbedObject.transform.position.z);
            //null the grabbed object reference
            grabbedObject = null;
        }
        //Check if we have nothing grabbed grab the detected item
        else
        {
            //Enable the isGrabbing bool
            isGrabbing = true;
            //assign the grabbed object to the object itself
            grabbedObject = detectedObject;
            //Parent the grabbed object to the player
            grabbedObject.transform.parent = transform;
            //Cache the y value of the object
            grabbedObjectYValue = grabbedObject.transform.position.y;
            //Adjust the position of the grabbed object to be closer to hands                        
            grabbedObject.transform.localPosition = grabPoint.localPosition;
        }
    }

    [PunRPC]
    void DropItem()
    {
        //Check if we do have a grabbed object => drop it
        if (isGrabbing)
        {
            //make isGrabbing false
            isGrabbing = false;
            //unparent the grabbed object
            grabbedObject.transform.parent = null;
            //set the y position to its origin
            grabbedObject.transform.position =
                new Vector3(grabbedObject.transform.position.x, grabbedObjectYValue, grabbedObject.transform.position.z);
            //null the grabbed object reference
            grabbedObject = null;
        }
        //Check if we have nothing grabbed grab the detected item
        else
        {
            //Enable the isGrabbing bool
            isGrabbing = true;
            //assign the grabbed object to the object itself
            grabbedObject = detectedObject;
            //Parent the grabbed object to the player
            grabbedObject.transform.parent = transform;
            //Cache the y value of the object
            grabbedObjectYValue = grabbedObject.transform.position.y;
            //Adjust the position of the grabbed object to be closer to hands                        
            grabbedObject.transform.localPosition = grabPoint.localPosition;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isExamining);
            stream.SendNext(isGrabbing);
            stream.SendNext(grabbedObjectYValue);
            stream.SendNext(grabPoint.localPosition);
            stream.SendNext(detectedObject != null ? detectedObject.GetPhotonView().ViewID : -1);
        }
        else
        {
            isExamining = (bool)stream.ReceiveNext();
            isGrabbing = (bool)stream.ReceiveNext();
            grabbedObjectYValue = (float)stream.ReceiveNext();
            grabPoint.localPosition = (Vector3)stream.ReceiveNext();
            int detectedObjectID = (int)stream.ReceiveNext();
            if (detectedObjectID == -1)
            {
                detectedObject = null;
            }
            else if (detectedObject == null || detectedObject.GetPhotonView().ViewID != detectedObjectID)
            {
                detectedObject = PhotonView.Find(detectedObjectID).gameObject;
            }
        }
    }

}