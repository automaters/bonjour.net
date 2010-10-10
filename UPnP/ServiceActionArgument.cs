using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Network.UPnP
{
    public enum ArgumentDirection
    {
        In,
        Out,
    }

    public static class ArgumentType
    {
        public const string ui1 = "ui1";
        public const string ui2 = "ui2";
        public const string ui4 = "ui4";
        public const string i1 = "i1";
        public const string i2 = "i2";
        public const string i4 = "i4";
        public const string @int = "int";
        public const string r4 = "r4";
        public const string r8 = "r8";
        public const string number = "number";
        public const string fixed14_4 = "fixed14.4";
        public const string @float = "float";
        public const string @char = "char";
        public const string @string = "string";
        public const string date = "date";
        public const string dateTime = "dateTime";
        public const string dateTime_tz = "dateTime.tz";
        public const string time = "time";
        public const string time_tz = "time.tz";
        public const string boolean = "boolean";
        public const string bin_base64 = "bin.base64";
        public const string bin_hex = "bin.hex";
        public const string uri = "uri";
        public const string uuid = "uuid";

    }

    public abstract class ServiceActionArgument
    {
        public ServiceActionArgument(string name, string type, ArgumentDirection direction)
        {
            Name = name;
            Type = type;
            Direction = direction;
        }

        public ArgumentDirection Direction { get; private set; }

        public string Name { get; private set; }

        public string Type { get; private set; }

        public object Value { get; protected set; }

        public abstract void SetValue(string value);
        public abstract void SetValue(int value);
        public abstract void SetValue(uint value);
        public abstract void SetValue(float value);
        public abstract void SetValue(byte value);
        public abstract void SetValue(short value);
        public abstract void SetValue(ushort value);
        public abstract void SetValue(long value);
        public abstract void SetValue(ulong value);
        public abstract void SetValue(double value);
        public abstract void SetValue(DateTime value);
        public abstract void SetValue(TimeSpan value);
        public abstract void SetValue(char value);
        public abstract void SetValue(bool value);
        public abstract void SetValue(byte[] value);
        public abstract void SetValue(Uri value);
    }

    public class ServiceActionArgument<T> : ServiceActionArgument
    {
        public ServiceActionArgument(string name, string type, ArgumentDirection direction)
            : base(name, type, direction)
        {

        }

        public override void SetValue(string value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    Value = Convert.ToBase64String(Encoding.Unicode.GetBytes(value));
                    break;
                case ArgumentType.bin_hex:
                    Value = Encoding.Unicode.GetBytes(value);
                    break;
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = TimeSpan.Parse(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    Value = new Uri(value);
                    break;
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    break;
            }
        }

        public override void SetValue(int value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(uint value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(float value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                //case ArgumentType.time:
                //case ArgumentType.time_tz:
                //    Value = new TimeSpan(value);
                    //break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(byte value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(short value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(ushort value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(long value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(ulong value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                //case ArgumentType.time:
                //case ArgumentType.time_tz:
                //    Value = new TimeSpan(value);
                //    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(double value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                //case ArgumentType.time:
                //case ArgumentType.time_tz:
                //    Value = new TimeSpan(value);
                //    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(DateTime value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = value;
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = value.TimeOfDay;
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(TimeSpan value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = value;
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(char value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    Value = new TimeSpan(value);
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(bool value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    throw new InvalidCastException();
                case ArgumentType.bin_hex:
                    throw new InvalidCastException();
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.fixed14_4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uri:
                    throw new InvalidCastException();
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(byte[] value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    Value = Convert.ToBase64String(value);
                    break;
                case ArgumentType.bin_hex:
                    Value = value;
                    break;
                case ArgumentType.boolean:
                    Value = Convert.ToBoolean(value);
                    break;
                case ArgumentType.@char:
                    Value = Convert.ToChar(value);
                    break;
                case ArgumentType.date:
                case ArgumentType.dateTime:
                case ArgumentType.dateTime_tz:
                    throw new InvalidCastException();
                    break;
                case ArgumentType.fixed14_4:
                    double doubleValue = Convert.ToDouble(value);
                    Value = doubleValue;
                    if (doubleValue % 100000000000000 != doubleValue)
                        throw new InvalidCastException();
                    double decimals = (doubleValue - Math.Floor(doubleValue)) * 10000;
                    if (Math.Floor(decimals) != decimals)
                        throw new InvalidCastException();
                    break;
                case ArgumentType.@float:
                case ArgumentType.r4:
                    Value = Convert.ToSingle(value);
                    break;
                case ArgumentType.i1:
                    Value = Convert.ToSByte(value);
                    break;
                case ArgumentType.i2:
                    Value = Convert.ToInt16(value);
                    break;
                case ArgumentType.i4:
                case ArgumentType.@int:
                    Value = Convert.ToInt32(value);
                    break;
                case ArgumentType.r8:
                case ArgumentType.number:
                    Value = Convert.ToDouble(value);
                    break;
                case ArgumentType.@string:
                    Value = value;
                    break;
                case ArgumentType.time:
                case ArgumentType.time_tz:
                    throw new InvalidCastException();
                    break;
                case ArgumentType.ui1:
                    Value = Convert.ToByte(value);
                    break;
                case ArgumentType.ui2:
                    Value = Convert.ToUInt16(value);
                    break;
                case ArgumentType.ui4:
                    Value = Convert.ToUInt32(value);
                    break;
                case ArgumentType.uuid:
                    Value = value;
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        public override void SetValue(Uri value)
        {
            switch (Type)
            {
                case ArgumentType.bin_base64:
                    Value = Convert.ToBase64String(Encoding.Unicode.GetBytes(value.ToString()));
                    break;
                case ArgumentType.bin_hex:
                    Value = Encoding.Unicode.GetBytes(value.ToString());
                    break;
                case ArgumentType.@string:
                    Value = value.ToString();
                    break;
                case ArgumentType.uri:
                    Value = value;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
    }
}
