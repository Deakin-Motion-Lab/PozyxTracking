Pozyx Position Application (pozyxPositioning.exe) - Instructions for use

This application provides a simple user menu interface and allows the user to make a selection of either single or multiple Pozyx tags for:
- positioning (x, y, z) in mm
- orientation (p, r, yaw) in degrees

The application outputs above data to console window + OSC (Open Sound Control) protocol packets, which can be read by other applications, such as Unity3D, etc (see NOTES below).

The application has been designed to be as simple to use as possible, and reads:
- tag data (tag ID in hexadecimal) from 'tagIDs.txt'
- anchor data (anchor ID in hexadecimal, co-ordinates in mm) from 'anchors.txt'

Both files are user editable, however tagIDs.txt should not be modified unless tagIDs are changed or additional tags are introduced to the system.

The user can run the application without needing to install Python / associated libraries, open Python code, nor make direct changes to code.


Operating Procedure:
1) Place Pozyx anchors in desired 3-D space and measure position relative to a point of origin
    - typically, select an anchor as point of origin and measure other anchors relative to this location
    - height (z) is measured individually for each anchor relative to the floor / ground
2) Open 'anchors.txt' file and set / update x, y, z co-ordinates for each anchor as per (1)
    - the application will automatically capture and reflect the updated data when executed
      (ie. if anchor co-ordinates are changed in the 'anchors.txt')
3) Ensure ST Virtual COM driver is installed on PC / Laptop (required for Pozyx to work)
   - Google Drive Link: https://drive.google.com/file/d/1f2j_OHPpYvawyQaeYCNG52dGGsCwoMNI/view?usp=sharing
3) Connect 1 Pozyx device (anchor or a converted tag acting as anchor - see NOTES below) to PC running this application (via USB)
   - If a secondary tag is connected, it is not necessary to add to 'anchors.txt' unless required by the user
4) Connect remaining Pozyx anchors / tag(s) to power
5) Run pozyxPositioning.exe


NOTES:
    - This application can operate a single or multiple tags from a user-defined selection
	- eg: Single
		select desired tag
		select r to run
	- eg: Multiple
		select desired tag
		select second desired tag
		...
		select nth desired tag
		select r to run
    - Ensure Pozyx Tag(s) are connected to power before running application
    - Through the menu system, user can also clear selection and review selection before running Pozyx positioning.
    - TAG -> ANCHOR
        - it is possible to convert a tag into an anchor by removing the T/A Jumper from the tag's circuit board (see Pozyx website for more information)
    	- this can then immediately be used as a "master" Pozyx communication device, connected to a PC / Laptop via USB
            - allows remaining devices (anchors and tag(s) in use) to be remotely connected
            - ie: avoids need for an anchor to be tethered to a PC / Laptop
        - this can also be introduced as an additional anchor for the system
            - measure position relative to origin and add device ID# and x, y, z co-ordinates to 'anchors.txt' file
    - OSC Protocol:
        - IP Address: 127.0.0.1 (local host)
        - Port: 8888
        - set OSC receiver to above IP / Port to read packets (ie. in Unity)