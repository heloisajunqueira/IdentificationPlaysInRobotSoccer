public class PolyhedralFrechetDistance : FrechetDistance {

    protected PolyhedralDistanceFunction distfunc;

    public PolyhedralFrechetDistance(PolyhedralDistanceFunction distfunc) {
        this.distfunc = distfunc;
    }

//    @Override
    public override double distance(double[] p, double[] q) {
        return distfunc.getDistance(p, q);
    }

 //   @Override
    protected override UpperEnvelope initializeRowUpperEnvelope(int row) {
        return new PolyhedralUpperEnvelope(distfunc, Q[row], Q[row + 1]);
    }

//    @Override
    protected override UpperEnvelope initializeColumnUpperEnvelope(int column) {
        return new PolyhedralUpperEnvelope(distfunc, P[column], P[column + 1]);
    }
}
