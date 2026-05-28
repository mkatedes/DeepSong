using UnityEngine;

public class BeginPlayMap : MonoBehaviour
{
    
    void Start()
    {
        AudioManager.Instance.PlayMusic("MainMusic");
        AudioManager.Instance.PlayMusic("ForestAmbient", true);
    }

}
