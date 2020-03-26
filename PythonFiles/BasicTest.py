"""
Pozyx basic troubleshooting (c) Pozyx Labs 2017

If you're experiencing trouble with Pozyx, this should be your first step to check for problems.
Please read the article on https://www.pozyx.io/Documentation/Tutorials/troubleshoot_basics/Python

Edited by P. Hammond, 11/12/2018 to allow user to select tag from menu
"""

from pypozyx import PozyxSerial, get_first_pozyx_serial_port, PozyxConstants, POZYX_SUCCESS
from pypozyx.structures.device_information import DeviceDetails
from pypozyx.definitions.registers import POZYX_WHO_AM_I
from File_IO import PozyxHardware
from time import sleep


def device_check(pozyx, remote_id=None):
    system_details = DeviceDetails()
    pozyx.getDeviceDetails(system_details, remote_id=remote_id)

    if remote_id is None:
        print("Local %s with id 0x%0.4x" % (system_details.device_name, system_details.id))
    else:
        print("%s with id 0x%0.4x" % (system_details.device_name.capitalize(), system_details.id))

    print("\tWho am i: 0x%0.2x" % system_details.who_am_i)
    print("\tFirmware version: v%s" % system_details.firmware_version_string)
    print("\tHardware version: v%s" % system_details.hardware_version_string)
    print("\tSelftest result: %s" % system_details.selftest_string)
    print("\tError: 0x%0.2x" % system_details.error_code)
    print("\tError message: %s" % system_details.error_message)


def network_check_discovery(pozyx, remote_id=None):
    pozyx.clearDevices(remote_id)
    if pozyx.doDiscovery(discovery_type=PozyxConstants.DISCOVERY_ALL_DEVICES, remote_id=remote_id) == POZYX_SUCCESS:
        print("Found devices:")
        pozyx.printDeviceList(remote_id, include_coordinates=False)


if __name__ == '__main__':
    serial_port = get_first_pozyx_serial_port()
    if serial_port is None:
        print("No Pozyx connected. Check your USB cable or your driver!")
        quit()
    else:
        # [PH] Added display for identification / location of serial port connection, eg COM7
        print("Pozyx is connected to serial port: " + serial_port)

    pozyx = PozyxSerial(serial_port)

    # Get user to select tag to test
    remote_id = PozyxHardware().SelectTag()
    print("\n")

    # Test tag
    device_check(pozyx, remote_id)
    network_check_discovery(pozyx, remote_id)

    userExit = input("\nPress any key to exit...")
    
