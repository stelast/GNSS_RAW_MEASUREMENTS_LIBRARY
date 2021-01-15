using Android;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public class GnssCoreService : Service
    {
        //private static UserNotifier userNotifier;

        ///**
        // * Tag used to mark module names for savedInstanceStates of the onCreate method.
        // */
        //private readonly string MODULE_NAMES_BUNDLE_TAG = "__module_names";

        //private static Bundle savedModulesBundle = null;

        //CalculationModulesArrayList calculationModules = new CalculationModulesArrayList();

        //Observable gnssCoreObservable = new Observable(){
        //    @Override
        //    public void notifyObservers() {
        //        setChanged();
        //        super.notifyObservers(calculationModules); // by default passing the calculation modules
        //    }
        //};

        ////CalculationModulesArrayList.PoseUpdatedListener poseListener = new CalculationModulesArrayList.PoseUpdatedListener(){
        ////    @Override
        ////    public void onPoseUpdated() {
        ////        gnssCoreObservable.notifyObservers();
        ////    }
        ////};

        //private static readonly string TAG = GnssCoreService.class.getSimpleName();

        //public class GnssCoreBinder : Binder{

        //    public void addObserver(Observer observer){
        //        gnssCoreObservable.addObserver(observer);
        //    }

        //    public void removeObserver(Observer observer){
        //        gnssCoreObservable.deleteObserver(observer);
        //    }

        //    public CalculationModulesArrayList getCalculationModules(){
        //        return calculationModules;
        //    }

        //    public void addModule(CalculationModule newModule){
        //        calculationModules.add(newModule);
        //    }

        //    public void removeModule(CalculationModule removedModule){
        //        calculationModules.remove(removedModule);
        //    }

        //    public bool getServiceStarted(){
        //        return serviceStarted;
        //    }

        //    public void assignUserNotifier(UserNotifier notifier) {
        //        userNotifier = notifier;
        //    }
        //}

        //private IBinder binder = new GnssCoreBinder();

        //private static bool serviceStarted = false;

        //public static bool isServiceStarted(){
        //    return serviceStarted;
        //}


        //@Override
        //public void onCreate() {
        //    super.onCreate();

        //    Constellation.initialize();
        //    Correction.initialize();
        //    PvtMethod.initialize();
        //    FileLogger.initialize();

        //    if(calculationModules.size() == 0){
        //        if(savedModulesBundle==null)
        //            createInitialCalculationModules();
        //        else{
        //            try {
        //                createCalculationModulesFromBundle();
        //            } catch (NullPointerException e){
        //                e.printStackTrace();
        //                Log.e(TAG, "onCreate: Failed to create bundled modules. Creating default...");
        //                calculationModules.clear();
        //                CalculationModule.clear();
        //                createInitialCalculationModules();
        //            }
        //        }

        //        try {
        //            while (!tryRegisterForGnssUpdates())
        //                Thread.sleep(500);
        //        } catch (InterruptedException e){
        //            e.printStackTrace();
        //        }

        //    }
        //}

        ///**
        // * Checks if the permission has been granted
        // * @return True of false depending on if permission has been granted
        // */
        //private bool hasGnssAndLogPermissions() {
        //    // Permissions granted at install time.
        //    return (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED &&
        //            ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE)  == PackageManager.PERMISSION_GRANTED);
        //}

        //private bool tryRegisterForGnssUpdates(){
        //    if (hasGnssAndLogPermissions()) {
        //        FusedLocationProviderClient fusedLocationClient = LocationServices.getFusedLocationProviderClient(this);
        //        LocationManager locationManager = (LocationManager) getApplicationContext().getSystemService(LOCATION_SERVICE);
        //        try {
        //            calculationModules.registerForGnssUpdates(fusedLocationClient, locationManager);
        //            calculationModules.assignPoseUpdatedListener(poseListener);
        //        } catch (IllegalStateException e){
        //            e.printStackTrace();
        //            calculationModules.unregisterFromGnssUpdates();
        //            return false;
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //@Override
        //public int onStartCommand(Intent intent, int flags, int startId) {

        //    serviceStarted = true;

        //    return super.onStartCommand(intent, flags, startId);
        //}

        ///**
        // * Waits for length*0.1s for the service to start
        // * @param length duration in length*0.1s
        // * @return true if service has started in defined time, false otherwise
        // */
        //public static bool waitForServiceStarted(int length){
        //    for(int i=0; i<length; i++) {
        //        try {
        //            Thread.sleep(100);
        //        } catch (InterruptedException e) {
        //            e.printStackTrace();
        //        }
        //        if(isServiceStarted())
        //            return true;
        //    }
        //    return false;
        //}

        ///**
        // * Waits for service to start for 15s.
        // * @return true if service has started within 15s, false otherwise
        // */
        //public static bool waitForServiceStarted(){
        //    return waitForServiceStarted(150);
        //}

        //@Override
        //public void onDestroy() {
        //    super.onDestroy();

        //    saveModulesDescriptionToBundle();

        //    calculationModules.unregisterFromGnssUpdates();
        //    calculationModules.clear();
        //    CalculationModule.clear();

        //    serviceStarted = false;

        //    stopSelf();
        //}

        ///**
        // * Saves the descriptions of calculation modules to a bundle object
        // * todo: move to calcualtionModulesArrayList?
        // */
        //private void saveModulesDescriptionToBundle() {

        //    ArrayList<string> modulesNames = new ArrayList<>();
        //    savedModulesBundle = new Bundle();

        //    for (CalculationModule module: calculationModules)
        //        modulesNames.add(module.getName());

        //    savedModulesBundle.putstringArrayList(MODULE_NAMES_BUNDLE_TAG, modulesNames);

        //    for (CalculationModule module : calculationModules){
        //        Bundle moduleDescription = module.getConstructorBundle();
        //        savedModulesBundle.putBundle(module.getName(), moduleDescription);
        //    }
        //}

        ///**
        // * Creates new calculation modules, based on data stored in the bundle
        // * todo: move to calcualtionModulesArrayList?
        // */
        //private void createCalculationModulesFromBundle() {

        //    if(savedModulesBundle!=null) {

        //        ArrayList<string> modulesNames = savedModulesBundle.getstringArrayList(MODULE_NAMES_BUNDLE_TAG);

        //        if (modulesNames != null) {
        //            for (string name : modulesNames) {
        //                try {
        //                    Bundle constructorBundle = savedModulesBundle.getBundle(name);
        //                    if (constructorBundle != null)
        //                        calculationModules.add(CalculationModule.fromConstructorBundle(constructorBundle));
        //                } catch (CalculationModule.NameAlreadyRegisteredException | CalculationModule.NumberOfSeriesExceededLimitException e) {
        //                    e.printStackTrace();
        //                }
        //            }
        //        }
        //        savedModulesBundle = null;

        //    }
        //}

        //private void createInitialCalculationModules(){
        //    final List<CalculationModule> initialModules = new ArrayList<>();

        //    try {
        //        initialModules.add(new CalculationModule(
        //                "Galileo+GPS",
        //                GalileoGpsConstellation.class,
        //                new ArrayList<Class<? extends Correction>>() {{
        //                    add(ShapiroCorrection.class);
        //                    add(TropoCorrection.class);
        //                }},
        //                DynamicExtendedKalmanFilter.class,
        //                NmeaFileLogger.class));

        //        initialModules.add(new CalculationModule(
        //                "GPS",
        //                GpsConstellation.class,
        //                new ArrayList<Class<? extends Correction>>() {{
        //                    add(ShapiroCorrection.class);
        //                    add(TropoCorrection.class);
        //                }},
        //                DynamicExtendedKalmanFilter.class,
        //                NmeaFileLogger.class));

        //        initialModules.add(new CalculationModule(
        //                "Galileo",
        //                GalileoConstellation.class,
        //                new ArrayList<Class<? extends Correction>>() {{
        //                    add(ShapiroCorrection.class);
        //                    add(TropoCorrection.class);
        //                }},
        //                DynamicExtendedKalmanFilter.class,
        //                NmeaFileLogger.class));
        //    } catch (Exception e){
        //        e.printStackTrace();
        //        Log.e(TAG, "createInitialCalculationModules: Exception when creating modules");
        //    }

        //    for(CalculationModule calculationModule : initialModules) {
        //        calculationModules.add(calculationModule); // when simplified to addAll, doesn't work properly
        //    }
        //}

        //public static void notifyUser(string text, int duration, string id){
        //    if (userNotifier!=null){
        //        userNotifier.notifyUser(text, duration, id);
        //    } else {
        //        Log.e(TAG, "displayMessage: userNotifier not set! " +
        //                "Set it by calling GnssCoreService.assignUserNotifier!");
        //    }
        //}

        //public static void notifyUser(string text, int duration){
        //    if (userNotifier!=null){
        //        userNotifier.notifyUser(text, duration, null);
        //    } else {
        //        Log.e(TAG, "displayMessage: userNotifier not set! " +
        //                "Set it by calling GnssCoreService.assignUserNotifier!");
        //    }
        //}

        //    public override IBinder OnBind(Intent intent)
        //    {
        //        return binder;
        //    }
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}