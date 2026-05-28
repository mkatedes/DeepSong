using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMotion : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float noiseScale = 0.5f;
    [SerializeField] private float movementRange = 5f;

    private Vector3 initialPosition;
    private float timeOffset;

    void Start()
    {
        initialPosition = transform.position;
        timeOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        // Utiliser Perlin Noise pour un mouvement fluide type vent
        float noiseX = Mathf.PerlinNoise(Time.time * speed + timeOffset, 0f) * 2f - 1f;
        float noiseZ = Mathf.PerlinNoise(0f, Time.time * speed + timeOffset) * 2f - 1f;

        // Appliquer le mouvement
        Vector3 offset = new Vector3(
            noiseX * movementRange,
            0f,
            noiseZ * movementRange
        );

        transform.position = Vector3.Lerp(
            transform.position,
            initialPosition + offset,
            noiseScale * Time.deltaTime
        );
    }
}