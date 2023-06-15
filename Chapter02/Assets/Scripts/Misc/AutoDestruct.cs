using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
    [SerializeField]
    public float destructTime = 2.0f;

    void Start()
    {
        Destroy(gameObject, destructTime);
    }
}
