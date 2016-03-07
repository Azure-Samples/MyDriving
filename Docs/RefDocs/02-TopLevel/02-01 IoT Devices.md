# Primer

## What is an IoT device?

![Architecture: IoT Device](media/device-architecture.png "IoT device role in architecture")

An IoT device is a physical device that collects data from one or more sensors and shares that data with a cloud-based service. For example, a simple device might use a temperature sensor to collect the temperature in the environment and send that data every second to a cloud-based monitoring system. In some scenarios, IoT devices receive and act upon commands sent from a cloud-based service. For example, a cloud-based monitoring system might send a command to a device telling it to open a valve.

Example scenarios where IoT devices may be used include:

- Trip tracking and car health
- Sports and fitness tracking
- Health monitoring
- Person or pet or livestock tracking
- Smart appliances
- Home automation including security monitors
- Physical object tracking
- Beacons or proximity sensors
- Smart vending machines
- Environmental monitoring such as pollution sensors
- Asset tracking
- Industrial automation controllers
- Industrial equipment monitoring for predictive maintenance
- Manufacturing process monitoring devices

As you can see from the previous list, there are a wide variety of IoT scenarios where you need IoT devices to collect and share data and in some cases act on commands sent from a service. Despite the variety of scenarios, IoT devices typically share some of the following characteristics:

| Device characteristic | Example |
| ----------------------| ------- |
| Often embedded systems with no human operator. | A smart vending machine tracks stock levels and automatically requests refills. |
| Typically a special-purpose device | Unlike a phone or a tablet, and IoT device usually has a spefic function, such as reporting the temperature in the environment |
| Can be in remote locations where physical access is very expensive. | A sensor attached to a pipe in a remote oil pumping installation. |
| May only be reachable through the solution back end. | An aircraft engine monitoring device may only be reachable from the monitoring service. |
| May have limited power and processing resources. | A health monitoring band worn on the wrist. |
| May have intermittent, slow, or expensive network connectivity. | A on-board diagnostics device in a car has no network access when there is no cellular coverage or when the car is in a tunnel. |
| May need to use proprietary, custom, or industry-specific application protocols. | Cars typically expose on-board diagnostics using one of the OBD protocols. Industrial automation controllers may use protocols such as DeviceNet, PROFIBUS-DP, or CAN |
| Can be created from a large set of popular hardware and software platforms. | Examples include Raspberry Pi, Arduino, or Beaglebone devices. |
| May only send data or may also receive data from a service (typically in the cloud). | A car on-board diagnostics system only sends telemetry to the back end system, whereas a home automation system reports information about the home as well as enabling you to control lights and temperature remotely. |
| May send or receive sensitive data that requires a secure communication channel. | A person tracking system for children should only allow parents or other designated individuals access to information about a child's location. |

## What is a field gateway?

Many IoT solutions include a field gateway device that sits between the IoT devices and the service they communicate with, and is typically located close to your devices. Field gateways are often used to enable connectivity and protocol translation for devices that either cannot or should not connect directly to the internet. An example of a device that cannot connect to the internet is one that only supports the Bluetooth communication protocol. An example of a device that should not connect to the internet is one that us unable to use a secure protocol when it connects to the service.

A field gateway differs from a simple traffic routing device (such as a network address translation (NAT) device or firewall) because it typically performs an active role in managing access and information flow in your solution. For example, a field gateway may:

- Manage local devices. For example, a field gateway could perform event rule processing and send commands to devices in response to specific telemetry data.
- Filter or aggregate telemetry data before it forwards it to the service. This can reduce the amount of data that is sent to the service and potentially reduce costs in your solution.
- Help to provision devices.
- Transform telemetry data to facilitate processing in your solution back end.
- Perform protocol translation to enable devices to communicate with your service when your devices do not use the transport protocols that your service supports.

We often characterize field gateways as being *transparent* or *opaque*. A transparent gateway forwards messages to the back end service leaving the original device id intact, which means that the service is aware of all the connected IoT devices. An opaque gateway forwards data to the back end service using its own id, which means that the service is only aware of the field gateway as a connected device.

![Transparent field gateway](media/field-gateway-transparent.png "Transparent field gateway")
![Opaque field gateway](media/field-gateway-opaque.png "Opaque field gateway")

> Note: While you typically deploy a field gateway that is local to your devices, in some scenarios, you might deploy a protocol translation gateway in the cloud.

# Devices in our solution: OBD

