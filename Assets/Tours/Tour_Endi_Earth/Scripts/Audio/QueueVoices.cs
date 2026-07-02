using UnityEngine.UIElements;

namespace Tour_Endi_Earth
{
    public class VoiceRequestQueue
    {
        private VoiceRequest?[] _buffer;
        private int _head, _tail, _count;
        public int Count { get { return _count; } }

        public VoiceRequestQueue(int capacity = 15)
        {
            _buffer = new VoiceRequest?[capacity];
            _head = _tail = _count = 0;
        }

        public void Enqueue(VoiceRequest? req)
        {
            if (_count == _buffer.Length)
            {
                return;
            }

            _buffer[_tail] = req;
            _tail = (_tail + 1) % _buffer.Length;
            _count++;
        }

        public VoiceRequest? Dequeue()
        {
            if (_count == 0) return null;

            var req = _buffer[_head];
            _buffer[_head] = null;
            _head = (_head + 1) % _buffer.Length;
            _count--;
            return req;
        }

        public void Clear()
        {
            for(int i = 0; i < _count; i++)
            {
                _buffer[i] = null;
            }
        }
    }
}