// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Threading;

namespace SharpGen.Runtime
{
    /// <summary>
    /// Base class for all callback objects written in managed code.
    /// </summary>
    public abstract class CallbackBase : DisposeBase, ICallbackable, IUnknown
    {
        private int count = 1;
        void IUnknown.QueryInterface(Guid guid, out IntPtr output)
        {
            var shadow = (ComObjectShadow)((ShadowContainer)((ICallbackable)this).Shadow).FindShadow(guid);
            if (shadow != null)
            {
                ((IUnknown)this).AddRef();
                output = shadow.NativePointer;
            }
            output = IntPtr.Zero;
        }

        uint IUnknown.AddRef()
        {
            return (uint)Interlocked.Increment(ref count);
        }

        uint IUnknown.Release()
        {
            return (uint)Interlocked.Decrement(ref count);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var callback = ((ICallbackable) this);
                if (callback.Shadow != null)
                {
                    callback.Shadow.Dispose();
                    callback.Shadow = null;
                }
            }
        }

        ShadowContainer ICallbackable.Shadow { get; set; }
    }
}