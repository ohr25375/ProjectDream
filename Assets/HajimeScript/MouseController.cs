using UnityEngine.InputSystem;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    Vector3 mousePos, pos;

    private void Update()
    {
        // マウスが存在することを確認してから位置を取得
        if (Mouse.current != null)
        {
            // InputSystemでのマウス位置の取得方法（Vector2で返ってきます）
            Vector2 currentMousePos = Mouse.current.position.ReadValue();

            mousePos = new Vector3(currentMousePos.x, currentMousePos.y, 0f);

            Debug.Log(mousePos);

            // Z座標に10（カメラからの距離）を指定してワールド座標に変換
            pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

            transform.position = pos;

        }
    }
}
