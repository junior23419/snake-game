using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyNode : MonoBehaviour
{
    public GameObject prevNode;
    SnakeController snakeController;
    // Start is called before the first frame update
    List<Vector3> targets;
    List<Vector3> directions;
    float travelledDistance = 0;


    void Start()
    {
        directions = new List<Vector3>();
        targets = new List<Vector3>();
        snakeController = GameObject.Find("SnakeHead").GetComponent<SnakeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count>0 && Vector3.Distance(prevNode.transform.position, gameObject.transform.position) > 1f)
        {
            transform.position += directions[0] * snakeController.speed*Time.deltaTime*1.2f;
            travelledDistance += directions[0].magnitude * snakeController.speed * Time.deltaTime * 1.2f;

            if (Vector3.Distance(targets[0], transform.position) < 0.05f||travelledDistance >1f) 
            {
                Debug.Log("travelled" + travelledDistance);
                transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
                travelledDistance = 0;
                targets.RemoveAt(0);
                directions.RemoveAt(0);
            }
        }
        //else if (Vector3.Distance(prevNode.transform.position,gameObject.transform.position)>1f)
        //{
        //    canMove = true;
        //}
    }

    public void SetTarget(Vector3 target)
    {
        Vector3 dir;
        if (targets.Count > 0)
            dir = target - targets[targets.Count - 1];
        else
            dir = target - transform.position;
        dir.Normalize();
        directions.Add(dir);
        targets.Add(target);
        
        //Debug.Log("direction " + direction +" target" +target + " current"+transform.position);
        //direction.Normalize();
        //if (direction.x > -0.1 && direction.x < 0.1)
        //    direction.x = 0;
        //else
        //    direction.x = direction.x < 0 ? -1 : 1;

        //direction.y = 0;

        //if (direction.z > -0.1 && direction.z < 0.1)
        //    direction.z = 0;
        //else
        //    direction.z = direction.z < 0 ? -1 : 1;

        //if(direction.x==0 && direction.z==0)
        //Debug.Log("direction " + direction);
    }

}
