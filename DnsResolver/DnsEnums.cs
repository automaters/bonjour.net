using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;

namespace Network.Dns
{
    public enum Qr
    {
        Query,
        Answer
    }

    public enum OpCode
    {
        Query,
        IQuery,
        Status
    }

    public enum ResponseCode
    {
        NoError = 0,
        FormatRequestError = 1,
        ServerError = 2,
        NameNotExisting = 3,
        NotImplemented = 4,
        Denied = 5
    }

    public enum Type
    {
        /// <summary>
        /// Host Address
        /// </summary>
        A = 1,

        /// <summary>
        /// IPv6 Host Address
        /// </summary>
        AAAA = 28,

        /// <summary>
        /// Authoritative Name Server
        /// </summary>
        NS = 2,
        /// <summary>
        /// Mail Destination
        /// </summary>
        [Obsolete("Use MX Instead")]
        MD = 3,

        /// <summary>
        /// Mail Forwarder
        /// </summary>
        [Obsolete("Use MX Instead")]
        MF = 4,

        /// <summary>
        /// Canonical name for an alias
        /// </summary>
        CNAME = 5,

        /// <summary>
        /// Marks the start of a zone of authority
        /// </summary>
        SOA = 6,

        /// <summary>
        /// Mailbox domain name
        /// </summary>
        [Obsolete("Experimental")]
        MB = 7,

        /// <summary>
        /// Mail Group Member
        /// </summary>
        [Obsolete("Experimental")]
        MG = 8,

        /// <summary>
        /// Mail Rename Domain Name
        /// </summary>
        [Obsolete("Experimental")]
        MR = 9,

        /// <summary>
        /// Null RR
        /// </summary>
        [Obsolete("Experimental")]
        NULL = 10,

        /// <summary>
        /// Well Known Service Description
        /// </summary>
        WKS = 11,

        /// <summary>
        /// Domain Name Pointer
        /// </summary>
        PTR = 12,

        /// <summary>
        /// Host Information
        /// </summary>
        HINFO = 13,

        /// <summary>
        /// Mailbox or Mail List Information
        /// </summary>
        MINFO = 14,

        /// <summary>
        /// Mail Exchange
        /// </summary>
        MX = 15,

        /// <summary>
        /// Text Strings
        /// </summary>
        TXT = 16,
        /// <summary>
        /// the location of the server(s) for a specific protocol and domain
        /// </summary>
        SRV = 33
    }

    public enum QType
    {
        /// <summary>
        /// Host Address
        /// </summary>
        A,

        /// <summary>
        /// Authoritative Name Server
        /// </summary>
        NS,
        /// <summary>
        /// Mail Destination
        /// </summary>
        [Obsolete("Use MX Instead")]
        MD,

        /// <summary>
        /// Mail Forwarder
        /// </summary>
        [Obsolete("Use MX Instead")]
        MF,

        /// <summary>
        /// Canonical name for an alias
        /// </summary>
        CNAME,

        /// <summary>
        /// Marks the start of a zone of authority
        /// </summary>
        SOA,

        /// <summary>
        /// Mailbox domain name
        /// </summary>
        [Obsolete("Experimental")]
        MB,

        /// <summary>
        /// Mail Group Member
        /// </summary>
        [Obsolete("Experimental")]
        MG,

        /// <summary>
        /// Mail Rename Domain Name
        /// </summary>
        [Obsolete("Experimental")]
        MR,

        /// <summary>
        /// Null RR
        /// </summary>
        [Obsolete("Experimental")]
        NULL,

        /// <summary>
        /// Well Known Service Description
        /// </summary>
        WKS,

        /// <summary>
        /// Domain Name Pointer
        /// </summary>
        PTR,

        /// <summary>
        /// Host Information
        /// </summary>
        HINFO,

        /// <summary>
        /// Mailbox or Mail List Information
        /// </summary>
        MINFO,

        /// <summary>
        /// Mail Exchange
        /// </summary>
        MX,

        /// <summary>
        /// Text Strings
        /// </summary>
        TXT,
        /// <summary>
        /// A request for a transfer of an entire zone
        /// </summary>
        AXFR = 252,
        /// <summary>
        /// A request for mailbox-related records (MB, MG or MR)
        /// </summary>
        MAILB = 253,
        /// <summary>
        /// A request for mail agent RRs
        /// </summary>
        [Obsolete("Use MX Instead")]
        MAILA = 254,
        /// <summary>
        /// A request for all records
        /// </summary>
        ALL = 255
    }

    public enum Class
    {
        /// <summary>
        /// the Internet
        /// </summary>
        IN = 1,
        /// <summary>
        /// the CSNET class
        /// </summary>
        [Obsolete("used only for examples in some obsolete RFCs", true)]
        CS = 2,
        /// <summary>
        /// the CHAOS class
        /// </summary>
        CH = 3,
        /// <summary>
        /// Hesiod [Dyer 87]
        /// </summary>
        HS = 4
    }

    public enum QClass
    {
        /// <summary>
        /// the Internet
        /// </summary>
        IN = 1,

        /// <summary>
        /// the CSNET class
        /// </summary>
        [Obsolete("used only for examples in some obsolete RFCs", true)]
        CS = 2,
        /// <summary>
        /// the CHAOS class
        /// </summary>
        CH = 3,
        /// <summary>
        /// Hesiod [Dyer 87]
        /// </summary>
        HS = 4,
        /// <summary>
        /// Any class
        /// </summary>
        ALL = 255
    }
}
