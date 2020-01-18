// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.
//
// using System.Collections.ObjectModel;
// using System.Diagnostics;
// using System.Diagnostics.CodeAnalysis;
// using System.Runtime.CompilerServices;
// using System.Threading;
//
// namespace System.Collections.Generic
// {
//     // Implements a variable-size List that uses an array of objects to store the
//     // elements. A List has a capacity, which is the allocated length
//     // of the internal array. As elements are added to a List, the capacity
//     // of the List is automatically increased as required by reallocating the
//     // internal array.
//     // 
//     [DebuggerDisplay("Count = {Count}")]
//     [Serializable]
//     [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
//     public class MyList<T> : IList<T>, IList, IReadOnlyList<T>
//     {
//         private const int DefaultCapacity = 4;
//
//         private T[] _items; // Do not rename (binary serialization)
//         private int _size; // Do not rename (binary serialization)
//         private int _version; // Do not rename (binary serialization)
//
//         private static readonly T[] s_emptyArray = new T[0];
//
//         // Constructs a List. The list is initially empty and has a capacity
//         // of zero. Upon adding the first element to the list the capacity is
//         // increased to DefaultCapacity, and then increased in multiples of two
//         // as required.
//         public MyList()
//         {
//             _items = s_emptyArray;
//         }
//
//         // Constructs a List with a given initial capacity. The list is
//         // initially empty, but will have room for the given number of elements
//         // before any reallocations are required.
//         // 
//
//
//         // Constructs a List, copying the contents of the given collection. The
//         // size and capacity of the new list will both be equal to the size of the
//         // given collection.
//         // 
//         public MyList(IEnumerable<T> collection)
//         {
//             if (collection is ICollection<T> c)
//             {
//                 int count = c.Count;
//                 if (count == 0)
//                 {
//                     _items = s_emptyArray;
//                 }
//                 else
//                 {
//                     _items = new T[count];
//                     c.CopyTo(_items, 0);
//                     _size = count;
//                 }
//             }
//             else
//             {
//                 _size = 0;
//                 _items = s_emptyArray;
//                 using (IEnumerator<T> en = collection!.GetEnumerator())
//                 {
//                     while (en.MoveNext())
//                     {
//                         Add(en.Current);
//                     }
//                 }
//             }
//         }
//
//         // Gets and sets the capacity of this list.  The capacity is the size of
//         // the internal array used to hold items.  When set, the internal 
//         // array of the list is reallocated to the given capacity.
//         // 
//         public int Capacity
//         {
//             get { return _items.Length; }
//             set
//             {
//                 if (value != _items.Length)
//                 {
//                     if (value > 0)
//                     {
//                         T[] newItems = new T[value];
//                         if (_size > 0)
//                         {
//                             Array.Copy(_items, 0, newItems, 0, _size);
//                         }
//
//                         _items = newItems;
//                     }
//                     else
//                     {
//                         _items = s_emptyArray;
//                     }
//                 }
//             }
//         }
//
//         // Read-only property describing how many elements are in the List.
//         public bool Remove(T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void CopyTo(Array array, int index)
//         {
//             throw new NotImplementedException();
//         }
//
//         public int Count => _size;
//
//         void IList.RemoveAt(int index)
//         {
//             throw new NotImplementedException();
//         }
//
//         bool IList.IsFixedSize => false;
//
//         // Is this List read-only?
//         bool ICollection<T>.IsReadOnly => false;
//
//         object? IList.this[int index]
//         {
//             get => null;
//             set { }
//         }
//
//         bool IList.IsReadOnly => false;
//
//         // Is this List synchronized (thread-safe)?
//         bool ICollection.IsSynchronized => false;
//
//         // Synchronization root for this object.
//         object ICollection.SyncRoot => this;
//
//         // Sets or Gets the element at the given index.
//         public int IndexOf(T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Insert(int index, T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Remove(object? value)
//         {
//             throw new NotImplementedException();
//         }
//
//         void IList<T>.RemoveAt(int index)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T this[int index]
//         {
//             get
//             {
//                 // Following trick can reduce the range check by one
//
//                 return _items[index];
//             }
//
//             set
//             {
//                 _items[index] = value;
//                 _version++;
//             }
//         }
//
//         // Adds the given object to the end of this list. The size of the list is
//         // increased by one. If required, the capacity of the list is doubled
//         // before adding the new element.
//         //
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void Add(T item)
//         {
//             _version++;
//             T[] array = _items;
//             int size = _size;
//             if ((uint) size < (uint) array.Length)
//             {
//                 _size = size + 1;
//                 array[size] = item;
//             }
//             else
//             {
//                 AddWithResize(item);
//             }
//         }
//
//         public int Add(object? value)
//         {
//             throw new NotImplementedException();
//         }
//
//         void IList.Clear()
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Contains(object? value)
//         {
//             throw new NotImplementedException();
//         }
//
//         public int IndexOf(object? value)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Insert(int index, object? value)
//         {
//             throw new NotImplementedException();
//         }
//
//         void ICollection<T>.Clear()
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Contains(T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void CopyTo(T[] array, int arrayIndex)
//         {
//             throw new NotImplementedException();
//         }
//
//         // Non-inline from List.Add to improve its code quality as uncommon path
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         private void AddWithResize(T item)
//         {
//             int size = _size;
//             EnsureCapacity(size + 1);
//             _size = size + 1;
//             _items[size] = item;
//         }
//
//         private void EnsureCapacity(int min)
//         {
//             if (_items.Length < min)
//             {
//                 int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
//                 // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
//                 // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
//                 if ((uint) newCapacity > int.MaxValue) newCapacity = int.MaxValue;
//                 if (newCapacity < min) newCapacity = min;
//                 Capacity = newCapacity;
//             }
//         }
//
//         public IEnumerator<T> GetEnumerator()
//         {
//             throw new NotImplementedException();
//         }
//
//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//     }
// }