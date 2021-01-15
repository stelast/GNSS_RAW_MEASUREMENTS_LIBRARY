using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections;
using Java.Sql;
using System;
using System.Collections.Generic;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public abstract class Correction
    {

        /**
         * Calculates current correction for given parameters
         * @param currentTime current timestamp
         * @param approximatedPose approximate pose of the receiver
         * @param satelliteCoordinates satellite coordinates
         * @param navigationProducer Klobuchar coefficients from the naivgation message (ephemeris)
         * @return calculated correction to be applied to the pseudorange
         */
        public abstract void calculateCorrection(
                Time currentTime,
                Coordinates<Matrix> approximatedPose,
                SatellitePosition satelliteCoordinates,
                NavigationProducer navigationProducer,
                Location initialLocation);

        /**
         *
         * @return calculated correction
         */
        public abstract double getCorrection();

        /**
         * stores all classes which extend the Correction class and were registered with the
         * register method
         */
        private static Dictionary<string, Correction> registeredObjects = new Dictionary<string, Correction>();

        /**
         * Registers the new Correction class
         * @param correctionName name of the class
         * @param objectClass reference to the class
         */
        protected static void register(string correctionName, Correction objectClass)
        {
            if (!registeredObjects.ContainsKey(correctionName))
                registeredObjects.Add(correctionName, objectClass);
        }

        /**
         * names of all registered classes
         */
        public static Dictionary<string, Correction>.KeyCollection getRegistered()
        {
            return registeredObjects.Keys;
        }

        /**
         *
         * @param name name of the class
         * @return class reference
         */
        public static Correction getClassByName(string name)
        {
            return registeredObjects[name];
        }

        /**
         *
         * @return name of the constellation
         */
        public abstract string getName();

        /**
         * Indicates if initialization has already been performed
         */
        private static bool initialized = false;

        /**
         * Registers all constellation classes which extend this
         */
        public static void initialize()
        {
            if (!initialized)
            {
                IonoCorrection.registerClass();
                ShapiroCorrection.registerClass();
                TropoCorrection.registerClass();
                initialized = true;
            }
        }
    }
}