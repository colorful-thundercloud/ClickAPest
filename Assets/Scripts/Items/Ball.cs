using UnityEngine;

public class Ball : InGameItem
{
    public AudioSource source;
    Rigidbody2D rb;

    void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        source.PlayOneShot(itemData.sound);
    }

    public void TakeHit()
    {
        rb.AddForce(new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)));
        rb.AddTorque(Random.Range(-100, 100));
        source.PlayOneShot(itemData.sound);
    }

}
