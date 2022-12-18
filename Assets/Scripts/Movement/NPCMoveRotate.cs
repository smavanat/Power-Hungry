using UnityEngine;
using System.Collections;

public class NPCMoveRotate : MonoBehaviour
{
    enum State {
        Idle,
        Chase, 
        Attack
    }
    private ContextSteering contextSteering;
    private ContextReturnData contextReturnData;
    public float movementSpeed = 5f;
    public float rotationSpeed = 150f;
    public float rotationOffset = 90f;
    private GameObject player; 
    private Transform target;
    State state = State.Idle;
    //Values for shooting
    public Transform enemyFirePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 1f;
    bool alreadyAttacked = false;
    int timeBetweenAttacks = 1;
    
    private void Start() {
        contextSteering = GetComponent<ContextSteering>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
    }

    private void Update() {
        switch(state) {
            case State.Idle:
                IdleState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                AttackState();
                break;
        }
    }

#region States
    private void IdleState() {
        if(Vector3.Distance(transform.position, target.position) < 10f) {
            state = State.Chase;
        }
    }
    private void ChaseState() {
        //Set the context to tick so it updates
        contextReturnData = contextSteering.Tick(target);

        if(Vector3.Distance(transform.position, target.position) > 2.5f){
            //Move
            transform.position += (contextReturnData.bestPoint - transform.position).normalized * movementSpeed * Time.deltaTime;
        
            RotateGameObject(contextReturnData.bestPoint, rotationSpeed, rotationOffset);
        }
        else if (Vector3.Distance(transform.position, target.position) < 2.5f){
            state = State.Attack;
        }
        else if(Vector3.Distance(transform.position, target.position) > 12f) {
            state = State.Idle;
        }
    }
    private void AttackState() {
        contextReturnData = contextSteering.Tick(target);
        RotateGameObject(contextReturnData.bestPoint, rotationSpeed, rotationOffset);
        if(!alreadyAttacked) {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        else if (Vector3.Distance(transform.position, target.position) > 3.5) {
            state = State.Chase;
        }
    }
#endregion

#region Calculations and Helpers
    private void RotateGameObject(Vector3 target, float rotationSpeed, float rotationOffset)
    {
        //get the direction of the other object from current object
        Vector3 dir = target - transform.position;
        //get the angle from current direction facing to desired target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //set the angle into a quaternion + sprite offset depending on initial sprite facing direction
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle - rotationOffset));
        //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


    private void Shoot() {
        SoundManager.PlaySound("enemyBullet");
        GameObject bullet = Instantiate(bulletPrefab, enemyFirePoint.position, enemyFirePoint.rotation);
        Rigidbody2D rb =  bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(enemyFirePoint.up * bulletForce, ForceMode2D.Impulse);
    }
    private void ResetAttack() {
        alreadyAttacked = false;
    }
}
#endregion