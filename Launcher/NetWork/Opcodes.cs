
 
using System;

public enum Opcodes : byte
{
    CL_CHECK    = 0x01,   // The client sends its version for verification (version of the launcher, start verif file)
    LCR_CHECK   = 0x02,   // Server reference the answer and disconnect if necessary.

    CL_START    = 0x03,   // The client requests permission to start. (Username + sha256 pass)
    LCR_START   = 0x04,   // The reference server leave or not to start the client, the client will launch with the server info

    CL_CREATE   = 0x05,   // The client requests to create an account (Username + Pass + IP);
    LCR_CREATE  = 0x06,   // Server response to customer's request (answer in the message string + bool error)

    CL_INFO     = 0x07,   // The client requests information on the realms
    LCR_INFO    = 0x08,   // Response from the server over the kingdoms?
};

public enum CheckResult : byte
{
    LAUNCHER_OK = 0x00,   // The Launcher is ok
    LAUNCHER_VERSION = 0x01,   // Wrong version of the launcher
    LAUNCHER_FILE = 0x02,   // Missing file in the launcher
};