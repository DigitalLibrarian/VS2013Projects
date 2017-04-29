using System;
namespace Tiles.Materials
{
    public interface IMaterialStressCalc
    {
        double ImpactCost1(IMaterial strickenMat, double layerVolume);
        double ImpactCost2(IMaterial strickenMat, double layerVolume);
        double ImpactCost3(IMaterial strickenMat, double layerVolume);
        double ShearCost1(IMaterial strikerMat, IMaterial strickenMat, double sharpness);
        double ShearCost2(IMaterial strikerMat, IMaterial strickenMat, double sharpness);
        double ShearCost3(IMaterial strikerMat, IMaterial strickenMat, double sharpness, double layerVolume);
    }
}
