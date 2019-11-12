using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * reference: https://learn.unity.com/tutorial/survival-shooter-training-day-phases?projectId=5c514921edbc2a002069465e#5c7f8528edbc2a002053b71e
 */
public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);

    Animator anim;
    AudioSource playerAudio;
    PlayerController2D playerControl;
    bool isDead;
    bool damaged;

   /**  private void Awake()
    {
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerControl = GetComponent<PlayerController2D>();
        currentHealth = startingHealth;
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerControl = GetComponent<PlayerController2D>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (damaged)
        {
            damageImage.color = flashColor;
        } else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

    public void TakeDamage (int amount)
    {
        damaged = true;
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        playerAudio.Play();
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        anim.SetTrigger("Die");
        playerControl.enabled = false;
    }
}
