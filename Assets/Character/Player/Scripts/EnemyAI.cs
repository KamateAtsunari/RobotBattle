using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //攻撃などの対象となるオブジェクト
    [SerializeField] private GameObject targetObj = null;
    //追尾範囲
    [SerializeField] private float detectionRadius = 4f;
    //追尾対象のオブジェクトのタグ
    [SerializeField] private List<string> enemyTags = new List<string>();
    //キャラクターの頭部を固定するかどうか
    [SerializeField] private bool enableHeadLook = false;
    [SerializeField] private float headLookIntensity = 1f;

    [SerializeField] private bool isPatrolling = false;
    [SerializeField] private List<GameObject> patrolPoints = null;
    [SerializeField] private bool patrolInOrder = true;

    private CharacterBase characterBase;
    private Animator p_Animator;
    private Animator animator;
    //攻撃可能かどうか
    private bool canAttack = true;
    private bool pointReached = false;
    private bool enemyFound = false;
    private GameObject patrolTarget;
    private int patrolCount = 0;


    [HideInInspector] public int CURRENT_STATE = CharacterStates.STATE_IDLE;

    public class CharacterStates
    {
        //キャラークターの状態
        public const int STATE_IDLE = 0;
        public const int STATE_DAME = 1;
        public const int STATE_ATTACK = 2;
        public const int STATE_FOLLOW = 3;
        public const int STATE_PATROL = 4;
        public const int STATE_GUARD = 5;
        public const int STATE_DEAD = 6;

        public const int ATTACK_BREAK = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterBase = GetComponent<CharacterBase>();
        p_Animator = targetObj.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPatrolling)
        {
            CURRENT_STATE = CharacterStates.STATE_PATROL;
        }
        else if (CURRENT_STATE == CharacterStates.STATE_PATROL)
        {
            CURRENT_STATE = CharacterStates.STATE_IDLE;
        }
        if (CURRENT_STATE != CharacterStates.STATE_DAME && CURRENT_STATE != CharacterStates.STATE_DEAD)
        {
            switch (CURRENT_STATE)
            {
                case CharacterStates.STATE_FOLLOW:
                    FollowTarget();
                    break;
                case CharacterStates.STATE_PATROL:
                    Patrol();
                    break;
                case CharacterStates.STATE_IDLE:
                    ResetToIDLE();
                    break;
                default:
                    break;
            }
            ScanForObjects(gameObject.transform.position, detectionRadius);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (CURRENT_STATE == CharacterStates.STATE_PATROL)
        {
            if (collision.gameObject.tag == "PatrolPoint" && collision.gameObject.name == patrolTarget.gameObject.name)
            {
                pointReached = true;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableHeadLook && targetObj != null)
        {
            animator.SetLookAtPosition(targetObj.transform.position);
            animator.SetLookAtWeight(headLookIntensity);
        }

    }
    private void ResetToIDLE()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        //Animator.SetBool("isAttacking", false);
    }
    protected void Patrol()
    {
        //
        //Method Name : void Patrol()
        //Purpose     : This method allows the character to progress towards the patrol points.
        //Re-use      : none
        //Input       : none
        //Output      : none
        //
        if (pointReached)
        {
            pointReached = false;
            if (patrolInOrder)
            {
                if (patrolCount < patrolPoints.Count - 1)
                {
                    patrolCount++;
                }
                else
                {
                    patrolCount = 0;
                }
            }
            else
            {
                Random rnd = new Random();
                patrolCount = Random.Range(0, patrolPoints.Count);
            }
        }
        else
        {
            if (targetObj == null)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", true);
                //Animator.SetBool("isAttacking", false);
                patrolTarget = patrolPoints[patrolCount];
                LookAt(patrolTarget);
                //transform.position += transform.forward * MovementSpeed * Time.deltaTime;
            }
            else
            {
                animator.SetBool("isWalking", false);
                PatrolFollow();
            }
        }

    }
    private void PatrolFollow()
    {
        //
        //Method Name : void PatrolFollow()
        //Purpose     : This method allows the character to follow an enemy,
        //              and then return to patroling once the enemy is dead or out of range.
        //Re-use      : none
        //Input       : none
        //Output      : none
        //
        if (targetObj != null)
        {
            float distance = Vector3.Distance(transform.position, targetObj.transform.position);
            if (distance <= characterBase.GetAttackRenge())
            {
                LookAt(targetObj);
                if (canAttack)
                {
                    animator.SetBool("isRunning", false);
                    //Animator.SetBool("isAttacking", true);
                    animator.SetTrigger("Attack");
                }
            }
            else
            {
                //transform.position += transform.forward * MovementSpeed * Time.deltaTime;

                animator.SetBool("isRunning", true);
                //Animator.SetBool("isAttacking", false);
            }
        }
    }
    protected void FollowTarget()
    {
        //Debug.Log("TEST");
        //
        //Method Name : void FollowTarget()
        //Purpose     : This method moves the character to where the target position is. In most casess, the player position.
        //Re-use      : none
        //Input       : none
        //Output      : none
        if (targetObj != null)
        {
            float distance = Vector3.Distance(transform.position, targetObj.transform.position);
            //攻撃範囲
            if (distance - 0.3f <= characterBase.GetAttackRenge())
            {
                //Debug.Log(canAttack);
                if (canAttack)
                {

                    CURRENT_STATE = CharacterStates.STATE_ATTACK;
                    animator.SetBool("isRunning", false);

                    if (CURRENT_STATE != CharacterStates.STATE_ATTACK && CURRENT_STATE != CharacterStates.STATE_PATROL)
                    {
                        if (CURRENT_STATE != CharacterStates.STATE_PATROL)
                        {
                            CURRENT_STATE = CharacterStates.STATE_IDLE;
                        }
                        return;
                    }

                    //Player防御時
                    if (p_Animator.GetBool("isGuard"))
                    {
                        int rand = Random.Range(0, 9);
                        canAttack = false;
                        if (rand < 5)
                        {
                            //canAttack = false;
                            animator.SetTrigger("Kick");
                        }
                        else
                        {
                            animator.SetTrigger("Attack");
                        }
                    }
                    //Playerスタン時
                    else if (p_Animator.GetBool("isStun"))
                    {
                        animator.SetTrigger("SAttack");
                        canAttack = false;
                        //Debug.Log("SA");
                        //animator.SetFloat("SAttackSpeed", 1.0f);
                        StartCoroutine(ChargeAttack(4f));
                        //}
                    }
                    else
                    {
                        animator.SetTrigger("Attack");
                        canAttack = false;
                    }

                    if (CURRENT_STATE != CharacterStates.STATE_PATROL)
                    {
                        CURRENT_STATE = CharacterStates.STATE_IDLE;
                    }

                }
                else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Kick") && !animator.IsInTransition(0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("SAttack") && !animator.IsInTransition(0))
                {
                    canAttack = true;
                }
            }
            //追尾範囲
            else
            {
                animator.SetBool("isRunning", true);
                canAttack = true;
            }
        }
    }
    private void ScanForObjects(Vector3 center, float radius)
    {

        //
        //Method Name : void ScanForObjects(Vector3 center, float radius) 
        //Purpose     : This method uses the Physics.OverlapSphere method to scan for objects within a given radius.
        //Re-use      : none
        //Input       : Vector3 center, float radius
        //Output      : none
        //
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        enemyFound = false;
        while (i < hitColliders.Length)
        {
            //Debug.Log("Ch");
            //Filter out all GameObjects to get only those that are enemies.
            if (enemyTags.Contains(hitColliders[i].tag))
            {
                targetObj = hitColliders[i].gameObject;
                LookAt(targetObj);
                if (CURRENT_STATE == CharacterStates.STATE_IDLE && CURRENT_STATE != CharacterStates.STATE_PATROL)
                {
                    CURRENT_STATE = CharacterStates.STATE_FOLLOW;
                }

                enemyFound = true;

            }
            i++;
        }
        if (!enemyFound)
        {
            targetObj = null;
            if (CURRENT_STATE != CharacterStates.STATE_PATROL)
            {
                CURRENT_STATE = CharacterStates.STATE_IDLE;
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                //Animator.SetBool("isAttacking", false);
            }
        }
    }
    private void LookAt(GameObject Target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
    }
    void Attack(){canAttack = true;}
    void Kick(){ canAttack = true; }
    void ComboEnd(){canAttack = true;}
    void SAttack(){ canAttack = true;}
    void Daeth(){canAttack = true;}
    IEnumerator ChargeAttack(float wait)
    {
        yield return new WaitForSeconds(wait);
        //Debug.Log("A");
        animator.SetFloat("SAttackSpeed", 1.0f);
        //interval = false;
    }
}
