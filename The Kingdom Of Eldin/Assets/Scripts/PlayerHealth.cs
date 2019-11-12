using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public int minHealth = 0;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    public Vector3 startPosition;

    Animator anim;
    AudioSource playerAudio;
    PlayerController2D playerControl;
    bool isDead;
    bool damaged;

    private void Awake()
    {
        startPosition = transform.position;
        print(startPosition);
    }

    void Death()
    {
        isDead = true;
        transform.position = startPosition;
    }
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
        healthSlider.value = currentHealth;
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
}
