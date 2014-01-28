
using System;

enum Opcodes : ushort
{
    CMSG_VerifyProtocolReq          = 0x01,
    SMSG_VerifyProtocolReply        = 0x02,
    CMSG_AuthInitialTokenReq        = 0x03,
    SMSG_AuthInitialTokenReply      = 0x04,
    CMSG_AuthSessionTokenReq        = 0x05,
    SMSG_AuthSessionTokenReply      = 0x06,
    CMSG_GetCharSummaryListReq      = 0x07,
    SMSG_GetCharSummaryListReply    = 0x08,
    CMSG_GetClusterListReq          = 0x09,
    SMSG_GetClusterListReply        = 0x0a,
    CMSG_GetAcctPropListReq         = 0x0d,
    SMSG_GetAcctPropListReply       = 0x0c,
    CMSG_MetricEventNotify          = 0x0b,
    MAX_OPCODE                      = 0x0b
   
};