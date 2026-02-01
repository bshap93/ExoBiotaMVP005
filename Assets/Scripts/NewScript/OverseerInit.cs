using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverseerInit : MonoBehaviour
{
    async void Awake()
    {
        await ConductOverseerInit();
    }

    async Task ConductOverseerInit()
    {
        await SceneManager.LoadSceneAsync("Overseer", LoadSceneMode.Additive);
    }
}
