using UnityEngine;

public class BeginPlayArena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlaySFX("Cheering");
        AudioManager.Instance.PlayMusic("Drumbs");
    }

}
