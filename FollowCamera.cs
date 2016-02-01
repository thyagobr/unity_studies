using UnityEngine;
using System.Collections.Generic;

class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public GameObject camera_target;

    void Start()
    {
    }

    void Update()
    {/*
        target = Overlord.instance.current_player.gameObject;

        if (target)
        {
            transform.LookAt(camera_target.transform.position);
            transform.position = target.transform.position + new Vector3(-2f, 2f, -5f);
        }*/
    }
}