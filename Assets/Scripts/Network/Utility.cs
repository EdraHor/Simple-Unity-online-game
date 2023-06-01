using Network;
using UnityEngine;

public static class Utility
{
    public static Position Vector3ToPos(Vector3 p)
    {
        return new Position(p.x, p.y, p.z);
    }
    public static Rotation QuaternionToRot(Quaternion rotation)
    {
        var r = rotation.eulerAngles;
        return new Rotation(r.x, r.y, r.z);
    }

    public static Vector3 PosToVector3(Position p)
    {
        return new Vector3(p.X, p.Y, p.Z);
    }
    public static Quaternion RotToQuaternion(Rotation r)
    {
        var rot = new Vector3(r.X, r.Y, r.Z);
        return Quaternion.Euler(rot);
    }
}

