﻿#if BASEMESSAGE
using System;

namespace Base.Message
{
	public class BufferManager
		: IBufferManager
	{
		public ArraySegment<byte> Allocate(int size)
		{
			return new ArraySegment<byte>(new byte[size], 0, size);
		}

		public void Reallocate(ref ArraySegment<byte> segment, int extraSize)
		{
			var newSegment = new ArraySegment<byte>(new byte[segment.Count + extraSize], 0, segment.Count + extraSize);

			Buffer.BlockCopy(segment.Array, 0, newSegment.Array, 0, segment.Count);

			segment = newSegment;
		}

		public void Free(ref ArraySegment<byte> segment)
		{
			segment = new ArraySegment<byte>();
		}
	}
}

#endif