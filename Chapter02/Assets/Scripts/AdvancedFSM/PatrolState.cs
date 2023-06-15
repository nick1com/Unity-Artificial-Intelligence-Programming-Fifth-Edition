using UnityEngine;public class PatrolState : FSMState
{    private Vector3 destPos;    private Transform[] waypoints;    private float curRotSpeed = 1.0f;    private float curSpeed = 100.0f;    private float playerNearRadius;    private float patrolRadius;    public PatrolState(Transform[] wp, float playerNearRadius, float patrolRadius)
    {        waypoints = wp;        stateID = FSMStateID.Patrolling;        this.playerNearRadius = playerNearRadius;        this.patrolRadius = patrolRadius;    }    public override void CheckTransitionRules(Transform player, Transform npc)
    {
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        if (Vector3.Distance(npc.position, player.position) <= playerNearRadius)
        {            Debug.Log("Switch to Chase State");            NPCTankController npcTankController = npc.GetComponent<NPCTankController>();            if (npcTankController != null)
            {                npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);            }
            else
            {                Debug.LogError("NPCTankController not found in NPC");            }        }    }    public override void RunState(Transform player, Transform npc)
    {
        //Find another random patrol point if the current point is reached

        if (Vector3.Distance(npc.position, destPos) <= patrolRadius)
        {            Debug.Log("Reached the destination point\ncalculating the next point");            FindNextPoint();        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, destPos - npc.position);        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);    }

















































    /// <summary>    /// Find the next semi-random patrol point    /// </summary>    public void FindNextPoint()
    {
        //Debug.Log("Finding next point");
        int rndIndex = Random.Range(0, waypoints.Length);        Vector3 rndPosition = Vector3.zero;        destPos = waypoints[rndIndex].position + rndPosition;    }}