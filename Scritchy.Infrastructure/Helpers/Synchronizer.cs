using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// from http://stackoverflow.com/questions/781189/how-to-lock-on-an-integer-in-c/7395638#7395638

namespace Scritchy.Infrastructure.Helpers
{
    /// <summary>
    /// Provides a way to lock a resource based on a value (such as an ID or path).
    /// </summary>
    public class Synchronizer<T>
    {

        private Dictionary<T, SyncLock> mLocks = new Dictionary<T, SyncLock>();
        private object mLock = new object();

        /// <summary>
        /// Returns an object that can be used in a lock statement. Ex: lock(MySync.Lock(MyValue)) { ... }
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyncLock Lock(T value)
        {
            lock (mLock)
            {
                SyncLock theLock;
                if (mLocks.TryGetValue(value, out theLock))
                    return theLock;

                theLock = new SyncLock(value, this);
                mLocks.Add(value, theLock);
                return theLock;
            }
        }

        /// <summary>
        /// Unlocks the object. Called from Lock.Dispose.
        /// </summary>
        /// <param name="theLock"></param>
        public void Unlock(SyncLock theLock)
        {
            mLocks.Remove(theLock.Value);
        }

        /// <summary>
        /// Represents a lock for the Synchronizer class.
        /// </summary>
        public class SyncLock
            : IDisposable
        {

            /// <summary>
            /// This class should only be instantiated from the Synchronizer class.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="sync"></param>
            internal SyncLock(T value, Synchronizer<T> sync)
            {
                Value = value;
                Sync = sync;
            }

            /// <summary>
            /// Makes sure the lock is removed.
            /// </summary>
            public void Dispose()
            {
                Sync.Unlock(this);
            }

            /// <summary>
            /// Gets the value that this lock is based on.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// Gets the synchronizer this lock was created from.
            /// </summary>
            private Synchronizer<T> Sync { get; set; }

        }

    }
}
