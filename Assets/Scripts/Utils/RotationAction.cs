using static CubeUtils;

public class RotationAction
{
    public Axis RotAxis;
    public int Coordinate;
    public bool Clockwise;

    public RotationAction(Axis rotAx, int coord, bool cw)
    {
        RotAxis = rotAx;
        Coordinate = coord;
        Clockwise = cw;
    }
}