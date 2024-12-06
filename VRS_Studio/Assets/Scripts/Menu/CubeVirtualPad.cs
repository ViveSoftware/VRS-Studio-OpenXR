using UnityEngine;

public class CubeVirtualPad : VirtualPadBase
{
    public Bounds bound;

    public override bool PadContainsTarget(Vector3 pos)
    {
        return bound.Contains(pos);
    }

    public override Vector3 ClosestLocalPointToPad(Vector3 pos)
    {
        return bound.ClosestPoint(pos);
    }

    public override Vector3 TargetNormalizedLocalPos(Vector3 pos)
    {
        var max = bound.max;
        var min = bound.min;
        return new Vector3()
        {
            x = InverseLerpUnclamp(max.x, min.x, pos.x),
            y = InverseLerpUnclamp(max.y, min.y, pos.y),
            z = InverseLerpUnclamp(max.z, min.z, pos.z),
        };
    }

    private static float InverseLerpUnclamp(float a, float b, float value)
    {
        return a == b ? 0f : ((value - a) / (b - a));
    }
}
