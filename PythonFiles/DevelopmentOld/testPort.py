'''
Basic program to identify the COM port id connecting the pozyx device to local machine

Paul Hammond, 7/11/18
'''

from pypozyx import *

print("Locally connected Pozyx device on: " + get_first_pozyx_serial_port())
