using System.Collections.Generic;
using UnityEngine;

public class Cubelet : MonoBehaviour
{
    private List<Sticker> stickers = new List<Sticker>();

    public void AddSticker(Sticker s)
    {
        stickers.Add(s);
    }

    public Sticker GetStickerFromSide(Vector3 dir)
    {
        foreach (Sticker s in stickers)
        {
            if (Vector3.Angle(s.transform.forward, dir) < 5f)
                return s;
        }
        
        //This shouldn't happen
        return stickers[0];
    }
}