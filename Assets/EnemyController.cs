using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.5f;
    public bool vertical;
    Rigidbody2D rigidbody2D_;
    public float changeTime = 3.0f;
    float timer;
    float direction = 0.5f;
    Animator animator;
    bool broken;
    AudioSource audioSource;
    public AudioClip questCompleteClip;

    public ParticleSystem smokeEffect;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D_ = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        broken = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        if (vertical)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            animator.SetFloat("Move Y", 0);
            animator.SetFloat("Move X", direction);
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 postition = rigidbody2D_.position;
        if (vertical)
        {
            postition.y = postition.y + Time.deltaTime * speed * direction;
        }
        else
        {
            postition.x = postition.x + Time.deltaTime * speed * direction;
        }

        rigidbody2D_.MovePosition(postition);
    }

    public void Fix()
    {
        broken = false;
        rigidbody2D_.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        audioSource.Stop();
        if (NonPlayerCharacter.mission)
        {
            audioSource.PlayOneShot(questCompleteClip);
            NonPlayerCharacter.mission = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}
