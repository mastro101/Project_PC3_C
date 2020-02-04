using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwitch : MonoBehaviour
{
    public GameObject Room_1;
    public GameObject Room_2;

    private void OnTriggerEnter(Collider collision)
    {
        if (Room_1.active == true)
        {
            Room_1.SetActive(false);
            Room_2.SetActive(true);
        }else if (Room_1.active == false)
        {
            Room_1.SetActive(true);
            Room_2.SetActive(false);
        }
    }


}
