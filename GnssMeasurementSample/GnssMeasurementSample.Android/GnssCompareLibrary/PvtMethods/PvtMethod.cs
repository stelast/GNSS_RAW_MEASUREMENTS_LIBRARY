using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.PvtMethods
{
    public abstract class PvtMethod
    {/**
     * Calculates pose based on given parameters
     * @param constellation satellite constellation object for which the calculations are to
     *                      be performed
     * @return calculated pose of the receiver
     */
        public abstract Coordinates<Matrix> calculatePose(
                Constellation constellation
        );

        private static Dictionary<string, object> registeredObjects = new Dictionary<string, object>();

        protected static void register<T>(string constellationName, T objectClass)
            where T: PvtMethod
        {
            //        if(registeredObjects.containsKey(constellationName))
            //            throw new IllegalArgumentException("This key is already registered! Select a different name!");
            if (!registeredObjects.ContainsKey(constellationName))
                registeredObjects.Add(constellationName, objectClass);
        }

        public static Dictionary<string, object>.KeyCollection getRegistered()
        {
            return registeredObjects.Keys;
        }

        /**
         * @return Name of the PVT method, which is to be displayed in the UI
         */
        public abstract string getName();

        public static T getClassByName<T>(string name)
            where T:PvtMethod
        {
            return (T)registeredObjects[name];
        }

        public abstract double getClockBias();

        private static bool initialized = false;

        public static void initialize()
        {
            if (!initialized)
            {
                //WeightedLeastSquares.registerClass();
                //StaticExtendedKalmanFilter.registerClass();
                //DynamicExtendedKalmanFilter.registerClass();
                //PedestrianStaticExtendedKalmanFilter.registerClass();
                initialized = true;
            }
        }

        virtual public void startLog(string name) { }

        virtual public void stopLog() { }

        virtual public void logError(double latError, double lonError) { }

        virtual public void logFineLocation(Location location) { }
    }
}