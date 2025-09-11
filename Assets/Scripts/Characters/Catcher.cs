using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Catcher : Character
{
    [SerializeField] private TrailRenderer trail;
    private Vector3 mousePos, tempPos;
    private bool isTouchingPest, almostTouchedPest, canAttack = true;
    private Pest prevTouchedPest, nowTouchedPest;
    private AudioSource audioSource;
    private GameOnlineManager gameManager;
    private Ball ball;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameOnlineManager>();
        audioSource = GetComponent<AudioSource>();

        SetInitialColor(sprite.color);

        textYou.gameObject.SetActive(view.IsMine);
        textNickname.text = view.Controller.NickName;

        StartCoroutine(SetCantAttack());
    }

    private void Update()
    {
        if (!view.IsMine) return;
        mousePos = Input.mousePosition;
        tempPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(tempPos.x, tempPos.y, -5);

        if (gameManager.roundEnd || gameManager.roundShop || IsEliminated || !canAttack) return;
        if (Input.GetMouseButtonDown(0))
        {
            view.RPC(nameof(RPC_AnimateAttack), RpcTarget.AllBuffered, view.ViewID);

            if (isTouchingPest && !nowTouchedPest.IsEliminated && !nowTouchedPest.IsInvincible)
            {
                nowTouchedPest.RPC_Send_Damaged();
                Score += ScoreAddValue;
            }
            else if (almostTouchedPest)
            {
                view.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, view.ViewID);
                nowTouchedPest.Score += nowTouchedPest.ScoreAddValue;
            }
            else if (ball is not null) ball.TakeHit();
            else view.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, view.ViewID);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            switch (collision)
            {
                case CapsuleCollider2D:
                    isTouchingPest = true;
                    almostTouchedPest = false;
                    break;
                case CircleCollider2D:
                    isTouchingPest = false;
                    almostTouchedPest = true;
                    break;
            }

            prevTouchedPest = nowTouchedPest;
        }
        collision.TryGetComponent(out ball);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            switch (collision)
            {
                case CapsuleCollider2D:
                    isTouchingPest = true;
                    almostTouchedPest = false;
                    break;
                case CircleCollider2D:
                    isTouchingPest = false;
                    almostTouchedPest = true;
                    break;
            }

            prevTouchedPest = nowTouchedPest;
        }
        collision.TryGetComponent(out ball);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out nowTouchedPest))
        {
            if (nowTouchedPest == prevTouchedPest)
            {
                isTouchingPest = false;
                almostTouchedPest = false;
                prevTouchedPest = null;
            }
        }
        if (collision.TryGetComponent<Ball>(out _)) ball = null;
    }

    public override void SetInfo(IngameObjectData data)
    {
        base.SetInfo(data);
        if (data is CatcherData cd)
            trail.colorGradient = cd.trailColor;
    }

    [PunRPC]
    public void RPC_AnimateAttack(int playerId)
    {
        PhotonView targetView = PhotonView.Find(playerId);
        if (targetView != null) targetView.GetComponent<Catcher>().StartCoroutine(AnimatedAttack());
    }

    private IEnumerator AnimatedAttack()
    {
        sprite.transform.Rotate(new Vector3(0, 0, 23));
        audioSource.PlayOneShot(sound);

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

    [PunRPC]
    private void RPC_UpdateHealth(int playerId)
    {
        PhotonView targetView = PhotonView.Find(playerId);
        if (targetView != null) targetView.GetComponent<Catcher>().TakeDamage();
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
}
