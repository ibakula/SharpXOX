using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace XOXClient
{
    public class Packet
    {
        public Packet(Opcodes opcode = Opcodes.NULL, bool receiving = true)
        {
            if (!receiving)
                Write(Convert.ToByte(opcode), false);
            else _buffer = new byte[53];

            _readPos = 0;
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

        public byte GetOpcode()
        {
            byte opcode = 0;
            Read(ref opcode);
            return opcode;
        }

        // Size when serialized (serialized length of ushort is 53 bytes but may differ on different dev env)
        // discludes the size of size itself but only the rest of the packet.
        public ushort GetPacketSize()
        {
            ushort size = 0;
            Read(ref size);
            return size;
        }

        public void Prepare(int size)
        {
            byte[] buffer = new byte[_buffer.Length + size];
            Array.Copy(_buffer, buffer, _buffer.Length);
            _buffer = buffer;
        }

        public void Read<T>(ref T data)
        {
            if (_readPos >= _buffer.Length)
                return;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(_buffer, _readPos, _buffer.Length - _readPos);
                ms.Position = 0;
                data = (T)bf.Deserialize(ms);
                _readPos += Convert.ToInt32(ms.Position);
            }
        }

        public void Settle()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ushort length = Convert.ToUInt16(_buffer.Length);
                bf.Serialize(ms, length);
                ms.Write(_buffer, 0, _buffer.Length);
                _buffer = ms.ToArray();
            }
        }

        public void Write<T>(T data, bool copyContents = true)
        {
            if (data == null)
                return;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                if (copyContents)
                    ms.Write(_buffer, 0, _buffer.Length);

                bf.Serialize(ms, data);
                _buffer = ms.ToArray();
            }
        }
    }
}
