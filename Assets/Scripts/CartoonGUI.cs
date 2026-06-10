using UnityEngine;

public static class CartoonGUI
{
    private static GUIStyle boxStyle;
    private static GUIStyle smallBoxStyle;
    private static GUIStyle centerBoxStyle;
    private static GUIStyle titleStyle;
    private static Font cartoonFont;
    private static Texture2D boxTexture;
    private static Texture2D brightBoxTexture;

    public static GUIStyle BoxStyle
    {
        get
        {
            EnsureStyles();
            return boxStyle;
        }
    }

    public static GUIStyle SmallBoxStyle
    {
        get
        {
            EnsureStyles();
            return smallBoxStyle;
        }
    }

    public static GUIStyle CenterBoxStyle
    {
        get
        {
            EnsureStyles();
            return centerBoxStyle;
        }
    }

    public static GUIStyle TitleStyle
    {
        get
        {
            EnsureStyles();
            return titleStyle;
        }
    }

    public static void DrawBox(Rect rect, string text)
    {
        EnsureStyles();
        GUI.Box(rect, text, BoxStyle);
    }

    public static void DrawSmallBox(Rect rect, string text)
    {
        EnsureStyles();
        GUI.Box(rect, text, SmallBoxStyle);
    }

    public static void DrawCenterBox(Rect rect, string text)
    {
        EnsureStyles();
        GUI.Box(rect, text, CenterBoxStyle);
    }

    public static float GetWrappedBoxHeight(string text, float width, bool small = false)
    {
        EnsureStyles();
        GUIStyle style = small ? SmallBoxStyle : BoxStyle;
        float contentWidth = Mathf.Max(120f, width - style.padding.left - style.padding.right);
        return Mathf.Max(48f, style.CalcHeight(new GUIContent(text), contentWidth) + style.padding.top + style.padding.bottom);
    }

    public static Font GetCartoonFont()
    {
        EnsureStyles();
        return cartoonFont;
    }

    private static void EnsureStyles()
    {
        if (boxStyle != null)
        {
            return;
        }

        // Prefer cartoon-like system fonts. If the OS does not have them, Unity falls back safely.
        cartoonFont = Font.CreateDynamicFontFromOSFont(
            new string[] { "Comic Sans MS", "Comic Sans", "Marker Felt", "Chalkboard SE", "Arial Rounded MT Bold", "Arial" },
            18
        );

        boxTexture = MakeTexture(new Color(0.05f, 0.045f, 0.035f, 0.86f));
        brightBoxTexture = MakeTexture(new Color(0.10f, 0.075f, 0.035f, 0.90f));

        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.font = cartoonFont;
        boxStyle.fontSize = 17;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.wordWrap = true;
        boxStyle.richText = true;
        boxStyle.clipping = TextClipping.Clip;
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.normal.textColor = new Color(1.0f, 0.93f, 0.63f);
        boxStyle.hover.textColor = boxStyle.normal.textColor;
        boxStyle.active.textColor = boxStyle.normal.textColor;
        boxStyle.padding = new RectOffset(16, 16, 12, 12);
        boxStyle.margin = new RectOffset(4, 4, 4, 4);
        boxStyle.normal.background = boxTexture;
        boxStyle.border = new RectOffset(8, 8, 8, 8);

        smallBoxStyle = new GUIStyle(boxStyle);
        smallBoxStyle.font = cartoonFont;
        smallBoxStyle.fontSize = 15;
        smallBoxStyle.wordWrap = true;
        smallBoxStyle.alignment = TextAnchor.UpperLeft;
        smallBoxStyle.padding = new RectOffset(12, 12, 9, 9);
        smallBoxStyle.normal.background = brightBoxTexture;

        centerBoxStyle = new GUIStyle(boxStyle);
        centerBoxStyle.font = cartoonFont;
        centerBoxStyle.alignment = TextAnchor.MiddleCenter;
        centerBoxStyle.fontSize = 18;
        centerBoxStyle.wordWrap = true;

        titleStyle = new GUIStyle(boxStyle);
        titleStyle.font = cartoonFont;
        titleStyle.fontSize = 20;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.wordWrap = true;
        titleStyle.normal.textColor = new Color(1.0f, 0.74f, 0.20f);
    }

    private static Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