The SmartKar [TODO change name of app] solution collects On-board Diagnostics (OBD) data from your car to send to the solution back end for analysis. Modern cars have a standard OBD-II Data Link Connector somewhere in the cabin that enables you plug in a OBD-II device that reads the OBD data and makes it available to other devices over USB, Bluetooth or a local WiFi network.

## What is OBD and does it do?

Summarize the types of data that ODB can collect and forward.
Callout any potential security issues as a disclaimer here.

## What devices did we use and why?

Describe the particular devices we used and why.

# The field gateway in our solution

## Decision point: why did we choose the phone

What other devices would work with this solution? Discuss some alternatives.
Suggest some possible command and control scenarios - hard breaking, activate dashcam, engine fault codes adjust route to pass by garage for maintenance.

## Resources - other ways to connect your OBD device to the internet

The current solution uses a cellular phone as the field gateway. The phone acts as protocol translator to convert the OBD-II data sent to the phone by the OBD device over Bluetooth or WiFi into AMQP [TODO - check this is protocol that is used] messages to forward to IoT Hub.

It is possible to connect some OBD devices directly to the internet rather than relying on the phone for connectivity. For example, the [Freematics ONE][lnk-freematics-one] OBD device has an xBee socket for connecting wireless communications modules such as GSM or WiFi. This particular device can also connect to a GPS receiver and has various on-board sensors such as a gyroscope and accelerometer.

This device is compatible with an Arduino UNO enabling you to program the device directly and customize the OBD and sensor data sent to IoT Hub.

# How data gets from a device to the backend

## Device requirements for working with IoT Hub

In this sample, the cellular phone in the car acting as a field gateway shares the data from the OBD with the back end service by sending it to an IoT Hub endpoint hosted in Azure. The next chapter describes the IoT Hub service and its role in this solution. For now, you can think of IoT Hub as the entry point for data from your devices into the cloud-hosted back end.

To communicate with IoT Hub directly, an IoT device or field gateway must use one of the secure protocols supported by IoT Hub:

| Protocol | Port(s) |
| -------- | ------- |
| HTTPS    | 443     |
| AMQPS     | 5671    |
| AMQPS over WebSockets | 443    |
| MQTT | 8883 |

> Note: These are all secure protocols that encrypt the data the device exchanges with IoT Hub. If your IoT device cannot use one of these protocols, you must use a protocol translation gateway.

## Decision point: which protocol do we use to communicate with IoT Hub?
[TODO add discussion of different IoT protocols - HTTP, AMQP, MQTT]

There are four key considerations in the choice of protocol for the device connecting to IoT Hub. Only the first two are relevant in this solution, and led us to choose the AMQPS protocol [TODO - check this, this is my assumption].

- **Cloud-to-device pattern**. HTTP does not have an efficient way to implement server push. As such, when using HTTP, devices must poll IoT Hub for cloud-to-device messages. This is very inefficient for both the device and IoT Hub and introduces latency in command delivery to a device. Although the current solution does not send commands to the device, a possible extension is to to send a command to switch on a dash-cam if sudden breaking is detected. To minimize latency in delivering commands, you should use the AMQPS or MQTT protocol.

- **Payload size**. AMQPS and MQTT are binary protocols, which are significantly more compact than HTTP. Using AMQPS or MQTT will help to minimize any charges that arise from the phone's connection to IoT Hub.

- **Field gateways**. When using HTTP or MQTT, you cannot connect multiple devices (each with its own per-device credentials) using the same TLS connection. It follows that these protocols are suboptimal when implementing a field gateway, because they require one TLS connection between the field gateway and IoT Hub for each device connected to the gateway. However, in the current solution there is only one OBD device per phone, so there will be only one TLS connection to IoT Hub.

- **Low resource devices**. The MQTT and HTTP libraries have a smaller footprint than the AMQP libraries. As such, if the device has few resources (for example, less than 1Mb RAM), these protocols might be the only protocol implementation available. However, modern smart phones typically have sufficient RAM to use the AMQPS protocol.

- **Network traversal**. MQTT uses port 8883. This could cause problems in networks that are closed to non-HTTP protocols. You can use both HTTPS and AMQPS over WebSockets in this scenario. However, this is unlikely to be an issue over the public network used by the smart phone.

# How does the OBD device communicate with the field gateway?

Choice of protocols, SDKs and libraries, field gateway implementation, encryption.
Relevant code walkthroughs here.
OBD to phone
Phone to IoT Hub