# Primer

## What is an IoT device?

[Graphic: highlighting where we are in the architecture]

An IoT device is a physical device that typically collects and shares data with a cloud-based service. In some scenarios, IoT devices can also receive and act upon commands sent from a cloud-based service. An IoT device typically consists of:

- One or more sensors that enable it to collect data.
- A networking connectivity component that enables it to share the data it collects and possibly receive commands from a service.
- Optionally, some mechanism that enables the device to control the hardware to which it is connected.

Example scenarios where IoT devices may be used include:

[Graphic: turn this list into a graphic?]

- Trip tracking and car health
- Sports and fitness tracking
- Health monitoring
- Person or pet or livestock tracking
- Physical object tracking
- Beacons or proximity sensors
- Smart vending machines
- Smart appliances
- Home automation including security monitors
- Environmental monitoring such as pollution sensors
- Industrial equipment monitoring for predictive maintenance
- Manufacturing process monitoring devices
- Asset tracking

As you can see from the previous list, there are a wide variety of IoT scenarios where you need IoT devices to collect and share data and in some cases act on commands sent from a service. Despite this variety, IoT devices typically share some of the following characteristics:

| Device characteristic | Example |
| ----------------------| ------- |
| Often embedded systems with no human operator. | A smart vending machine tracks stock levels and automatically requests refills. |
| Can be in remote locations where physical access is very expensive. | A sensor attached to a pipe in a remote oil pumping installation. |
| May only be reachable through the solution back end. | An aircraft engine monitoring device may only be reachable from the monitoring service. |
| May have limited power and processing resources. | A health monitoring band worn on the wrist. |
| May have intermittent, slow, or expensive network connectivity. | A on-board diagnostics device in a car has no network access when there is no cellular coverage or when the car is in a tunnel. |
| May need to use proprietary, custom, or industry-specific application protocols. | Cars typically expose on-board diagnostics using one of the OBD protocols. |
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

[Graphic: Highlight opaque v. transparent]

> Note: While you typically deploy a field gateway that is local to your devices, in some scenarios, you might deploy a protocol translation gateway in the cloud.

# Devices in our solution: OBD

## What is OBD and does it do?

## What devices did we use and why?

Describe the particular devices we used and why.

# The field gateway in our solution

## Decision point: why did we choose the phone

What other devices would work with this solution? Discuss some alternatives

## Resources - other ways to connect your OBD device to the internet

Reference a DIY project to connect to an OBD

# How data gets from here to the backend

## Device requirements for working with IoT Hub

In this sample, the cellular phone in the car acting as a field gateway shares the data from the OBD with the back end service by sending it to an IoT Hub endpoint hosted in Azure. The next chapter describes the IoT Hub service and its role in this solution. For now, you can think of IoT Hub as the entry point for data from your devices into the cloud-hosted back end.

To communicate with IoT Hub directly, an IoT device or field gateway must use one of the secure protocols supported by IoT Hub:

| Protocol | Port(s) |
| -------- | ------- |
| HTTPS    | 443     |
| AMQP     | 5671    |
| AMQP over WebSockets | 443    |
| MQTT | 8883 |

> Note: These are all secure protocols that encrypt the data the device exchanges with IoT Hub. If your IoT device cannot use one of these protocols, you must use a protocol translation gateway.

# How does the OBD device communicate with IoT Hub?

Choice of protocols, SDKs and libraries, field gateway implementation, encryption.