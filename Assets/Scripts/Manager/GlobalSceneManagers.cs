using UnityEngine;

public class GlobalSceneManagers : MonoBehaviour
{
    public static GlobalSceneManagers Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Initialization code can go here if needed
    }

    // Update is called once per frame
    private void Update()
    {
        // Update logic can go here if needed
    }
}