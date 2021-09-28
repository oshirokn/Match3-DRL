using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentMatch3 : Agent
{
    public Camera camera;
    public Board board;
    [SerializeField] float timer = 30;
    int totalReward;
    [SerializeField]  int maxReward = 15;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator TimerBeforeEndEpisode()
    {
        yield return new WaitForSeconds(timer);
        Debug.Log("Total Episode Reward:" + totalReward);
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        totalReward = 0;
        StartCoroutine(TimerBeforeEndEpisode());
    }

 

    public override void OnActionReceived(ActionBuffers actions)
    {
        Transform objectHit;

        // Get screen position X Y
        int positionX = actions.DiscreteActions[0];
        int positionY = actions.DiscreteActions[1];
        Vector2 newMousePosition = new Vector2(positionX, positionY);
        //Debug.Log("Raycast position:" + newMousePosition);
        // Get movement direction
        int moveGemDirection = actions.DiscreteActions[2];
        int offsetX = 0, offsetY= 0;
        switch (moveGemDirection)
        {
            case 0:
                // Move mouse up
                offsetY = 5;
                break;
            case 1:
                // Move mouse right
                offsetX = 5;
                break;
            case 2:
                // Move mouse down
                offsetY = -5;
                break;
            case 3:
                // Move mouse left
                offsetX = -5;
                break;
        }

        // Raycast from camera to the selected point and move gem if hit
        
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("Gem");
        Ray ray = camera.ScreenPointToRay(newMousePosition);
        if (Physics.Raycast(ray, out hit, layer_mask))
        {
            //Debug.Log("Gem hit");
            objectHit = hit.transform;
            objectHit.GetComponent<Dot>().MoveGem(offsetX, offsetY);
        }
        else
        {
            //Debug.Log("Did not hit");
            //SetAgentReward(-1);
        }

        /*

        // TO DO Move mouse to new position
        //InputState.Change(Mouse.current.position.ReadValue(), selectGem);  // New Input system
        // TO DO MouseDown

        // Get action: mouse direction up, down, left or right
        int moveGemDirection = actions.DiscreteActions[2];
        switch(moveGemDirection)
        {
            case 0:
                // Move mouse up
                newMousePosition = new Vector2(newMousePosition.x, newMousePosition.y + 5);
                break;
            case 1:
                // Move mouse right
                newMousePosition = new Vector2(newMousePosition.x +5, newMousePosition.y);
                break;
            case 2:
                // Move mouse down
                newMousePosition = new Vector2(newMousePosition.x, newMousePosition.y - 5);
                break;
            case 3:
                newMousePosition = new Vector2(newMousePosition.x -5, newMousePosition.y);
                // Move mouse left
                break;
        }
        // MouseUp 
            // TO DO
            */

    }

    public void SetAgentReward(int reward)
    {
        SetReward(reward);
        totalReward += reward;
        if(totalReward > maxReward)
        {
            Debug.Log("Total Episode Reward:" + totalReward);
            Debug.Log("Maximum reward reached, end episode");
            StopCoroutine(TimerBeforeEndEpisode());
            EndEpisode();
        }
    }
}
