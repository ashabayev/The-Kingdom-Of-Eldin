using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay;
    public int max;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTimeDelay());
    }

    IEnumerator SpawnTimeDelay()
    {
        while (true)
        {
            Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
            count++;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

}
