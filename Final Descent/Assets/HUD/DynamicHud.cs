using UnityEngine;

public class DynamicHud : MonoBehaviour
{
    public Transform player;
    private Camera cam;
    public float speed;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) >= 1)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(player.position);

            float step = speed * Time.deltaTime;
            Vector3 l = Vector3.Lerp(transform.position, screenPos, step);
            transform.position = l;
        }
    }
}
