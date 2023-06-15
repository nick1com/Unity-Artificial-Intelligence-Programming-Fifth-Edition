using UnityEngine;

public class NicksSimpleFSM : NicksFSM
{
    public enum FSMState
    {
        None, Patrol, Chase, Attack, Dead,
    }

    // Current state that the NPC is Reaching
    public FSMState CurrentState = FSMState.Patrol;

    // Speed of the tank
    private float CurrentSpeed = 150.0f;

    // tank rotation speed
    private float CurrentRotationSpeed = 2.0f;

    //Bullet 
    public GameObject Bullet;

    //Wheter the npc is destroyed or not.
    private bool bDead = false;
    private int Health = 100;

    //We overwrite the deprecated built-in rigidbody variable
    new private Rigidbody RigidBody;

    //Player transform
    protected Transform PlayerTransform;

    // Next destination position of the npc tank
    protected Vector3 DestinationPosition;

    //list of points for patrolling
    protected GameObject[] PointList;

    //Bullet Shooting Rate
    protected float ShootRate = 3.0f;
    protected float ElapsedTime = 0.0f;
    public float MaxFireAimError = 0.001f;

    //Status Radius
    public float PatrollingRadius = 100.0f;
    public float AttackRadius = 200.0f;
    public float PlayerNearRadius = 300.0f;

    // Tank turret
    public Transform Turret;
    public Transform BulletSpawnPoint;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        //Get list of points
        PointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        // Set random destination point first 
        FindNextPoint();

        //Get the target Enemy (the Player)
        GameObject ObjPlayer = GameObject.FindGameObjectWithTag("Player");

        // Get rigidbody 
        RigidBody = GetComponent<Rigidbody>();
        PlayerTransform = ObjPlayer.transform;
        if (!PlayerTransform)
        {
            print("Player doesn't exist. Please Add one with the Tag name 'Player'.");
        }
    }

    protected override void FSMUpdate()
    {
        switch (CurrentState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }
        //Update the Time 
        ElapsedTime += Time.deltaTime;

        //Go to Dead State is no health left
        if (Health <= 0)
        {
            CurrentState = FSMState.Dead;
        }
    }

    protected void UpdatePatrolState()
    {
        if (Vector3.Distance(transform.position, DestinationPosition) <= PatrollingRadius)
        {
            print("Reached to the destination point \n calculating the next pont");
            CurrentState = FSMState.Chase;

        }
        // Rotate to the Target Point
        Quaternion targetRotation = Quaternion.LookRotation(DestinationPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * CurrentRotationSpeed);

        // Go forward 
        transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed);

    }

    protected void FindNextPoint()
    {
        print("Finding Next Point");
        int RNDIndex = Random.Range(0, PointList.Length);
        float RNDRadius = 10.0f;
        Vector3 RNDPosition = Vector3.zero;
        DestinationPosition = PointList[RNDIndex].transform.position + RNDPosition;

        // Check range to decide the random point as the same as before
        if (IsInCurrentRange(DestinationPosition))
        {
            RNDPosition = new Vector3(Random.Range(-RNDRadius, RNDRadius), 0.0f, Random.Range(-RNDRadius, RNDRadius));
            DestinationPosition = PointList[RNDIndex].transform.position + RNDPosition;
        }
    }
    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50) return true;
        return false;
    }

    protected void UpdateChaseState()
    {
        // Set The target position as the player position
        DestinationPosition = PlayerTransform.position;

        //Check The distance with player tank when the distance is near transition to attack state
        float Distance = Vector3.Distance(transform.position, PlayerTransform.position);
        if (Distance <= AttackRadius)
        {
            CurrentState = FSMState.Attack;
        }
        else if (Distance >= PlayerNearRadius)
        {
            CurrentState = FSMState.Patrol;
        }
        transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed);
    }
    protected void UpdateAttackState()
    {
        DestinationPosition = PlayerTransform.position;

        Vector3 FrontVector = Vector3.forward;

        //Check the distance with the player tank
        float Distance = Vector3.Distance(transform.position, PlayerTransform.position);
        if (Distance >= AttackRadius && Distance < PlayerNearRadius)
        {
            // Rotate target point
            // The rotation is only around the vertical axis of the tank.
            Quaternion TargetRotation = Quaternion.FromToRotation(FrontVector, DestinationPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * CurrentRotationSpeed);

            //Go Forward
            transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed);
            CurrentState = FSMState.Attack;
        }
        else if (Distance >= PlayerNearRadius)
        {
            CurrentState = FSMState.Patrol;
        }

        // Rotate the turrent to the target point
        // The rotation is only around the vertical axis of the tank.
        Quaternion TurretRotation = Quaternion.FromToRotation(FrontVector, DestinationPosition - transform.position);
        Turret.rotation = Quaternion.Slerp(Turret.rotation, TurretRotation, Time.deltaTime * CurrentRotationSpeed);

        // Shoot the Bullet
        if (Mathf.Abs(Quaternion.Dot(TurretRotation, Turret.rotation)) > 1.0f - MaxFireAimError)
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (ElapsedTime >= ShootRate)
        {
            Instantiate(Bullet, BulletSpawnPoint.position, BulletSpawnPoint.rotation);
            ElapsedTime = 0.0f;
        }
    }

    protected void UpdateDeadState()
    {
        // Show the dead animation with some physics Effect
        if (!bDead)
        {
            bDead = true;
            Explode();
        }

    }
    protected void Explode()
    {
        float RNDX = Random.Range(10.0f, 30.0f);
        float RNDZ = Random.Range(10.0f, 30.0f);
        for ( int i = 0; i <3; i++)
        {
            RigidBody.AddExplosionForce(10000.0f, transform.position - new Vector3(RNDX, 10.0f, RNDZ), 40.0f, 10.0f);
            RigidBody.velocity = transform.TransformDirection(new Vector3(RNDX, 20.0f, RNDZ));
        }
        Destroy(gameObject, 1.5f);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Reduce Health
        if(collision.gameObject.tag == "Bullet")
        {
            Health -= collision.gameObject.GetComponent<NicksBullet>().Damage;
        }
    }
}
