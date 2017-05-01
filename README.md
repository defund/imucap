# IMUCAP

IMUCAP is a motion capture solution to enable the future use of multiplayer VR games.

An IMU tracks accelerometer, gyroscope, and magnetometer data. We can use this data to get the orientation of the object by applying a quaternion filter. Using the built-in Digital Motion Processing (DMP) unit on the MPU itself, we apply the AHRS quaternion filter to apply feedback force to all measurements and get an accurate orientation of the unit.

IMUs are placed next to all joints in the body. With the orientation of each sensing unit, we are able to get a body orientation. Rather than having positional tracking, this method of joint rotational tracking is more effective because it allows for scalable motions on different sized player models.

We plan to create a system for gesture recognition, so that if superhuman abilities are required in a VR game, a natural gesture can be used to accomplish this. 

Here's our [Devpost submission](https://devpost.com/software/vrmc) to HackUMBC Fall 2016.
