//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using Spring.Threading.Future;


//namespace Spring.Threading.Collections {
//    /// <summary>
//    /// An unbounded <see cref="IBlockingQueue{T}"/> of
//    /// <see cref="IDelayed"/> elements, in which an element can only be taken
//    /// when its delay has expired. 
//    /// </summary>
//    /// <author>Doug Lea</author>
//    /// <author>Griffin Caprio (.NET)</author>
//    public class DelayQueue<T> : AbstractQueue<T>, IBlockingQueue<T> where T : IDelayed {
//        [NonSerialized]
//        private object lockObject = new object();

//        private readonly PriorityQueue<T> _queue = new PriorityQueue<T>();

//        /// <summary>
//        /// Creates a new, empty <see cref="DelayQueue{T}"/>
//        /// </summary>
//        public DelayQueue() { }

//        /// <summary>
//        ///Creates a <see cref="DelayQueue{T}"/> initially containing the elements of the
//        ///given collection of <see cref="IDelayed"/> instances.
//        /// </summary>
//        /// <param name="collection">collection of elements to populate queue with.</param>
//        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
//        /// <exception cref="NullReferenceException">if any of the elements of the collection are null</exception>
//        public DelayQueue(ICollection<T> collection) {
//            AddAll(collection);
//        }

//        /// <summary>
//        /// Inserts the specified element into this delay queue.
//        /// </summary>
//        /// <param name="element">element to add</param>
//        /// <returns><see lang="true"/></returns>
//        /// <exception cref="NullReferenceException">if the specified element is <see lang="null"/></exception>
//        public override bool Offer(T element) {
//            lock(lockObject) {
//                Object first = _queue.Peek();
//                _queue.Offer(element);
//                if(first == null || ((IDelayed)element).CompareTo(first) < 0) {
//                    Monitor.PulseAll(lockObject);
//                }
//                return true;
//            }
//        }

//        /// <summary>
//        ///	Inserts the specified element into this delay queue. As the queue is
//        ///	unbounded this method will never block.
//        /// </summary>
//        /// <param name="element">element to add</param>
//        /// <exception cref="NullReferenceException">if the element is <see lang="null"/></exception>
//        public void Put(T element) {
//            Offer(element);
//        }

//        /// <summary>
//        /// Returns the capacity of this queue. Since this is a unbounded queue, <see cref="int.MaxValue"/> is returned.
//        /// </summary>
//        public override int Capacity {
//            get { return Int32.MaxValue; }
//        }

//        #region IBlockingQueue Members

//        /// <summary> 
//        /// Inserts the specified element into this queue, waiting up to the
//        /// specified wait time if necessary for space to become available.
//        /// </summary>
//        /// <param name="objectToAdd">the element to add</param>
//        /// <param name="duration">how long to wait before giving up</param>
//        /// <returns> <see lang="true"/> if successful, or <see lang="false"/> if
//        /// the specified waiting time elapses before space is available
//        /// </returns>
//        /// <exception cref="System.InvalidOperationException">
//        /// If the element cannot be added at this time due to capacity restrictions.
//        /// </exception>
//        /// <exception cref="System.InvalidCastException">
//        /// If the class of the supplied <paramref name="objectToAdd"/> prevents it
//        /// from being added to this queue.
//        /// </exception>
//        /// <exception cref="System.ArgumentNullException">
//        /// If the specified element is <see lang="null"/> and this queue does not
//        /// permit <see lang="null"/> elements.
//        /// </exception>
//        /// <exception cref="System.ArgumentException">
//        /// If some property of the supplied <paramref name="objectToAdd"/> prevents
//        /// it from being added to this queue.
//        /// </exception>
//        public bool Offer(T objectToAdd, TimeSpan duration) {
//            return Offer(objectToAdd);
//        }

