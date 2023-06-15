    using UnityEngine;

public class NicksPlayerTankController : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject Turret;
    public GameObject BulletSpawnPoint;

    public float RotationSpeed = 150.0f;
    public float TurretRotationSpeed = 10.0f;
    public float CurrentSpeed;
    public float MaxForwardSpeed = 300.0f;
    public float MaxBackwardsSpeed = -300.0f;
    public float ShootRate = 0.5f;
    public float SmoothingFactor = 7.0f;

    private float TargetSpeed;
    //     public float curSpeed, targetSpeed;


    protected float ElapsedTime;
    //    private float elapsedTime;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeapon();
        UpdateControl();

    }

    void UpdateWeapon()
    {
        ElapsedTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (ElapsedTime >= ShootRate)
            {
                //Reset the Time
                ElapsedTime = 0.0f;
                //Instantiate The Bullet
                Instantiate(Bullet, BulletSpawnPoint.transform.position, BulletSpawnPoint.transform.rotation);
            }
        }
    }

    void UpdateControl()
    {
        //Aiming with the mouse
        // Generate a plane that intersects the Transform's
        // position with an upwards normal.
        Plane PlayerPlane = new Plane(Vector3.up, transform.position + new Vector3(0.0f, 0.0f, 0.0f));

        // Generate a rat from the cursors position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects
        // The Plane.


        // Tbh dont know was in the new code... 
        // originally was float HitDist = 0; 
        // , out var hiDist replaced this.

        if (PlayerPlane.Raycast(RayCast, out var HitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);



            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);
            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * TurretRotationSpeed);


        }
        // This whole section
        // If the ray is parrallel to the plane, RayCast Will Return False.
        // Originally


        /* HitDist = 0; 
         * 
         * if(PlayerPlace.Raycast(RayCast, out HitDist)){
         *      //Get the point along the ray that hits the calculated distance.
         *      
         *  Vector3 rayHitPoint = RayCast.GetPoint(HitDist);
         * 
         *  Quarternion TargetRotation = Quarternion.LookRotation(RayHitPoint - transform.position);
         * 
         *  Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, TargetRotation, Time.deltaTime * TurretRotationSpeed
         *  }
         * }
         */


        if (Input.GetKey(KeyCode.W))
        {
            TargetSpeed = MaxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            TargetSpeed = MaxBackwardsSpeed;
        }
        else
        {
            TargetSpeed = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, RotationSpeed * Time.deltaTime, 0.0f);
        }

        CurrentSpeed = Mathf.Lerp(CurrentSpeed, TargetSpeed, SmoothingFactor * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed);
    }
}
