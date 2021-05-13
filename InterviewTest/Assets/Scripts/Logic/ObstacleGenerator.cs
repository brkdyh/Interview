using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject ObstacleTemplate;

    void Start()
    {
        ObstacleTemplate = Resources.Load<GameObject>("Prefabs/Obstacle");
        StartCoroutine(GenerateObstacle());
    }

    IEnumerator GenerateObstacle()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            var obstacle = Instantiate(ObstacleTemplate, transform).GetComponent<Obstacle>();
            obstacle.FallDown(new Vector3(Random.Range(-3.5f, 3.5f), 5, -3));
        }
    }
}
