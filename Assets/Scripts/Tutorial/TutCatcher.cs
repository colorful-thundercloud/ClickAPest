using System.Collections;
using UnityEngine;

public class TutCatcher : Character
{
    [SerializeField] TrailRenderer trail;
    [HideInInspector]
    public Transform goal;
    Vector3 mousePos, tempPos;
    bool isTouchingPest, almostTouchedPest, canAttack = true;
    TutPest prevTouchedPest, nowTouchedPest;
    Ball ball;
    public bool isEnemy;

    void Start()
    {
        SetInitialColor(sprite.color);

        textNickname.gameObject.SetActive(false);
        textYou.gameObject.SetActive(!isEnemy);

        StartCoroutine(SetCantAttack());
        if (isEnemy) InvokeRepeating(nameof(MoveTowards), 1f, 0.025f);
    }

    void Update()
    {
        if (!isEnemy)
        {
            mousePos = Input.mousePosition;
            tempPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(tempPos.x, tempPos.y, -5);


            if (!IsEliminated && canAttack && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(AnimatedAttack());

                if (isTouchingPest && !nowTouchedPest.IsEliminated && !nowTouchedPest.IsInvincible)
                {
                    nowTouchedPest.TakeDamage();
                    Score += ScoreAddValue;
                }
                else if (almostTouchedPest)
                {
                    nowTouchedPest.Score += nowTouchedPest.ScoreAddValue;
                    TakeDamage();
                }
                else if (ball != null) ball.TakeHit();
                else TakeDamage();
            }
        }
        else
        {
            if (!IsEliminated && canAttack)
            {
                if (isTouchingPest && !nowTouchedPest.IsEliminated && !nowTouchedPest.IsInvincible)
                {
                    StartCoroutine(AnimatedAttack());
                    nowTouchedPest.TakeDamage();
                    Score += ScoreAddValue;
                }
                else if (almostTouchedPest)
                {
                    StartCoroutine(AnimatedAttack());
                    nowTouchedPest.Score += nowTouchedPest.ScoreAddValue;
                    TakeDamage();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            if (collision.GetType() == typeof(CapsuleCollider2D))
            {
                isTouchingPest = true;
                almostTouchedPest = false;
            }
            else if (collision.GetType() == typeof(CircleCollider2D))
            {
                isTouchingPest = false;
                almostTouchedPest = true;
            }
            prevTouchedPest = nowTouchedPest;
        }
        collision.gameObject.TryGetComponent(out ball);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            if (collision.GetType() == typeof(CapsuleCollider2D))
            {
                isTouchingPest = true;
                almostTouchedPest = false;
            }
            else if (collision.GetType() == typeof(CircleCollider2D))
            {
                isTouchingPest = false;
                almostTouchedPest = true;
            }
            prevTouchedPest = nowTouchedPest;
        }
        collision.gameObject.TryGetComponent(out ball);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            if (nowTouchedPest == prevTouchedPest)
            {
                isTouchingPest = false;
                almostTouchedPest = false;
                nowTouchedPest = null;
            }
        }
        if (ball == collision.gameObject.TryGetComponent(out Ball ball1)) ball = null;
    }

    public override void SetInfo(IngameObjectData data)
    {
        base.SetInfo(data);
        if (data is CatcherData cd)
        {
            trail.colorGradient = cd.trailColor;
        }
    }

    IEnumerator AnimatedAttack()
    {
        sprite.transform.Rotate(new Vector3(0, 0, 23));
        yield return new WaitForSeconds(0.1f);
        sprite.transform.Rotate(new Vector3(0, 0, -23));
    }

    public override void TakeDamage()
    {
        if (IsEliminated || !canAttack) return;

        if (Health > 0)
        {
            Health--;
            if (Health != 0) StartCoroutine(SetCantAttack());
            else Eliminate();
        }

        else Eliminate();
    }

    private IEnumerator SetCantAttack()
    {
        SetColor(colorDamaged);
        canAttack = false;

        yield return new WaitForSeconds(3f);

        SetColor(colorNormal);
        canAttack = true;
    }

    public override void Respawn()
    {
        base.Respawn();
        StartCoroutine(SetCantAttack());
    }

    void MoveTowards()
    {
        if (!IsEliminated)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position - new Vector3(0, 1f, 0), 0.1f);
        }
    }
}
