/* Using example from:
 * https://nielson.io/2015/08/the-pixel-grid-better-2d-in-unity-part-1/
 * A different but functionally identical script:
 * https://hackernoon.com/making-your-pixel-art-game-look-pixel-perfect-in-unity3d-3534963cad1d
 * 
 * It seems the sprite needs to be on a child object so the game's "real" physics can be smooth
 * but the aesthetics of the physics can be NES/SNES-like
 */

using UnityEngine;

public class SnapToPixelGrid : MonoBehaviour
{
    [SerializeField]
    private int pixelsPerUnit = 16;

    private Transform parent;

    private void Start()
    {
        parent = transform.parent;
    }

    /// <summary>
    /// Snap the object to the pixel grid determined by the given pixelsPerUnit.
    /// Using the parent's world position, this moves to the nearest pixel grid location by 
    /// offseting this GameObject by the difference between the parent position and pixel grid.
    /// </summary>
    private void LateUpdate()
    {
        Vector3 newLocalPosition = Vector3.zero;

        newLocalPosition.x = (Mathf.Round(parent.position.x * pixelsPerUnit) / pixelsPerUnit) - parent.position.x;
        newLocalPosition.y = (Mathf.Round(parent.position.y * pixelsPerUnit) / pixelsPerUnit) - parent.position.y;

        transform.localPosition = newLocalPosition;
    }
}