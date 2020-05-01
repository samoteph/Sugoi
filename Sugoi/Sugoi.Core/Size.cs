// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sugoi.Core
{
    /// <summary>
    /// Stores an ordered pair of integers, which specify a height and width.
    /// </summary>
    /// <remarks>
    /// This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
    /// as it avoids the need to create new values for modification operations.
    /// </remarks>
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        /// Represents a <see cref="Size"/> that has Width and Height values set to zero.
        /// </summary>
        public static readonly Size Empty = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="value">The width and height of the size.</param>
        public Size(int value)
            : this()
        {
            this.Width = value;
            this.Height = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">The width of the size.</param>
        /// <param name="height">The height of the size.</param>
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        public Size(Size size)
            : this()
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct from the given <see cref="Point"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        public Size(Point point)
        {
            this.Width = point.X;
            this.Height = point.Y;
        }

        /// <summary>
        /// Gets or sets the width of this <see cref="Size"/>.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of this <see cref="Size"/>.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Size"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty => this.Equals(Empty);

        /// <summary>
        /// Converts the given <see cref="Size"/> into a <see cref="Point"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Point(Size size) => new Point(size.Width, size.Height);

        /// <summary>
        /// Computes the sum of adding two sizes.
        /// </summary>
        /// <param name="left">The size on the left hand of the operand.</param>
        /// <param name="right">The size on the right hand of the operand.</param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size operator +(Size left, Size right) => Add(left, right);

        /// <summary>
        /// Computes the difference left by subtracting one size from another.
        /// </summary>
        /// <param name="left">The size on the left hand of the operand.</param>
        /// <param name="right">The size on the right hand of the operand.</param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size operator -(Size left, Size right) => Subtract(left, right);

        /// <summary>
        /// Multiplies a <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Multiplier of type <see cref="int"/>.</param>
        /// <param name="right">Multiplicand of type <see cref="Size"/>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        public static Size operator *(int left, Size right) => Multiply(right, left);

        /// <summary>
        /// Multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="right">Multiplier of type <see cref="int"/>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        public static Size operator *(Size left, int right) => Multiply(left, right);

        /// <summary>
        /// Divides <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Dividend of type <see cref="Size"/>.</param>
        /// <param name="right">Divisor of type <see cref="int"/>.</param>
        /// <returns>Result of type <see cref="Size"/>.</returns>
        public static Size operator /(Size left, int right) => new Size(unchecked(left.Width / right), unchecked(left.Height / right));

        /// <summary>
        /// Compares two <see cref="Size"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Size"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Size"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Size left, Size right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Size"/> objects for inequality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Size"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Size"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Size left, Size right) => !left.Equals(right);

        /// <summary>
        /// Performs vector addition of two <see cref="Size"/> objects.
        /// </summary>
        /// <param name="left">The size on the left hand of the operand.</param>
        /// <param name="right">The size on the right hand of the operand.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size Add(Size left, Size right) => new Size(unchecked(left.Width + right.Width), unchecked(left.Height + right.Height));

        /// <summary>
        /// Contracts a <see cref="Size"/> by another <see cref="Size"/>.
        /// </summary>
        /// <param name="left">The size on the left hand of the operand.</param>
        /// <param name="right">The size on the right hand of the operand.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size Subtract(Size left, Size right) => new Size(unchecked(left.Width - right.Width), unchecked(left.Height - right.Height));

        /// <summary>
        /// Deconstructs this size into two integers.
        /// </summary>
        /// <param name="width">The out value for the width.</param>
        /// <param name="height">The out value for the height.</param>
        public void Deconstruct(out int width, out int height)
        {
            width = this.Width;
            height = this.Height;
        }

        /// <inheritdoc/>
        //public override int GetHashCode() => HashCode.Combine(this.Width, this.Height);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => $"Size [ Width={this.Width}, Height={this.Height} ]";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Size other && this.Equals(other);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Size other) => this.Width.Equals(other.Width) && this.Height.Equals(other.Height);

        /// <summary>
        /// Multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="size">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="multiplier">Multiplier of type <see cref="int"/>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        private static Size Multiply(Size size, int multiplier) =>
            new Size(unchecked(size.Width * multiplier), unchecked(size.Height * multiplier));
    }
}