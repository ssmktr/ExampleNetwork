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

    }

    void InitStyle()
    {
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