//        /// <summary> 
//        /// Retrieves and removes the head of this queue, waiting if necessary
//        /// until an element becomes available and/or expired.
//        /// </summary>
//        /// <returns> the head of this queue</returns>
//        public T Take() {
//            lock(lockObject) {
//                for(; ; ) {
//                    object first = _queue.Peek();
//                    if(first == null) {
//                        Monitor.Wait(lockObject);
//                    }
//                    else {
//                        TimeSpan delay = ((IDelayed)first).GetRemainingDelay();
//                        if(delay.Ticks > 0) {
//                            Monitor.Wait(lockObject, delay);
//                        }
//                        else {
//                            T x = _queue.Poll();
//                            //Debug.Assert(x != null);
//                            if(_queue.Count != 0) {
//                                Monitor.PulseAll(lockObject);
//                            }
//                            return x;
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary> 
//        /// Retrieves and removes the head of this queue
//        /// or returns <see lang="null"/> if this queue is empty or if the head has not expired.
//        /// </summary>
//        /// <returns> 
//        /// The head of this queue, or <see lang="null"/> if this queue is empty or if the head has not expired.
//        /// </returns>
//        public override T Poll() {
//            lock(lockObject) {
//                if(IsEmpty)
//                    return default(T);
//                T first = _queue.Peek();
//                if(((IDelayed)first).GetRemainingDelay().Ticks > 0) {
//                    return default(T);
//                }

//                Debug.Assert(!IsEmpty);
//                T x = _queue.Poll();

//                if(_queue.Count != 0) {
//                    Monitor.PulseAll(lockObject);
//                }
//                return x;
//            }
//        }

//        /// <summary> 
//        /// Retrieves and removes the head of this queue, waiting if necessary
//        /// until an element with an expired delay is available on this queue,
//        /// or the specified wait time expires.
//        /// </summary>
//        /// <param name="duration">how long to wait before giving up</param>
//        /// <returns> 
//        /// the head of this queue, or <see lang="null"/> if the
//        /// specified waiting time elapses before an element is available
//        /// </returns>
//        public T Poll(TimeSpan duration) {
//            lock(lockObject) {
//                DateTime deadline = DateTime.Now.Add(duration);
//                for(; ; ) {
//                    T first = _queue.Peek();
//                    if(first == null) {
//                        if(duration.Ticks <= 0) {
//                            return default(T); // null;
//                        }
//                        else {
//                            Monitor.Wait(lockObject, duration);
//                            duration = deadline.Subtract(DateTime.Now);
//                        }
//                    }
//                    else {
//                        TimeSpan delay = ((IDelayed)first).GetRemainingDelay();
//                        if(delay.Ticks > 0) {
//                            if(delay > duration) {
//                                delay = duration;
//                            }
//                            Monitor.Wait(lockObject, delay);
//                            duration = deadline.Subtract(DateTime.Now);
//                        }
//                        else {
//                            T x = _queue.Poll();
//                            Debug.Assert(x != null);
//                            if(_queue.Count != 0) {
//                                Monitor.PulseAll(lockObject);
//                            }
//                            return x;
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary> 
//        /// Returns the number of additional elements that this queue can ideally
//        /// (in the absence of memory or resource constraints) accept without
//        /// blocking, or <see cref="System.Int32.MaxValue"/> if there is no intrinsic
//        /// limit.
//        /// 
//        /// <p/>
//        /// Note that you <b>cannot</b> always tell if an attempt to insert
//        /// an element will succeed by inspecting <see cref="Spring.Threading.Collections.IBlockingQueue.RemainingCapacity"/>
//        /// because it may be the case that another thread is about to
//        /// insert or remove an element.
//        /// </summary>
//        /// <returns> the remaining capacity</returns>
//        public int RemainingCapacity {
//            get { return Int32.MaxValue; }
//        }

