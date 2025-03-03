using UnityEngine;

public class EntityPlacer : MonoBehaviour
{
    public GameObject[] entities;
    private int idx = 0;

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        UpdateIdx();
    }

    private void UpdateIdx()
    {
        if (Input.GetKeyDown("right"))
        {
            idx++;
            if (idx == entities.Length)
            {
                idx = 0;
            }
        }

        if (Input.GetKeyDown("left"))
        {
            idx--;
            if (idx < 0)
            {
                idx = entities.Length - 1;
            }
        }
    }
}
