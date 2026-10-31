using System.Collections;
using UnityEngine;

public class FireballInteraction : VirusInteraction
{
    [SerializeField] GameObject particle;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;
    private Coroutine deathCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        
        GameObject par = Instantiate(particle, transform.position, Quaternion.identity);
        par.transform.SetParent(null);
        par.GetComponent<ParticleSystem>().Play();
        
        Destroy(par, 2);
    }

    public void Initialize(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        if (deathCoroutine != null) StopCoroutine(deathCoroutine);
        deathCoroutine = StartCoroutine(DestroyCoroutine(lifetime));
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            Vector2 hitDirection = new Vector2(other.transform.position.x - transform.position.x, 1).normalized;
            
            if (player.isDead || !player.isActive) return;
            player.TakeDamage(hitDirection, damage, 200);
            
            if (deathCoroutine != null) StopCoroutine(deathCoroutine);
            deathCoroutine = StartCoroutine(DestroyCoroutine(0));
        }
    }

    private IEnumerator DestroyCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        GameObject par = Instantiate(particle, transform.position, Quaternion.identity);
        par.transform.SetParent(null);
        par.GetComponent<ParticleSystem>().Play();
        
        Destroy(par, 2);
        Destroy(gameObject);
    }
}
