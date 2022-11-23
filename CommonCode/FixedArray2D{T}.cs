namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using RobinHood70.CommonCode.Properties;

	/// <summary>This class embodies a fix-sized, two-dimensional array. Its designed primarily to be used in conjunction with properties, so the traditional issues with using an array as a property is avoided.</summary>
	/// <remarks>Currently, this is a bare-bones class which allows modification of elements within the array, though not changing the array size itself. That functionality could easily be expanded to include a read-only mode (throwing an error of the setter is accessed).</remarks>
	/// <typeparam name="T">The type of each element in the array.</typeparam>
	public class FixedArray2D<T>
	{
		#region Fields
		private readonly T[,] array;
		#endregion

		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="FixedArray2D{T}"/> class from a two-dimensional array.</summary>
		/// <param name="array">The source array.</param>
		public FixedArray2D(T[,] array)
		{
			this.array = array;
		}

		/// <summary>Initializes a new instance of the <see cref="FixedArray2D{T}"/> class from a one-dimensional array.</summary>
		/// <param name="array">The array to re-arrange.</param>
		/// <param name="rows">The number of rows in the array.</param>
		/// <param name="columns">The number of columns in the array.</param>
		/// <param name="readHorizontalFirst"><see langword="true"/> to read items from the source array as elements of a row, with additional data defining additional rows; false to read items from the source array as elements of a column, with additional data defining additional columns.</param>
		/// <param name="offset">The offset into the array to start reading at.</param>
		/// <exception cref="ArgumentException">Thrown when the source array is too small to contain the requested number of rows and columns.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when either <paramref name="columns"/> or <paramref name="rows"/> is less than or equal to zero.</exception>
		/// <remarks>As implied by the rest of the documentation, there is currently no facility in this constructor to read an arbitrary number of rows or columns.</remarks>
		public FixedArray2D(T[] array, int rows, int columns, bool readHorizontalFirst, int offset)
		{
			// Currently ignores if array is too big.
			if (array.NotNull().Length < rows * columns + offset)
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

		/// <summary>Gets the number of columns in the array.</summary>
		public int ColumnCount => this.array.GetUpperBound(1);

		/// <summary>Gets the number of rows in the array.</summary>
		public int RowCount => this.array.GetUpperBound(0);
		#endregion

		#region Public Indexers

		/// <summary>Gets or sets the value at a specified row and column.</summary>
		/// <param name="row">The row of the item to get.</param>
		/// <param name="column">The column of the item to get.</param>
		/// <returns>The value at the specified location.</returns>
		public T this[int row, int column]
		{
			get => this.array[row, column];
			set => this.array[row, column] = value;
		}
		#endregion

		#region Public Methods

		/// <summary>Creates a shallow copy of the requested row.</summary>
		/// <param name="columnNum">The row number to copy.</param>
		/// <returns>A shallow copy of the row.</returns>
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

		/// <summary>Creates a shallow copy of the requested row.</summary>
		/// <param name="rowNum">The row number to copy.</param>
		/// <returns>A shallow copy of the row.</returns>
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