using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private GameObject _sprite;
    [SerializeField] private Tileset _tileset;
    private List<SpriteController> _spriteControllers = new();

    private void Awake()
    {
        _spriteControllers = new(new SpriteController[_size.x * _size.y]);
        for (var y = 0; y < _size.y; y++)
        {
            for (var x = 0; x < _size.x; x++)
            {
                var obj = Instantiate(_sprite, transform);
                obj.transform.localPosition = new Vector2(x, y) - (_size - Vector2.one) / 2;
                var index = x + y * _size.x;
                var spriteController = obj.GetComponent<SpriteController>();
                spriteController.SetSprite(_tileset.GetSprite(1));
                MaterialPropertyBlock mpb = new();
                mpb.SetFloat("_Palette", 1);
                mpb.SetFloat("_IsBackground", 1);
                spriteController.SetPropertyBlock(mpb);
                _spriteControllers[index] = spriteController;
            }
        }
    }
}
