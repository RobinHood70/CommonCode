namespace RobinHood70.CommonCode
{
	using System;
	using System.Text;
	using RobinHood70.CommonCode.Properties;

	/// <summary>A class to make BitConverter act a bit more like BinaryReader.</summary>
	/// <remarks>There is currently almost no buffer overflow checking apart from that provided by the BitConverter calls, so those methods will simply crash if you do overflow.</remarks>
	public class BitReader
	{
		#region Fields
		private readonly byte[] rawData;
		private int offset;
		#endregion

		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="BitReader"/> class.</summary>
		/// <param name="rawData">The data to work on.</param>
		public BitReader(byte[] rawData)
		{
			this.rawData = rawData
				.Validate()
				.NotNullOrEmpty()
				.Value;
		}
		#endregion

		#region Public Static Properties

		/// <summary>Gets or sets the encoding to use if not otherwise specified. Defaults to UTF8.</summary>
		public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
		#endregion

		#region Public Properties

		/// <summary><see langword="true"/>Gets a value indicating whether the pointer is at the end of the buffer; otherwise <see langword="false"/>.</summary>
		public bool EOF => this.offset >= this.rawData.Length;

		/// <summary>Gets the length of the data buffer.</summary>
		public int Length => this.rawData.Length;
		#endregion

		#region Public Methods

		/// <summary>Raises an error if the current position is not the end of the buffer.</summary>
		/// <exception cref="InvalidOperationException">Thrown if the current position isn't the end of the buffer.</exception>
		public void CheckComplete()
		{
			if (!this.EOF)
			{
				throw new InvalidOperationException(Globals.CurrentCulture(Resources.NotAtEnd, nameof(BitReader)));
			}
		}

		/// <summary>Gets the entire data buffer.</summary>
		/// <returns>The data buffer.</returns>
		/// <remarks>This has no effect on the position.</remarks>
		public byte[] GetRawData() => this.rawData;

		/// <summary>Reads a boolean value.</summary>
		/// <returns>The boolean value found at the current position.</returns>
		public bool ReadBoolean()
		{
			var retval = BitConverter.ToBoolean(this.rawData, this.offset);
			this.offset++;
			return retval;
		}

		/// <summary>Reads a byte value.</summary>
		/// <returns>The byte value found at the current position.</returns>
		public byte ReadByte()
		{
			var retval = this.rawData[this.offset];
			this.offset++;
			return retval;
		}

		/// <summary>Reads a consecutive region of bytes.</summary>
		/// <param name="numBytes">The number of bytes to read.</param>
		/// <returns>The bytes found at the current position.</returns>
		public byte[] ReadBytes(int numBytes)
		{
			var retval = this.rawData[this.offset..(this.offset + numBytes)];
			this.offset += numBytes;
			return retval;
		}

		/// <summary>Reads a char value.</summary>
		/// <returns>The char value found at the current position.</returns>
		public char ReadChar()
		{
			var retval = BitConverter.ToChar(this.rawData, this.offset);
			this.offset++;
			return retval;
		}

		/// <summary>Reads a char value using the specified encoding.</summary>
		/// <param name="encoding">The encoding to use, or <see langword="null"/> to use the <see cref="DefaultEncoding"/>.</param>
		/// <returns>The char value found at the current position.</returns>
		public char ReadChar(Encoding? encoding)
		{
			encoding ??= DefaultEncoding;
			char retval;
			if (encoding.IsSingleByte)
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

		/// <summary>Reads a double value.</summary>
		/// <returns>The double value found at the current position.</returns>
		public double ReadDouble()
		{
			var retval = BitConverter.ToDouble(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		/// <summary>Reads a short value.</summary>
		/// <returns>The short value found at the current position.</returns>
		public short ReadInt16()
		{
			var retval = BitConverter.ToInt16(this.rawData, this.offset);
			this.offset += 2;
			return retval;
		}

		/// <summary>Reads an integer value.</summary>
		/// <returns>The integer value found at the current position.</returns>
		public int ReadInt32()
		{
			var retval = BitConverter.ToInt32(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		/// <summary>Reads a long.</summary>
		/// <returns>The long value found at the current position.</returns>
		public long ReadInt64()
		{
			var retval = BitConverter.ToInt64(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		/// <summary>Reads a signed byte value.</summary>
		/// <returns>The signed byte value found at the current position.</returns>
		[CLSCompliant(false)]
		public sbyte ReadSByte()
		{
			var retval = (sbyte)this.rawData[this.offset];
			this.offset++;
			return retval;
		}

		/// <summary>Reads a float value.</summary>
		/// <returns>The float value found at the current position.</returns>
		public float ReadSingle()
		{
			var retval = BitConverter.ToSingle(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		/// <summary>Reads the specified number of bytes into a string.</summary>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>A string based on the specified data.</returns>
		public string ReadString(int count) => this.ReadString(count, null);

		/// <summary>Reads the specified number of bytes into a string.</summary>
		/// <param name="count">The number of bytes to read.</param>
		/// <param name="encoding">The encoding to use, or <see langword="null"/> to use the <see cref="DefaultEncoding"/>.</param>
		/// <returns>The string value found at the current position with the specified length.</returns>
		public string ReadString(int count, Encoding? encoding)
		{
			var retval = (encoding ?? DefaultEncoding).GetString(this.rawData, this.offset, count);
			this.offset += count;
			return retval;
		}

		/// <summary>Reads an unsigned short value.</summary>
		/// <returns>The unsigned short value found at the current position.</returns>
		[CLSCompliant(false)]
		public ushort ReadUInt16()
		{
			var retval = BitConverter.ToUInt16(this.rawData, this.offset);
			this.offset += 2;
			return retval;
		}

		/// <summary>Reads an unsigned integer value.</summary>
		/// <returns>The unsigned integer value found at the current position.</returns>
		[CLSCompliant(false)]
		public uint ReadUInt32()
		{
			var retval = BitConverter.ToUInt32(this.rawData, this.offset);
			this.offset += 4;
			return retval;
		}

		/// <summary>Reads an unsigned long value.</summary>
		/// <returns>The unsigned long value found at the current position.</returns>
		[CLSCompliant(false)]
		public ulong ReadUInt64()
		{
			var retval = BitConverter.ToUInt64(this.rawData, this.offset);
			this.offset += 8;
			return retval;
		}

		/// <summary>Reads a null-terminated string using the specified encoding.</summary>
		/// <param name="count">The number of characters to read.</param>
		/// <returns>The null-terminated string found at the current position with the specified byte-length.</returns>
		public string ReadZString(int count) => this.ReadZString(count, null);

		/// <summary>Reads a null-terminated string using the specified encoding.</summary>
		/// <param name="count">The number of characters to read.</param>
		/// <param name="encoding">The encoding to use, or <see langword="null"/> to use the <see cref="DefaultEncoding"/>.</param>
		/// <returns>The null-terminated string found at the current position with the specified byte-length.</returns>
		public string ReadZString(int count, Encoding? encoding) => this.ReadString(count, encoding).Split(TextArrays.Null, 2)[0];

		/// <summary>Skips past the specified number of bytes.</summary>
		/// <param name="numBytes">The number of bytes to skip.</param>
		public void Skip(int numBytes) => this.offset += numBytes;
		#endregion
	}
}
