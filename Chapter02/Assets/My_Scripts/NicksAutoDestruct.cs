using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicksAutoDestruct : MonoBehaviour
{

    [SerializeField]
    private float DestructTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestructTime);

    }

}
