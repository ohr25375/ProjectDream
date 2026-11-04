using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTest : MonoBehaviour
{
    [SerializeField,Header("Player移動速度")] private float speed = 1.0f;

    private Transform _transform;
    private CharacterController _characterController;

    private Vector2 inputMove;

    // 移動Action(PlayerInput側から呼ばれる)
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力値を保持しておく
        inputMove = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 操作入力
        var moveVelocity = new Vector3(inputMove.x * speed, inputMove.y * speed, 0f);
    }
}

// ===== 使い方(Key用) =====
//// 現在のキーボード情報
//var current = Keyboard.current;
//
//// キーボード接続チェック
//if (current == null)
//{
//    // キーボードが接続されていないとKeyboard.currentがnullになる
//    return;
//}
//// Aキーの入力状態取得
////var aKey = current.aKey;でもいい
//var aKey = current[Key.A];
//
//// Aキーが押された瞬間かどうか
//if (aKey.wasPressedThisFrame)
//{
//    Debug.Log("押したよ");
//}
//
//// Aキーが離された瞬間かどうか
//if (aKey.wasReleasedThisFrame)
//{
//    Debug.Log("離したよ");
//}
//
//// Aキーが押されているかどうか
//if (aKey.isPressed)
//{
//    Debug.Log("現在押されているよ");
//}