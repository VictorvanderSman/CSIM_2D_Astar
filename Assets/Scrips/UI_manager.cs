using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_manager : MonoBehaviour
{
    public GameObject agent;

    public int AgentCounter;

    public int agents_per_press = 5;
    public Vector3 Starting_position;

    public Text QueueCounter;  // public if you want to drag your text object in there manually

    public Text Agent_count;
    int scoreCounter = 0;
    
    PathfindingManager Manager;

    void Start() 
    {
        Manager = GetComponent<PathfindingManager>();
        

        // QueueCounter = GetComponent<Text>(); 
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space key was pressed, time to spawn some agents");
            for(int i = 0; i < agents_per_press; i++ )
            {
                CreateAgent();
            }
        }

        // See how many agents are currently requesting a path 
        scoreCounter = (Manager.pathRequestQueue.Count);

        Agent_count.text = "Agents in the scene: " + AgentCounter.ToString();

        QueueCounter.text = "In queue at the moment: " + scoreCounter.ToString();  // make it a string to output to the Text object
    }
    void CreateAgent (){
        
        AgentCounter++;

        Instantiate(agent, Starting_position, Quaternion.identity);
    }
}