//        /// <summary> 
//        /// Removes all available elements from this queue and adds them
//        /// to the given collection.  
//        /// </summary>
//        /// <remarks>
//        /// This operation may be more
//        /// efficient than repeatedly polling this queue.  A failure
//        /// encountered while attempting to add elements to
//        /// collection <paramref name="collection"/> may result in elements being in neither,
//        /// either or both collections when the associated exception is
//        /// thrown.  Attempts to drain a queue to itself result in
//        /// <see cref="System.ArgumentException"/>. Further, the behavior of
//        /// this operation is undefined if the specified collection is
//        /// modified while the operation is in progress.
//        /// </remarks>
//        /// <param name="collection">the collection to transfer elements into</param>
//        /// <returns> the number of elements transferred</returns>
//        /// <exception cref="System.InvalidOperationException">
//        /// If the queue cannot be drained at this time.
//        /// </exception>
//        /// <exception cref="System.InvalidCastException">
//        /// If the class of the supplied <paramref name="collection"/> prevents it
//        /// from being used for the elemetns from the queue.
//        /// </exception>
//        /// <exception cref="System.ArgumentNullException">
//        /// If the specified collection is <see lang="null"/>.
//        /// </exception>
//        /// <exception cref="System.ArgumentException">
//        /// If <paramref name="collection"/> represents the queue itself.
//        /// </exception>
//        public int DrainTo(ICollection<T> collection) {
//            if(null == collection) {
//                throw new ArgumentNullException("collection", "Collection cannot be null.");
//            }
//            if(this == collection) {
//                throw new ArgumentException("Cannot drain queue to itself.");
//            }
//            lock(lockObject) {
//                int n = 0;
//                for(; ; ) {
//                    Object first = _queue.Peek();
//                    if(first == null || ((IDelayed)first).GetRemainingDelay().Ticks > 0) {
//                        break;
//                    }
//                    collection.Add(_queue.Poll());
//                    ++n;
//                }
//                if(n > 0) {
//                    Monitor.PulseAll(lockObject);
//                }
//                return n;
//            }
//        }

//        /// <summary> Removes at most the given number of available elements from
//        /// this queue and adds them to the given collection.  
//        /// </summary>
//        /// <remarks> 
//        /// This operation may be more
//        /// efficient than repeatedly polling this queue.  A failure
//        /// encountered while attempting to add elements to
//        /// collection <paramref name="collection"/> may result in elements being in neither,
//        /// either or both collections when the associated exception is
//        /// thrown.  Attempts to drain a queue to itself result in
//        /// <see cref="System.ArgumentException"/>. Further, the behavior of
//        /// this operation is undefined if the specified collection is
//        /// modified while the operation is in progress.
//        /// </remarks>
//        /// <param name="collection">the collection to transfer elements into</param>
//        /// <param name="maxElements">the maximum number of elements to transfer</param>
//        /// <returns> the number of elements transferred</returns>
//        /// <exception cref="System.InvalidOperationException">
//        /// If the queue cannot be drained at this time.
//        /// </exception>
//        /// <exception cref="System.InvalidCastException">
//        /// If the class of the supplied <paramref name="collection"/> prevents it
//        /// from being used for the elemetns from the queue.
//        /// </exception>
//        /// <exception cref="System.ArgumentNullException">
//        /// If the specified collection is <see lang="null"/>.
//        /// </exception>
//        /// <exception cref="System.ArgumentException">
//        /// If <paramref name="collection"/> represents the queue itself.
//        /// </exception>
//        public int DrainTo(ICollection<T> collection, int maxElements) {
//            if(null == collection) {
//                throw new ArgumentNullException("collection", "Collection cannot be null.");
//            }
//            if(this == collection) {
//                throw new ArgumentException("Cannot drain queue to itself.");
//            }
//            if(maxElements <= 0) {
//                return 0;
//            }
//            lock(lockObject) {
//                int n = 0;
//                while(n < maxElements) {
//                    Object first = _queue.Peek();
//                    if(first == null || ((IDelayed)first).GetRemainingDelay().Ticks > 0) {
//                        break;
//                    }
//                    collection.Add(_queue.Poll());
//                    ++n;
//                }
//                if(n > 0) {
//                    Monitor.PulseAll(lockObject);
//                }
//                return n;
//            }
//        }

