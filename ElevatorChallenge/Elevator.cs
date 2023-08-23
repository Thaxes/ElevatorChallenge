using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge
{
    internal class Elevator
    {
    }
    public class buttons
    {
        private int floor;
        private int direction;
        private bool quit;
        public buttons(int floor, int direction)
        {
            this.floor = floor;
            this.direction = direction;
            this.quit = false;
        }
        //getters and setters
        public bool getQuit()
        {
            return quit;
        }
        public int getFloor()
        {
            return floor;
        }
        public int getDirection()
        {
            return direction;
        }
        public void setQuit(bool quit)
        {
            this.quit = quit;
        }
        public void setFloor(int floor)
        {
            this.floor = floor;
        }
        public void setDirection(int direction)
        {
            this.direction = direction;
        }
        //update the sensor object with the floor and direction flags
        public void updateSensor(sensor elevatorSensor)
        {
            if (this.direction == 1)
            {
                if (floor > 0 && floor < elevatorSensor.getFloorLimit())
                    elevatorSensor.onButtonsUp(floor);  
                elevatorSensor.onButtonsUp(floor);
            }
            else if (this.direction == 2)
            {
                if (floor > 0 && floor < elevatorSensor.getFloorLimit())
                    elevatorSensor.onButtonsDown(floor);
            }
            if (quit == true)
                elevatorSensor.setQuit(true);
        }
        public void updateSensorElevatorButton(sensor elevatorSensor)
        {
            if (floor > 0 && floor < elevatorSensor.getFloorLimit())
                elevatorSensor.onElevatorButtons(floor);
        }

    }
    public class sensor
    {
        private int floorLimit;
        private int currentFloor;
        private int destinationFloor;
        private int weightLimit;
        private int currentWeight;
        private int direction;
        private bool moving;
        private bool quit;
        private bool quitElevator;
        private bool[] buttonsUp;
        private bool[] buttonsDown;
        private bool[] elevatorButtons;
        //Private constructor to place restrictions on the input
        private sensor(int floorLimit, int currentFloor, int DestinationFloor, int weightLimit, int currentWeight, int direction, bool moving)
        {
            this.floorLimit = floorLimit;
            this.currentFloor = currentFloor;
            this.destinationFloor = DestinationFloor;
            this.weightLimit = weightLimit;
            this.currentWeight = currentWeight;
            this.direction = direction;
            this.moving = moving;
            this.buttonsUp = new bool[floorLimit];
            this.buttonsDown = new bool[floorLimit];
            this.elevatorButtons = new bool[floorLimit];
            this.quit = false;
        }
        //public static method to create a sensor object with restrictions on the input
        public static sensor createSensor(int floorLimit, int currentFloor, int DestinationFloor, int weightLimit, int currentWeight, int direction, bool moving)
        {
            if (currentFloor > 0 && currentFloor < floorLimit && currentWeight < weightLimit)
                return new sensor(floorLimit, currentFloor, DestinationFloor, weightLimit, currentWeight, direction, moving);
            else
            {
                Console.WriteLine("Invalid input");
                return null;
            }
        }
        //because the functions are set up to only execute if conditions are met, call them all in a loop to execute whichever applies.
        public void runElevatorForever(elevator elevator)
        {
            while (true)
            {
                //if going up, move up
                moveElevatorUp();
                //if going down, move down
                moveElevatorDown();
                //if going up and elevator is at up destination, stop
                visitDestUp();
                //if going down and elevator is at down destination, stop
                visitDestDown();
                //if elevator is at destination, stop
                vistDestElevator();
                //update the elevator
                updateElevator(elevator);
                //write information about what the elevator is doing
                Console.WriteLine("Current Floor: " + currentFloor + " " + "Direction: " + direction + " " + "Destination floor:" + destinationFloor);
                //if quitElevator is true, break the loop
                if (quitElevator == true)
                {
                    Console.WriteLine("Elevator has quit");
                    break;
                }
            }
        }
        //Updates the direction of the elevator in all situations.
        public void updtdateDirectionForever()
        {
            while(true)
            {
                //if moving up, update destination
                bool x = updtDestWhileUpMoving();
                //if moving down, update destination
                bool y = updtDestWhileDownMoving();
                //if still, update direction. If return is true, there is NOT a button pressed.
                 bool z = updtDirectionUniAtRest();

                //if no buttons are pressed and elevator is still and quit has been entered, quit.
                if (x == false && y == false && z == false && quit == true)
                {
                    setQuitElevator(true);
                    break;
                }
            }
        }
        //If the elevator is still and no buttons are pressed, it will scan for the closest floor with a button pressed.
        private bool updtDirectionUniAtRest()
        {   

            //evaluate if there are any buttons pressed
            bool x = updtDestWhileUpStill();
            bool y = updtDestWhileDownStill();
            
            
            //if the above are false and elevator is not moving, start scanning for the nearest floor with a button pressed
            if (x == false && y == false && moving == false)
            {   
                //if return is true, there is NOT a button currently pressed.
                
                //repeat this for loop but in the opposite direction to check for every floor.
                for (int i = currentFloor; i >= 1; i--)
                {
                    int j = currentFloor;
                    if (j < floorLimit)
                    {
                        j++;
                    }
                    { //if button is pressed above, set destination and direction to up.
                        if (ButtonsUp(i) == true)
                        {
                            direction = 1;
                            destinationFloor = i;
                            break;
                        } //if button is pressed above for down, set direction to down and destination to that floor.
                        else if (ButtonsDown(i) == true)
                        {
                            direction = 2;
                            destinationFloor = i;
                            break;
                        } //if button is pressed below for Up, set direction to up and destination to that floor.
                        else if (ButtonsUp(j) == true)
                        {
                            direction = 1;
                            destinationFloor = j;
                            break;
                        } // if button is pressed below for down, set direction to down and destination to that floor.
                        else if (ButtonsDown(j) == true)
                        {
                            direction = 2;
                            destinationFloor = j;
                            break;
                        }
                    }
                } //repeat of above for loop to check in the opposite direction.
                for (int i = currentFloor; i < floorLimit; i++)
                {
                    int j = currentFloor;
                    if (j < floorLimit)
                    {
                        j++;
                    }
                    { //if button is pressed above, set destination and direction to up.
                        if (ButtonsUp(i) == true)
                        {
                            direction = 1;
                            destinationFloor = i;
                            break;
                        } //if button is pressed above for down, set direction to down and destination to that floor.
                        else if (ButtonsDown(i) == true)
                        {
                            direction = 2;
                            destinationFloor = i;
                            break;
                        } //if button is pressed below for Up, set direction to up and destination to that floor.
                        else if (ButtonsUp(j) == true)
                        {
                            direction = 1;
                            destinationFloor = j;
                            break;
                        } // if button is pressed below for down, set direction to down and destination to that floor.
                        else if (ButtonsDown(j) == true)
                        {
                            direction = 2;
                            destinationFloor = j;
                            break;
                        }
                    }
                }
                return false;
            }
            return true;
        }
        //if elevator is moving up, it will scan for the next floor with a button pressed. It will skip the very next floor because it is already going up. It will return true if it finds a button.
        public bool updtDestWhileUpMoving()
        {
            if (moving == true && direction == 1)
            {   //skip the next floor because it is unsafe to stop while in motion.
                for (int i = currentFloor + 2; i < floorLimit; i++)
                {
                    if (i > floorLimit)
                    {
                        return false ;
                    } //checks if someone wants to go up or stop on the way. Do not update destination to a further away floor.
                    if ((ButtonsUp(i) == true || ButtonsElevator(i) == true) && destinationFloor > i)
                    {
                        destinationFloor = i;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        //If the elevator is moving up, it will scan for the next floor with a button pressed. It will not skip the very next floor because it is not moving. It will return if it finds a button.
        public bool updtDestWhileUpStill()
        {
            if (moving == false && direction ==1)
            {
                for (int i = currentFloor; i < floorLimit; i++)
                {
                    if (i > floorLimit)
                    {
                        return false;
                    }   //checks if someone wants to go up or stop on the way. Do not update destination to a further away floor.
                    if (ButtonsUp(i) == true || ButtonsElevator(i) == true)
                    {
                        destinationFloor = i;
                        return true ;
                    }
                }
                return false ;
            }
            return false;
        }
        //if the elevator is moving down, it will scan for the next floor with a button pressed. It will skip the very next floor because it is moving. It will return true if it finds a button.
        public bool updtDestWhileDownMoving()
        {
            if (moving == true && direction == 2)
            {   //skip the next floor because it is unsafe to stop while in motion.
                for (int i = currentFloor - 2; i < floorLimit; i--)
                {
                    if (i < 1)
                    {
                        return false;
                    }   //check if someone wants to go down or stop on the way. Do not update destination to a further away floor.
                    if ((ButtonsDown(i) == true || ButtonsElevator(i) == true) && destinationFloor < i)
                    {
                        destinationFloor = i;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        //if the elevator is moving down, it will scan for the next floor with a button pressed. It will not skip the very next floor because it is not moving. It will return true if it finds a button.
        public bool updtDestWhileDownStill()
        {
            if (moving == false && direction == 2)
            {
                for (int i = currentFloor; i < floorLimit; i--)
                {
                    if (i < 1)
                    {
                        return false;
                    }   //check is someone wants to go down or stop on the way.
                    if (ButtonsDown(i) == true || ButtonsElevator(i) == true)
                    {
                        destinationFloor = i;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        //if current floor is destination floor and going in the correct direction, stop.
        public void visitDestUp()
        {   //if the elevator is not going down and the current floor is the destination floor, stop.
            if (direction != 2 && currentFloor == destinationFloor)
            {
                setMoving(false);
                offButtonsUp(currentFloor);
                Console.WriteLine("All going Up, aboard!");
                Thread.Sleep(1000);
            }
        }
        //if current floor is destination floor and going in the correct direction, stop.
        public void visitDestDown()      
        {   //if the current direction is not up and this is the destination floor, stop.
            if (direction != 1 && currentFloor == destinationFloor)
            {
                setMoving(false);
                offButtonsDown(currentFloor);
                Console.WriteLine("All going Down, aboard!");
                Thread.Sleep(1000);
            }
        }
        public void vistDestElevator()
        {   //if the current floor is the destination floor set from inside the elevator, stop.
            if (ButtonsElevator(currentFloor) == true)
            {
                setMoving(false);
                offElevatorButtons(currentFloor);
                Console.WriteLine("This is a stop for floor: " + currentFloor);
                Thread.Sleep(1000);
            }
        }
        //move the elevator one floor up. Wait three seconds.
        public void moveElevatorUp()
        {
           if (currentFloor < destinationFloor)
            {
                setMoving(true);
                Thread.Sleep(3000);
                currentFloor++;
            }   
        }
        //Move the elevator one floor down. Wait three seconds.
        public void moveElevatorDown()
        {
            if (currentFloor > destinationFloor)
            {
                setMoving(true);
                Thread.Sleep(3000);
                currentFloor--;
            }
        }
        //Update the elevator's onboard data.
        public void updateElevator(elevator elevator)
        {
            elevator.setCurrentFloor(currentFloor);
            elevator.setDestinationFloor(destinationFloor);
            elevator.setCurrentWeight(currentWeight);
            elevator.setDirection(direction);
            elevator.setMoving(moving);
        }
        //return true if the floor is flagged to stop at.
        public bool ButtonsUp(int floor)
        {
            return buttonsUp[floor - 1];
        }
        //return true if the floor is flagged to stop at.
        public bool ButtonsDown(int floor)
        {
            return buttonsDown[floor - 1];
        }
        //return true if the floor is flagged to stop at.
        public bool ButtonsElevator(int floor)
        {
            return elevatorButtons[floor - 1];
        }
        //return the number of floors in the building.
        public int getFloorLimit()
        {
            return floorLimit;
        }
        //return the current floor the elevator is on.
        public int getCurrentFloor()
        {
            return currentFloor;
        }
        public bool getQuit()
        {
            return quit;
        }
        //return the destination floor.
        public int getDestinationFloor()
        {
            return destinationFloor;
        }
        //return the weight limit of the elevator.
        public int getWeightLimit()
        {
            return weightLimit;
        }
        //return the current weight of the elevator.
        public int getCurrentWeight()
        {
            return currentWeight;
        }
        //getDirection 0 if stopped, 1 if up, 2 if down. Could be changed to boolean when used with the moving variable.
        public int getDirection()
        {
            return direction;
        }
        //return true if the elevator is moving.
        public bool getMoving()
        {
            return moving;
        }
        //set a floor to be flagged for stopping when going up.
        public void onButtonsUp(int floor)
        {
            buttonsUp[floor -1] = true;
        }
        //set a floor to be flagged for stopping when going down.
        public void onButtonsDown(int floor)
        {
            buttonsDown[floor - 1] = true;
        }
        //set a floor to be flagged for stopping when inside the elevator.
        public void onElevatorButtons(int floor)
        {
            elevatorButtons[floor - 1] = true;
        }
        //disable a floor from being flagged for stopping when going up.
        public void offButtonsUp(int floor)
        {
            buttonsUp[floor - 1] = false;
        }
        //disable a floor from being flagged for stopping when going down.
        public void offButtonsDown(int floor)
        {
            buttonsDown[floor - 1] = false;
        }
        //disable a floor from being flagged for stopping when inside the elevator.
        public void offElevatorButtons(int floor)
        {
            elevatorButtons[floor - 1] = false;
        }
        //set the number of floors in the building.
        public void setFloorLimit(int floorLimit)
        {
            this.floorLimit = floorLimit;
        }
        //set the current floor the elevator is on.
        public void setQuit(bool quit)
        {
            this.quit = quit;
        }
        public void setCurrentFloor(int currentFloor)
        {
            this.currentFloor = currentFloor;
        }
        //set the destination floor.
        public void setDestinationFloor(int DestinationFloor)
        {
            this.destinationFloor = DestinationFloor;
        }
        //set the weight limit of the elevator.
        public void setWeightLimit(int weightLimit)
        {
            this.weightLimit = weightLimit;
        }
        //set the current weight of the elevator.
        public void setCurrentWeight(int currentWeight)
        {
            this.currentWeight = currentWeight;
        }
        //set the direction of the elevator.
        public void setDirection(int direction)
        {
            this.direction = direction;
        }
        //set the moving variable to true or false.
        public void setMoving(bool moving)
        {
            this.moving = moving;
        }
        public bool getQuitElevator()
        {
            return quitElevator;
        }
        public void setQuitElevator(bool quitElevator)
        {
            this.quitElevator = quitElevator;
        }



    }
    public class elevator
    {
        int currentFloor;
        int destinationFloor;
        int direction;
        bool moving;
        //Create an elevator with the current floor, destination floor, direction, and moving status.
        public elevator(int currentFloor, int destinationFloor, int direction, bool moving)
        {
            this.currentFloor = currentFloor;
            this.destinationFloor = destinationFloor;
            this.direction = direction;
            this.moving = moving;
        }
        //get | set methods for the elevator.
        public int getCurrentFloor()
        {
            return currentFloor;
        }
        public int getDestinationFloor()
        {
            return destinationFloor;
        }
        public int getDirection()
        {
            return direction;
        }
        public bool getMoving()
        {
            return moving;
        }
        public void setCurrentFloor(int currentFloor)
        {
            this.currentFloor = currentFloor;
        }
        public void setDestinationFloor(int destinationFloor)
        {
            this.destinationFloor = destinationFloor;
        }
        public void setDirection(int direction)
        {
            this.direction = direction;
        }
        public void setMoving(bool moving)
        {
            this.moving = moving;
        }
        internal void setCurrentWeight(int currentWeight)
        {
            this.currentFloor = currentWeight;
        }
    }
}