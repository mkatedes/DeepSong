using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DeathCanvaCall : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField]private GameObject firstButton;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void Retry()
    {
        SceneManager.LoadScene("Map");
    }

    public void ShowDeathCanva()
    {
        canvas.enabled = true;
        FindAnyObjectByType<EventSystem>().SetSelectedGameObject(firstButton);
    }
}
