using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Collector_NPC : MonoBehaviour
{
    [Header("Needed Objects")] 
    [SerializeField] private Transform golfCart_StopPoint;
    [SerializeField] private Transform ballCarryPoint;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    
    [Header("UI")] 
    [SerializeField] private Slider staminaBar;
    
    [Header("Stats")] 
    [SerializeField] private float maxStamina;
    [SerializeField] private LayerMask ballLayer;


    private bool isResting;
    private float currentStamina;
    private Vector3 lastPosition;
    private BallSC ballToPickUp;
    private States currentState;

    private enum States
    {
        Searching,
        Moving,
        PickingUp,
        Delivering,
        Resting
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
        GameManager.instance.SetStaminaInformation(maxStamina);
        currentState = States.Searching;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.Searching:
                FindBallToPickUp();
                break;
            case States.Moving:
                Moving();
                if(ballToPickUp != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)//If the NPC is going to pick up a ball
                {
                    animator.SetTrigger("Gather");
                    currentState = States.PickingUp;
                }
                break;
            case States.Delivering:
                Moving();
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    GameManager.instance.UpdateScore(ballToPickUp.pts);
                    Destroy(ballToPickUp.gameObject);
                    currentState = States.Resting;
                }
                break;
            case States.Resting:
                if(!isResting)
                    StartCoroutine(Rest());
                break;
        }
    }

    private void FindBallToPickUp()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, currentStamina / 2, ballLayer); //We are checking half distance because NPC should come back to deliver the ball in order to score it.
        
        if(colliders.Length > 0)//If we can still collect balls.
            ballToPickUp = colliders[0].gameObject.GetComponent<BallSC>();
        
        foreach (Collider collider in colliders)
        {
            if (Vector3.Distance(collider.transform.position, transform.position) <= Vector3.Distance(ballToPickUp.transform.position, transform.position))//Check if its closer
            {
                if (collider.GetComponent<BallSC>().ballLevel < ballToPickUp.ballLevel)//If it's a lower level ball skip it.
                {
                    continue;
                }
                else
                {
                    ballToPickUp = collider.GetComponent<BallSC>();
                }
            }
        }

        if (ballToPickUp != null)
        {
            agent.SetDestination(ballToPickUp.transform.position);
            currentState = States.Moving;
        }
        else
        {
            GameManager.instance.EndGame();
        }
    }

    private void Moving()//Handles NPC walking controls and spends stamina accordingly.
    {
        if (agent.remainingDistance > agent.stoppingDistance)//Spend stamina if moving.
        {
            currentStamina -= Vector3.Distance(transform.position, lastPosition);
            staminaBar.value = currentStamina;
            lastPosition = transform.position;
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        
        GameManager.instance.UpdateStaminaInformation(currentStamina);
    }
    
    private float CalculateStamina(float distance)
    {
        float staminaToSpend = 2 * distance; //To go to target and come back.
        return staminaToSpend;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, currentStamina / 2);
    }

    public void GatherEvent() //Animation event for gathering the ball.
    {
        agent.SetDestination(golfCart_StopPoint.position);
        ballToPickUp.transform.SetParent(transform);
        ballToPickUp.transform.position = ballCarryPoint.position;
        currentState = States.Delivering;
    }

    private IEnumerator Rest()
    {
        isResting = true;
        yield return new WaitForSeconds(2f);
        currentState = States.Searching;
        isResting = false;
    }
}
