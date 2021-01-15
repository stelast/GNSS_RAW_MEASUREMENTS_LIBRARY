# GNSS_RAW_MEASUREMENTS_LIBRARY
This library consumes the information from Galileo's satellites and 
calculates latitude, longitude and altitude based on Raw GNSS Measurements. 

 

To enable Signis' mobile application to access the location via Galileo 
satellites, this library has been created based on the code of the "GNSS 
Compare" application. Since this application is made with the Java 
programming language, as it is native to Android and since Signis' mobile 
application is made in Xamarin(C#), it has been necessary to rewrite all the 
methods in charge of exploring the Galileo constellation, filtering the available 
satellites, calculating the position of these satellites in relation to the user, 
calculating the error related to the position, calculating the azimuth and 
pseudorange, among others. 

 

This extensive work is composed of a total of three different libraries. Our 
main difficulty has been to access the positioning service.  
Since only the application mentioned above is available, it has been 
decided to publish this library as an open source license to facilitate 
communication with Galileo to other developers in the community. 
