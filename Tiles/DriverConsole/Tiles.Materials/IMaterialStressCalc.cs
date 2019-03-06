using System;
namespace Tiles.Materials
{
    public interface IMaterialStressCalc
    {
        double ShearCost1(double strickenYield, double strikerYield, double sharpness);
        double ShearCost2(double strickenFracture, double strikerFracture, double sharpness);
        double ShearCost3(double strickenFracture, double strikerFracture, double sharpness, double layerVolume);

        double BluntCost1(double strickenYield, double layerVolume);
        double BluntCost2(double strickenYield, double strickenFracture, double layerVolume);
        double BluntCost3(double strickenYield, double strickenFracture, double layerVolume);
    }
}
