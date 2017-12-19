# IMUCAP

IMUCAP is a motion capture solution to enable the future use of multiplayer VR games.

An IMU tracks accelerometer, gyroscope, and magnetometer data. We can use this data to get the orientation of the object by applying an AHRS quaternion filter.

IMUs are placed next to all joints of the body. We are able model it entirely with the orientation from each sensing unit. Rather than having positional tracking, this method of joint rotational tracking is more effective because it allows for scalable motions on different sized player models.

We plan to create a system for gesture recognition, so that if superhuman abilities are required in a VR game, a natural gesture can be used to accomplish this. 

Here's our [Devpost submission](https://devpost.com/software/vrmc) to HackUMBC Fall 2016.
