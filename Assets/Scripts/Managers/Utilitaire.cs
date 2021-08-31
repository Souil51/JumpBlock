using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utilitaire : MonoBehaviour
{
    public enum PointBlock { HautGauche = 1, HautDroite = 2, BasDroite = 3, BasGauche = 4, Haut = 5, Droite = 6, Bas = 7, Gauche = 8}

    public static Vector2 GetPoint(PointBlock point, SpriteRenderer spriteRenderer, Vector3 position, bool bGravityReversed = false)
    {
        float resX = position.x;
        float resY = position.y;

        float midWidth = spriteRenderer.bounds.size.x / 2;
        float midHeight = spriteRenderer.bounds.size.y / 2;

        int nSens = 1;

        if (bGravityReversed)
            nSens = -1;

        switch (point)
        {
            case PointBlock.HautGauche:
                resX -= midWidth;
                resY += midHeight * nSens;
                break;
            case PointBlock.HautDroite:
                resX += midWidth;
                resY += midHeight * nSens; ;
                break;
            case PointBlock.BasDroite:
                resX += midWidth;
                resY -= midHeight * nSens; ;
                break;
            case PointBlock.BasGauche:
                resX -= midWidth;
                resY -= midHeight * nSens; ;
                break;
            case PointBlock.Haut:
                resY += midHeight * nSens; ;
                break;
            case PointBlock.Droite:
                resX += midWidth;
                break;
            case PointBlock.Bas:
                resY -= midHeight * nSens; ;
                break;
            case PointBlock.Gauche:
                resX -= midWidth;
                break;
        }

        return new Vector2(resX, resY);
    }

}
