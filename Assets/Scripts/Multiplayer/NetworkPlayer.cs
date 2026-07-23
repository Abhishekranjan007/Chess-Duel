using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    //public float moveSpeed = 5f;

    //public override void FixedUpdateNetwork()
    //{
    //    if (!Object.HasInputAuthority)
    //        return;

    //    Vector3 move = Vector3.zero;

    //    if (Keyboard.current.wKey.isPressed)
    //        move += Vector3.up;

    //    if (Keyboard.current.sKey.isPressed)
    //        move += Vector3.down;

    //    if (Keyboard.current.aKey.isPressed)
    //        move += Vector3.left;

    //    if (Keyboard.current.dKey.isPressed)
    //        move += Vector3.right;

    //    transform.position += move * moveSpeed * Runner.DeltaTime;
    //}

    public override void Spawned()
    {
        //Debug.Log(
        //   $"NetworkPlayer Spawned\n" +
        //   $"InputAuthority : {Object.InputAuthority}\n" +
        //   $"LocalPlayer    : {Runner.LocalPlayer}\n" +
        //   $"IsMine         : {Object.HasInputAuthority}");

    }
}
