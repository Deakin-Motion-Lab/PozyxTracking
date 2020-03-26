'''
Python script to automatically install Pozyx and OSC related Python libraries to operate POZYX system

NOTE: if an individual module fails to install correctly, the following libraries are required to be installed.
        o	pip install pypozyx
        o	pip install python-osc
        o	pip install requests
        o	pip install pyserial

Written by P. Hammond, 11/12/2018
'''

import subprocess
from time import sleep

def Install(package):
    subprocess.call(['pip', 'install', package])

def Main():
    # Packages to be installed
    packages = ['pypozyx', 'python-osc', 'requests', 'pyserial']

    # Display instructions for user
    print("This program will automatically install the following Pozyx and OSC related Python libraries / modules.")
    print("------------------------------")
    print("o	pip install pypozyx")
    print("o	pip install python-osc")
    print("o	pip install requests")
    print("o	pip install pyserial")
    print("------------------------------")
    sleep(3)
    print("PLEASE NOTE: Internet access is required")
    sleep(3)
    print("\nNOTE: if an individual module fails to install correctly, at the prompt, enter: \t\'pip install <PACKAGE>\'")
    print("Eg. C:\> pip install pypozyx\n")
    sleep(3)

    # Install packages
    for package in packages:
        Install(package)
        sleep(2)

Main()
    
