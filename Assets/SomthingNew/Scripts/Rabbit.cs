using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Rabbit : MonoBehaviour
{
    public GameObject deathPrefab;
    public bool runningAI = false;
    public LayerMask itemLayerMask;
    public GameObject PlusPrefab;
    public NavMeshAgent agent;
    public float Speed
    {
        get
        {
            return 10f; // GameManager.instance.bunnySpeed;
        }
    }

    public float moveChance = .005f;
    public float turningSpeed = 6f;
    bool moving = false;
    Vector3 movePos;
    float growthCounter = 0;
    float scale = 0.5f;

    private void OnEnable()
    {
        // if (GameManager.instance.BunnyCount > 2)
        // {
        //     transform.localScale = new Vector3(scale, scale, scale);
        // }
        //  else
        //  {
        scale = Random.Range(.5f, 1f);
        transform.localScale = new Vector3(scale, scale, scale);
        //     transform.localScale = new Vector3(1, 1, 1);
        // }
        // GameObject p = Pooling.Pooler.root.GetPooledInstance(PlusPrefab);
        // p.transform.position = transform.position + transform.up;
        // p.SetActive(true);
        StartCoroutine(RabbitAI());


    }

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        moving = false;

    }


    void Update()
    {

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void MoveTowardTarget(Vector3 targetVector)
    {
        agent.destination = targetVector;
        agent.angularSpeed = turningSpeed;

    }
    Vector3 lastPos = Vector3.zero;
    private IEnumerator RabbitAI()
    {
        while (true)
        {
            if (!moving)
            {
                if (Random.Range(0f, 1f) <= moveChance)
                {

                    Vector3 randomDirection = Random.insideUnitSphere * 5f;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, 5f, 1);
                    movePos = hit.position;

                    moving = true;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, movePos) <= .05f)
                {
                    moving = false;
                }
                else
                {
                    MoveTowardTarget(movePos);

                    if (lastPos != transform.position)
                    {
                        moving = false;
                    }
                }
            }
            lastPos = transform.position;
            yield return null;
        }
    }
}
