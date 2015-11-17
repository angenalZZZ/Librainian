// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Potpourri.cs" was last cleaned by Rick on 2015/10/05 at 5:51 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;

    [DataContract( IsReference = true )]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class Potpourri<TKey> : BetterDisposableClass, IPotpourri<TKey> where TKey : class {

        [DataMember( IsRequired = true )]
        [NotNull]
        protected readonly ConcurrentDictionary<TKey, BigInteger> Container = new ConcurrentDictionary<TKey, BigInteger>();

        [NotNull]
        public String FriendlyName => Types.Name( () => this );

        protected String DebuggerDisplay => $"{this.FriendlyName}({this.Container.Select( pair => pair.Key.ToString() ).ToStrings()}) ";

        public void Add( TKey key, BigInteger count ) {
            if ( Equals( key, default( TKey ) ) ) {
                return;
            }
            this.Container.AddOrUpdate( key: key, addValue: count, updateValueFactory: ( particles, integer ) => integer + count );
        }

        public void Add( KeyValuePair<TKey, BigInteger> keyValuePair ) => this.Add( keyValuePair.Key, keyValuePair.Value );

        public void Clear() => this.Container.Clear();

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Boolean Contains( TKey key ) {
            if ( key == null ) {
                throw new ArgumentNullException( nameof( key ) );
            }
            BigInteger value;
            if ( !this.Container.TryGetValue( key, out value ) ) {
                return false;
            }
            return value > BigInteger.Zero;
        }

        public BigInteger Count() => this.Container.Aggregate( BigInteger.Zero, ( current, kvp ) => current + kvp.Value );

        public BigInteger Count<TParticle>() => this.Container.Where( pair => pair.Key is TParticle )
                                                      .Aggregate( BigInteger.Zero, ( current, kvp ) => current + kvp.Value );

        /// <summary>
        ///     Get all particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, BigInteger>> Get() => this.Container;

        /// <summary>
        ///     Get all particles of type( <see cref="TParticle" />).
        /// </summary>
        /// <typeparam name="TParticle"></typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TParticle, BigInteger>> Get<TParticle>() {
            //var results = this.Container.Where( pair => pair.Key is TParticle );
            var results = this.Container.Cast<KeyValuePair<TParticle, BigInteger>>();
            return results;

            //return results;
        }

        //public IEnumerable<KeyValuePair<TKey, BigInteger>> Get<TCertainType>() {
        //    var keys = this.Container.Keys.Cast<TCertainType>();
        //    foreach ( TKey certainType in keys ) {
        //        if ( certainType != null ) {
        //            yield return new KeyValuePair<TCertainType, BigInteger>( certainType, this.Container[ certainType ] );
        //        }
        //    }
        //    //yield return new KeyValuePair< TCertainType, BigInteger >( result.Key as TCertainType, result.Value );

        //}

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, BigInteger>> GetEnumerator() => this.Container.GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Remove( TKey key, BigInteger count ) {
            var before = this.Count();
            count.Should()
                 .BeGreaterOrEqualTo( before );
            if ( count > before ) {
                count = before; //only remove what is there at the moment.
            }
            var newValue = this.Container.AddOrUpdate( key: key, addValue: 0, updateValueFactory: ( particles, integer ) => integer - count );
            return before != newValue;
        }

        public Boolean RemoveAll( TKey key ) {
            BigInteger value;
            return this.Container.TryRemove( key, out value );
        }

        public void Add( TKey key ) {
            if ( Equals( key, default( TKey ) ) ) {
                return;
            }
            this.Container.AddOrUpdate( key: key, addValue: BigInteger.One, updateValueFactory: ( particles, integer ) => integer + BigInteger.One );
        }

        public void Add( Tuple<TKey, BigInteger> keyValuePair ) => this.Add( keyValuePair.Item1, keyValuePair.Item2 );

    }

}
