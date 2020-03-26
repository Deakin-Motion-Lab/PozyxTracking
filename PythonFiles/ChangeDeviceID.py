"""
    change_network_id.py - Changes a local/remote device's network ID.

    [PH]: Modified by P. Hammond, 17/12/2018
        Changes:
            - User input funcionality added
            - Expression checks added
"""


from pypozyx import *
from pypozyx.definitions.registers import *
from time import sleep
import sys

def set_new_id(pozyx, new_id, remote_id):
    print("Setting the Pozyx ID to 0x%0.4x" % new_id)
    pozyx.setNetworkId(new_id, remote_id)
    if pozyx.saveConfiguration(POZYX_FLASH_REGS, [POZYX_NETWORK_ID], remote_id) == POZYX_SUCCESS:
        print("Saving new ID successful! Resetting system...")
        if pozyx.resetSystem(remote_id) == POZYX_SUCCESS:
            print("Done")

''' Checks ID is a valid hexadecimal number '''
def CheckID(id):
    idOK = False

    if CheckIDLength(id):
        for number in id:
            if number >= "0" and number <= "9" or number >= "a" and number <= "f":
                idOK = True
            else:
                idOK = False
                break
    
    return idOK

''' Checks input length contains 4 values '''
def CheckIDLength(id):
    lengthOK = False

    if (len(id) == 4):
        lengthOK = True

    return lengthOK

''' Get device ID from user input '''
def GetID(idType):
    deviceID = 0
    attempts = 0
    maxAttempts = 3
    
    while (True):
        userInput = input("\nEnter " + idType + " ID number in format \"xxxx\" (eg. 001a)):> ")

        if CheckID(userInput):
            deviceID = int((userInput), 16)
            break
        else:
            print("\nYou must enter a valid hexadecimal value between 0001 and ffff\n")
            attempts += 1
            if (attempts == maxAttempts):
                pauseInput = input("Exiting program after 3 invalid attemtps...press any key to continue")
                sys.exit()
    

    return deviceID

if __name__ == "__main__":
    serial_port = get_first_pozyx_serial_port()
    if serial_port is None:
        print("No Pozyx connected. Check your USB cable or your driver!")
        quit()

    # Change Pozyx Device IDs
    # Get Current ID from User
    old_id = GetID("current")

    # Get New ID from User
    new_id = GetID("new")

    # Execute the ID change
    remote = True           # whether to use the remote device
    
    if not remote:
        old_id = None

    pozyx = PozyxSerial(serial_port)
    set_new_id(pozyx, new_id, old_id)

    print("\nNOTE: remember to update 'tagIDs.txt' or 'anchors.txt' files with new device ID number")
    pause = input("\nPress any key to exit...")
