using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Pest : Character
{
    [SerializeField] SpriteRenderer healthSlider, healthValue;
    [SerializeField] private float moveSpeed;
    float curMoveSpeed;
    Rigidbody2D rb;
    CapsuleCollider2D col;
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
        col = GetComponent<CapsuleCollider2D>();
        curMoveSpeed = moveSpeed;

        initialHealthSize = healthValue.size;

        SetInitialColor(Color.white);

        textYou.gameObject.SetActive(view.IsMine);
        textNickname.text = view.Controller.NickName;

        StartCoroutine(SetInvincibility());
    }

    private void FixedUpdate()
    { //move
        if (view.IsMine && !IsEliminated)
        {
            float moveH = Input.GetAxisRaw("Horizontal");
            velocity.x = (moveH != 0) ? curMoveSpeed * moveH : 0;

            float moveV = Input.GetAxisRaw("Vertical");
            velocity.y = (moveV != 0) ? curMoveSpeed * moveV : 0;

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
        textYou.gameObject.transform.rotation = Quaternion.identity;
        textNickname.transform.parent.gameObject.transform.rotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        curMoveSpeed /= collision.rigidbody.mass;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        curMoveSpeed = moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Physics2D.IsTouching(col, collision))
            curMoveSpeed = moveSpeed;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!Physics2D.IsTouching(col, collision))
            curMoveSpeed = moveSpeed;
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

    public void RPC_Send_Damaged()
    {
        view.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, view.ViewID);
    }

    [PunRPC]
    private void RPC_UpdateHealth(int playerId)
    {
        PhotonView targetView = PhotonView.Find(playerId);
        if (targetView != null) targetView.GetComponent<Pest>().TakeDamage();
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

}
