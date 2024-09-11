using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject water;
    [SerializeField] float waterForce= 42;
    [Space]    
    [SerializeField] GameObject spawn;
    [SerializeField] float spawnRate = 4;
    [SerializeField] GameObject[] obstaclePrefab;

    private int obstaclesCount;


    void Start()
    {
        if (!water) { water = GameObject.Find("Water"); }
        if (!spawn) { spawn = GameObject.Find("Spawn"); }

        //InvokeRepeating("SpawnObstacle", startDelay, spawnRate);
        //StartCoroutine(SpawnObstacle());

    }
    
    void FixedUpdate()
    {
        /*Moves Obstacles Parent to simulate water flow event if the tube is not spinning*/
        water.transform.Rotate(Vector3.forward, Time.deltaTime * waterForce);

    }

    int index;

    IEnumerator SpawnObstacle()
    {
        yield return new WaitForSeconds(spawnRate);

        //if no more obstacle present and the game is active and player is not dead
        if (!FindAnyObjectByType<Obstacle>() && GameManager.Instance.isGameActive && !GameManager.Instance.playerController.isDead)
        {
            //wave1: easy
            if (obstaclesCount < 1)
            {
                index = Random.Range(0, 1); ;
            }
            //wave2: 50-50
            else if (obstaclesCount < 4)
            {
                index = Random.Range(0, (obstaclePrefab.Length-1));
                spawnRate = 2;
            }
            //wave3+: 2x harder
            else
            {
                index = Random.Range(0, (obstaclePrefab.Length * 2));
                if (index >= obstaclePrefab.Length)
                {
                    index = Random.Range(2, 3); ;
                }

                spawnRate = 1;
            }

            var instance = Instantiate(obstaclePrefab[index], spawn.transform.position, spawn.transform.rotation, water.transform);
            instance.transform.localScale = spawn.transform.localScale;
            obstaclesCount++;
        }

        StartCoroutine(SpawnObstacle());

    }       


}
