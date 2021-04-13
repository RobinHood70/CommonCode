namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using RobinHood70.CommonCode.Properties;
	using static RobinHood70.CommonCode.Globals;

	public class FixedArray2D<T>
	{
		#region Fields
		private readonly T[,] array;
		#endregion

		#region Constructors
		public FixedArray2D(T[,] array) => this.array = array;

		public FixedArray2D(T[] array, int rows, int columns, bool readHorizontalFirst, int offset)
		{
			// Currently ignores if array is too big.
			ThrowNull(array, nameof(array));
			if (array.Length < rows * columns + offset)
			{
				throw new ArgumentException(Resources.ArrayTooSmall, nameof(array));
			}

			if (rows <= 0 || columns <= 0)
			{
				throw new ArgumentOutOfRangeException(Resources.InvalidArrayBounds);
			}

			this.array = new T[rows, columns];
			for (var i = 0; i < rows; i++)
			{
				for (var j = 0; j < columns; j++)
				{
					if (readHorizontalFirst)
					{
						this.array[i, j] = array[offset];
					}
					else
					{
						this.array[j, i] = array[offset];
					}
				}

				offset++;
			}
		}
		#endregion

		#region Public Properties
		public int Columns => this.array.GetUpperBound(1);

		public int Rows => this.array.GetUpperBound(0);
		#endregion

		#region Public Indexers
		public T this[int row, int column]
		{
			get => this.array[row, column];
			set => this.array[row, column] = value;
		}
		#endregion

		#region Public Methods
		public IList<T> CopyOfColumn(int columnNum)
		{
			var columnSize = this.array.GetUpperBound(0);
			var list = new T[columnSize];
			for (var i = 0; i < columnSize; i++)
			{
				list[i] = this.array[i, columnNum];
			}

			return list;
		}

		public IList<T> CopyOfRow(int rowNum)
		{
			var rowSize = this.array.GetUpperBound(1);
			var list = new T[rowSize];
			for (var i = 0; i < rowSize; i++)
			{
				list[i] = this.array[rowNum, i];
			}

			return list;
		}
		#endregion
	}
}