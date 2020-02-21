using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SnakeController : MonoBehaviour
{

    public float speed;
    public GameObject bodyPrefab;
    public GameObject markerPrefab;
    public Text scoreText;
    public GameObject TextPanel;

    List<BodyNode> bodies;
    bool dead = false;
    bool isMovingHorizontally = false;
    [HideInInspector] public List<Vector3> path;
    Vector3 latestStamp;
    public LevelController levelController;
    int point = 0;
    Vector3 originPos;
    Vector3 originRotation;
    // Start is called before the first frame update
    void Start()
    {
        dead = true;
        originRotation = transform.eulerAngles;
        originPos = transform.position;
        path = new List<Vector3>();
        bodies = new List<BodyNode>();
        ResetLevel();
    }

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
            transform.eulerAngles = new Vector3(0, -90, 0);
            Vector3 snapPos = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(snapPos, latestStamp) < 0.1f) //samepos
            {
                snapPos += new Vector3(-1, 0, 0);
                //latestStamp = snapPos;
            }
            transform.position = snapPos;

            isMovingHorizontally = true;


        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !isMovingHorizontally)
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
            Vector3 snapPos = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(snapPos, latestStamp) < 0.1f) //samepos
            {
                snapPos += new Vector3(1, 0, 0);
                //latestStamp = snapPos;
            }
            isMovingHorizontally = true;
            transform.position = snapPos;


        }
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isMovingHorizontally)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            Vector3 snapPos = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(snapPos, latestStamp) < 0.1f) //samepos
            {
                snapPos += new Vector3(0, 0, 1);
                //latestStamp = snapPos;
            }
            isMovingHorizontally = false;
            transform.position = snapPos;

        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isMovingHorizontally)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            Vector3 snapPos = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            if (Vector3.Distance(snapPos, latestStamp) < 0.1f) //samepos
            {
                snapPos += new Vector3(0, 0, -1);
                //latestStamp = snapPos;
            }
            isMovingHorizontally = false;
            transform.position = snapPos;

        }

        transform.position += transform.forward * Time.deltaTime * speed;
        if(Input.GetKeyDown(KeyCode.Space))
            Grow(); 
    }

    void ManagePath()
    {
        //if (path.Count == 1)
        //    return;

        for(int i=0;i<path.Count-1;i++)
        {
            path[path.Count-1-i] = path[path.Count-2-i];
        }

        path[0] = latestStamp;

        for (int i=0;i<bodies.Count;i++)
        {
            bodies[i].SetTarget(path[i]);
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
            go.GetComponent<BodyNode>().isFirst = true;
            go.GetComponent<BodyNode>().killAble = false;
            //go.transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            go.transform.position = transform.position;
        }
        bodies.Add(go.GetComponent<BodyNode>());
        path.Add(new Vector3());
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collide" + other.name);
        if (other.gameObject.tag == "Wall")
        {
            //die
            Die();
        }
        else if (other.gameObject.tag == "Body")
        {
            if (other.GetComponent<BodyNode>().killAble)
                Die();
        }
        else if (other.gameObject.tag == "Item" )
        {
            Grow();
            levelController.RandomPosition();
            point++;
            scoreText.text = point.ToString();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Body")
        {
            other.GetComponent<BodyNode>().killAble = true;
        }
    }

    private void Die()
    {
        //show Ui stop
        dead = true;
        TextPanel.SetActive(true);
        //scoreText.gameObject.SetActive(true);
    }

    public void ResetLevel()
    {
        

        KillAllBodies();
        bodies.Clear();
        path.Clear();
        TextPanel.SetActive(false);
        point = 0;
        scoreText.text = point.ToString();


        dead = false;
        isMovingHorizontally = true;
        transform.position = originPos;
        transform.eulerAngles = originRotation;
        latestStamp = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
        path.Add(new Vector3(latestStamp.x, latestStamp.y, latestStamp.z));
    }

    void KillAllBodies()
    {
        foreach(BodyNode go in bodies)
        {
            Destroy(go.transform.gameObject);
        }
    }
}
