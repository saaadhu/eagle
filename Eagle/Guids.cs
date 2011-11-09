// Guids.cs
// MUST match guids.h
using System;

namespace SenthilKumarSelvaraj.Eagle
{
    static class GuidList
    {
        public const string guidEaglePkgString = "4216293a-83e5-4341-8bfb-c4e40928a4b6";
        public const string guidEagleCmdSetString = "ee97015b-2500-4d77-a82f-caf73382be6e";

        public static readonly Guid guidEagleCmdSet = new Guid(guidEagleCmdSetString);
    };
}