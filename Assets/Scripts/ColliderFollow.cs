using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFollow : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z);
    }
}
