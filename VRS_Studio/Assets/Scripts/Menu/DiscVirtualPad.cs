using UnityEngine;

public class DiscVirtualPad : VirtualPadBase
{
    public float Radius = 0.1f;
    public float Height = 0.015f;

    public override bool PadContainsTarget(Vector3 pos)
    {
        if (Mathf.Abs(pos.y) > Height) { return false; }
        return new Vector2(pos.x, pos.z).sqrMagnitude <= Radius * Radius;
    }

    public override Vector3 ClosestLocalPointToPad(Vector3 pos)
    {
        var th = Mathf.Abs(pos.y);
        var tr = new Vector2(pos.x, pos.z).magnitude;
        if (th * Radius < tr * Height) // th / tr < Height / Radius
        {
            var xz = Vector2.ClampMagnitude(new Vector2(pos.x, pos.z), Radius);
            return new Vector3()
            {
                x = xz.x,
                y = pos.y * Radius / tr,
                z = xz.y,
            };
        }
        else
        {
            var xz = Vector2.ClampMagnitude(new Vector2(pos.x, pos.z), tr * Height / th);
            return new Vector3()
            {
                x = xz.x,
                y = Mathf.Sign(pos.y) * Height,
                z = xz.y,
            };
        }
    }

    public override Vector3 TargetNormalizedLocalPos(Vector3 pos)
    {
        return new Vector3()
        {
            x = pos.x / 0.1f,
            y = pos.y / Height,
            z = pos.z / 0.1f,
        };
    }
}
