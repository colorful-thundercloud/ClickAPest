using System.Collections;
using UnityEngine;

public class TutPest : Character
{
    [SerializeField] SpriteRenderer healthSlider, healthValue;
    [SerializeField] private float moveSpeed;
    public GameObject hitRadiusImage;
    public bool isEnemy;
    Rigidbody2D rb;
    Vector2 velocity, initialHealthSize;
    bool isInvincible;
    public bool IsInvincible
    {
        get { return isInvincible; }
        private set { isInvincible = value; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        initialHealthSize = healthValue.size;
        textNickname.gameObject.SetActive(false);
        textYou.gameObject.SetActive(!isEnemy);
        if (hitRadiusImage != null) hitRadiusImage.SetActive(false);

        SetInitialColor(Color.white);

        StartCoroutine(SetInvincibility());
        if (isEnemy) StartCoroutine(RandomMovement());
    }

    private void FixedUpdate()
    { //move
        if (!IsEliminated && !isEnemy)
        {
            float moveH = Input.GetAxisRaw("Horizontal");
            velocity.x = (moveH != 0) ? moveSpeed * moveH : 0;

            float moveV = Input.GetAxisRaw("Vertical");
            velocity.y = (moveV != 0) ? moveSpeed * moveV : 0;

            rb.linearVelocity = velocity;
        }
    }

    void LateUpdate()
    { // rotate
        if (velocity != Vector2.zero)
        {
            float rotationAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 90));
            rb.rotation = 0;
        }
        textNickname.transform.parent.gameObject.transform.rotation = Quaternion.identity;
        textYou.gameObject.transform.rotation = Quaternion.identity;
    }

    public override void TakeDamage()
    {
        if (IsInvincible || IsEliminated) return;

        if (Health > 0)
        {
            Health--;
            healthValue.size = new Vector2(Health * initialHealthSize.x / MaxHealth, initialHealthSize.y);
            if (Health != 0) StartCoroutine(SetInvincibility());
            else Eliminate();

        }
        else Eliminate();
    }

    private IEnumerator SetInvincibility()
    {
        IsInvincible = true;
        SetColor(colorDamaged);

        yield return new WaitForSeconds(3f);

        IsInvincible = false;
        SetColor(colorNormal);
    }


    public override void Respawn()
    {
        base.Respawn();
        healthValue.size = initialHealthSize;
        StartCoroutine(SetInvincibility());
    }

    IEnumerator RandomMovement()
    {
        if (!IsEliminated)
        {
            velocity.x = moveSpeed * Random.Range(-0.5f, 0.5f);
            velocity.y = moveSpeed * Random.Range(-0.5f, 0.5f);
            rb.linearVelocity = velocity;
        }

        yield return new WaitForSeconds(Random.Range(1, 2.5f));

        StartCoroutine(RandomMovement());
    }

}
