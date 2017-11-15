﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Memory
{
    public class StreamWrapper : Stream, IBuffer
    {
        public StreamWrapper(Stream stream)
        {
            Stream = stream;
        }

        public Stream Stream { get; }

        public override bool CanRead
            => Stream?.CanRead ?? false;

        public override bool CanSeek
            => Stream?.CanSeek ?? false;

        public override bool CanWrite
            => Stream?.CanWrite ?? false;

        public override long Length
            => Stream?.Length ?? -1;

        public override long Position
        {
            get => Stream?.Position ?? -1;
            set
            {
                Debug.Assert(Stream != null, "Stream != null");
                Stream.Position = value;
            }
        }

        public long Remaining
            => Length - Position;

        public byte[] ToBytes()
        {
            var length = (int) (Stream?.Length - Stream?.Position ?? 0);
            var data = new byte[length];
            Stream?.Read(data, 0, length);
            return data;
        }

        public bool Has(long bytes)
            => bytes <= Remaining;

        public bool Read(out bool value)
        {
            if (!Has(sizeof(bool)))
            {
                value = default(bool);
                return false;
            }

            var bytes = new byte[sizeof(bool)];
            Stream?.Read(bytes, 0, sizeof(bool));
            value = BitConverter.ToBoolean(bytes, 0);
            return true;
        }

        public bool Read(out byte value)
        {
            if (!Has(sizeof(byte)))
            {
                value = default(byte);
                return false;
            }

            Debug.Assert(Stream != null, "Stream != null");
            value = (byte) (Stream.ReadByte() & 0xFF);
            return true;
        }

        public bool Read(out byte[] value)
        {
            if (!Read(out int count))
            {
                value = default(byte[]);
                return false;
            }

            value = new byte[count];
            return Read(ref value, 0, count);
        }

        public bool Read(out byte[] value, long count)
        {
            value = new byte[count];
            return Read(ref value, 0, count);
        }

        public bool Read(ref byte[] value, long offset, long count)
        {
            if (value == null) value = new byte[offset + count];
            return count == Stream?.Read(value, (int) offset, (int) count);
        }

        public bool Read(out char value)
        {
            if (Has(sizeof(char)) && Read(out byte[] bytes, sizeof(char)))
            {
                value = BitConverter.ToChar(bytes, 0);
                return true;
            }

            value = default(char);
            return false;
        }

        public bool Read(out decimal value)
        {
            value = default(decimal);

            var bits = new int[4];
            if (!Read(out bits[0])) return false;
            if (!Read(out bits[1])) return false;
            if (!Read(out bits[2])) return false;
            if (!Read(out bits[3])) return false;

            value = new decimal(bits);
            return true;
        }

        public bool Read(out double value)
        {
            if (!Has(sizeof(double)))
            {
                value = default(double);
                return false;
            }

            var bytes = new byte[sizeof(double)];
            Stream?.Read(bytes, 0, sizeof(double));
            value = BitConverter.ToDouble(bytes, 0);
            return true;
        }

        public bool Read(out float value)
        {
            if (!Has(sizeof(float)))
            {
                value = default(float);
                return false;
            }

            var bytes = new byte[sizeof(float)];
            Stream?.Read(bytes, 0, sizeof(float));
            value = BitConverter.ToSingle(bytes, 0);
            return true;
        }

        public bool Read(out int value)
        {
            if (!Has(sizeof(int)))
            {
                value = default(int);
                return false;
            }

            var bytes = new byte[sizeof(int)];
            Stream?.Read(bytes, 0, sizeof(int));
            value = BitConverter.ToInt32(bytes, 0);
            return true;
        }

        public bool Read(out long value)
        {
            if (!Has(sizeof(long)))
            {
                value = default(long);
                return false;
            }

            var bytes = new byte[sizeof(long)];
            Stream?.Read(bytes, 0, sizeof(long));
            value = BitConverter.ToInt64(bytes, 0);
            return true;
        }

        public bool Read(out sbyte value)
        {
            if (!Has(sizeof(sbyte)))
            {
                value = default(sbyte);
                return false;
            }

            var bytes = new byte[sizeof(sbyte)];
            Stream?.Read(bytes, 0, sizeof(sbyte));
            value = (sbyte) bytes[0];
            return true;
        }

        public bool Read(out short value)
        {
            if (!Has(sizeof(short)))
            {
                value = default(short);
                return false;
            }

            var bytes = new byte[sizeof(short)];
            Stream?.Read(bytes, 0, sizeof(short));
            value = BitConverter.ToInt16(bytes, 0);
            return true;
        }

        public bool Read(out string value)
            => Read(out value, Encoding.UTF8);

        public bool Read(out string value, Encoding encoding, bool nullTerminated = false)
        {
            if (encoding == null) throw new ArgumentNullException();
            if (Stream == null)
            {
                value = default(string);
                return false;
            }

            int length;
            if (nullTerminated)
            {
                if (!CanSeek)
                {
                    throw new InvalidOperationException(
                        "Unable to read null-terminated strings on a Stream without seek.");
                }

                var originalPosition = Stream.Position;
                while (Length - Position > 0)
                {
                    if (Stream.ReadByte() == 0) break;
                    Stream.Position++;
                }

                Stream.Position = originalPosition;
                length = (int) (Length - originalPosition);
            }
            else if (!Read(out length))
            {
                value = default(string);
                return false;
            }

            switch (length)
            {
                case 0:
                    value = "";
                    break;

                case -1:
                    value = null;
                    break;

                default:
                    value = Read(out byte[] bytes, length) ? encoding.GetString(bytes, 0, length) : null;
                    break;
            }

            return true;
        }

        public bool Read(out uint value)
        {
            if (!Has(sizeof(uint)))
            {
                value = default(uint);
                return false;
            }

            var bytes = new byte[sizeof(uint)];
            Stream?.Read(bytes, 0, sizeof(uint));
            value = BitConverter.ToUInt32(bytes, 0);
            return true;
        }

        public bool Read(out ulong value)
        {
            if (!Has(sizeof(ulong)))
            {
                value = default(ulong);
                return false;
            }

            var bytes = new byte[sizeof(ulong)];
            Stream?.Read(bytes, 0, sizeof(ulong));
            value = BitConverter.ToUInt64(bytes, 0);
            return true;
        }

        public bool Read(out ushort value)
        {
            if (!Has(sizeof(ushort)))
            {
                value = default(ushort);
                return false;
            }

            var bytes = new byte[sizeof(ushort)];
            Stream?.Read(bytes, 0, sizeof(ushort));
            value = BitConverter.ToUInt16(bytes, 0);
            return true;
        }

        public bool ReadBool()
        {
            if (Read(out bool value)) return value;
            throw new OutOfMemoryException();
        }

        public bool ReadBoolean()
        {
            if (Read(out bool value)) return value;
            throw new OutOfMemoryException();
        }

        public new byte ReadByte()
        {
            Debug.Assert(Stream != null, "Stream != null");
            return (byte) (Stream.ReadByte() & 0xFF);
        }

        public byte ReadUInt8()
        {
            if (Read(out byte value)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes()
        {
            if (Read(out byte[] value)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes(long count)
        {
            if (Read(out byte[] value, count)) return value;
            throw new OutOfMemoryException();
        }

        public byte[] ReadBytes(ref byte[] bytes, long offset, long count)
        {
            if (Read(ref bytes, offset, count)) return bytes;
            throw new OutOfMemoryException();
        }

        public char ReadChar()
        {
            if (Read(out char value)) return value;
            throw new OutOfMemoryException();
        }

        public char ReadCharacter()
        {
            if (Read(out char value)) return value;
            throw new OutOfMemoryException();
        }

        public decimal ReadDecimal()
        {
            if (Read(out decimal value)) return value;
            throw new OutOfMemoryException();
        }

        public double ReadDouble()
        {
            if (Read(out double value)) return value;
            throw new OutOfMemoryException();
        }

        public float ReadFloat()
        {
            if (Read(out float value)) return value;
            throw new OutOfMemoryException();
        }

        public float ReadSingle()
        {
            if (Read(out float value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInt()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInt32()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public int ReadInteger()
        {
            if (Read(out int value)) return value;
            throw new OutOfMemoryException();
        }

        public long ReadInt64()
        {
            if (Read(out long value)) return value;
            throw new OutOfMemoryException();
        }

        public long ReadLong()
        {
            if (Read(out long value)) return value;
            throw new OutOfMemoryException();
        }

        public sbyte ReadInt8()
        {
            if (Read(out sbyte value)) return value;
            throw new OutOfMemoryException();
        }

        public sbyte ReadSByte()
        {
            if (Read(out sbyte value)) return value;
            throw new OutOfMemoryException();
        }

        public short ReadInt16()
        {
            if (Read(out short value)) return value;
            throw new OutOfMemoryException();
        }

        public short ReadShort()
        {
            if (Read(out short value)) return value;
            throw new OutOfMemoryException();
        }

        public string ReadString()
        {
            if (Read(out string value)) return value;
            throw new OutOfMemoryException();
        }

        public string ReadString(Encoding encoding, bool nullTerminated = false)
        {
            if (Read(out string value, encoding, nullTerminated)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUInt()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUInt32()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public uint ReadUnsignedInteger()
        {
            if (Read(out uint value)) return value;
            throw new OutOfMemoryException();
        }

        public ulong ReadULong()
        {
            if (Read(out ulong value)) return value;
            throw new OutOfMemoryException();
        }

        public ulong ReadUInt64()
        {
            if (Read(out ulong value)) return value;
            throw new OutOfMemoryException();
        }

        public ushort ReadUInt16()
        {
            if (Read(out ushort value)) return value;
            throw new OutOfMemoryException();
        }

        public ushort ReadUShort()
        {
            if (Read(out ushort value)) return value;
            throw new OutOfMemoryException();
        }

        public void Write(bool value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(bool));

        public void Write(byte value)
            => WriteByte(value);

        public void Write(byte[] value)
        {
            if (value == null) throw new ArgumentNullException();
            Write(value.Length);
            Write(value, 0, value.Length);
        }

        public void Write(byte[] value, long count)
            => Write(value, 0, count);

        public void Write(byte[] value, long offset, long count)
            => Stream?.Write(value ?? new byte[0], (int) offset, (int) count);

        public void Write(char value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(char));

        public void Write(decimal value)
        {
            var bits = decimal.GetBits(value);
            Write(bits[0]);
            Write(bits[1]);
            Write(bits[2]);
            Write(bits[3]);
        }

        public void Write(double value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(double));

        public void Write(float value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(float));

        public void Write(int value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(int));

        public void Write(long value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(long));

        public void Write(sbyte value)
            => WriteByte((byte) value);

        public void Write(short value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(short));

        public void Write(string value)
            => Write(value, Encoding.UTF8);

        public void Write(string value, Encoding encoding, bool nullTerminated = false)
        {
            if (encoding == null) throw new ArgumentNullException();

            if (!nullTerminated)
                Write(value?.Length ?? -1);

            if (value == null) return;
            if (value.Length < 1) return;
            var bytes = encoding.GetBytes(value);
            Write(bytes, bytes.Length);
        }

        public void Write(uint value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(uint));

        public void Write(ulong value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(ulong));

        public void Write(ushort value)
            => Write(BitConverter.GetBytes(value), 0, sizeof(ushort));

        public override void Flush()
            => Stream?.Flush();

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (Stream == null) throw new NullReferenceException();
            return Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
            => Stream?.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Stream?.Read(buffer, offset, count) ?? -1;
        }

        public override void Write(byte[] buffer, int offset, int count)
            => Stream?.Write(buffer, offset, count);
    }
}