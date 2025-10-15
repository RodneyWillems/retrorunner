using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WooooThatsMe : MonoBehaviour
{
    #region Variables
    [Header("Sprites")]
    [SerializeField] private Sprite m_jumpSprite;
    [SerializeField] private Sprite m_fallSprite;
    [SerializeField] private Sprite[] m_runningSprites;

    [Header("Values")]
    [SerializeField] private int m_movementSpeed;
    [SerializeField] private float m_animationSpeed;
    [SerializeField] private int m_jumpingForce;
    [SerializeField] private Transform m_feet;

    private bool m_shouldBeRunning;
    private bool m_falling;
    private bool m_jumping;
    private bool m_runningRight;
    private int m_spriteCycle;

    [Header("FILL IN")]
    [SerializeField] private GameObject m_camera;
    [SerializeField] private GameObject m_deathScreen;
    [SerializeField] private GameObject m_winScreen;
    [SerializeField] private LayerMask m_floorLayer;

    // Miscellaneous
    private Rigidbody2D m_rb;
    private SpriteRenderer m_sr;
    private Suffering m_controls;
    private Coroutine m_runningRoutine;
    private Coroutine m_cameraRoutine;
    private float m_stuckTime;
    #endregion

    #region Setup

    private void Awake()
    {
        Application.targetFrameRate = 60;
        m_rb = GetComponent<Rigidbody2D>();
        m_sr = GetComponent<SpriteRenderer>();

        m_runningRight = true;
        GetSprites();

        m_cameraRoutine = StartCoroutine(CameraFollow());

        Time.timeScale = 1f;
    }

    private void GetSprites()
    {
        m_runningSprites = SkinsMagnetron.Instance.ReturnSprite();
        m_fallSprite = SkinsMagnetron.Instance.ReturnFallSprite();
        m_jumpSprite = SkinsMagnetron.Instance.ReturnJumpSprite();
        m_sr.sprite = m_fallSprite;
    }

    private void OnEnable()
    {
        m_controls = new();
        m_controls.Reason7.Jump.performed += Jump;
        m_controls.Enable();
    }

    private void OnDisable()
    {
        m_controls.Disable();
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This is simply to start the game in a silly way
         if (collision.transform.CompareTag("Floor") && !m_shouldBeRunning)
        {
            m_shouldBeRunning = true;
            m_runningRoutine = StartCoroutine(StartMoving());
        }
        if (collision.transform.CompareTag("Finish"))
        {
            m_winScreen.SetActive(true);
            StopAllCoroutines();
            Time.timeScale = 0f;
            m_controls.Disable();
        }
        // If you're falling against a wall just stop moving to drop faster
        RaycastHit2D cast = Physics2D.Raycast(m_feet.position, Vector2.down);
        if (collision.transform.CompareTag("Floor") && cast && cast.transform.CompareTag("Death"))
        {
            m_shouldBeRunning = false;
            StopCoroutine(m_runningRoutine);
        }
        // Stop falling when you hit the floor duhhh???
        else if (collision.transform.CompareTag("Floor") && m_falling && !m_jumping || collision.transform.CompareTag("Object") && m_falling && !m_jumping && Physics2D.BoxCast(m_feet.position, new Vector2(0.5f, 0.1f), 0, Vector2.down, 0.2f, m_floorLayer))
        {
            m_falling = false;
            m_spriteCycle = 0;
            m_sr.sprite = m_runningSprites[m_spriteCycle];
        }
        // When you hit an object you flip around
        else if (collision.transform.CompareTag("Object"))
        {
            m_sr.flipX = !m_sr.flipX;
            m_runningRight = !m_runningRight;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!Physics2D.Raycast(m_feet.position, Vector2.down, 0.2f) && !m_jumping)
        {
            m_falling = true;
            m_sr.sprite = m_fallSprite;
        }
        m_stuckTime = 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Object"))
            m_stuckTime += Time.deltaTime;
        if (m_stuckTime > 0.5f)
        {
            if (m_runningRight)
                transform.position += Vector3.left * 0.2f;
            else
                transform.position += Vector3.right * 0.2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            StopCoroutine(m_cameraRoutine);
            StartCoroutine(DeathFall());
        }
    }

    private IEnumerator StartMoving()
    {
        while (m_shouldBeRunning)
        {
            if (m_runningRight)
                m_rb.linearVelocityX = m_movementSpeed;
            else
                m_rb.linearVelocityX = -m_movementSpeed;
            if (!m_falling && !m_jumping)
            {
                m_spriteCycle++;
                if (m_spriteCycle == m_runningSprites.Length)
                {
                    m_spriteCycle = 0;
                }
                m_sr.sprite = m_runningSprites[m_spriteCycle];
                yield return new WaitForSeconds(m_animationSpeed);
            }
            yield return null;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        try
        {
            RaycastHit2D cast = Physics2D.Raycast(transform.position, Vector2.down, 1f, m_floorLayer);
            if (cast && !m_jumping && !m_falling)
            {
                m_rb.AddForce(Vector2.up * m_jumpingForce, ForceMode2D.Impulse);
                StartCoroutine(CheckFalling());
                m_sr.sprite = m_jumpSprite;
                m_falling = false;
                m_jumping = true;
            }
        }
        catch 
        {
            Debug.LogError("You're not standing on anything lololol");
        }
    }

    private IEnumerator CheckFalling()
    {
        while (m_rb.linearVelocityY > 0.1f)
        {
            yield return null;
        }
        m_falling = true;
        m_jumping = false;
        m_sr.sprite = m_fallSprite;
    }

    private IEnumerator CameraFollow()
    {
        while (true)
        {
            m_camera.transform.position = transform.position + new Vector3(6, 0, -10);
            yield return null;
        }
    }

    private IEnumerator DeathFall()
    {
        m_shouldBeRunning = false;
        StopCoroutine(m_runningRoutine);
        yield return new WaitForSeconds(1.5f);
        m_deathScreen.SetActive(true);
        m_controls.Disable();
    }
}
