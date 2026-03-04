using UnityEngine;

public class HallwayPaintingsController : MonoBehaviour
{
    public GameObject paintings;

    void Start()
    {
        paintings.SetActive(false);
    }

    public void ShowPaintings()
    {
        paintings.SetActive(true);
    }
}