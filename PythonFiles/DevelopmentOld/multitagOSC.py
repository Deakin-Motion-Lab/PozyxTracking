"""
[PH]: This program builds upon "ready_to_localize.py" and "multitag_positioning.py"from Pozyx Tutorials 2 and 4,
      extracting Position Data (x, y, z) and Orientation Data (p, r, y) from multiple Pozyx tags.

      Extracted data is outputted to console plus transmitted to local host via OSC protocol, which can be
      accessed with other applications such as Visual Studio and Unity.

      Pozyx tags transmit data on a TDMA basis (ie. each tag transmits its data "in-turn") according to their
      position in the "tag_ids" array

      Requires 4 x anchors + > 1 tags

      IMPORTANT: This script must be run PRIOR to running the corresponding scene in Unity.
"""

from time import sleep

from pypozyx import (PozyxConstants, Coordinates, SensorData, POZYX_SUCCESS, PozyxRegisters, version,
                     DeviceCoordinates, PozyxSerial, get_first_pozyx_serial_port, SingleRegister)
from pythonosc.udp_client import SimpleUDPClient

from pypozyx.tools.version_check import perform_latest_version_check


class MultitagPositioning(object):
    """Continuously performs multitag positioning"""
    def __init__(self, pozyx, osc_udp_client, tag_ids, anchors, algorithm=PozyxConstants.POSITIONING_ALGORITHM_UWB_ONLY,
                 dimension=PozyxConstants.DIMENSION_3D, height=1000):
        self.pozyx = pozyx
        self.osc_udp_client = osc_udp_client
        self.tag_ids = tag_ids
        self.anchors = anchors
        self.algorithm = algorithm
        self.dimension = dimension
        self.height = height

    def setup(self):
        """Sets up the Pozyx for positioning by calibrating its anchor list."""
        print("------------POZYX MULTITAG POSITIONING V{} -------------".format(version))
        print("")
        print(" - System will manually calibrate the tags")
        print("")
        print(" - System will then auto start positioning")
        print("")
        if None in self.tag_ids:
            for device_id in self.tag_ids:
                self.pozyx.printDeviceInfo(device_id)
        else:
            for device_id in [None] + self.tag_ids:
                self.pozyx.printDeviceInfo(device_id)
        print("")
        print("------------POZYX MULTITAG POSITIONING V{} -------------".format(version))
        print("")

        self.setAnchorsManual(save_to_flash=False)
        self.printPublishAnchorConfiguration()

    def loop(self):
        """ Performs positioning and prints the results. """
        for tag_id in self.tag_ids:
            position = Coordinates()
            status = self.pozyx.doPositioning(
                position, self.dimension, self.height, self.algorithm, remote_id=tag_id)
            if status == POZYX_SUCCESS:
                self.printPublishPosition(position, tag_id)
            else:
                self.printPublishErrorCode("positioning", tag_id)

    """ [PH]: Prints the Pozyx's position to console and outputs a OSC packet """
    def printPublishPosition(self, position, network_id):
        if network_id is None:
            network_id = 0

        # Get Orientation Data
        orientationValues = self.GetOrientation(network_id)

        # Print output to console
        print("POS ID {}, x(mm): {pos.x} y(mm): {pos.y} z(mm): {pos.z}, orientation: {ori[0]} {ori[1]} {ori[2]}".format(
            "0x%0.4x" % network_id, pos=position, ori=orientationValues))

        # Transmit output to OSC
        if self.osc_udp_client is not None:
            self.osc_udp_client.send_message("/position", [network_id, int(position.x), int(position.y), int(position.z),
                                                           orientationValues[0], orientationValues[1], orientationValues[2]])


    def setAnchorsManual(self, save_to_flash=False):
        """ Adds the manually measured anchors to the Pozyx's device list one for one. """
        for tag_id in self.tag_ids:
            status = self.pozyx.clearDevices(tag_id)
            for anchor in self.anchors:
                status &= self.pozyx.addDevice(anchor, tag_id)
            if len(anchors) > 4:
                status &= self.pozyx.setSelectionOfAnchors(PozyxConstants.ANCHOR_SELECT_AUTO, len(anchors),
                                                           remote_id=tag_id)
            # enable these if you want to save the configuration to the devices.
            if save_to_flash:
                self.pozyx.saveAnchorIds(tag_id)
                self.pozyx.saveRegisters([PozyxRegisters.POSITIONING_NUMBER_OF_ANCHORS], tag_id)

            self.printPublishConfigurationResult(status, tag_id)

    def printPublishConfigurationResult(self, status, tag_id):
        """ Prints the configuration explicit result, prints and publishes error if one occurs """
        if tag_id is None:
            tag_id = 0
        if status == POZYX_SUCCESS:
            print("Configuration of tag %s: success" % tag_id)
        else:
            self.printPublishErrorCode("configuration", tag_id)

    def printPublishErrorCode(self, operation, network_id):
        """ Prints the Pozyx's error and possibly sends it as a OSC packet """
        error_code = SingleRegister()
        status = self.pozyx.getErrorCode(error_code, network_id)
        if network_id is None:
            network_id = 0
        if status == POZYX_SUCCESS:
            print("Error %s on ID %s, %s" %
                  (operation, "0x%0.4x" % network_id, self.pozyx.getErrorMessage(error_code)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message(
                    "/error_%s" % operation, [network_id, error_code[0]])
        else:
            # should only happen when not being able to communicate with a remote Pozyx.
            self.pozyx.getErrorCode(error_code)
            print("Error % s, local error code %s" % (operation, str(error_code)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message("/error_%s" % operation, [0, error_code[0]])

    def printPublishAnchorConfiguration(self):
        for anchor in self.anchors:
            print("ANCHOR,0x%0.4x,%s" % (anchor.network_id, str(anchor.pos)))
            if self.osc_udp_client is not None:
                self.osc_udp_client.send_message(
                    "/anchor", [anchor.network_id, anchor.pos.x, anchor.pos.y, anchor.pos.z])
                sleep(0.025)

    """ [PH]: Extract Orientation data from Pozyx sensor """
    def GetOrientation(self, tag_id):
        # Get Orientation data from Pozyx sensor
        orientation = SensorData().euler_angles
        self.pozyx.getEulerAngles_deg(orientation, tag_id)

        # Extract ornetation data (pitch, roll, yaw)
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

    #[PH]: OSC configuration
    ip = "127.0.0.1"
    network_port = 8888
    use_OSC = True                   # enable to send position data through OSC
    osc_udp_client = None
    
    if use_OSC:
        osc_udp_client = SimpleUDPClient(ip, network_port)

    # -----------------------------------------------------------------------------------------------------------------------------------------
    """ [PH]: Manually assign TAG IDs (hexadecimal value) for remote positioning """
    # NOTES:
    # If a local tag is connected (ie. to PC via USB), change its tag id to "None" 
    #     eg: tag_ids = [None, 0x6720, 0x6729]   # TDMA order: Local, 0x6720, 0x6729
    #     *Locally connected tag may not transmit orientation data
    tag_ids = [0x0001, 0x0002]
    #tag_ids = [0x6720, 0x6729, 0x6735]    # 3 x Tags

    """ [PH]: Manually assign ANCHOR IDs and coordinate data for calibration """
    # Coordinates (x, y, z) are in mm
    # Coordinates last updated 15/11/18
    #    refer C:\Projects\Pozyx\Anchor Location Coordinates\Pozyx_Anchor_Location_Coordinates.xlsx
    anchors = [DeviceCoordinates(0x675a, 1, Coordinates(0, 0, 1675)),
               DeviceCoordinates(0x674d, 1, Coordinates(7000, -1810, 1175)),
               DeviceCoordinates(0x675b, 1, Coordinates(7000, 2190, 1990)),
               DeviceCoordinates(0x6717, 1, Coordinates(0, 2190, 2225))]

    # -----------------------------------------------------------------------------------------------------------------------------------------

    # positioning algorithm to use, other is PozyxConstants.POSITIONING_ALGORITHM_TRACKING
    algorithm = PozyxConstants.POSITIONING_ALGORITHM_UWB_ONLY
    # positioning dimension. Others are PozyxConstants.DIMENSION_2D, PozyxConstants.DIMENSION_2_5D
    dimension = PozyxConstants.DIMENSION_3D
    # height of device, required in 2.5D positioning
    height = 1000

    pozyx = PozyxSerial(serial_port)

    r = MultitagPositioning(pozyx, osc_udp_client, tag_ids, anchors, algorithm, dimension, height)
    r.setup()
    while True:
        r.loop()