//        #endregion

//        /// <summary> 
//        /// Removes a single instance of the specified element from this
//        /// queue, if it is present, whether or not it has expired.
//        /// </summary>
//        public override bool Remove(T o) {
//            lock(lockObject) {
//                return _queue.Remove(o);
//            }
//        }

//        /// <summary> 
//        /// Retrieves, but does not remove, the head of this queue,
//        /// or returns <see lang="null"/> if this queue is empty.
//        /// </summary>
//        /// <returns> 
//        /// The head of this queue, or <see lang="null"/> if this queue is empty.
//        /// </returns>
//        public override T Peek() {
//            lock(lockObject) {
//                return _queue.Peek();
//            }
//        }

//        /// <summary> 
//        /// Inserts the specified element into this delay queue.
//        /// </summary>
//        ///
//        /// <param name="element">element to add</param>
//        /// <returns><see lang="true"/></returns>
//        /// <exception cref="ArgumentNullException">if the element is <see lang="null"/></exception>
//        public override void Add(T element) {
//            Offer(element);
//        }

//        #region ICollection Members

//        /// <summary>
//        ///When implemented by a class, gets an object that can be used to synchronize access to the ICollection.  For this implementation,
//        ///always return null, indicating the array is already synchronized.
//        /// </summary>
//        public override object SyncRoot {
//            get { return null; }
//        }

//        /// <summary>
//        /// When implemented by a class, gets a value indicating whether access to the ICollection is synchronized (thread-safe).
//        /// </summary>
//        public override Boolean IsSynchronized {
//            get { return true; }
//        }

//        /// <summary>
//        /// Returns the current number of elements in this queue.
//        /// </summary>
//        public override int Count {
//            get {
//                lock(lockObject) {
//                    return _queue.Count;
//                }
//            }
//        }
//        /// <summary>
//        /// Returns <see lang="true"/> if there are no elements in the <see cref="IQueue"/>, <see lang="false"/> otherwise.
//        /// </summary>
//        public override bool IsEmpty {
//            get { return _queue.Count == 0; }
//        }

//        /// <summary>
//        /// When implemented by a class, copies the elements of the ICollection to an Array, starting at a particular Array index.
//        /// </summary>
//        /// <param name="targetArray">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
//        /// <param name="index">The zero-based index in array at which copying begins. </param>
//        public override void CopyTo(T[] targetArray, Int32 index) {
//            if(null == targetArray) throw new ArgumentNullException("targetArray", "destination array is null");
//            lock(lockObject) {
//                int size = _queue.Count;
//                if(targetArray.Length < size) {
//                    targetArray = new T[size];
//                }
//                int k = 0;
//                foreach(T currentItem in _queue) {
//                    targetArray.SetValue(currentItem, k++);
//                }
//            }
//        }

//        #endregion

//        /// <summary> 
//        /// Removes all of the elements from this queue.
//        /// </summary>
//        /// <remarks>
//        /// <p>
//        /// The queue will be empty after this call returns.
//        /// </p>
//        /// <p>
//        /// This implementation repeatedly invokes
//        /// <see cref="Spring.Collections.AbstractQueue.Poll()"/> until it
//        /// returns <see lang="null"/>.
//        /// </p>
//        /// </remarks>
//        public override void Clear() {
//            lock(lockObject) {
//                _queue.Clear();
//            }
//        }

//        #region IEnumerable Members
//        /// <summary>
//        /// Gets the <see cref="IEnumerator"/> for this queue.
//        /// </summary>
//        /// <returns><see cref="IEnumerator"/></returns>
//        public override IEnumerator<T> GetEnumerator() {
//            return _queue.GetEnumerator();
//        }

//        #endregion

//        public override bool Contains(T item) {
//            throw new NotImplementedException();
//        }
//    }
//}