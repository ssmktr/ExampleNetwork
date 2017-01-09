using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {

    GUIStyle HpStyle;
    GUIStyle BackStyle;
    Combat _ComBat;

	void Start () {
        _ComBat = GetComponent<Combat>();
	}

    private void OnGUI()
    {
        InitStyle();

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.color = Color.gray;
        GUI.backgroundColor = Color.grey;
        GUI.Box(new Rect(pos.x - 26, Screen.height - pos.y + 20, Combat.MaxHp / 2, 7), ".", BackStyle);

        GUI.color = Color.green;
        GUI.backgroundColor = Color.green;
        GUI.Box(new Rect(pos.x - 25, Screen.height - pos.y + 21, _ComBat.Hp / 2, 5), ".", HpStyle);
    }

    void InitStyle()
    {
        if (HpStyle == null)
        {
            HpStyle = new GUIStyle(GUI.skin.box);
            HpStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 1f));
        }

        if (BackStyle == null)
        {
            BackStyle = new GUIStyle(GUI.skin.box);
            BackStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1f));
        }
    }

    Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
