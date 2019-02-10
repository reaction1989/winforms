// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    
    
    /// <devdoc>
    ///     <para>
    ///       A collection that stores <see cref='System.Windows.Forms.InputLanguage'/> objects.
    ///    </para>
    /// </devdoc>
    public class InputLanguageCollection : ReadOnlyCollectionBase {
        
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='System.Windows.Forms.InputLanguageCollection'/> containing any array of <see cref='System.Windows.Forms.InputLanguage'/> objects.
        ///    </para>
        /// </devdoc>
        internal InputLanguageCollection(InputLanguage[] value) {
            InnerList.AddRange(value);
        }
        
        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='System.Windows.Forms.InputLanguage'/>.</para>
        /// </devdoc>
        public InputLanguage this[int index] {
            get {
                return ((InputLanguage)(InnerList[index]));
            }
        }
        
        /// <devdoc>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='System.Windows.Forms.InputLanguageCollection'/> contains the specified <see cref='System.Windows.Forms.InputLanguage'/>.</para>
        /// </devdoc>
        public bool Contains(InputLanguage value) {
            return InnerList.Contains(value);
        }
        
        /// <devdoc>
        /// <para>Copies the <see cref='System.Windows.Forms.InputLanguageCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(InputLanguage[] array, int index) {
            InnerList.CopyTo(array, index);
        }
        
        /// <devdoc>
        ///    <para>Returns the index of a <see cref='System.Windows.Forms.InputLanguage'/> in 
        ///       the <see cref='System.Windows.Forms.InputLanguageCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(InputLanguage value) {
            return InnerList.IndexOf(value);
        }
    }
}
