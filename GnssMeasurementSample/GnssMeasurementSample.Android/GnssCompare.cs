using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Locations;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations;
using GnssMeasurementSample.Droid.GnssCompareLibrary.PvtMethods;
using GnssMeasurementSample.Library;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Toast;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using static Xamarin.Essentials.Permissions;

[assembly: Dependency(typeof(GnssCompare))]
namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class GnssCompare : IGnssCompare
    {
        bool enablePermission = false;
        MyGnssMeasurementEventCallback my_gnss;
        FusedLocationProviderClient fusedLocationProviderClient;

        public async void checkPermissions()
        {
            var status = await CheckAndRequestPermissionAsync(new Permissions.LocationWhenInUse());
            if (status != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                Console.WriteLine("El usuario nego el permiso para obtener la geolocalizacion");
                enablePermission = false;
            } else
            {
                enablePermission = true;
            }
        }

        public void init()
        {
            Console.WriteLine("Llamada a la funcion init de GnssCompare en Android");
            if (!enablePermission)
            {
                App.Current.MainPage.DisplayAlert("Alerta", "Es necesario que nos otorgue el permiso se acceder a su localizacion", "Entendido");
                return;
            }
            // TODO, poner un tiempo de espera visual hasta que reciba ongnss

            my_gnss = new MyGnssMeasurementEventCallback();
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            locationManager.RegisterGnssMeasurementsCallback(my_gnss);
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(Android.App.Application.Context);
            GetLastLocation();
            locationManager.RegisterGnssMeasurementsCallback(my_gnss);
        }

        private void GetLastLocation()
        {
            Android.Gms.Tasks.Task lastLocation = fusedLocationProviderClient.LastLocation;
            lastLocation.AddOnSuccessListener(new LastLocationSuccess(MainActivity.CurrentActivity, my_gnss)).
                AddOnFailureListener(new LastLocationFailure(MainActivity.CurrentActivity, my_gnss));
        }
        public class LastLocationSuccess : Java.Lang.Object, IOnSuccessListener
        {
            private MainActivity mainActivity;
            private MyGnssMeasurementEventCallback gnssCompare;

            public LastLocationSuccess(MainActivity locationActivity, MyGnssMeasurementEventCallback compare)
            {
                this.mainActivity = locationActivity;
                this.gnssCompare = compare;
            }
            
            public void OnSuccess(Java.Lang.Object obj)
            {
                //var lastLocation = (Android.Locations.Location)obj;
                //gnssCompare.locationFromGoogleServices = lastLocation;

                Android.Locations.Location lastLocation = new Android.Locations.Location("hi");
                lastLocation.Altitude = 1;
                lastLocation.Longitude = -5.9650355;
                lastLocation.Latitude = 37.371402;





                this.gnssCompare.pvtMethod = new WeightedLeastSquares();
                this.gnssCompare.constellation = new GalileoConstellation();
                //this.gnssCompare.locationFromGoogleServices = lastLocation;
                this.gnssCompare.locationFromGoogleServices = lastLocation;
                this.gnssCompare.pvtMethod.logFineLocation(lastLocation);


                Device.BeginInvokeOnMainThread(() =>
                {
                    CrossToastPopUp.Current.ShowToastMessage("OnSuccess" + this.gnssCompare.poseInitialized + " " + (this.gnssCompare.locationFromGoogleServices != null));
                });

                if (!this.gnssCompare.poseInitialized && this.gnssCompare.locationFromGoogleServices != null)
                {
                    this.gnssCompare.pose = Coordinates<Matrix>.globalGeodInstance(
                            this.gnssCompare.locationFromGoogleServices.Latitude,
                            this.gnssCompare.locationFromGoogleServices.Longitude,
                            this.gnssCompare.locationFromGoogleServices.Altitude);
                    this.gnssCompare.poseInitialized = true;
                }
            }
        }
        public class LastLocationFailure : Java.Lang.Object, IOnFailureListener
        {
            private MainActivity mainActivity;
            private MyGnssMeasurementEventCallback gnssCompare;

            public LastLocationFailure(MainActivity locationActivity, MyGnssMeasurementEventCallback compare)
            {
                this.mainActivity = locationActivity;
                this.gnssCompare = compare;
            }

            public void OnFailure(Java.Lang.Exception ex)
            {
                Toast.MakeText(mainActivity, "Getting last location failed: " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }


        public class MyGnssMeasurementEventCallback : GnssMeasurementsEvent.Callback
        {
            public GalileoConstellation constellation = new GalileoConstellation();
            public WeightedLeastSquares pvtMethod;
            public Android.Locations.Location locationFromGoogleServices;
            public Coordinates<Matrix> pose = Coordinates<Matrix>.globalGeodInstance(-5.9650355, 37.371402, 1); // can't be zeros - ECEF conversion crashes


            /**
             * Flag indicating that the locationFromGoogleServices has been initialized.
             */
            public bool poseInitialized = false;

            public override void OnGnssMeasurementsReceived(GnssMeasurementsEvent? eventArgs)
            {
                Console.WriteLine("OnGnssMeasurementsReceived");
                Console.WriteLine(poseInitialized);

                /*
                Device.BeginInvokeOnMainThread(() =>
                {
                    CrossToastPopUp.Current.ShowToastMessage("OnGnssMeasurementsReceived");
                });
                */
                

                // Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Alert", "Hola", "OK").RunSynchronously();

                // Toast.MakeText(context, "Pose status: " + poseInitialized, ToastLength.Long).Show();

                // aca inicia con el uso de la libreria transcrita desde Java a C#

                // 1. Clase Constellation llama al metodo updateMeasurement, para ello se ecoge previamente uno de los 7 metodos o clases
                // que implementan la clase abstracta Constellation
                //eventArgs.Measurements

                // 2. Clase Constellatio llama al metodo calculateSatPosition 
                if (poseInitialized)
                {
                    constellation.updateMeasurements(eventArgs);
                    constellation.calculateSatPosition(locationFromGoogleServices, pose);
                    Console.WriteLine(constellation.getUsedConstellationSize());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CrossToastPopUp.Current.ShowToastMessage("poseInitialized");
                    });

                    var a = constellation.getUsedConstellationSize();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CrossToastPopUp.Current.ShowToastMessage("constellationSize: " +a);
                    });
                    if (a != 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CrossToastPopUp.Current.ShowToastMessage("hay satelites");
                        });
                        /*
                        Coordinates<Matrix> poseOut = pvtMethod.calculatePose(constellation);
                        Log.Info("GnssMeasurementSample", "newPose: " + poseOut.getGeodeticLatitude() + ", " + poseOut.getGeodeticLongitude() + ", " + poseOut.getGeodeticHeight());

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CrossToastPopUp.Current.ShowToastMessage("newPose" + poseOut.getGeodeticLatitude() + " " + poseOut.getGeodeticLongitude());
                        });
                        */
                        //Toast.MakeText(g, "newPose: " + poseOut.getGeodeticLatitude() + ", " + poseOut.getGeodeticLongitude() + ", " + poseOut.getGeodeticHeight(), ToastLength.Long).Show();
                    }
                }

            }
            public override void OnStatusChanged([GeneratedEnum] GnssMeasurementCallbackStatus status)
            {
                Console.WriteLine("OnStatusChanged");

            }
        }
        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
                    where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }

    }
}