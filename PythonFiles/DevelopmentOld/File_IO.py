"""
[PH]: This class opens 'tagIDs.txt' and 'anchors.txt' and extracts appropriate data for use with the pozyxSingleTag.py program.

      A menu is presented to user to select the appropriate Pozyx Tag for use
            
      Requires 4 x anchors + 1 x tag

      Written by P.Hammond, 12/12/2018
"""
class PozyxHardware():
    def __init__(self):
        self.tags = []
        self.anchors = {}
        self.fileData = []
    
    def OpenFile(self, path):
        # Attempt to Open File and read data from file
        try:
            f = open(path, 'r')
            self.fileData = f.readlines()
            f.close()
        except:
            print("\n*** Error: could not access " + path + ".  Please ensure file is available in local directory ***\n\n")
            errorInput = input("Press any key to exit...")

    def ExtractTagData(self):
        # Extract tagIDs (ignore comments)
        for line in self.fileData:
            if not line.startswith("0"):
                continue
            else:
                self.tags.append(line)
       
    def GetTag(self, selection):
        # adjust menu selection to match array indexing (ie 1 = index 0, 2 = index 1, etc)
        index = int(selection) - 1

        # get tag number, convert to hexadecimal, and return
        return int(self.tags[index], 16)

    def SelectTag(self):
        path = 'tagIDs.txt'

        # Open tag file and extract tag data
        self.OpenFile(path)
        self.ExtractTagData()

        # Display Menu Options for user
        while (True):
            print("\n\nSelect Pozyx Tag to use:\n")
            print(" (1) TagID# {0}" .format(self.tags[0]))
            print(" (2) TagID# {0}" .format(self.tags[1]))
            print(" (3) TagID# {0}" .format(self.tags[2]))
            print(" (4) TagID# {0}" .format(self.tags[3]))
            print(" (5) TagID# {0}" .format(self.tags[4]))

            selection = input("\nEnter selection (1 to 5):> ") 
            if (selection == "1" or selection == "2" or selection == "3" or selection == "4" or selection == "5"):
                break
            else:
                print("\nInvalid selection...please try again\n\n")
            
        return self.GetTag(selection)

    def ConvertArray(self, stringArray):
        intArray = []

        # Create an integer array, convert string values into integers and store in new array
        for val in stringArray:
            intArray.append(int(val))

        return intArray
    
    def ExtractAnchorData(self):
        # Extract anchor ids (hexadecimal values) and (x, y, z) coordinates (integer values)
        for line in self.fileData:
            if not line.startswith("0"):
                continue
            else:
                arrayLine = line.split(',')
                keyItem = int(arrayLine[0],16)                               # separate first element (anchorID) and convert string to hex number
                del arrayLine[0]                                             # remove first element from array (this is the key)
                self.anchors[keyItem] = self.ConvertArray(arrayLine)         # convert remaining array values (co-ords) from string to integer and assign to dictionary under anchorID (key)
                
    def GetAnchors(self):
        path = 'anchors.txt'

        self.OpenFile(path)
        self.ExtractAnchorData()

        return self.anchors
        
    
        

            
