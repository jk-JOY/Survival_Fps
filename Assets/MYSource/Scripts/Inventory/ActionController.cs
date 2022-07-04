using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionController : MonoBehaviour
{

    [SerializeField]
    private float range;
    private bool pickupActivated;


    private RaycastHit hitInfo;

    //æ∆¿Ã≈€ ∑π¿ÃæÓ∏∏
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text actionText;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

 

    private void CheckItem()
    {

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if(hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
        {
            ItemInfoDisAppear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickup>().item.itemName + "»πµÊ" + "<color= yellow>" + "(E)" + "</color>";

    }
    private void ItemInfoDisAppear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickup>().item.itemName + "»πµÊ");
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisAppear();
            }
        }
    }
}
