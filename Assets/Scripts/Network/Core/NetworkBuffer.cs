using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineGame.Networking
{
    /// <summary>
    /// Этот класс помогает организовать данные, необходимые для
    /// чтение и запись в сетевой поток
    /// </summary>
    public class NetworkBuffer
    {
        public byte[] WriteBuffer;
        public byte[] ReadBuffer;
        public int CurrentWriteByteCount;
    }
}