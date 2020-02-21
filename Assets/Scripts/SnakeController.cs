using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    enum DIRECTION { LEFT,RIGHT,UP,DOWN};
    DIRECTION direction;
    public float speed;
    public GameObject bodyPrefab;
    public GameObject markerPrefab;
    List<BodyNode> bodies;
    bool dead = false;
    bool isMovingHorizontally = false;
    [HideInInspector] public List<Vector3> path;
    Vector3 latestStamp;
    public LevelController levelController;
    int point = 0;

    // Start is called before the first frame update
    void Start()
    {
        path = new List<Vector3>();
        bodies = new List<BodyNode>();
        isMovingHorizontally = true;
        latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
        GameObject go = Instantiate(markerPrefab);
        go.transform.position = latestStamp;
        path.Add(new Vector3(latestStamp.x, latestStamp.y, latestStamp.z));
        direction = DIRECTION.LEFT;
    }
    int lastX = 0;
    int lastY = 0;
    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;
        if(Vector3.Distance(latestStamp,transform.position) >= 1f) // cross grid
        {
            latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            GameObject go = Instantiate(markerPrefab);
            go.transform.position = latestStamp;
            ManagePath();
        }

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))&& !isMovingHorizontally)
        {
            Debug.Log("turn");
            //latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            transform.eulerAngles = new Vector3(0, -90, 0);
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));

            //sameposition
            if (Vector3.Distance(transform.position, latestStamp) < 0.1f)
            {
                //transform.position += new Vector3(-1, 0, 0);
            }
            isMovingHorizontally = true;
            direction = DIRECTION.LEFT;
            //ManagePath();
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !isMovingHorizontally)
        {
            Debug.Log("turn");
            //latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            transform.eulerAngles = new Vector3(0, 90, 0);
            isMovingHorizontally = true;
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(transform.position, latestStamp) < 0.1f)
            {
                //transform.position += new Vector3(1, 0, 0);
            }
            direction = DIRECTION.RIGHT;
            //ManagePath();
        }
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isMovingHorizontally)
        {
            Debug.Log("turn");
            //latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            transform.eulerAngles = new Vector3(0, 0, 0);
            isMovingHorizontally = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(transform.position, latestStamp) < 0.1f)
            {
                //transform.position += new Vector3(0, 0, 1);
            }
            direction = DIRECTION.UP;
            //ManagePath();
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isMovingHorizontally)
        {
            Debug.Log("turn");
            //latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            transform.eulerAngles = new Vector3(0, -180, 0);
            isMovingHorizontally = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(transform.position, latestStamp) < 0.1f)
            {
                //transform.position += new Vector3(0, 0, -1);
            }
            direction = DIRECTION.DOWN;
            //ManagePath();
        }

        transform.position += transform.forward * Time.deltaTime * speed;


        if(Input.GetKeyDown(KeyCode.Space))
        {
            Grow();
        }
    }

    void ManagePath()
    {
        //latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));

        if (path.Count == 1)
            return;

        for(int i=0;i<path.Count-1;i++)
        {
            path[path.Count-1-i] = path[path.Count-2-i];
        }

        path[0] = latestStamp;

        for (int i=0;i<bodies.Count;i++)
        {
            bodies[i].SetTarget(path[i]);
            //Debug.Log("path " + i + " " + path[i]);
        }
    }

    void Grow()
    {
        GameObject go = Instantiate(bodyPrefab);
        //print("body count" + bodies.Count);

        if (bodies.Count > 0)
        {
            go.transform.position = bodies[bodies.Count -1].transform.position;

            go.GetComponent<BodyNode>().prevNode = bodies[bodies.Count - 1].gameObject;
            //go.transform.position = new Vector3(Mathf.Round(bodies[bodies.Count - 1].transform.position.x), bodies[bodies.Count - 1].transform.position.y, Mathf.Round(bodies[bodies.Count - 1].transform.position.z));
        }
        else
        {

            go.GetComponent<BodyNode>().prevNode = this.gameObject;
            //go.transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            go.transform.position = transform.position;
        }
        bodies.Add(go.GetComponent<BodyNode>());
        path.Add(new Vector3());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collide" + other.name);
        if(other.gameObject.tag =="Wall")
        {
            //die
            Die();
        }
        else if(other.gameObject.tag == "Item")
        {
            Grow();
            levelController.RandomPosition();
            point++;
        }
        
    }

    private void Die()
    {
        //show Ui stop
        dead = true;
    }
}
