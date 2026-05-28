using Unity.Cinemachine;
using UnityEngine;

public class CinematiqueBoss : MonoBehaviour
{
    
    [SerializeField] private CinemachineCamera cam;

    public void ChangeCam()
    {
        cam.enabled = false;

    }

}
