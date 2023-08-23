using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ElevatorChallenge;

bool quit = false;
buttons elevatorButtons = new buttons(1,true);
sensor elevatorSensor = sensor.createSensor(10, 1, 1, 1000, 1, 1, false);
elevator elevator = new elevator(1,1,0,false);
Thread thread = new Thread(() => elevatorSensor.updtdateDirectionForever());
thread.Start();
Thread thread1 = new Thread(() => elevatorSensor.runElevatorForever(elevator));
thread1.Start();

readInput(elevatorButtons);


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
                elevatorButtons.setDirection(true);
                elevatorButtons.setFloor(numericPart);
                Console.WriteLine("Floor: " + elevatorButtons.getFloor() + " Direction: " + elevatorButtons.getDirection());
            }
            else if (alphaPart == "D" || alphaPart == "d")
            {
                elevatorButtons.setDirection(false);
                elevatorButtons.setFloor(numericPart);
                Console.WriteLine("Floor: " + elevatorButtons.getFloor() + " Direction: " + elevatorButtons.getDirection());
            }
            else if (alphaPart == "Q" || alphaPart == "q")
            {
                quit = true;
                break;
            }
            elevatorButtons.updateSensor(elevatorSensor);
            //elevatorSensor.sensorScan();
            Console.WriteLine("Current Floor: " + elevatorSensor.getCurrentFloor() + " Destination Floor: " + elevatorSensor.getDestinationFloor() + " Direction: " + elevatorSensor.getDirection() + " Moving: " + elevatorSensor.getMoving());
        }
    } while (alphaPart != "Q" || alphaPart != "q");
}