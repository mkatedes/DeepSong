using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ShowBars : MonoBehaviour
{

    [SerializeField] GameObject CanvaBar;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] GameObject Dust;
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject collider;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            director.Play();
        }
    }


    public void HideBar()
    {
        CanvaBar.SetActive(false);
        cam.Priority = 0;
        Dust.SetActive(false);
        collider.SetActive(false);
    }
}
