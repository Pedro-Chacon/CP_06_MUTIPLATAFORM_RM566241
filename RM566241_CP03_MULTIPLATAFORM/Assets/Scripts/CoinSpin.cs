using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 120f;

    private void Update()
    {
        transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
    }
}