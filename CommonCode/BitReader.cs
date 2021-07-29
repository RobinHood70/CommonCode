namespace RobinHood70.CommonCode
{
	using System;
	using System.Text;
	using RobinHood70.CommonCode.Properties;

	/// <summary>A quick hack of a class to make BitConverter act a bit more like BinaryReader.</summary>
	/// <remarks>There is currently almost no buffer overflow checking apart from that provided by the BitConverter calls, so those methods will simply crash if you do overflow.</remarks>
	public class BitReader
	{
		#region Fields
		private readonly byte[] rawData;
		private int offset;
		#endregion

		#region Constructors
		public BitReader(byte[] rawData) => this.rawData = rawData
			.Validate(nameof(rawData))
			.NotNullOrEmpty()
			.Value;
		#endregion

		#region Public Static Properties
		public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
		#endregion

		#region Public Properties
		public bool EOF => this.offset >= this.rawData.Length;

		public int Length => this.rawData.Length;
		#endregion

		#region Public Methods
		public void CheckComplete()
		{
			if (this.offset < this.rawData.Length)
			{
				throw new InvalidOperationException(Globals.CurrentCulture(Resources.NotAtEnd, nameof(BitReader)));
			}
		}

		public byte[] GetRawData() => this.rawData;

		public bool ReadBoolean()
		{
			var retval = BitConverter.ToBoolean(this.rawData, this.offset);
			this.offset++;
			return retval;
		}

		public byte ReadByte()
		{
			var retval = this.rawData[this.offset];
			this.offset++;
			return retval;
		}

		public byte[] ReadBytes(int numBytes)
		{
			var retval = this.rawData[this.offset..(this.offset + numBytes)];
			this.offset += numBytes;
			return retval;
		}

		public char ReadChar()
		{
			var retval = BitConverter.ToChar(this.rawData, this.offset);
			this.offset++;
			return retval;
		}

		public char ReadChar(Encoding encoding)
		{
			char retval;
			if (encoding.NotNull(nameof(encoding)).IsSingleByte)
			{
				retval = encoding.GetChars(this.rawData, this.offset, 1)[0];
				this.offset++;
			}
			else
			{
				var numBytes = encoding.GetMaxByteCount(1);
				if (numBytes > this.rawData.Length - this.offset)
				{
					numBytes = this.rawData.Length - this.offset;
				}

				retval = encoding.GetChars(this.rawData, this.offset, numBytes)[0];
				this.offset += encoding.GetByteCount(new[] { retval });
			}

			return retval;
		}

		public double ReadDouble()
		{
			var retval = BitConverter.ToDouble(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		public short ReadInt16()
		{
			var retval = BitConverter.ToInt16(this.rawData, this.offset);
			this.offset += 2;
			return retval;
		}

		public int ReadInt32()
		{
			var retval = BitConverter.ToInt32(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		public long ReadInt64()
		{
			var retval = BitConverter.ToInt64(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		[CLSCompliant(false)]
		public sbyte ReadSByte()
		{
			var retval = (sbyte)this.rawData[this.offset];
			this.offset++;
			return retval;
		}

		public float ReadSingle()
		{
			var retval = BitConverter.ToSingle(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		public string ReadString(int count) => this.ReadString(count, DefaultEncoding);

		public string ReadString(int count, Encoding encoding)
		{
			var retval = (encoding ?? DefaultEncoding).GetString(this.rawData, this.offset, count);
			this.offset += count;
			return retval;
		}

		[CLSCompliant(false)]
		public ushort ReadUInt16()
		{
			var retval = BitConverter.ToUInt16(this.rawData, this.offset);
			this.offset += 2;
			return retval;
		}

		[CLSCompliant(false)]
		public uint ReadUInt32()
		{
			var retval = BitConverter.ToUInt32(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		[CLSCompliant(false)]
		public ulong ReadUInt64()
		{
			var retval = BitConverter.ToUInt64(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		public string ReadZString(int count) => this.ReadZString(count, DefaultEncoding);

		public string ReadZString(int count, Encoding encoding) => this.ReadString(count, encoding).Split(TextArrays.Null, 2)[0];

		public void Skip(int numBytes) => this.offset += numBytes;
		#endregion
	}
}
