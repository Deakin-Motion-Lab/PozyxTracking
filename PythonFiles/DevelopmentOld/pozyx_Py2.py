'''
Python script created to test MQTT connection protocol and Pozyx

This program establishes an MQTT connection with the Pozyx Application via local host,
and requires the Pozyx Application to be operating during execution

Paul Hammond, 1/11/2018
'''

# NOTE: Execute Pozyx application prior to running this script

import paho.mqtt.client as mqtt
import ssl

host = "localhost"
port = 1883
topic = "tagsLive" 

def on_connect(client, userdata, flags, rc):
    print(mqtt.connack_string(rc))

# callback triggered by a new Pozyx data packet
def on_message(client, userdata, msg):
    print("Positioning update:", msg.payload.decode())

def on_subscribe(client, userdata, mid, granted_qos):
    print("Subscribed to topic!")

client = mqtt.Client()

# set callbacks
client.on_connect = on_connect
client.on_message = on_message
client.on_subscribe = on_subscribe
client.connect(host, port=port)
client.subscribe(topic)

# works blocking, other, non-blocking, clients are available too.
client.loop_forever()

