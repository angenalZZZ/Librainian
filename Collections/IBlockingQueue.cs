// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IBlockingQueue.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A <see cref="IQueue{T}" /> that additionally supports operations that wait for the queue to
    ///     become non-empty when retrieving an element, and wait for space to become available in the
    ///     queue when storing an element.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="IBlockingQueue{T}" /> methods come in four forms, with different ways of handling
    ///         operations that cannot be satisfied immediately, but may be satisfied at some point in the
    ///         future:
    ///         <list type="bullet">
    ///             <item>one throws an exception,</item>
    ///             <item>
    ///                 the second returns a
    ///                 special value (either <c>default(T)</c> or <see langword="false" />, depending on the
    ///                 operation) ,
    ///             </item>
    ///             <item>
    ///                 the third blocks the current thread indefinitely until the
    ///                 operation can succeed, and
    ///             </item>
    ///             <item>
    ///                 the fourth blocks for only a given maximum time
    ///                 limit before giving up.
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         A <see cref="IBlockingQueue{T}" /> may be capacity bounded. At any given time it may have a
    ///         <see cref="IQueue{T}.RemainingCapacity" /> beyond which no additional elements can be
    ///         <see cref="IBlockingQueue{T}.Put(T)" /> without blocking. A <see cref="IBlockingQueue{T}" />
    ///         without any intrinsic capacity constraints always reports a remaining capacity of
    ///         <see cref="System.Int32.MaxValue" />.
    ///     </para>
    ///     <para>
    ///         <see cref="IBlockingQueue{T}" /> implementations are designed to be used primarily for
    ///         producer-consumer queues, but additionally support the <see cref="ICollection{T}" />
    ///         interface. So, for example, it is possible to remove an arbitrary element from a queue using
    ///         <see cref="ICollection{T}.Remove(T)" />. However, such operations are in general <b>not</b>
    ///         performed very efficiently, and are intended for only occasional use, such as when a queued
    ///         message is cancelled.
    ///     </para>
    ///     <para>
    ///         A <see cref="IBlockingQueue{T}" /> does <b>not</b> intrinsically support any kind of 'close'
    ///         or 'shutdown' operation to indicate that no more items will be added. The needs and usage of
    ///         such features tend to be implementation-dependent. For example, a common tactic is for
    ///         producers to insert special <b>end-of-stream</b> or <b>poison</b> objects, that are
    ///         interpreted accordingly when taken by consumers.
    ///     </para>
    ///     <para>
    ///         A <see cref="IBlockingQueue{T}" /> can safely be used with multiple producers and multiple consumers.
    ///     </para>
    ///     <example>
    ///         Usage example, based on a typical producer-consumer scenario.
    ///         TODO: Convert non-generic example below to using generic version
    ///         <code>
    ///  class Producer : IRunnable {
    ///         private IBlockingQueue queue;
    ///      Producer(IBlockingQueue q) { queue = q; }
    ///      public void Run() {
    ///          try {
    ///              while (true) {
    ///                  queue.Put(produce());
    ///              }
    ///          } catch (InterruptedException ex) {
    ///              ... handle ...
    ///          }
    ///      }
    ///      Object Produce() { ... }
    ///  }
    ///
    ///  class Consumer : IRunnable {
    ///         private IBlockingQueue queue;
    ///      Consumer(IBlockingQueue q) { queue = q; }
    ///      public void Run() {
    ///          try {
    ///              while (true) { Consume(queue.Take()); }
    ///          } catch (InterruptedException ex) { ... handle ...}
    ///      }
    ///      void Consume(object x) { ... }
    ///  }
    ///
    ///  class Setup {
    ///         void Main() {
    ///          IBlockingQueue q = new SomeQueueImplementation();
    ///          Producer p = new Producer(q);
    ///          Consumer c1 = new Consumer(q);
    ///          Consumer c2 = new Consumer(q);
    ///          new Thread(new ThreadStart(p.Run)).Start();
    ///          new Thread(new ThreadStart(c1.Run)).Start();
    ///          new Thread(new ThreadStart(c2.Run)).Start();
    ///      }
    ///  }
    /// </code>
    ///     </example>
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio(.NET)</author>
    /// <author>Kenneth Xu</author>
    internal interface IBlockingQueue<T> : IQueue<T> //JDK_1_6
    {
		/// <summary>
		///     Removes all available elements from this queue and adds them to the given collection.
		/// </summary>
		/// <remarks>
		///     This operation may be more efficient than repeatedly polling this queue. A failure
		///     encountered while attempting to add elements to collection
		///     <paramref name="collection" /> may result in elements being in neither, either or both
		///     collections when the associated exception is thrown. Attempts to drain a queue to itself
		///     result in <see cref="System.ArgumentException" />. Further, the behavior of this
		///     operation is undefined if the specified collection is modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">the collection to transfer elements into</param>
		/// <returns>the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		///     If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		///     If the class of the supplied <paramref name="collection" /> prevents it from being used
		///     for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     If the specified collection is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		///     If <paramref name="collection" /> represents the queue itself.
		/// </exception>
		/// <seealso cref="IQueue{T}.Drain(System.Action{T})" />
		/// <seealso cref="DrainTo(ICollection{T},Int32)" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T},Int32)" />
		Int32 DrainTo( ICollection<T> collection );

		/// <summary>
		///     Removes all available elements that meet the criteria defined by
		///     <paramref name="predicate" /> from this queue and adds them to the given collection.
		/// </summary>
		/// <remarks>
		///     This operation may be more efficient than repeatedly polling this queue. A failure
		///     encountered while attempting to add elements to collection
		///     <paramref name="collection" /> may result in elements being in neither, either or both
		///     collections when the associated exception is thrown. Attempts to drain a queue to itself
		///     result in <see cref="System.ArgumentException" />. Further, the behavior of this
		///     operation is undefined if the specified collection is modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">The collection to transfer elements into</param>
		/// <param name="predicate">The criteria to filter the elements</param>
		/// <returns>the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		///     If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		///     If the class of the supplied <paramref name="collection" /> prevents it from being used
		///     for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     If the specified collection is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		///     If <paramref name="collection" /> represents the queue itself.
		/// </exception>
		/// <seealso cref="IQueue{T}.Drain(System.Action{T})" />
		/// <seealso cref="DrainTo(ICollection{T},Int32)" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T},Int32)" />
		Int32 DrainTo( ICollection<T> collection, Predicate<T> predicate );

		/// <summary>
		///     Removes at most the given number of available elements from this queue and adds them to
		///     the given collection.
		/// </summary>
		/// <remarks>
		///     This operation may be more efficient than repeatedly polling this queue. A failure
		///     encountered while attempting to add elements to collection
		///     <paramref name="collection" /> may result in elements being in neither, either or both
		///     collections when the associated exception is thrown. Attempts to drain a queue to itself
		///     result in <see cref="System.ArgumentException" />. Further, the behavior of this
		///     operation is undefined if the specified collection is modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">the collection to transfer elements into</param>
		/// <param name="maxElements">the maximum number of elements to transfer</param>
		/// <returns>the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		///     If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		///     If the class of the supplied <paramref name="collection" /> prevents it from being used
		///     for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     If the specified collection is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		///     If <paramref name="collection" /> represents the queue itself.
		/// </exception>
		/// <seealso cref="DrainTo(ICollection{T})" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T})" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T},Int32)" />
		Int32 DrainTo( ICollection<T> collection, Int32 maxElements );

		/// <summary>
		///     Removes at most the given number of available elements that meet the criteria defined by
		///     <paramref name="predicate" /> from this queue and adds them to the given collection.
		/// </summary>
		/// <remarks>
		///     This operation may be more efficient than repeatedly polling this queue. A failure
		///     encountered while attempting to add elements to collection
		///     <paramref name="collection" /> may result in elements being in neither, either or both
		///     collections when the associated exception is thrown. Attempts to drain a queue to itself
		///     result in <see cref="System.ArgumentException" />. Further, the behavior of this
		///     operation is undefined if the specified collection is modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">the collection to transfer elements into</param>
		/// <param name="maxElements">the maximum number of elements to transfer</param>
		/// <param name="predicate">The criteria to filter the elements.</param>
		/// <returns>the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		///     If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		///     If the class of the supplied <paramref name="collection" /> prevents it from being used
		///     for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     If the specified collection is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		///     If <paramref name="collection" /> represents the queue itself.
		/// </exception>
		/// <seealso cref="DrainTo(ICollection{T})" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T})" />
		/// <seealso cref="IQueue{T}.Drain(System.Action{T},Int32)" />
		Int32 DrainTo( ICollection<T> collection, Int32 maxElements, Predicate<T> predicate );

        /// <summary>
        ///     Inserts the specified element into this queue, waiting up to the specified wait time if
        ///     necessary for space to become available.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="duration">How long to wait before giving up.</param>
        /// <returns>
        ///     <see langword="true" /> if successful, or <see langword="false" /> if the specified
        ///     waiting time elapses before space is available.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     If the specified element is <see langword="null" /> and this queue does not permit
        ///     <c>null</c> elements.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     If some property of the supplied <paramref name="element" /> prevents it from being
        ///     added to this queue.
        /// </exception>
        Boolean Offer( T element, TimeSpan duration );

        /// <summary>
        ///     Retrieves and removes the head of this queue, waiting up to the specified wait time if
        ///     necessary for an element to become available.
        /// </summary>
        /// <param name="element">
        ///     Set to the head of this queue. <c>default(T)</c> if queue is empty.
        /// </param>
        /// <param name="duration">How long to wait before giving up.</param>
        /// <returns>
        ///     <c>false</c> if the queue is still empty after waited for the time specified by the
        ///     <paramref name="duration" />. Otherwise <c>true</c>.
        /// </returns>
        Boolean Poll( TimeSpan duration, out T element );

        /// <summary>
        ///     Inserts the specified element into this queue, waiting if necessary for space to become available.
        /// </summary>
        /// <param name="element">the element to add</param>
        /// <exception cref="ArgumentException">if interrupted while waiting.</exception>
        /// <exception cref="System">
        ///     If some property of the supplied <paramref name="element" /> prevents it from being
        ///     added to this queue.
        /// </exception>
        void Put( T element );

        /// <summary>
        ///     Retrieves and removes the head of this queue, waiting if necessary until an element
        ///     becomes available.
        /// </summary>
        /// <returns>the head of this queue</returns>
        T Take();
    }
}