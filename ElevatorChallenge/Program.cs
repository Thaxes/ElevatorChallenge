﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ElevatorChallenge;

buttons elevatorButtons = new buttons(1,1);
sensor elevatorSensor = sensor.createSensor(10, 1, 1, 1000, 1, 1, false);
elevator elevator = new elevator(1,1,0,false);
Thread sensorThread = new Thread(() => elevatorSensor.updtdateDirectionForever());
sensorThread.Start();
Thread runElevatorThread = new Thread(() => elevatorSensor.runElevatorForever(elevator));
runElevatorThread.Start();
Thread readInputsForButtons = new Thread(() => readInput(elevatorButtons));
readInputsForButtons.Start();

readInputsForButtons.Join();
sensorThread.Join();
runElevatorThread.Join();


//This method reads user input and updates the button and sensor objects accordingly. The sensorScan method updates the destination floor.
 void readInput(buttons elevatorButtons)
{
    string alphaPart = "t";
    do
    {
        //create a regex expression to separate the floor number from the direction.
        Regex re = new Regex(@"(\d+)([u,U,d,D,q,Q]+)");

        Console.WriteLine("Enter the floor number and direction, denoted as U or D, without spaces: ");

        //Save user input to a variable
        var floorDirection = Console.ReadLine();

        //check if it comes from inside the elevator
        if (floorDirection.Length < 2 && floorDirection.Length > 0)
        {
            if (floorDirection == "q" || floorDirection == "Q")
            {
                gelevatorButtons.setQuit(true);
                elevatorButtons.updateSensor(elevatorSensor);
                break;
            }
            int floorNumber = int.Parse(floorDirection);
            elevatorButtons.setFloor(floorNumber);
            elevatorButtons.updateSensorElevatorButton(elevatorSensor);

            Console.WriteLine("Current Floor: " + elevatorSensor.getCurrentFloor() + " Destination Floor: " + elevatorSensor.getDestinationFloor() + " Direction: " + elevatorSensor.getDirection() + " Moving: " + elevatorSensor.getMoving());
        }
        else
        {
            //evaluate if the user input follows the regex expression
            Match result = re.Match(floorDirection);

            //if the user input does not follow the regex expression, prompt the user to re-enter the input
            while (!result.Success)
            {
                Console.WriteLine("Invalid input. Please enter the floor number and direction, denoted as U or D, without spaces: ");
                floorDirection = Console.ReadLine();
                result = re.Match(floorDirection);
            }
            //if the user input follows the regex expression, save the floor number and direction to the button object
            if (result.Success)
            {
                alphaPart = result.Groups[2].Value;
                int numericPart = Int32.Parse(result.Groups[1].Value);

                if (alphaPart == "u" || alphaPart == "U")
                {
                    elevatorButtons.setDirection(1);
                    elevatorButtons.setFloor(numericPart);
                    Console.WriteLine("Floor: " + elevatorButtons.getFloor() + " Direction: " + elevatorButtons.getDirection());
                }
                else if (alphaPart == "D" || alphaPart == "d")
                {
                    elevatorButtons.setDirection(2);
                    elevatorButtons.setFloor(numericPart);
                    Console.WriteLine("Floor: " + elevatorButtons.getFloor() + " Direction: " + elevatorButtons.getDirection());
                }
                elevatorButtons.updateSensor(elevatorSensor);
                Console.WriteLine("Current Floor: " + elevatorSensor.getCurrentFloor() + " Destination Floor: " + elevatorSensor.getDestinationFloor() + " Direction: " + elevatorSensor.getDirection() + " Moving: " + elevatorSensor.getMoving());
            }
        }
    } while (alphaPart != "Q" || alphaPart != "q");
}
