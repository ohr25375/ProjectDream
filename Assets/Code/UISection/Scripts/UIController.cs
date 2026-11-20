using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private GameObject _sprite;
    [SerializeField] private Tileset _tileset;
    private List<SpriteController> _spriteControllers;
    public List<SpriteController> SpriteControllers => _spriteControllers;

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
                var spriteController = new SpriteController();
                if (obj.GetComponent<SpriteRenderer>() == null)
                {
                    Debug.LogError("Sprite Renderer not found. Check prefab", this);
                }
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                spriteController.Initialize(spriteRenderer);
                spriteController.SetUI(true);
                spriteController.SetSprite(_tileset.GetSprite(1));
                spriteController.SetPalette(Palette.B);
                spriteController.SetIsBackground(true);
                spriteController.UpdateRenderer();
                _spriteControllers[index] = spriteController;
            }
        }
    }
    public SpriteController GetSprite(int x, int y)
    {
        if (x < 0 || x >= _size.x || y < 0 || y >= _size.y) return null;
        
        return _spriteControllers[x+y*_size.x];
    }
}
