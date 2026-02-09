using UnityEngine;

public class SphereSmokeProjectileImpacts : MonoBehaviour
{
    public float lifetime = 5f;
    private float timer;
    
    void Start()
    {
        timer = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
        
    }
}
