using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthHeartBar : MonoBehaviour
{
    [SerializeField] private GameObject healthHeart;
    [SerializeField] private EntityAttributes entityAttributes;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        if (entityAttributes != null)
        {
            entityAttributes.OnPlayerDamage += DrawHeart;
        }
    }

    private void OnDisable()
    {
        if (entityAttributes != null)
        {
            entityAttributes.OnPlayerDamage -=DrawHeart;
        }
    }

    private void Start()
    {
        DrawHeart();
    }

    public void DrawHeart()
    {
        ClearHeart();
        float maxHealthReminder = entityAttributes.MaxHealth % 2;
        int heartToMake = (int)((entityAttributes.MaxHealth / 2) + maxHealthReminder);
        for (int i = 0; i < heartToMake; i++)
        {
            CreateEmptyHeart();
        }
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainders = (int)Mathf.Clamp(entityAttributes.CurrentHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainders);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(healthHeart);
        newHeart.transform.SetParent(transform);
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHeart()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts.Clear();
    }
}