using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * reference: https://learn.unity.com/tutorial/survival-shooter-training-day-phases?projectId=5c514921edbc2a002069465e#5c7f8528edbc2a002053b71f
 */
public class EnemyHealth : MonoBehaviour
{

    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue = 10;
    public AudioClip deathClip;
    AudioSource enemyAudio;
    bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        enemyAudio = GetComponent<AudioSource>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead)
        {
            return;
        }


        enemyAudio.Play(); //hurt sound effect
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;

        //ScoreManager.score += scoreValue;
        //destroy
        enemyAudio.clip = deathClip;
        enemyAudio.Play();
        Destroy(gameObject, 0f);
    }
}
