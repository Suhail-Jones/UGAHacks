using UnityEngine;

/// <summary>
/// Generates simple white sprites at runtime.
/// Usage: SpriteHelper.Square  or  SpriteHelper.Circle
/// </summary>
public static class SpriteHelper
{
    private static Sprite _square;
    private static Sprite _circle;

    public static Sprite Square
    {
        get
        {
            if (_square == null)
            {
                Texture2D tex = new Texture2D(4, 4);
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 4; x++)
                        tex.SetPixel(x, y, Color.white);
                tex.Apply();
                tex.filterMode = FilterMode.Point;
                _square = Sprite.Create(tex, new Rect(0, 0, 4, 4),
                                        new Vector2(0.5f, 0.5f), 4);
            }
            return _square;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (_circle == null)
            {
                int s = 64;
                Texture2D tex = new Texture2D(s, s);
                float c = s / 2f, r = s / 2f - 1f;
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x + .5f, y + .5f),
                                                   new Vector2(c, c));
                        tex.SetPixel(x, y, d <= r ? Color.white : Color.clear);
                    }
                tex.Apply();
                tex.filterMode = FilterMode.Bilinear;
                _circle = Sprite.Create(tex, new Rect(0, 0, s, s),
                                        new Vector2(0.5f, 0.5f), s);
            }
            return _circle;
        }
    }
}