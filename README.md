# :flight_departure:	Flight-Inspection-App :flight_arrival:	


> First milestone
> 1) Using the .NET Framework, OxyPlot and Caliburn.Micro Framework to create the GUI interface.
> 2) Using MVVM architecture in a multi-threaded environment.
> 3) Implementing a TCP Client to send the data to FlightGear.
> 4) If the images in the README look blurry, click on the image to improve the quality.


## Project Description
This is an application that allows to display flight data on a dedicated simulator (FlightGear) and perform n data investigation.
The application is designed for flight researchers, Air Force pilots or any pilot who wants to improve himself for future flights. 
The application presents the pilot all of the recorded flight's data in various display forms, as a post flight process.
It allows the pilot to control the speed of the playback (slower or faster) and perform an in-depth analysis of data relative to the pilot's other flights: Steering position, speed, direction of flight, altitude, yaw and roll indices, pitch, perform corrosion relative to other flights, load algorithms that study the information, etc ...

## :film_projector:	 Demonstration Video
- :fist_right: [------->Link](https://youtu.be/rmG7bNQnbgo)



## Special features

-	Ability to watch the data in several different views, at any given time during the flight.
-	Ability to present the flight playback at a variable speed
-	The ability to present the aircraft joystick along with other metrics
-	Check correlations from other flights
-	Load different algorithms to perform in-depth research
-	A user interface that displays only the necessary information at a time, thus preventing overloading of the researcher.

## MVVM architecture
Our code is divided into three main parts, the first part is the `<Model>` In this part we have the FGClient that communicates with the flightgear, As part of the communicates with the FGClient, we sends to the flightgear all the information from the CSV. In the second part is the `<ViewModel>` which implementation the main for the joystick display, displaying the graphs and calculating the correlations algorithms. addition in this section is the implementation of all user actions. In the third part `<View>` is the implementation of the GUI and the user interface.
 
## Media controls


<img src="https://user-images.githubusercontent.com/73064092/114400836-09bc9980-9bab-11eb-92ba-4e20fe11a545.png"  width="750">

1) Move the playback 5 seconds backwards
2) Move the playback backwards (until another button is pressed)
 3) Stop the playback
 4) Start the playback video
 5) Stop and return to the beginning of the playback
 6) Moves the playback forward (until another button is pressed)
 7) Moves the playback 5 seconds forward

## Preliminary requirements
- [x] Make sure that flightgear application and visual studio 2019 are installed successfully.
## setup instructions
1. Open the flightgear application, with the following settings:
<img src="https://user-images.githubusercontent.com/73064092/114318178-87819600-9b14-11eb-9cde-430bbafa9edf.png"  width="750">

2. Then click on the "Fly" button in the application
3. Open visual studio 2019 with the attached code and run it.

## Operating instructions
- [x] To configure the client, click on the `<Setup>` button
<img src="https://user-images.githubusercontent.com/73064092/114602294-1e7a5980-9c9f-11eb-8206-188b76cf972f.png"  width="500">


- [x] In the home screen, write the flightGear Server IP, FlightGear Server Port and CSV path
<img src="https://user-images.githubusercontent.com/73064092/114602142-effc7e80-9c9e-11eb-89ec-e4cce9e0062d.png"  width="500">

- [x] *If one of the data entered is incorrect, the above alert window will be displayed*
<img src="https://user-images.githubusercontent.com/73064092/114318467-c237fe00-9b15-11eb-9450-15d68172d048.png"  width="320">

- [x] After initial setup, the option to start the simulation opens
<img src="https://user-images.githubusercontent.com/73064092/114602601-7749f200-9c9f-11eb-955a-e02d30daf51e.png"  width="500">

- [x] When all information is open
<img src="https://user-images.githubusercontent.com/73064092/114605580-e6751580-9ca2-11eb-8fcd-a40cbbefb9ea.png"  width="500">

- [x] To load a flight to compare in the algorithm click ADsetup:
<img src="https://user-images.githubusercontent.com/73064092/114891869-ee54c700-9e14-11eb-8430-e6cbdcd6afbb.png"  width="500">


## UML diagram
![image](https://user-images.githubusercontent.com/73064092/114601586-57fe9500-9c9e-11eb-97f2-479d84dec26f.png)



:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:
:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:
:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	
![image](https://user-images.githubusercontent.com/73064092/114769242-a1201900-9d72-11eb-91a7-79e803afa9c8.png)

:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:
:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:
:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	:world_map:  :compass:	
