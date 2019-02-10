// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Windows.Forms;


    /// <devdoc>
    ///    <para> 
    ///       Implements a Windows message.</para>
    /// </devdoc>
    [SuppressMessage("Microsoft.Security", "CA2108:ReviewDeclarativeSecurityOnValueTypes")]
    public struct Message {
#if DEBUG
        static TraceSwitch AllWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

        IntPtr hWnd;
        int msg;
        IntPtr wparam;
        IntPtr lparam;
        IntPtr result;
        
        /// <devdoc>
        ///    <para>Specifies the window handle of the message.</para>
        /// </devdoc>

        public IntPtr HWnd {
            get { return hWnd; }
            set { hWnd = value; }
        }

        /// <devdoc>
        ///    <para>Specifies the ID number for the message.</para>
        /// </devdoc>
        public int Msg {
            get { return msg; }
            set { msg = value; }
        }

        /// <devdoc>
        /// <para>Specifies the <see cref='System.Windows.Forms.Message.wparam'/> of the message.</para>
        /// </devdoc>
        public IntPtr WParam {
            get { return wparam; }
            set { wparam = value; }
        }

        /// <devdoc>
        /// <para>Specifies the <see cref='System.Windows.Forms.Message.lparam'/> of the message.</para>
        /// </devdoc>
        public IntPtr LParam {
            get { return lparam; }
            set { lparam = value; }
        }

        /// <devdoc>
        ///    <para>Specifies the return value of the message.</para>
        /// </devdoc>
        public IntPtr Result {
             get { return result; }
             set { result = value; }
        }

        /// <devdoc>
        /// <para>Gets the <see cref='System.Windows.Forms.Message.lparam'/> value, and converts the value to an object.</para>
        /// </devdoc>
        public object GetLParam(Type cls) {
            return UnsafeNativeMethods.PtrToStructure(lparam, cls);
        }

        /// <devdoc>
        /// <para>Creates a new <see cref='System.Windows.Forms.Message'/> object.</para>
        /// </devdoc>
        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
            Message m = new Message();
            m.hWnd = hWnd;
            m.msg = msg;
            m.wparam = wparam;
            m.lparam = lparam;
            m.result = IntPtr.Zero;
            
#if DEBUG
            if(AllWinMessages.TraceVerbose) {
                Debug.WriteLine(m.ToString());
            }
#endif
            return m;
        }
        
        public override bool Equals(object o) {
            if (!(o is Message)) {
                return false;
            }
            
            Message m = (Message)o;
            return hWnd == m.hWnd && 
                   msg == m.msg && 
                   wparam == m.wparam && 
                   lparam == m.lparam && 
                   result == m.result;
        }

        public static bool operator !=(Message a, Message b) {
            return !a.Equals(b);
        }

        public static bool operator ==(Message a, Message b) {
            return a.Equals(b);
        }

        public override int GetHashCode() {
            return (int)hWnd << 4 | msg;
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override string ToString() {
            return MessageDecoder.ToString(this);
        }
    }
}

