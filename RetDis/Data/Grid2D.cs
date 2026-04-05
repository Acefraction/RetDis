using System;
using System.Collections;
using System.Collections.Generic;

namespace RetDis
{
    /// <summary>
    /// 内部は 1 次元バッファで保持しつつ、2 次元アクセスを提供する汎用グリッド。
    /// </summary>
    public sealed class Grid2D<T> : IEnumerable<T>
    {
        // 画像系データの実体は 1 次元に統一し、利用側には 2 次元として見せる。
        private readonly T[] buffer;

        public Grid2D(int width, int height)
            : this(width, height, new T[checked(width * height)])
        {
        }

        public Grid2D(int width, int height, T[] buffer)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "幅は 1 以上である必要があります。");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "高さは 1 以上である必要があります。");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length != checked(width * height))
            {
                throw new ArgumentException("バッファ長が幅 x 高さと一致しません。", nameof(buffer));
            }

            Width = width;
            Height = height;
            this.buffer = buffer;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Count
        {
            get { return buffer.Length; }
        }

        public T[] Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// x, y 座標から要素へ直接アクセスする。
        /// </summary>
        public T this[int x, int y]
        {
            get { return buffer[GetIndex(x, y)]; }
            set { buffer[GetIndex(x, y)] = value; }
        }

        public void Clear()
        {
            Array.Clear(buffer, 0, buffer.Length);
        }

        public Grid2D<T> Clone()
        {
            return new Grid2D<T>(Width, Height, (T[])buffer.Clone());
        }

        /// <summary>
        /// 2 次元座標を内部 1 次元インデックスへ変換する。
        /// </summary>
        public int GetIndex(int x, int y)
        {
            if ((uint)x >= (uint)Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if ((uint)y >= (uint)Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return checked((y * Width) + x);
        }

        public bool TryGetIndex(int x, int y, out int index)
        {
            if ((uint)x < (uint)Width && (uint)y < (uint)Height)
            {
                index = (y * Width) + x;
                return true;
            }

            index = -1;
            return false;
        }

        public bool TryGetValue(int x, int y, out T value)
        {
            int index;
            if (TryGetIndex(x, y, out index))
            {
                value = buffer[index];
                return true;
            }

            value = default(T);
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
