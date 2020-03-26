"""
[PH]: This program builds upon "ready_to_localize.py" from Pozyx Tutorial 2, extracting Position Data (x, y, z) and Orientation Data (p, r, y).
      Extracted data is outputted to console plus transmitted to local host via OSC protocol, which can be accessed with other applications such as Visual Studio and Unity.

      Requires 4 x anchors + 1 x tag

      IMPORTANT: This script must be run PRIOR to running the corresponding scene in Unity.
"""
from time import sleep

from pypozyx import (POZYX_POS_ALG_UWB_ONLY, POZYX_3D, Coordinates, SensorData, POZYX_SUCCESS, PozyxConstants, version,
                     DeviceCoordinates, PozyxSerial, get_first_pozyx_serial_port, SingleRegister, DeviceList, PozyxRegisters)
from pythonosc.udp_client import SimpleUDPClient

from pypozyx.tools.version_check import perform_latest_version_check


class ReadyToLocalize(object):
    """Continuously calls the Pozyx positioning function and prints its position."""

    def __init__(self, pozyx, osc_udp_client, anchors, algorithm=POZYX_POS_ALG_UWB_ONLY, dimension=POZYX_3D, height=1000, remote_id=None):
        self.pozyx = pozyx
        self.osc_udp_client = osc_udp_client
        self.anchors = anchors
        self.algorithm = algorithm
        self.dimension = dimension
        self.height = height
        self.remote_id = remote_id

    def setup(self):
        """Sets up the Pozyx for positioning by calibrating its anchor list."""
        print("------------POZYX POSITIONING V{} -------------".format(version))
        print("")
        print("- System will manually configure tag")
        print("")
        print("- System will auto start positioning")
        print("")
        if self.remote_id is None:
            self.pozyx.printDeviceInfo(self.remote_id)
        else:
            for device_id in [None, self.remote_id]:
                self.pozyx.printDeviceInfo(device_id)
        print("")
        print("------------POZYX POSITIONING V{} -------------".format(version))
        print("")

        self.setAnchorsManual(save_to_flash=False)
        self.printPublishConfigurationResult()

    def loop(self):
        """Performs positioning and displays/exports the results."""
        position = Coordinates()
        status = self.pozyx.doPositioning(
            position, self.dimension, self.height, self.algorithm, remote_id=self.remote_id)
        if status == POZYX_SUCCESS:
            self.printPublishPosition(position)
        else:
            self.printPublishErrorCode("positioning")

    """ [PH]: Prints the Pozyx's position to console and outputs a OSC packet """
    def printPublishPosition(self, position):
        network_id = self.remote_id
        if network_id is None:
            network_id = 0

        # Get Orientation Data
        orientationValues = self.GetOrientation()

        # Print output to console
        print("POS ID {}, x(mm): {pos.x} y(mm): {pos.y} z(mm): {pos.z}, orientation: {ori[0]} {ori[1]} {ori[2]}".format(
            "0x%0.4x" % network_id, pos=position, ori=orientationValues))

        # Transmit output to OSC
        if self.osc_udp_client is not None:
            self.osc_udp_client.send_message("/position", [network_id, int(position.x), int(position.y), int(position.z), orientationValues[0], orientationValues[1], orientationValues[2]])
            

    def printPublishErrorCode(self, operation):
        """Prints the Pozyx's error and possibly sends it as a OSC packet"""
        error_code = SingleRegister()
        network_id = self.remote_id
        if network_id is None:
            self.pozyx.getErrorCode(error_code)
            print("LOCAL ERROR %s, %s" % (operation, self.pozyx.getErrorMessage(error_code)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message("/error", [operation, 0, error_code[0]])
            return
        status = self.pozyx.getErrorCode(error_code, self.remote_id)
        if status == POZYX_SUCCESS:
            print("ERROR %s on ID %s, %s" %
                  (operation, "0x%0.4x" % network_id, self.pozyx.getErrorMessage(error_code)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message(
                    "/error", [operation, network_id, error_code[0]])
        else:
            self.pozyx.getErrorCode(error_code)
            print("ERROR %s, couldn't retrieve remote error code, LOCAL ERROR %s" %
                  (operation, self.pozyx.getErrorMessage(error_code)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message("/error", [operation, 0, -1])
            # should only happen when not being able to communicate with a remote Pozyx.

    def setAnchorsManual(self, save_to_flash=False):
        """Adds the manually measured anchors to the Pozyx's device list one for one."""
        status = self.pozyx.clearDevices(remote_id=self.remote_id)
        for anchor in self.anchors:
            status &= self.pozyx.addDevice(anchor, remote_id=self.remote_id)
        if len(self.anchors) > 4:
            status &= self.pozyx.setSelectionOfAnchors(PozyxConstants.ANCHOR_SELECT_AUTO, len(self.anchors),
                                                       remote_id=self.remote_id)

        if save_to_flash:
            self.pozyx.saveAnchorIds(remote_id=self.remote_id)
            self.pozyx.saveRegisters([PozyxRegisters.POSITIONING_NUMBER_OF_ANCHORS], remote_id=self.remote_id)
        return status

    def printPublishConfigurationResult(self):
        """Prints and potentially publishes the anchor configuration result in a human-readable way."""
        list_size = SingleRegister()

        self.pozyx.getDeviceListSize(list_size, self.remote_id)
        print("List size: {0}".format(list_size[0]))
        if list_size[0] != len(self.anchors):
            self.printPublishErrorCode("configuration")
            return
        device_list = DeviceList(list_size=list_size[0])
        self.pozyx.getDeviceIds(device_list, self.remote_id)
        print("Calibration result:")
        print("Anchors found: {0}".format(list_size[0]))
        print("Anchor IDs: ", device_list)

        for i in range(list_size[0]):
            anchor_coordinates = Coordinates()
            self.pozyx.getDeviceCoordinates(device_list[i], anchor_coordinates, self.remote_id)
            print("ANCHOR, 0x%0.4x, %s" % (device_list[i], str(anchor_coordinates)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message(
                    "/anchor", [device_list[i], int(anchor_coordinates.x), int(anchor_coordinates.y), int(anchor_coordinates.z)])
                sleep(0.025)

    def printPublishAnchorConfiguration(self):
        """Prints and potentially publishes the anchor configuration"""
        for anchor in self.anchors:
            print("ANCHOR,0x%0.4x,%s" % (anchor.network_id, str(anchor.coordinates)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message(
                    "/anchor", [anchor.network_id, int(anchor.coordinates.x), int(anchor.coordinates.y), int(anchor.coordinates.z)])
                sleep(0.025)

    #[PH]: Extract Orientation data from Pozyx sensor
    def GetOrientation(self):
        # Get Orientation data from Pozyx sensor
        orientation = SensorData().euler_angles
        self.pozyx.getEulerAngles_deg(orientation, self.remote_id)

        # Extract key data (pitch, roll, yaw)
        orientationData = str(orientation).split(',')
        yaw = float(orientationData[0].lstrip("Heading: "))
        roll = float(orientationData[1].lstrip("Roll: "))
        pitch = float(orientationData[2].lstrip("Pitch: "))
        
        return (pitch, roll, yaw)


if __name__ == "__main__":
    # Check for the latest PyPozyx version. Skip if this takes too long or is not needed by setting to False.
    check_pypozyx_version = True
    if check_pypozyx_version:
        perform_latest_version_check()

    # Find Serial Port connection (COM port)
    serial_port = get_first_pozyx_serial_port()
    if serial_port is None:
        print("No Pozyx connected. Check your USB cable or your driver!")
        quit()
    else:
        print("Pozyx connected on port: " + serial_port)

    '''************************************************************************************************************************************************'''
    # Set Remote ID
    tag_id = 0x6720                   # Pozyx Tag ID (in hexadecimal)
    remote = True                     # set to TRUE when Pozyx 'Tag' is operating remotely or FALSE if Pozyx 'Tag' is connected to PC via USB cable
    '''************************************************************************************************************************************************'''
    if not remote:
        tag_id = None

    # OSC configuration
    ip = "127.0.0.1"
    network_port = 8888
    use_OSC = True                   # enable to send position data through OSC
    osc_udp_client = None
    
    if use_OSC:
        osc_udp_client = SimpleUDPClient(ip, network_port)

    '''*****************************************************************************************************************'''
    # Manually assign anchor ids and coordiante data for calibration
    # Coordinates (x, y, z) are in mm
    # Coordinates last updated 5/12/18 - see C:\Projects\Pozyx\Documentation\Pozyx_Anchor_Location_Coordinates.xlsx
    anchors = [DeviceCoordinates(0x675a, 1, Coordinates(0, 0, 1300)),    
               DeviceCoordinates(0x674d, 1, Coordinates(3290, -2670, 130)),
               DeviceCoordinates(0x675b, 1, Coordinates(3290, 3510, 2110)),
               DeviceCoordinates(0x6717, 1, Coordinates(-280, 3670, 120))]
    '''*****************************************************************************************************************'''

    # positioning algorithm to use, other is PozyxConstants.POSITIONING_ALGORITHM_TRACKING
    algorithm = PozyxConstants.POSITIONING_ALGORITHM_UWB_ONLY
    # positioning dimension. Others are PozyxConstants.DIMENSION_2D, PozyxConstants.DIMENSION_2_5D
    dimension = PozyxConstants.DIMENSION_3D
    # height of device, required in 2.5D positioning [does not apply for 3D positioning]
    height = 1

    pozyx = PozyxSerial(serial_port)
    r = ReadyToLocalize(pozyx, osc_udp_client, anchors, algorithm, dimension, height, tag_id)
    r.setup()
 
    while True:
        r.loop()

