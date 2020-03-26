"""
[PH]: This class opens 'tagIDs.txt' and 'anchors.txt' and extracts appropriate data for use with the pozyxSingleTag.py and pozyxMultiTags.py programs.

      A menu is presented to user to select the appropriate Pozyx Tag(s) for use

      NOTE: if tags / anchors are added / removed / renamed, the menu will automatically update to reflect those changes
            
      Requires 4 x anchors + 1 (or more) tag(s)

      Written by P.Hammond, 12/12/2018

      Modified by P.Hammond, 17/12/2018
"""
import sys
import os

class PozyxHardware():
    def __init__(self):
        self.tags = []
        self.anchors = {}
        self.fileData = []
        self.tagChoices = []
    
    def OpenFile(self, path):
        # Attempt to Open File and read data from file
        try:
            f = open(path, 'r')
            self.fileData = f.readlines()
            f.close()
        except:
            print("\n*** Error: could not access " + path + ".  Please ensure file is available in local directory ***\n\n")
            errorInput = input("Press \'ENTER\' to exit...")
            sys.exit()

    def ExtractTagData(self):
        # Extract tagIDs from file (ignore comments / blank lines)
        for line in self.fileData:
            if not line.startswith("0"):
                continue
            else:
                self.tags.append(line)
       
    def GetTags(self, selection):
        selectedTags = []
        # Iterate through selection list
        for selectedItem in selection:
            # adjust menu selection to match array indexing (ie 1 = index 0, 2 = index 1, etc)
            index = int(selectedItem) - 1

            # get tag and convert to hexadecimal number
            selectedTags.append(int(self.tags[index], 16))
        
        return selectedTags

    def DrawMenu(self):
        count = 1
        # Display Menu Options for user
        print("\n====================================")
        print("Select Pozyx Tag(s) to use:\n")
        for tag in self.tags:
            print(" ({0}) TagID# {1}" .format(count, tag))
            self.tagChoices.append(str(count))
            count += 1
        print("====================================")
        print(" (r) RUN Pozyx Positioning Operation")
        print("====================================")
        print("Additional Options:")
        print("\n (s) Show Selected Tags")
        print("\n (c) Clear Selection")
        print("\n (x) Exit Application")
        print("====================================")

    def ClearScreen(self):
        os.system('cls' if os.name == 'nt' else 'clear')
        
    def SelectTags(self):
        path = 'tagIDs.txt'
        selectedTags = []
        
        # Open tag file and extract tag data
        self.OpenFile(path)
        self.ExtractTagData()

        # User Selection Operations
        self.DrawMenu()
        while (True):
            selection = input("\nEnter selection:> ") 
            # Check if valid selection
            # Add tags to selection list / array
            if selection in self.tagChoices:
                if not selection in selectedTags:
                    selectedTags.append(selection)
                    print("TagID# {0} added" .format(selection))
                else:
                    print("TagID# {0} already selected" .format(selection))
            # Run Positioning
            elif (selection == "r"):
                # Check at least 1 tag selected
                if len(selectedTags) == 0:
                    print("\nYou have not selected any tags...please select at least one tag from menu!")
                else:
                    print("\nRunning positioning...")
                    break
            # Clear Selection
            elif (selection == "c"):
                totalElements = len(selectedTags)
                selectedTags = []
                print("\nTag selection cleared")
            # Show Selection (contents of list / array)
            elif (selection == "s"):
                if (len(selectedTags) == 0):
                    print("\nYou have not selected any tags...please select at least one tag from menu!")
                else:
                    print("Current selection: ")
                    print(selectedTags)
            # Exit Application
            elif (selection == "x"):
                sys.exit()
            else:
                print("Invalid selection...please select from MENU items")

        return self.GetTags(selectedTags)

    def ConvertArray(self, stringArray):
        intArray = []

        # Create an integer array, convert string values into integers and store in new array
        for val in stringArray:
            intArray.append(int(val))

        return intArray
    
    def ExtractAnchorData(self):
        # Extract anchor IDs (hexadecimal values) and (x, y, z) coordinates (integer values)
        for line in self.fileData:
            if not line.startswith("0"):
                continue
            else:
                arrayLine = line.split(',')
                keyItem = int(arrayLine[0],16)                               # separate first element (anchorID) and convert string to hex number
                del arrayLine[0]                                             # remove first element from array (this is the key)
                self.anchors[keyItem] = self.ConvertArray(arrayLine)         # convert remaining array values (co-ordinates) from string to integer and assign to dictionary under anchorID (key)
                
    def GetAnchors(self):
        path = 'anchors.txt'

        self.OpenFile(path)
        self.ExtractAnchorData()

        return self.anchors
        
    
        

            
