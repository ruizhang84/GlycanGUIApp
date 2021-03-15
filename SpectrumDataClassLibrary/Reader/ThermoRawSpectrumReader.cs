using MSFileReaderLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumData.Spectrum;

namespace SpectrumData.Reader
{
    public class ThermoRawSpectrumReader : ISpectrumReader
    {
        protected IXRawfile5 rawConnect;

        public ThermoRawSpectrumReader()
        {
            rawConnect = new MSFileReader_XRawfile() as IXRawfile5;
        }

        ~ThermoRawSpectrumReader()
        {
            rawConnect.Close();
        }

        public int GetFirstScan()
        {
            int startScanNum = 0;
            rawConnect.GetFirstSpectrumNumber(ref startScanNum);

            return startScanNum;
        }

        public int GetLastScan()
        {
            int lastScanNum = 0;
            rawConnect.GetLastSpectrumNumber(ref lastScanNum);
            return lastScanNum;
        }

        public int GetMSnOrder(int scanNum)
        {
            int msnOrder = 0;
            rawConnect.GetMSOrderForScanNum(scanNum, ref msnOrder);
            return msnOrder;
        }

        public double GetPrecursorMass(int scanNum, int msOrder)
        {
            double pdPrecursorMass = 0.0;
            rawConnect.GetPrecursorMassForScanNum(scanNum, msOrder, ref pdPrecursorMass);
            return pdPrecursorMass;
        }

        public double GetRetentionTime(int scanNum)
        {
            double retention = 0;
            rawConnect.RTFromScanNum(scanNum, ref retention);
            return retention;
        }

        public ISpectrum GetSpectrum(int scanNum)
        {
            double retention = GetRetentionTime(scanNum);
            ISpectrum spectrum = new GeneralSpectrum(scanNum, retention);

            string szFilter = "";
            int pnScanNumber = scanNum;
            int nIntensityCutoffType = 0;
            int nIntensityCutoffValue = 0;
            int nMaxNumberOfPeaks = 0;
            int bCentroidResult = 0;
            int pnArraySize = 0;
            double pdCentroidPeakWidth = 0;
            object pvarMassList = null;
            object pvarPeakFlags = null;

            rawConnect.GetMassListFromScanNum(ref pnScanNumber, szFilter, nIntensityCutoffType, nIntensityCutoffValue,
                nMaxNumberOfPeaks, bCentroidResult, ref pdCentroidPeakWidth, ref pvarMassList, ref pvarPeakFlags, ref pnArraySize);

            ////construct peaks
            double[,] value = (double[,])pvarMassList;
            for (int pn = 0; pn < pnArraySize; pn++)
            {
                double mass = value[0, pn];
                double intensity = value[1, pn];
                if (intensity > 0)
                {
                    spectrum.Add(new GeneralPeak(mass, intensity));
                }
            }

            return spectrum;
        }

        public TypeOfMSActivation GetActivation(int scanNum)
        {
            int pnActivationType = 0;
            rawConnect.GetActivationTypeForScanNum(scanNum, GetMSnOrder(scanNum), ref pnActivationType);
            return (TypeOfMSActivation)pnActivationType;
        }

        public void GetScanHeaderInfoForScanNum(int nScanNum,
            ref double dLowMass, ref double dHighMass, ref double dTIC,
            ref double dBasePeakMass, ref double dBasePeakIntensity)
        {
            int nPackets = 0;
            double dStartTime = 0.0;
            int nChannels = 0;
            int bUniformTime = 0;
            double dFrequency = 0.0;
            rawConnect.GetScanHeaderInfoForScanNum(nScanNum, ref nPackets, ref dStartTime,
                ref dLowMass, ref dHighMass, ref dTIC, ref dBasePeakMass, ref dBasePeakIntensity,
                ref nChannels, ref bUniformTime, ref dFrequency);
        }

        public void Init(string fileName)
        {
            rawConnect.Open(fileName);
            rawConnect.SetCurrentController(0, 1);
        }
    }
}
