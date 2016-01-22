using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace XOXServer
{
    struct Packet
    {
        public Packet(byte opcode = 0)
        {
            _buffer = new byte[3];
            _buffer[0] = opcode;
            _readPos = 1;
        }

        private byte[] _buffer;
        private int _readPos;

        public byte[] GetData
        {
            get
            {
                return _buffer;
            }
            set
            {
                _buffer = value;
            }
        }

        public void Finalize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, _buffer.Length);
                Array.Copy(_buffer, 1, ms.ToArray(), 0, 2);
            }
        }

        public byte GetOpcode()
        {
            return _buffer[0];
        }

        public ushort GetPacketSize()
        {
            ushort size = 0;
            Read<ushort>(ref size, Marshal.SizeOf(size));
            return size;
        }

        public void Read<T>(ref T data, int length) where T: struct
        {
            if (_readPos >= _buffer.Length || (_readPos+length) >= _buffer.Length)
                return;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(_buffer, _readPos, length);
                ms.Seek(0, SeekOrigin.Begin);
                data = (T)bf.Deserialize(ms);
            }
            _readPos += length;
        }

        public string Read()
        {
            if (_readPos >= _buffer.Length)
                return String.Empty;

            char[] deserialized = Encoding.ASCII.GetChars(_buffer, _readPos, _buffer.Length);
            string str = String.Empty;
            uint pos = 0;
            for (; pos < deserialized.Length; ++pos)
            {
                if (deserialized[pos] == '\0')
                    break;

                str += deserialized[pos];
            }
            _readPos += (int)pos;
            return str;
        }

        public void Write<T>(T data)
        {
            if (data == null)
                return;

            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                byte[] serialized = ms.ToArray();
                byte[] buffer = new byte[_buffer.Length + serialized.Length];
                Array.Copy(_buffer, buffer, _buffer.Length);
                Array.Copy(serialized, 0, buffer, _buffer.Length, serialized.Length);
                _buffer = buffer;
            }
        }
    }
}
