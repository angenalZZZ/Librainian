#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/ConcurrentStackNoBlock.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="http://www.coderbag.com/Concurrent-Programming/Building-Concurrent-Stack" />
    public class ConcurrentStackNoBlock<T> {
        private Node _head;

        public ConcurrentStackNoBlock( ) {
            this._head = new Node( item: default(T), next: this._head );
        }

        public int Count { get; private set; }

        public void Add( T item ) => this.Push( item );

        public void Add( IEnumerable<T> items ) => Parallel.ForEach( items, ThreadingExtensions.Parallelism, this.Push );

        public void Add( ParallelQuery<T> items ) => items.ForAll( this.Push );

        public long LongCount( ) => this.Count;

        public void Push( T item ) {
            if ( Equals( default(T), item ) ) {
                return;
            }

            var nodeNew = new Node {
                Item = item
            };

            Node tmp;
            do {
                tmp = this._head;
                nodeNew.Next = tmp;
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( ref this._head, nodeNew, tmp ) != tmp );
#pragma warning restore 420
            ++this.Count;
        }

        public Boolean TryPop( out T result ) {
            result = default(T);

            Node ret;

            do {
                ret = this._head;
                if ( ret.Next == default(Node) ) {

                    //throw new IndexOutOfRangeException( "Stack is empty" );
                    return false;
                }
#pragma warning disable 420
            } while ( Interlocked.CompareExchange( ref this._head, ret.Next, ret ) != ret );
#pragma warning restore 420
            --this.Count;
            result = ret.Item;
            return !Equals( result, default(T) );
        }

        /// <summary>
        /// Attempt two <see cref="TryPop" />
        /// </summary>
        /// <param name="itemOne"></param>
        /// <param name="itemTwo"></param>
        /// <returns></returns>
        public Boolean TryPopPop( out T itemOne, out T itemTwo ) {
            if ( !this.TryPop( out itemOne ) ) {
                itemTwo = default(T);
                return false;
            }
            if ( !this.TryPop( out itemTwo ) ) {
                this.Push( itemOne );
                return false;
            }
            return true;
        }

        internal class Node {
            internal T Item;

            internal Node Next;

            public Node( ) {
            }

            public Node( T item, Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }
}