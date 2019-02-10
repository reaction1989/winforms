﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.ComponentModel;
    using System.Collections;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;

    /// <devdoc>
    /// <para>Manages the collection of System.Windows.Forms.BindingManagerBase
    /// objects for a Win Form.</para>
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged))]
    public class BindingContext : ICollection {

        private Hashtable listManagers;

        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Gets the total number of System.Windows.Forms.BindingManagerBases
        /// objects.
        /// </para>
        /// </devdoc>
        int ICollection.Count {
            get {
                ScrubWeakRefs();
                return listManagers.Count;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Copies the elements of the collection into a specified array, starting
        /// at the collection index.
        /// </para>
        /// </devdoc>
        void ICollection.CopyTo(Array ar, int index)
        {
            ScrubWeakRefs();
            listManagers.CopyTo(ar, index);
        }

        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Gets an enumerator for the collection.
        /// </para>
        /// </devdoc>
        IEnumerator IEnumerable.GetEnumerator()
        {
            ScrubWeakRefs();
            return listManagers.GetEnumerator();
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the collection is read-only.
        ///    </para>
        /// </devdoc>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Gets a value indicating whether the collection is synchronized.
        /// </para>
        /// </devdoc>
        bool ICollection.IsSynchronized {
            get {
                // so the user will know that it has to lock this object
                return false;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        /// <para>Gets an object to use for synchronization (thread safety).</para>
        /// </devdoc>
        object ICollection.SyncRoot {
            get {
                return null;
            }
        }


        /// <devdoc>
        /// <para>Initializes a new instance of the System.Windows.Forms.BindingContext class.</para>
        /// </devdoc>
        public BindingContext() {
            listManagers = new Hashtable();
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the System.Windows.Forms.BindingManagerBase
        ///       associated with the specified data source.
        ///    </para>
        /// </devdoc>
        public BindingManagerBase this[object dataSource] {
            get {
                return this[dataSource, ""];
            }
        }

        /// <devdoc>
        /// <para>Gets the System.Windows.Forms.BindingManagerBase associated with the specified data source and
        ///    data member.</para>
        /// </devdoc>
        public BindingManagerBase this[object dataSource, string dataMember] {
            get {
                return EnsureListManager(dataSource, dataMember);
            }
        }

        /// <devdoc>
        /// Adds the listManager to the collection.  An ArgumentNullException is thrown if this listManager
        /// is null.  An exception is thrown if a listManager to the same target and Property as an existing listManager or
        /// if the listManager's column isn't a valid column given this DataSource.Table's schema.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        internal protected void Add(object dataSource, BindingManagerBase listManager) {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            AddCore(dataSource, listManager);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataSource));
        }

        /// <devdoc>
        /// </devdoc>
        protected virtual void AddCore(object dataSource, BindingManagerBase listManager) {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));
            if (listManager == null)
                throw new ArgumentNullException(nameof(listManager));

            // listManagers[dataSource] = listManager;
            listManagers[GetKey(dataSource, "")] = new WeakReference(listManager, false);
        }


        /// <devdoc>
        ///    <para>
        ///       Occurs when the collection has changed.
        ///    </para>
        /// </devdoc>
        [SRDescription(nameof(SR.collectionChangedEventDescr)), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged {
            /* !!THIS EVENT IS OBSOLETE AND UNUSED!! */
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
            add {
                throw new NotImplementedException();
            }
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
            remove {
            }
        }

        /// <devdoc>
        /// Clears the collection of any bindings.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        internal protected void Clear() {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            ClearCore();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <devdoc>
        ///    <para>
        ///       Clears the collection.
        ///    </para>
        /// </devdoc>
        protected virtual void ClearCore() {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            listManagers.Clear();
        }

        /// <devdoc>
        /// <para>Gets a value indicating whether the System.Windows.Forms.BindingContext
        /// contains the specified
        /// data source.</para>
        /// </devdoc>
        public bool Contains(object dataSource) {
            return Contains(dataSource, "");
        }

        /// <devdoc>
        /// <para>Gets a value indicating whether the System.Windows.Forms.BindingContext
        /// contains the specified data source and data member.</para>
        /// </devdoc>
        public bool Contains(object dataSource, string dataMember) {
            return listManagers.ContainsKey(GetKey(dataSource, dataMember));
        }

        internal HashKey GetKey(object dataSource, string dataMember) {
            return new HashKey(dataSource, dataMember);
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        //
        internal class HashKey {
            WeakReference wRef;
            int dataSourceHashCode;
            string dataMember;

            internal HashKey(object dataSource, string dataMember) {
                if (dataSource == null)
                    throw new ArgumentNullException(nameof(dataSource));
                if (dataMember == null)
                    dataMember = "";
                // The dataMember should be case insensitive.
                // so convert the dataMember to lower case
                //
                this.wRef = new WeakReference(dataSource, false);
                this.dataSourceHashCode = dataSource.GetHashCode();
                this.dataMember = dataMember.ToLower(CultureInfo.InvariantCulture);
            }

            /// <internalonly/>
            /// <devdoc>
            /// </devdoc>
            public override int GetHashCode() {
                return dataSourceHashCode * dataMember.GetHashCode();
            }

            /// <internalonly/>
            /// <devdoc>
            /// </devdoc>
            public override bool Equals(object target) {
                if (target is HashKey) {
                    HashKey keyTarget = (HashKey)target;
                    return wRef.Target == keyTarget.wRef.Target && dataMember == keyTarget.dataMember;
                }
                return false;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        ///    This method is called whenever the collection changes.  Overriders
        ///    of this method should call the base implementation of this method.
        ///    NOTE: This shipped in Everett, so we need to keep it, but we don't do
        ///    anything here.
        /// </devdoc>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent) {
        }

        /// <devdoc>
        /// Removes the given listManager from the collection.
        /// An ArgumentNullException is thrown if this listManager is null.  An ArgumentException is thrown
        /// if this listManager doesn't belong to this collection.
        /// The CollectionChanged event is fired if it succeeds.
        /// </devdoc>
        internal protected void Remove(object dataSource) {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            RemoveCore(dataSource);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataSource));
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected virtual void RemoveCore(object dataSource) {
            /* !!THIS METHOD IS OBSOLETE AND UNUSED!! */
            listManagers.Remove(GetKey(dataSource, ""));
        }

        /// <devdoc>
        ///    Create a suitable binding manager for the specified dataSource/dataMember combination.
        ///    - If one has already been created and cached by this BindingContext, return that instead.
        ///    - If the data source is an ICurrencyManagerProvider, just delegate to the data source.
        /// </devdoc>
        internal BindingManagerBase EnsureListManager(object dataSource, string dataMember) {
            BindingManagerBase bindingManagerBase = null;

            if (dataMember == null)
                dataMember = "";

            // Check whether data source wants to provide its own binding managers
            // (but fall through to old logic if it fails to provide us with one)
            //
            if (dataSource is ICurrencyManagerProvider) {
                bindingManagerBase = (dataSource as ICurrencyManagerProvider).GetRelatedCurrencyManager(dataMember);

                if (bindingManagerBase != null) {
                    return bindingManagerBase;
                }
            }

            // Check for previously created binding manager
            //
            HashKey key = GetKey(dataSource, dataMember);
            WeakReference wRef;
            wRef = listManagers[key] as WeakReference;
            if (wRef != null)
                bindingManagerBase = (BindingManagerBase) wRef.Target;
            if (bindingManagerBase != null) {
                return bindingManagerBase;
            }

            if (dataMember.Length == 0) {
                // No data member specified, so create binding manager directly on the data source
                //
                if (dataSource is IList || dataSource is IListSource) {
                    // IListSource so we can bind the dataGrid to a table and a dataSet
                    bindingManagerBase = new CurrencyManager(dataSource);
                }
                else {
                    // Otherwise assume simple property binding
                    bindingManagerBase = new PropertyManager(dataSource);
                }
            }
            else {
                // Data member specified, so get data source's binding manager, and hook a 'related' binding manager to it
                //
                int lastDot = dataMember.LastIndexOf(".");            
                string dataPath = (lastDot == -1) ? "" : dataMember.Substring(0, lastDot);
                string dataField = dataMember.Substring(lastDot + 1);

                BindingManagerBase formerManager = EnsureListManager(dataSource, dataPath);

                PropertyDescriptor prop = formerManager.GetItemProperties().Find(dataField, true);
                if (prop == null)
                    throw new ArgumentException(string.Format(SR.RelatedListManagerChild, dataField));

                if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                    bindingManagerBase = new RelatedCurrencyManager(formerManager, dataField);
                else
                    bindingManagerBase = new RelatedPropertyManager(formerManager, dataField);
            }

            // if wRef == null, then it is the first time we want this bindingManagerBase: so add it
            // if wRef != null, then the bindingManagerBase was GC'ed at some point: keep the old wRef and change its target
            if (wRef == null)
                listManagers.Add(key, new WeakReference(bindingManagerBase, false));
            else
                wRef.Target = bindingManagerBase;

            ScrubWeakRefs();
            // Return the final binding manager
            return bindingManagerBase;
        }

        // may throw
        private static void CheckPropertyBindingCycles(BindingContext newBindingContext, Binding propBinding) {
            if (newBindingContext == null || propBinding == null)
                return;
            if (newBindingContext.Contains(propBinding.BindableComponent, "")) {
                // this way we do not add a bindingManagerBase to the
                // bindingContext if there isn't one already
                BindingManagerBase bindingManagerBase = newBindingContext.EnsureListManager(propBinding.BindableComponent, "");
                for (int i = 0; i < bindingManagerBase.Bindings.Count; i++) {
                    Binding binding = bindingManagerBase.Bindings[i];
                    if (binding.DataSource == propBinding.BindableComponent) {
                        if (propBinding.BindToObject.BindingMemberInfo.BindingMember.Equals(binding.PropertyName))
                            throw new ArgumentException(string.Format(SR.DataBindingCycle, binding.PropertyName), "propBinding");
                    } else if (propBinding.BindToObject.BindingManagerBase is PropertyManager)
                        CheckPropertyBindingCycles(newBindingContext, binding);
                }
            }
        }

        private void ScrubWeakRefs() {
            ArrayList cleanupList = null;
            foreach (DictionaryEntry de in listManagers) {
                WeakReference wRef = (WeakReference) de.Value;
                if (wRef.Target == null) {
                    if (cleanupList == null) {
                        cleanupList = new ArrayList();
                    }
                    cleanupList.Add(de.Key);
                }
            }

            if (cleanupList != null) {
                foreach (object o in cleanupList) {
                    listManagers.Remove(o);
                }
            }
        }

        /// <devdoc>
        ///     Associates a Binding with a different BindingContext. Intended for use by components that support
        ///     IBindableComponent, to update their Bindings when the value of IBindableComponent.BindingContext
        ///     is changed.
        /// </devdoc>
        public static void UpdateBinding(BindingContext newBindingContext, Binding binding) {
            BindingManagerBase oldManager = binding.BindingManagerBase;
            if (oldManager != null) {
                oldManager.Bindings.Remove(binding);
            }

            if (newBindingContext != null) {
                // we need to first check for cycles before adding this binding to the collection
                // of bindings.
                if (binding.BindToObject.BindingManagerBase is PropertyManager)
                    CheckPropertyBindingCycles(newBindingContext, binding);

                BindToObject bindTo = binding.BindToObject;
                BindingManagerBase newManager = newBindingContext.EnsureListManager(bindTo.DataSource, bindTo.BindingMemberInfo.BindingPath);
                newManager.Bindings.Add(binding);
            }
        }
    }
}
