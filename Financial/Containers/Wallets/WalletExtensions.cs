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
// "Librainian/WalletExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Containers.Wallets {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Currency;
    using Currency.BankNotes;
    using Currency.Coins;
    using Extensions;
    using JetBrains.Annotations;
    using NUnit.Framework;
    using Threading;

    public static class WalletExtensions {

        static WalletExtensions() {
            foreach ( var denomination in typeof( IBankNote ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
                PossibleDenominations.Add( denomination );
            }
            foreach ( var denomination in typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
                PossibleDenominations.Add( denomination );
            }
        }

        [NotNull]
        public static HashSet<IDenomination> PossibleDenominations { get; } = new HashSet<IDenomination>();

        /// <summary>
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="message"></param>
        /// <remarks>Performs locks on the internal tables.</remarks>
        /// <returns>Returns the new quantity.</returns>
        public static Boolean Deposit( [NotNull] this Wallet wallet, TransactionMessage message ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
			if ( message.Denomination is IBankNote bankNote ) {
				return wallet.Deposit( bankNote, message.Quantity );
			}

			if ( message.Denomination is ICoin coin ) {
				return wallet.Deposit( coin, message.Quantity ) > Decimal.Zero;
			}

			throw new NotImplementedException( $"Unknown denomination {message.Denomination}" );
        }

        /// <summary>
        ///     Deposit <paramref name="bankNotes" /> and <paramref name="coins" /> into this wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="bankNotes"></param>
        /// <param name="coins"></param>
        public static void Deposit( [NotNull] this Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IBankNote, UInt64>> bankNotes = null, IEnumerable<KeyValuePair<ICoin, UInt64>> coins = null ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            bankNotes = bankNotes ?? Enumerable.Empty<KeyValuePair<IBankNote, UInt64>>();
            foreach ( var pair in bankNotes ) {
                wallet.Deposit( denomination: pair.Key, quantity: pair.Value );
            }

            coins = coins ?? Enumerable.Empty<KeyValuePair<ICoin, UInt64>>();
            foreach ( var pair in coins ) {
                wallet.Deposit( denomination: pair.Key, quantity: pair.Value );
            }
        }

        public static void Fund( [NotNull] this Wallet wallet, [CanBeNull] params KeyValuePair<IDenomination, UInt64>[] sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            wallet.Fund( sourceAmounts.AsEnumerable() );
        }

        public static void Fund( [NotNull] this Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            if ( null == sourceAmounts ) {
                return;
            }
            Parallel.ForEach( sourceAmounts, pair => wallet.Deposit( pair.Key, pair.Value ) );
        }

        /// <summary>
        ///     Adds the optimal amount of <see cref="IBankNote" /> and <see cref="ICoin" />. Returns
        ///     any unused portion of the money (fractions of the smallest <see cref="ICoin" />).
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task<Decimal> Fund( [NotNull] this Wallet wallet, Decimal amount ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            var leftOverFund = Decimal.Zero;
            var notesAndCoins = amount.ToOptimal( ref leftOverFund );
            await StartDeposit( wallet, notesAndCoins );
            return leftOverFund;
        }

        /// <summary>
        ///     Create a TPL dataflow task for depositing large volumes of money.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="sourceAmounts"></param>
        /// <returns></returns>
        public static async Task StartDeposit( [NotNull] Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            sourceAmounts = sourceAmounts ?? Enumerable.Empty<KeyValuePair<IDenomination, UInt64>>();
            var actionBlock = new ActionBlock<KeyValuePair<IDenomination, UInt64>>( pair => wallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeSensible );
            Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
            actionBlock.Complete();
            await actionBlock.Completion;
        }

        /// <summary>
        ///     Transfer everything FROM the <paramref name="source" /><see cref="Wallet" /> into this <paramref name="target" />
        ///     <see cref="Wallet" />.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [NotNull]
        public static Task<ConcurrentDictionary<IDenomination, UInt64>> StartTransfer( [CanBeNull] this Wallet source, [CanBeNull] Wallet target ) => Task.Run( () => {
            if ( null == source || null == target ) {
                return new ConcurrentDictionary<IDenomination, UInt64>();
            }

            return new ConcurrentDictionary<IDenomination, UInt64>( Transfer( source, target ) );
        } );

        /// <summary>
        ///     Given the <paramref name="amount" />, return the optimal amount of
        ///     <see cref="IBankNote" /> and <see cref="ICoin" /> ( <see cref="Wallet.Total" />) it
        ///     would take to <see cref="Wallet" /> the <paramref name="amount" />.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="leftOverAmount">
        ///     Fractions of Dollars/Pennies not accounted for. OfficeSpace, Superman III"...
        /// </param>
        /// <returns></returns>
        public static Dictionary<IDenomination, UInt64> ToOptimal( this Decimal amount, ref Decimal leftOverAmount ) {
            var denominations = new List<IDenomination>( PossibleDenominations );
            var optimal = denominations.ToDictionary<IDenomination, IDenomination, UInt64>( denomination => denomination, denomination => 0 );

            leftOverAmount += amount;

            while ( leftOverAmount > Decimal.Zero && denominations.Any() ) {
                var highestBill = denominations.OrderByDescending( denomination => denomination.FaceValue ).First();

                var chunks = ( UInt64 )( leftOverAmount / highestBill.FaceValue );

                if ( chunks > Decimal.Zero ) {
                    optimal[ highestBill ] += chunks;
                    leftOverAmount -= chunks * highestBill.FaceValue;
                }
                denominations.Remove( highestBill );
            }

            Assert.Less( leftOverAmount, PossibleDenominations.OrderBy( denomination => denomination.FaceValue ).First().FaceValue );
            return optimal;
        }

        public static IEnumerable<KeyValuePair<IDenomination, UInt64>> Transfer( [NotNull] this Wallet source, [NotNull] Wallet target ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }

            var transferred = new ConcurrentDictionary<IDenomination, UInt64>();

            foreach ( var pair in source.GetNotesGrouped() ) {
                if ( !source.Transfer( target, pair ) ) {
                    continue;
                }
                var denomination = pair.Key;
                var count = pair.Value;
                transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
            }

            foreach ( var pair in source.GetCoinsGrouped() ) {
                if ( !source.Transfer( target, pair ) ) {
                    continue;
                }
                var denomination = pair.Key;
                var count = pair.Value;
                transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
            }

            return transferred;
        }

        public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<IDenomination, UInt64> denominationAndAmount ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }
            return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value );
        }

        public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<IBankNote, UInt64> denominationAndAmount ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }
            return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value );
        }

        public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }
            return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value ) > 0m;
        }

        /// <summary>
        ///     Create a TPL dataflow task for depositing large volumes of money into this wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="sourceAmounts"></param>
        /// <returns></returns>
        public static Task Transfer( [NotNull] Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            var block = new ActionBlock<KeyValuePair<IDenomination, UInt64>>( pair => wallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeSensible );
            block.Complete();
            return block.Completion;
        }
    }
}