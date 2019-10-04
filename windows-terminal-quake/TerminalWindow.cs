using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
    class TerminalWindow
    {
        public const int MinWidth = 400;
        public const int MinHeight = 120;
        public const int DefaultWidth = 800;
        public const int DefaultHeight = 400;

        private User32.Rect _rect;
        private System.Drawing.Rectangle _bounds;
        public int Width
        {
            get
            {
                return this._rect.Right - this._rect.Left;
            }
            set
            {
                this._rect.Right = Math.Min(value + this._rect.Left, this._bounds.Width);
            }
        }
        public int Height
        {
            get
            {
                return this._rect.Bottom - this._rect.Top;
            }
            set
            {
                this._rect.Bottom = Math.Min(value + this._rect.Top, this._bounds.Height);
            }
        }
        public int Top {
            get 
            {
                return Math.Min(this._rect.Top, this._bounds.Height - this.Height);
            } 
            set
            {
                var height = this.Height;
                this._rect.Top = value;
                this.Height = height;
            }
        }
        public int Left { 
            get
            {
                return Math.Min(this._rect.Left, this._bounds.Width - this.Width);
            } 
            set
            {
                var width = this.Width;
                this._rect.Left = value;
                this.Width = width;
            }
        }
        public int ScreenX
        {
            get
            {
                return Left + _bounds.X;
            }
        }
        public int ScreenY
        {
            get
            {
                return Top + _bounds.Y;
            }
        }

        public TerminalWindow(User32.Rect rect, System.Drawing.Rectangle bounds)
        {
            this._rect = new User32.Rect
            {
                Left = rect.Left,
                Right = rect.Right,
                Top = rect.Top,
                Bottom = rect.Bottom
            };
            this._bounds = new System.Drawing.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public void CenterHorizontally()
        {
            this.Left = (int)Math.Floor((this._bounds.Width - this.Width) / 2.0);
        }

        public void FitWidth()
        {
            this._rect.Left = 0;
            this._rect.Right = this._bounds.Width;
        }

        public bool IsMaximized()
        {
            return this.Top <= 0 && this.Left <= 0 && this.Height >= _bounds.Height && this.Width >= _bounds.Width;
        }
        public void Maximize()
        {
            this.Left = 0;
            this.Top = 0;
            this.Width = _bounds.Width;
            this.Height = _bounds.Height;
        }
        public bool IsVisible()
        {
            // when minimizing, the window seems to still be within the screen bounary, but very small. Just checking it's within
            // bounds is incorrect sometimes; so we also compare to a minimum size
            return Top < _bounds.Height &&
                Top + Height > 0 &&
                Left < _bounds.Width &&
                Left + Width > 0 &&
                Width >= MinWidth &&
                Height >= MinHeight;
        }

        public bool IsDocked()
        {
            return Top <= 0;
        }
        public void EnsureVisible()
        {
            if (Height <= MinHeight || Width <= MinWidth) {
                // ensure visible if it got hidden
                Height = DefaultHeight;
                Width = DefaultWidth;
                CenterHorizontally();
            }
            if (Left + Width <= 0)
            {
                CenterHorizontally();
            }
            Top = 0;
        }
    }
}
