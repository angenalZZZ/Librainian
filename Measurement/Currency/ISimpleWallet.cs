﻿// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/ISimpleWallet.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    public interface ISimpleWallet {

        Decimal Balance {
            get;
        }

        [CanBeNull]
        Label LabelToFlashOnChanges {
            get; set;
        }

        [CanBeNull]
        Action<Decimal> OnAfterDeposit {
            get; set;
        }

        [CanBeNull]
        Action<Decimal> OnAfterWithdraw {
            get; set;
        }

        [CanBeNull]
        Action<Decimal> OnAnyUpdate {
            get; set;
        }

        [CanBeNull]
        Action<Decimal> OnBeforeDeposit {
            get; set;
        }

        [CanBeNull]
        Action<Decimal> OnBeforeWithdraw {
            get; set;
        }

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryAdd( Decimal amount, Boolean sanitize = true );

        Boolean TryAdd( [NotNull] SimpleWallet wallet, Boolean sanitize = true );

        /// <summary>Attempt to deposit amoount (larger than zero) to the <see cref="SimpleWallet.Balance" />.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryDeposit( Decimal amount, Boolean sanitize = true );

        Boolean TryTransfer( Decimal amount, ref SimpleWallet intoWallet, Boolean sanitize = true );

        Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true );

        void TryUpdateBalance( SimpleWallet simpleWallet );

        Boolean TryWithdraw( Decimal amount, Boolean sanitize = true );

        Boolean TryWithdraw( [NotNull] SimpleWallet wallet );
    }
}