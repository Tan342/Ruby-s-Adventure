using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class RubyController : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public int health { get { return currentHealth; } }
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public float speed = 2.5f;
    bool isInvincible;
    float invinciableTimer;
    public float timeInvincible = 2.0f;
    Animator animator;
    Vector2 lookDirection;
    public GameObject projectilePrefab;
    AudioSource audioSource;
    public AudioClip launchClip;
    public AudioClip playerHit;
    RubyInputAction inputActions;
    Vector2 currentInput;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentInput = new Vector2();
        inputActions = new RubyInputAction();
        inputActions.Ruby.Enable();
        inputActions.Ruby.Launch.performed += Launch;
        inputActions.Ruby.Movement.performed += onMovement;
        inputActions.Ruby.Movement.canceled += onMovement;
        inputActions.Ruby.Talk.performed += Talking;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");

            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            PlaySound(playerHit);
            invinciableTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");
            PlaySound(launchClip);
        }

    }

    void onMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentInput = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            currentInput = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isInvincible)
        {
            invinciableTimer -= Time.deltaTime;
            if (invinciableTimer < 0)
                isInvincible = false;
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentInput.magnitude > 0.01)
        {
            Vector2 position = transform.position;
            position = position + currentInput * speed * Time.deltaTime;
            rigidbody2d.MovePosition(position);
        }
        // horizontal = Input.GetAxis("Horizontal");
        // vertical = Input.GetAxis("Vertical");


        Vector2 move = new Vector2(currentInput.x, currentInput.y);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }

    void Talking(InputAction.CallbackContext context)
    {
        if (context.performed)
        {


            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }

            }
        }
    }

}
