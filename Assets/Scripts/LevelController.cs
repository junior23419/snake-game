using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject item;
    public SnakeController snake;

    // Start is called before the first frame update
    void Start()
    {
        RandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomPosition()
    {
        var exclude = snake.path;
        var range = Enumerable.Range(-5, 11).Select(i => Enumerable.Range(-5, 11).Select(j => new Vector3(i, snake.transform.position.y, j)));
        List<List<Vector3>> filteredVectors = new List<List<Vector3>>();
        int size = 0;
        foreach (IEnumerable<Vector3> a in range.ToList())
        {
            var filtered = a.ToList().Where(vector=> !IsVectorUsed(vector, exclude));
            filteredVectors.Add(filtered.ToList());
            size += filtered.ToList().Count;
        }
        int randomIndex = UnityEngine.Random.Range(0, filteredVectors.Count);
        item.transform.position = filteredVectors[randomIndex][UnityEngine.Random.Range(0, filteredVectors[randomIndex].Count)];
    }

    bool IsVectorUsed(Vector3 vec,List<Vector3> path)
    {
        foreach(Vector3 vectorX in path)
        {
            if(Vector3.Distance(vectorX,vec) < 0.01f)
            {
                return true;
            }
        }
        return false;
    }
}
