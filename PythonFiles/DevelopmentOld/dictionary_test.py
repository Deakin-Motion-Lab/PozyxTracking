# Testing dictionary functionality

from File_IO import File

f = File().GetAnchors()

print(str(f))

for key in f:
    print(hex(key))

