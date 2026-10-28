using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoublePlayer : NetworkBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private CharacterController controller;

    [Header("Aparência")]
    [SerializeField] private MeshRenderer bodyRenderer;

    [Networked, OnChangedRender(nameof(ApplyColor))]
    public byte TeamColor { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            TeamColor = (byte)(Runner.IsSharedModeMasterClient ? 0 : 1);
        }
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (bodyRenderer == null)
            return;

        bodyRenderer.material.color = TeamColor == 0 ? Color.cyan : Color.red;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        RoundManager round = RoundManager.Instance;
        if (round == null || !round.CanPlay)
            return;

        Move();
    }

    private void Move()
    {
        if (Keyboard.current == null)
            return;

        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) -
                  (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) -
                  (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 direction = new Vector3(x, 0, z).normalized;
        Vector3 velocity = direction * speed + Vector3.down * 2f;

        controller.Move(velocity * Runner.DeltaTime);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority)
            return;

        if (!other.CompareTag("Coin"))
            return;

        NetworkObject coin = other.GetComponentInParent<NetworkObject>();
        RoundManager round = RoundManager.Instance;

        if (coin == null || round == null || !round.CanPlay)
            return;

        round.RPC_CollectCoin(coin.Id);
    }
}