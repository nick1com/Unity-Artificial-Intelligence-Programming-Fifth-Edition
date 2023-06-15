using UnityEngine;

public class NicksBullet : MonoBehaviour
{

    //Explosion Effect
    [SerializeField]
    // Used to expose th the inspector private fields!
    private GameObject Explosion;
    [SerializeField]
    private float Speed = 600.0f;
    [SerializeField]
    private float LifeTime = 3.0f;
    public int Damage = 50;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;

    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint Contact = collision.contacts[0];
        Instantiate(Explosion, Contact.point, Quaternion.identity);
        Destroy(gameObject);
    }
}