from pypozyx import (POZYX_POS_ALG_UWB_ONLY, POZYX_3D, Coordinates, SensorData, POZYX_SUCCESS, PozyxConstants, version,
                     DeviceCoordinates, PozyxSerial, get_first_pozyx_serial_port, SingleRegister, DeviceList, PozyxRegisters)
from pythonosc.udp_client import SimpleUDPClient

from time import sleep

class MyPozyx():
    def __init__(self, pozyx, osc_udp_client, anchors, algorithm=POZYX_POS_ALG_UWB_ONLY, dimension=POZYX_3D, height=1000, remote_id=None):
        self.pozyx = pozyx
        self.osc_udp_client = osc_udp_client
        self.anchors = anchors
        self.algorithm = algorithm
        self.dimension = dimension
        self.height = height
        self.remote_id = remote_id

    def loop(self):
        self.printOrientation()
        sleep(0.5)
        
    def printOrientation(self):
        # Get Orientation data from Pozyx sensor
        orientation = SensorData().euler_angles
        self.pozyx.getEulerAngles_deg(orientation, self.remote_id)

        # Extract key data (pitch, roll, yaw)
        orientationData = str(orientation).split(',')
        yaw = orientationData[0].lstrip("Heading: ")
        roll = orientationData[1].lstrip("Roll: ")
        pitch = orientationData[2].lstrip("Pitch: ")
        
        #print("%s, %s, %s" %(pitch, roll, yaw))
        
        return "%s, %s, %s" %(pitch, roll, yaw)


# configure if you want to route OSC to outside your localhost. Networking knowledge is required.
use_processing = True
ip = "127.0.0.1"
network_port = 8888

osc_udp_client = None

remote_id = 0x6720                # remote device network ID
remote = True                    # whether to use a remote device
if not remote:
    remote_id = None
        
if use_processing:
    osc_udp_client = SimpleUDPClient(ip, network_port)
        
serial_port = get_first_pozyx_serial_port()
pozyx = PozyxSerial(serial_port)

# positioning algorithm to use, other is PozyxConstants.POSITIONING_ALGORITHM_TRACKING
algorithm = PozyxConstants.POSITIONING_ALGORITHM_UWB_ONLY
# positioning dimension. Others are PozyxConstants.DIMENSION_2D, PozyxConstants.DIMENSION_2_5D
dimension = PozyxConstants.DIMENSION_3D
# height of device, required in 2.5D positioning
height = 1000

anchors = [DeviceCoordinates(0x675a, 1, Coordinates(0, 0, 2050)),
               DeviceCoordinates(0x674d, 1, Coordinates(7000, 0, 1175)),
               DeviceCoordinates(0x675b, 1, Coordinates(7000, 4000, 2095)),
               DeviceCoordinates(0x6717, 1, Coordinates(0, 4000, 1855))]

p = MyPozyx(pozyx, osc_udp_client, anchors, algorithm, dimension, height, remote_id)
'''
while(True):
    p.loop()
'''
mystring = p.printOrientation()

print(mystring)
