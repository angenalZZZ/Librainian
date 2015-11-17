// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Factoid.cs" was last cleaned by Rick on 2015/06/12 at 2:59 PM

namespace Librainian.Knowledge {

    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Maths;

    [DataContract( IsReference = true )]
    public class Factoid {

        //TODO

        [DataMember]
        public String Description {
            get; private set;
        }

        [DataMember]
        public Percentage Truthiness {
            get; private set;
        }

        public Factoid([NotNull] String description, Percentage truthiness) {
            if ( description == null ) {
                throw new ArgumentNullException( nameof( description ) );
            }
            this.Description = description;
            this.Truthiness = truthiness;
        }
    }

    /*
        [DataContract( IsReference = true )]
        public class Teacher  {
            [DataMember] public ThreadSafeList< Topic > Topics = new ThreadSafeList< Topic >();

            public void AddTopic( String topic ) {

                //TODO uh....... where was I going with this ?
                this.Topics.Add( new Topic() );
            }

            public override void Deserialize() {
                try {
                    this.IsDeserializing = true;
                    if ( String.IsNullOrWhiteSpace( this.PersistPath ) ) {
                        throw new ArgumentException( "PersistFileName needs to be set." );
                    }
                    this.PersistPath.Loader< Teacher >( onLoad: database => database.DeepClone( destination: this ) );
                }
                finally {
                    this.IsDeserializing = false;
                }
            }

            public override void Serialize() {
                try {
                    this.IsSerializing = true;
                    if ( String.IsNullOrWhiteSpace( this.PersistPath ) ) {
                        throw new ArgumentException( "PersistFileName needs to be set." );
                    }
                    this.Saver( this.PersistPath );
                }
                finally {
                    this.IsSerializing = false;
                }
            }
        }
    */
}