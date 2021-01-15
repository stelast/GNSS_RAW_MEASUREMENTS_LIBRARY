using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using Java.IO;
using Java.Sql;
using System;
using System.Collections.Generic;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public abstract class Constellation
    {
        /**
         * Indicates if initialization has already been performed
         */
        private static bool initialized = false;

        private static string TAG = "Constellation";

        protected static string RNP_NULL_MESSAGE = "RNP_NULL_MESSAGE";



        /**
         * Registers all constellation classes which extend this
         */
        public static void initialize()
        {
            if (!initialized)
            {

                GpsConstellation.registerClass();
                //GalileoConstellation.registerClass();
                //GalileoGpsConstellation.registerClass();
                //GpsL5Constellation.registerClass();
                //GpsIonoFreeConstellation.registerClass();
                //GalileoE5aConstellation.registerClass();
                //GalileoIonoFreeConstellation.registerClass();

                initialized = true;
            }
        }

        /**
         * Additional definition of an ID for a new constellation type
         */
        public static readonly int CONSTELLATION_GALILEO_GPS = 999; //todo is there a better way to define this?
        public static readonly int CONSTELLATION_GALILEO_IonoFree = 998; //todo is there a better way to define this?
        public static readonly int CONSTELLATION_GPS_IonoFree = 997; //todo is there a better way to define this?

        /**
         *
         * @return the estimated receiver position
         */
        public abstract Coordinates<Matrix> getRxPos();

        /**
         *
         * @param rxPos new estimated receiver position
         */
        public abstract void setRxPos(Coordinates<Matrix> rxPos);

        /**
         * Factory method for converting RxPos to a SimpleMatrix
         * @param rxPos rxPos Coordinates object
         * @return RxPos as a 4z1 vector
         */
        public static SimpleMatrix<Matrix> getRxPosAsVector(Coordinates<Matrix> rxPos)
        {
            SimpleMatrix<Matrix> rxPosSimpleVector = new SimpleMatrix<Matrix>(4, 1);
            rxPosSimpleVector.set(0, rxPos.getX());
            rxPosSimpleVector.set(1, rxPos.getY());
            rxPosSimpleVector.set(2, rxPos.getZ());
            rxPosSimpleVector.set(3, 0);

            return rxPosSimpleVector;
        }

        /**
         *
         * @param index id
         * @return satellite of that id
         */
        public abstract SatelliteParameters getSatellite(int index);

        /**
         *
         * @return all satellites registered in the object
         */
        public abstract List<SatelliteParameters> getSatellites();

        public abstract List<SatelliteParameters> getUnusedSatellites();

        /**
         *
         * @return size of the visible constellation
         */
        public abstract int getVisibleConstellationSize();

        /**
         *
         * @return size of the used constellation
         */
        public abstract int getUsedConstellationSize();

        /**
         * Method which is to calculate the satellite positions based on current satellite parameters
         * and passed location objects
         * @param initialLocation initial location, can be used to retrieve the navigation message
         * @param position current position of the receiver
         */
        public abstract void calculateSatPosition(Location initialLocation, Coordinates<Matrix> position);

        /**
         * stores all classes which extend the Constellation class and were registered with the
         * register method
         */
        //private static Dictionary<string, Class<? extends Constellation>> registeredObjects = new Dictionary<string,>();
        private static Dictionary<string, object> registeredObjects = new Dictionary<string, object>();

        public void Add<T>(string key, T value) where T : Constellation
        {
            registeredObjects.Add(key, value);
        }

        public T GetValue<T>(string key) where T : Constellation
        {
            return registeredObjects[key] as T;
        }

        /**
         * Registers the new Constellation class
         * @param constellationName name of the class
         * @param objectClass reference to the class
         */
        protected static void register<T>( string constellationName, T objectClass)
            where T : Constellation
        {

            if (!registeredObjects.ContainsKey(constellationName))
            {
                registeredObjects.Add(constellationName, objectClass);
            }
        }

        /**
         *
         * @return names of all registered classes
         */
        public static Dictionary<string,object>.KeyCollection getRegistered()
        {
            return registeredObjects.Keys;
        }

        /**
         *
         * @param name name of the class
         * @return class reference
         */
        public static T getClassByName<T>(string name)
            where T: Constellation
        {
            return (T) registeredObjects[name];
        }

        /**
         * Returns signal strength to a satellite given by an index.
         * Warning: index is the index of the satellite as stored in internal list, not it's id.
         * @param index index of satellite
         * @return signal strength for the satellite given by {@code index}.
         */
        public abstract double getSatelliteSignalStrength(int index);

        /**
         * @return ID of the constellation
         */
        public abstract GnssConstellationType getConstellationId();

        /**
         * Adds corrections to the processing flow
         * @param corrections corrections to be added
         */
        public abstract void addCorrections(List<Correction> corrections);

        /**
         *
         * @return time of measurement
         */
        public abstract Time getTime();

        /**
         *
         * @return name of the constellation
         */
        public abstract string getName();

        /**
         * method invoked on every GNSS measurement event update. It should update satellite's internal
         * parameters.
         * @param event GNSS event
         */
        public abstract void updateMeasurements(GnssMeasurementsEvent evento);
        }
}