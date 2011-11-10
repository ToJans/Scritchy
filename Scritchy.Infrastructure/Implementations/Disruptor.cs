using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.Infrastructure.Implementations
{
    // "Muahaha, I've written an LMAX disruptor. I shall ask 1 millon dollars" - Dr. Evil.
    // Not sure whether this will ever be of practical use, but I just felt like it. 
    
    // Completely untested, so do not shoot me if it blows up on yah !!

    // ToJans@Twitter

    public class Disruptor
    {
        // you need to init these fields yourself with the proper values before accessing
        public uint[] ThreadPositions = new uint[MAXTHREADS]; // contains the index where each thread currently is
        public uint[][] ThreadNrsToWaitFor = new uint[MAXTHREADS][]; // contains the different thread numbers each thread number has to wait for
        
        // blah
        public const int MAXTHREADS = 32;
        uint RingIndexMask;
        object[] Items;

        
        uint[] ThreadFreeItemsLeft = new uint[MAXTHREADS]; // contains the amount of items a thread can read before it needs to check other thread indexes

        public Disruptor(uint size)
        {
            var ringsize = 1; // find a ringsize which is a power of 2 with at least size value
            while (size > 0)
            {
                ringsize <<= 1;
                size >>= 1;
            }
            Items = new object[ringsize];
            RingIndexMask = size - 1;
        }

        void DarthLocker(uint ThreadNr)
        {
            var i = ThreadPositions[ThreadNr];
            while (ThreadFreeItemsLeft[ThreadNr] == 0)
            {
                uint itemsleft = RingIndexMask;
                do
                {
                    foreach (var OtherThreadId in ThreadNrsToWaitFor[ThreadNr])
                    {
                        var otheritemsleft = (ThreadPositions[OtherThreadId] - i) & RingIndexMask;
                        if (otheritemsleft < itemsleft)
                            itemsleft = otheritemsleft;
                    }
                } while (itemsleft == 0);

                ThreadFreeItemsLeft[ThreadNr] = itemsleft;
            }
            ThreadFreeItemsLeft[ThreadNr]--;
            ThreadPositions[ThreadNr] = (ThreadPositions[ThreadNr]++) & RingIndexMask;
        }

        public object this[uint ThreadNr]
        {
            get
            {
                if (ThreadNr >= MAXTHREADS) throw new Exception("WTF are you doing mate");
                DarthLocker(ThreadNr);
                return Items[ThreadPositions[ThreadNr]];
            }
            set
            {
                if (ThreadNr >= MAXTHREADS) throw new Exception("WTF are you doing mate");
                DarthLocker(ThreadNr);
                Items[ThreadPositions[ThreadNr]] = value;
            }
        }

    }
}
